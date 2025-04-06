using System;
using Xunit;
using ccJsonParser;
using ccJsonParser.Tests.Models;

namespace ccJsonParser.Tests
{
    public class JsonParserTests
    {
        [Fact]
        public void Deserialize_ValidJson_ReturnsPersonObject()
        {
            // Arrange
            string? json = @"{
                ""name"": ""John Doe"",
                ""age"": 30,
                ""isActive"": true,
                ""address"": {
                    ""street"": ""123 Main St"",
                    ""city"": ""Any town""
                },
                ""tags"": [""developer"", ""programmer"", ""coder""],
                ""nullValue"": null,
                ""numbers"": [1, 2.5, -3.7, 1e3],
                ""nested"": {
                    ""array"": [1, 2, 3],
                    ""object"": {
                        ""key"": ""value""
                    }
                }
            }";

            // Act
            var person = JsonParser.Deserialize<Person>(json);

            // Assert
            Assert.NotNull(person);
            Assert.Equal("John Doe", person.Name);
            Assert.Equal(30, person.Age);
            Assert.True(person.IsActive);
            Assert.NotNull(person.Address);
            Assert.Equal("123 Main St", person.Address.Street);
            Assert.Equal("Any town", person.Address.City);
            Assert.NotNull(person.Tags);
            Assert.Equal(3, person.Tags.Count);
            Assert.Contains("developer", person.Tags);
            Assert.Contains("programmer", person.Tags);
            Assert.Contains("coder", person.Tags);
            Assert.NotNull(person.Numbers);
            Assert.Equal(4, person.Numbers.Length);
            Assert.Equal(1, person.Numbers[0]);
            Assert.Equal(2.5, person.Numbers[1]);
            Assert.Equal(-3.7, person.Numbers[2]);
            Assert.Equal(1000, person.Numbers[3]);
            Assert.NotNull(person.Nested);
            Assert.NotNull(person.Nested.Array);
            Assert.Equal(3, person.Nested.Array.Length);
            Assert.Equal(1, person.Nested.Array[0]);
            Assert.Equal(2, person.Nested.Array[1]);
            Assert.Equal(3, person.Nested.Array[2]);
            Assert.NotNull(person.Nested.Object);
            Assert.Single(person.Nested.Object);
            Assert.Equal("value", person.Nested.Object["key"]);
        }

        [Fact]
        public void Deserialize_InvalidJson_ThrowsException()
        {
            // Arrange
            string invalidJson = "{ \"name\": \"John Doe\", \"age\": }";

            // Act & Assert
            Assert.Throws<Exception>(() => JsonParser.Deserialize<Person>(invalidJson));
        }

        [Fact]
        public void Deserialize_EmptyJson_ThrowsException()
        {
            // Arrange
            var emptyJson = "";

            // Act & Assert
            Assert.Throws<Exception>(() => JsonParser.Deserialize<Person>(emptyJson));
        }

        [Fact]
        public void Deserialize_NullJson_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => JsonParser.Deserialize<Person>(null));
        }

        [Fact]
        public void Deserialize_SimpleTypes_WorksCorrectly()
        {
            // Arrange
            var json = @"{
                ""stringValue"": ""test"",
                ""intValue"": 42,
                ""doubleValue"": 3.14,
                ""boolValue"": true,
                ""nullValue"": null
            }";

            // Act
            var result = JsonParser.Deserialize<SimpleTypes>(json);

            // Assert
            Assert.Equal("test", result?.StringValue);
            Assert.Equal(42, result?.IntValue);
            Assert.Equal(3.14, result?.DoubleValue);
            Assert.True(result?.BoolValue);
            Assert.Null(result?.NullValue);
        }

        [Fact]
        public void Deserialize_Arrays_WorksCorrectly()
        {
            // Arrange
            const string json = """
                                {
                                     "stringArray": ["a", "b", "c"],
                                     "intArray": [1, 2, 3],
                                     "doubleArray": [1.1, 2.2, 3.3],
                                     "boolArray": [true, false, true]
                                }
                                """;

            // Act
            var result = JsonParser.Deserialize<ArrayTypes>(json);

            // Assert
            Assert.Equal(3, result.StringArray.Length);
            Assert.Equal("a", result.StringArray[0]);
            Assert.Equal("b", result.StringArray[1]);
            Assert.Equal("c", result.StringArray[2]);

            Assert.Equal(3, result.IntArray.Length);
            Assert.Equal(1, result.IntArray[0]);
            Assert.Equal(2, result.IntArray[1]);
            Assert.Equal(3, result.IntArray[2]);

            Assert.Equal(3, result.DoubleArray.Length);
            Assert.Equal(1.1, result.DoubleArray[0]);
            Assert.Equal(2.2, result.DoubleArray[1]);
            Assert.Equal(3.3, result.DoubleArray[2]);

            Assert.Equal(3, result.BoolArray.Length);
            Assert.True(result.BoolArray[0]);
            Assert.False(result.BoolArray[1]);
            Assert.True(result.BoolArray[2]);
        }

        [Fact]
        public void Deserialize_SimpleObject_ReturnsCorrectObject()
        {
            // Arrange
            const string json = "{\"name\":\"John Doe\",\"age\":30,\"isActive\":true}";

            // Act
            var person = JsonParser.Deserialize<Person>(json);

            // Assert
            Assert.NotNull(person);
            Assert.Equal("John Doe", person.Name);
            Assert.Equal(30, person.Age);
            Assert.True(person.IsActive);
        }

        [Fact]
        public void Deserialize_ComplexObject_ReturnsCorrectObject()
        {
            // Arrange
            const string json = """
                                {
                                    "name": "John Doe",
                                    "age": 30,
                                    "address": {
                                        "street": "123 Main St",
                                        "city": "Any town"
                                    },
                                    "tags": ["developer", "programmer"]
                                }
                                """;

            // Act
            var person = JsonParser.Deserialize<Person>(json);

            // Assert
            Assert.NotNull(person);
            Assert.Equal("John Doe", person.Name);
            Assert.Equal(30, person.Age);
            Assert.NotNull(person.Address);
            Assert.Equal("123 Main St", person.Address.Street);
            Assert.Equal("Any town", person.Address.City);
            Assert.NotNull(person.Tags);
            Assert.Equal(2, person.Tags.Count);
            Assert.Contains("developer", person.Tags);
            Assert.Contains("programmer", person.Tags);
        }

        [Fact]
        public void Deserialize_Array_ReturnsCorrectArray()
        {
            // Arrange
            const string json = "[1, 2, 3, 4, 5]";

            // Act
            var numbers = JsonParser.Deserialize<int[]>(json);

            // Assert
            Assert.NotNull(numbers);
            Assert.Equal(5, numbers.Length);
            Assert.Equal(1, numbers[0]);
            Assert.Equal(2, numbers[1]);
            Assert.Equal(3, numbers[2]);
            Assert.Equal(4, numbers[3]);
            Assert.Equal(5, numbers[4]);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public Address Address { get; set; }
        public List<string> Tags { get; set; }
        public double[] Numbers { get; set; }
        public NestedData Nested { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    public class NestedData
    {
        public int[] Array { get; set; }
        public Dictionary<string, string> Object { get; set; }
    }

    public class SimpleTypes
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public bool BoolValue { get; set; }
        public object NullValue { get; set; }
    }

    public class ArrayTypes
    {
        public string[] StringArray { get; set; }
        public int[] IntArray { get; set; }
        public double[] DoubleArray { get; set; }
        public bool[] BoolArray { get; set; }
    }
} 