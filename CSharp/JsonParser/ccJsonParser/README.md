# ccJsonParser

A lightweight JSON parser for .NET applications that provides simple deserialization functionality.

#### This project was created heavily with Cursor, I created the project in VS2022, created a file structure and then generated in cursor, debugged and tested in VS

## Features

- Simple and intuitive API
- Strongly-typed deserialization
- Support for complex objects and arrays
- No external dependencies

## Usage

### Basic Usage

```csharp
using ccJsonParser;

// Deserialize a simple object
string json = "{\"name\":\"John Doe\",\"age\":30}";
var person = JsonParser.Deserialize<Person>(json);

// Deserialize an array
string numbersJson = "[1, 2, 3, 4, 5]";
var numbers = JsonParser.Deserialize<int[]>(numbersJson);
```

### Complex Objects

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
    public List<string> Tags { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

// Deserialize a complex object
string json = @"{
    ""name"": ""John Doe"",
    ""age"": 30,
    ""address"": {
        ""street"": ""123 Main St"",
        ""city"": ""Anytown""
    },
    ""tags"": [""developer"", ""programmer""]
}";

var person = JsonParser.Deserialize<Person>(json);
```

## Supported Types

- Strings
- Numbers (int, double, etc.)
- Booleans
- Null values
- Arrays and Lists
- Complex objects
- Nested objects and arrays

## Error Handling

The parser throws exceptions with detailed error messages when:
- The JSON string is invalid
- The JSON structure doesn't match the target type
- Required properties are missing
- Type conversion fails

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
