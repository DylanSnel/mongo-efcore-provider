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

using System.Reflection;
using MongoDB.Bson;

namespace MongoDB.EntityFrameworkCore.FunctionalTests.Mapping;

[XUnitCollection("MappingTests")]
public class ClrTypeMappingTests : IClassFixture<TemporaryDatabaseFixture>
{
    private readonly Random _random = new();
    private readonly TemporaryDatabaseFixture _tempDatabase;

    public ClrTypeMappingTests(TemporaryDatabaseFixture tempDatabase)
    {
        _tempDatabase = tempDatabase;
    }

    class Entity<TValue>
    {
        public ObjectId _id { get; set; }
        public TValue Value { get; set; }
    }

    class IdEntity
    {
        public ObjectId _id { get; set; }
    }

    class GuidEntity : IdEntity
    {
        public Guid aGuid { get; set; }
    }

    class StringEntity : IdEntity
    {
        public string aString { get; set; }
    }

    class Int16Entity : IdEntity
    {
        public Int16 anInt16 { get; set; }
    }

    class Int32Entity : IdEntity
    {
        public Int32 anInt32 { get; set; }
    }

    class Int64Entity : IdEntity
    {
        public Int64 anInt64 { get; set; }
    }

    class ByteEntity : IdEntity
    {
        public Byte aByte { get; set; }
    }

    class CharEntity : IdEntity
    {
        public Char aChar { get; set; }
    }

    class DecimalEntity : IdEntity
    {
        public Decimal aDecimal { get; set; }
    }

    class SingleFloatEntity : IdEntity
    {
        public Single aSingle { get; set; }
    }

    class DoubleFloatEntity : IdEntity
    {
        public Double aDouble { get; set; }
    }

    [Fact]
    public void Guid_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<GuidEntity>();

        var expected = Guid.NewGuid();
        collection.InsertOne(new GuidEntity {_id = ObjectId.GenerateNewId(), aGuid = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aGuid);
    }

    [Fact]
    public void String_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<StringEntity>();

        var expected = Guid.NewGuid().ToString();
        collection.InsertOne(new StringEntity {_id = ObjectId.GenerateNewId(), aString = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aString);
    }

    [Fact]
    public void Int16_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int16Entity>();

        var expected = (Int16)_random.Next(1, Int16.MaxValue);
        collection.InsertOne(new Int16Entity {_id = ObjectId.GenerateNewId(), anInt16 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt16);
    }

    [Fact]
    public void Int16_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int16Entity>();

        collection.InsertOne(new Int16Entity {_id = ObjectId.GenerateNewId(), anInt16 = 0});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.anInt16);
    }

    [Fact]
    public void Int16_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int16Entity>();

        var expected = (Int16)_random.Next(Int16.MinValue, -1);
        collection.InsertOne(new Int16Entity {_id = ObjectId.GenerateNewId(), anInt16 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt16);
    }

    [Fact]
    public void Int32_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int32Entity>();

        var expected = _random.Next(1, Int32.MaxValue);
        collection.InsertOne(new Int32Entity {_id = ObjectId.GenerateNewId(), anInt32 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt32);
    }

    [Fact]
    public void Int32_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int32Entity>();

        collection.InsertOne(new Int32Entity {_id = ObjectId.GenerateNewId(), anInt32 = 0});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.anInt32);
    }

    [Fact]
    public void Int32_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int32Entity>();

        var expected = _random.Next(Int32.MinValue, -1);
        collection.InsertOne(new Int32Entity {_id = ObjectId.GenerateNewId(), anInt32 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt32);
    }

    [Fact]
    public void Int64_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int64Entity>();

        var expected = _random.NextInt64(1, Int64.MaxValue);
        collection.InsertOne(new Int64Entity {_id = ObjectId.GenerateNewId(), anInt64 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt64);
    }

    [Fact]
    public void Int64_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int64Entity>();

        collection.InsertOne(new Int64Entity {_id = ObjectId.GenerateNewId(), anInt64 = 0});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.anInt64);
    }

    [Fact]
    public void Int64_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Int64Entity>();

        var expected = _random.NextInt64(Int64.MinValue, -1);
        collection.InsertOne(new Int64Entity {_id = ObjectId.GenerateNewId(), anInt64 = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.anInt64);
    }

    [Fact]
    public void Byte_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<ByteEntity>();

        var expected = (Byte)_random.Next(0, Byte.MaxValue);
        collection.InsertOne(new ByteEntity {_id = ObjectId.GenerateNewId(), aByte = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aByte);
    }

    [Fact]
    public void Char_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<CharEntity>();

        var expected = (Char)_random.Next(1, 0xD799);
        collection.InsertOne(new CharEntity {_id = ObjectId.GenerateNewId(), aChar = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aChar);
    }

    [Fact]
    public void Decimal_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DecimalEntity>();

        decimal expected = 0m;
        while (expected <= 0)
            expected = _random.NextDecimal();

        collection.InsertOne(new DecimalEntity {_id = ObjectId.GenerateNewId(), aDecimal = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aDecimal);
    }

    [Fact]
    public void Decimal_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DecimalEntity>();

        collection.InsertOne(new DecimalEntity {_id = ObjectId.GenerateNewId(), aDecimal = 0m});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.aDecimal);
    }

    [Fact]
    public void Decimal_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DecimalEntity>();

        decimal expected = 0m;
        while (expected >= 0)
            expected = _random.NextDecimal();

        collection.InsertOne(new DecimalEntity {_id = ObjectId.GenerateNewId(), aDecimal = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aDecimal);
    }

    [Fact]
    public void Single_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<SingleFloatEntity>();

        var expected = _random.NextSingle() * Single.MaxValue;
        collection.InsertOne(new SingleFloatEntity {_id = ObjectId.GenerateNewId(), aSingle = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aSingle);
    }

    [Fact]
    public void Single_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<SingleFloatEntity>();

        collection.InsertOne(new SingleFloatEntity {_id = ObjectId.GenerateNewId(), aSingle = 0});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.aSingle);
    }

    [Fact]
    public void Single_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<SingleFloatEntity>();

        var expected = _random.NextSingle() * Single.MinValue;
        collection.InsertOne(new SingleFloatEntity {_id = ObjectId.GenerateNewId(), aSingle = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aSingle);
    }

    [Fact]
    public void Double_positive_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DoubleFloatEntity>();

        var expected = _random.NextDouble() * Double.MaxValue;
        collection.InsertOne(new DoubleFloatEntity {_id = ObjectId.GenerateNewId(), aDouble = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aDouble);
    }

    [Fact]
    public void Double_zero_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DoubleFloatEntity>();

        collection.InsertOne(new DoubleFloatEntity {_id = ObjectId.GenerateNewId(), aDouble = 0});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(0, actual.aDouble);
    }

    [Fact]
    public void Double_negative_read()
    {
        var collection = _tempDatabase.CreateTemporaryCollection<DoubleFloatEntity>();

        var expected = _random.NextDouble() * Double.MinValue;
        collection.InsertOne(new DoubleFloatEntity {_id = ObjectId.GenerateNewId(), aDouble = expected});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual.aDouble);
    }

    [Theory]
    [InlineData(typeof(short), (short)0)]
    [InlineData(typeof(short), (short)-15)]
    [InlineData(typeof(short), (short)42)]
    [InlineData(typeof(short?), null)]
    [InlineData(typeof(short?), (short)0)]
    [InlineData(typeof(short?), (short)-15)]
    [InlineData(typeof(short?), (short)42)]
    [InlineData(typeof(int), 0)]
    [InlineData(typeof(int), -15)]
    [InlineData(typeof(int), 42)]
    [InlineData(typeof(int?), null)]
    [InlineData(typeof(int?), 0)]
    [InlineData(typeof(int?), -15)]
    [InlineData(typeof(int?), 42)]
    [InlineData(typeof(TestEnum), TestEnum.EnumValue0)]
    [InlineData(typeof(TestEnum), TestEnum.EnumValue1)]
    [InlineData(typeof(TestEnum?), null)]
    [InlineData(typeof(TestEnum?), TestEnum.EnumValue0)]
    [InlineData(typeof(TestEnum?), TestEnum.EnumValue1)]
    [InlineData(typeof(TestByteEnum), TestByteEnum.EnumValue0)]
    [InlineData(typeof(TestByteEnum), TestByteEnum.EnumValue1)]
    [InlineData(typeof(TestByteEnum?), null)]
    [InlineData(typeof(TestByteEnum?), TestByteEnum.EnumValue0)]
    [InlineData(typeof(TestByteEnum?), TestByteEnum.EnumValue1)]
    [InlineData(typeof(int[]), null)]
    [InlineData(typeof(int[]), new[] {-5, 0, 128, 10})]
    // TODO: investigate and fix IEnumerable property support
    // [InlineData(typeof(IEnumerable<int>), new[] { -5, 0, 128, 10 })]
    [InlineData(typeof(IList<int>), null)]
    [InlineData(typeof(IList<int>), new[] {-5, 0, 128, 10})]
    [InlineData(typeof(ICollection<int>), new[] {-5, 0, 128, 10})]
    [InlineData(typeof(IReadOnlyList<int>), new[] {-5, 0, 128, 10})]
    [InlineData(typeof(List<int>), new[] {-5, 0, 128, 10})]
    [InlineData(typeof(string[]), null)]
    [InlineData(typeof(string[]), new[] {"one", "two"})]
    [InlineData(typeof(IList<string>), null)]
    [InlineData(typeof(List<string>), new[] {"one", "two"})]
    public void Type_mapping_test(Type valueType, object? value)
    {
        if (value != null && !value.GetType().IsAssignableTo(valueType))
        {
            value = Activator.CreateInstance(valueType, value);
        }

        var methodInfo = this.GetType().GetMethod(nameof(ClrTypeMappingTestImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        methodInfo.MakeGenericMethod(valueType).Invoke(this, new[] {value});
    }

    private enum TestEnum
    {
        EnumValue0 = 0,
        EnumValue1 = 1
    }

    private enum TestByteEnum : byte
    {
        EnumValue0 = 0,
        EnumValue1 = 1
    }

    private void ClrTypeMappingTestImpl<TValue>(TValue value)
    {
        var collection = _tempDatabase.CreateTemporaryCollection<Entity<TValue>>("ClrTypeMapping", typeof(TValue), value);
        collection.InsertOne(new Entity<TValue> {_id = ObjectId.GenerateNewId(), Value = value});

        var db = SingleEntityDbContext.Create(collection);
        var actual = db.Entitites.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(value, actual.Value);
    }
}
