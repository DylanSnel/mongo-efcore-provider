﻿/* Copyright 2023-present MongoDB Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.Metadata;

namespace MongoDB.EntityFrameworkCore.Infrastructure;

/// <summary>
/// Ensures rules required by the MongoDB provider upon a given <see cref="IModel"/> are adhered to.
/// </summary>
public class MongoModelValidator : ModelValidator
{
    /// <summary>
    /// Create a <see cref="MongoModelValidator"/>.
    /// </summary>
    /// <param name="dependencies">The <see cref="ModelValidatorDependencies"/> required by this validator.</param>
    public MongoModelValidator(ModelValidatorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    /// Validate that the <paramref name="model"/> is correct and can be used by the
    /// MongoDB provider.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/> to validate for correctness.</param>
    /// <param name="logger">A logger to receive validation diagnostic information.</param>
    public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.Validate(model, logger);

        ValidateElementNames(model, logger);
        ValidateNoShadowProperties(model, logger);
        ValidateNoMutableKeys(model, logger);
        ValidatePrimaryKeys(model, logger);
    }

    /// <summary>
    /// Validate that required primary keys exist and that they are aligned with the
    /// requirement of MongoDB that they be named "_id".
    /// </summary>
    /// <param name="model">The <see cref="IModel"/> to validate primary key correctness.</param>
    /// <param name="logger">A logger to receive validation diagnostic information.</param>
    /// <exception cref="NotSupportedException">Thrown when composite keys are encountered which are not supported yet.</exception>
    /// <exception cref="InvalidOperationException">Throw when an entity requiring a key does not have one or it is not mapped to "_id".</exception>
    protected void ValidatePrimaryKeys(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes().Where(e => e.IsDocumentRoot()))
        {
            ValidateEntityPrimaryKey(entityType);
        }
    }

    /// <summary>
    /// Validate that element names meet the requirements of MongoDB and that there
    /// are no element names duplicated for an entity.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/> to validate primary key correctness.</param>
    /// <param name="logger">A logger to receive validation diagnostic information.</param>
    /// <exception cref="NotSupportedException">Thrown when composite keys are encountered which are not supported.</exception>
    /// <exception cref="InvalidOperationException">Throw when an entity requiring a key does not have one or it is not mapped to "_id".</exception>
    protected void ValidateElementNames(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            ValidateEntityElementNames(entityType);
        }
    }


    private static void ValidateEntityPrimaryKey(IEntityType entityType)
    {
        // We must have a primary key on root documents
        var primaryKey = entityType.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count == 0)
        {
            throw new InvalidOperationException(
                $"The entity type '{entityType.DisplayName()}' is a root document but does not have a primary key set.");
        }

        if (primaryKey.Properties.Count == 1)
        {
            // The primary key must map to "_id"
            var primaryKeyProperty = primaryKey.Properties[0];
            string primaryKeyElementName = primaryKeyProperty.GetElementName();
            if (primaryKeyElementName != "_id")
            {
                throw new InvalidOperationException(
                    $"The entity type '{entityType.DisplayName()}' primary key property '{primaryKeyProperty.Name}' must be mapped to element '_id'.");
            }
        }
    }

    private static void ValidateEntityElementNames(IEntityType entityType)
    {
        var elementPropertyMap = new Dictionary<string, IPropertyBase>();

        foreach (var property in entityType.GetProperties())
        {
            string elementName = property.GetElementName();
            if (string.IsNullOrWhiteSpace(elementName)) continue;

            if (elementName.StartsWith("$"))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Name}' on entity type '{entityType.DisplayName()}' may not map to element '{elementName}' as it starts with the reserved character '$'.");
            }

            if (elementName.Contains('.'))
            {
                throw new InvalidOperationException(
                    $"Property '{property.Name}' on entity type '{entityType.DisplayName()}' may not map to element '{elementName}' as it contains the reserved character '.'.");
            }

            if (elementPropertyMap.TryGetValue(elementName, out var otherProperty))
            {
                throw new InvalidOperationException(
                    $"Properties '{property.Name}' and '{otherProperty.Name}' on entity type '{entityType.DisplayName()}' are mapped to element '{elementName}'. Map one of them to a different BSON element.");
            }

            elementPropertyMap[elementName] = property;
        }


        var elementNavigationMap = new Dictionary<string, INavigation>();

        foreach (var navigation in entityType.GetNavigations().Where(n => n.IsEmbedded()))
        {
            string? elementName = navigation.TargetEntityType.GetContainingElementName();
            if (elementName != null)
            {
                if (elementName.StartsWith("$"))
                {
                    throw new InvalidOperationException(
                        $"Property '{navigation.Name}' on entity type '{entityType.DisplayName()}' may not map to element '{elementName}' as it starts with the reserved character '$'.");
                }

                if (elementName.Contains('.'))
                {
                    throw new InvalidOperationException(
                        $"Property '{navigation.Name}' on entity type '{entityType.DisplayName()}' may not map to element '{elementName}' as it contains the reserved character '.'.");
                }

                if (elementPropertyMap.TryGetValue(elementName, out var otherProperty))
                {
                    throw new InvalidOperationException(
                        $"Navigation '{navigation.Name}' and Property '{otherProperty.Name}' on entity type '{entityType.DisplayName()}' are mapped to element '{elementName}'. Map one of them to a different BSON element.");
                }

                if (elementNavigationMap.TryGetValue(elementName, out var otherNavigation))
                {
                    throw new InvalidOperationException(
                        $"Navigations '{navigation.Name}' and '{otherNavigation.Name}' on entity type '{entityType.DisplayName()}' are mapped to element '{elementName}'. Map one of them to a different BSON element.");
                }

                elementNavigationMap[elementName] = navigation;
            }
        }
    }

    /// <summary>
    /// Validate that no entities have shadow properties.
    /// </summary>
    /// <param name="model">The <see cref="IModel"/> to validate for whether shadow properties are present.</param>
    /// <param name="logger">A logger to receive validation diagnostic information.</param>
    /// <exception cref="NotSupportedException">Thrown when shadow properties are encountered which are not supported.</exception>
    protected void ValidateNoShadowProperties(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes().Where(e => e.IsDocumentRoot()))
        {
            var shadowProperty = entityType.GetProperties().FirstOrDefault(p => p.IsShadowProperty());
            if (shadowProperty != null)
            {
                throw new NotSupportedException(
                    $"Unsupported shadow property '{shadowProperty.Name}' identified on entity type '{entityType.DisplayName()}'.");
            }
        }
    }

    /// <summary>
    /// Validates that the only keys that can actually be changed are shadow keys used by owned entities.
    /// </summary>
    /// <param name="model">The model to validate.</param>
    /// <param name="logger">The logger to use.</param>
    protected override void ValidateNoMutableKeys(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            foreach (var key in entityType.GetDeclaredKeys())
            {
                var mutableProperty = key.Properties.FirstOrDefault(p => p.ValueGenerated.HasFlag(ValueGenerated.OnUpdate));
                if (mutableProperty != null && !mutableProperty.IsOwnedCollectionShadowKey())
                {
                    throw new InvalidOperationException(CoreStrings.MutableKeyProperty(mutableProperty.Name));
                }
            }
        }
    }
}
