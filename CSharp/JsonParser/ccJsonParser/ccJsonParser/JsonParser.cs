using System.Reflection;
using ccJsonParser.Models;
using ccJsonParser.Parsers;
using ccJsonParser.Token;

namespace ccJsonParser;

/// <summary>
///     Provides methods for deserializing JSON strings into strongly-typed objects.
/// </summary>
public static class JsonParser
{
    /// <summary>
    ///     Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type T populated with the data from the JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the json parameter is null.</exception>
    /// <exception cref="Exception">Thrown when the JSON string is invalid or cannot be deserialized to the specified type.</exception>
    /// <example>
    ///     <code>
    /// string json = "{\"name\":\"John Doe\",\"age\":30}";
    /// var person = JsonDeserializer.Deserialize&lt;Person&gt;(json);
    /// </code>
    /// </example>
    public static T? Deserialize<T>(string? json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));
        var tokenizer = new JsonTokenizer(json);
        var parser = new JsonTokenParser(tokenizer.Tokenize());
        var jsonValue = parser.Parse();
        return jsonValue switch
        {
            null => default,
            JsonObject jsonObject => DeserializeObject<T>(jsonObject),
            JsonArray jsonArray => DeserializeArray<T>(jsonArray),
            JsonString jsonString => (T)Convert.ChangeType(jsonString.GetValue(), typeof(T)),
            JsonNumber jsonNumber => (T)Convert.ChangeType(jsonNumber.GetValue(), typeof(T)),
            JsonBoolean jsonBoolean => (T)Convert.ChangeType(jsonBoolean.GetValue(), typeof(T)),
            JsonNull => default,
            _ => throw new InvalidOperationException(
                $"Cannot deserialize {jsonValue.GetType().Name} to {typeof(T).Name}")
        };

    }

    private static T DeserializeObject<T>(JsonObject jsonObject)
    {
        var jsonDict = (Dictionary<string, JsonValue?>)jsonObject.GetValue();

        // Handle dictionary types
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var keyType = typeof(T).GetGenericArguments()[0];
            var valueType = typeof(T).GetGenericArguments()[1];
            var result = Activator.CreateInstance<T>();
            var addMethod = typeof(T).GetMethod("Add");

            foreach (var kvp in jsonDict)
            {
                var key = Convert.ChangeType(kvp.Key, keyType);
                var value = DeserializeValue(kvp.Value, valueType);
                addMethod.Invoke(result, [key, value]);
            }

            return result;
        }

        var obj = Activator.CreateInstance<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
            // Try exact match first
            if (jsonDict.TryGetValue(prop.Name, out var value))
            {
                var propValue = DeserializeValue(value, prop.PropertyType);
                prop.SetValue(obj, propValue);
            }
            // Try case-insensitive match
            else
            {
                var matchingKey = jsonDict.Keys.FirstOrDefault(k =>
                    string.Equals(k, prop.Name, StringComparison.OrdinalIgnoreCase));
                if (matchingKey != null)
                {
                    var propValue = DeserializeValue(jsonDict[matchingKey], prop.PropertyType);
                    prop.SetValue(obj, propValue);
                }
            }

        foreach (var field in fields)
            // Try exact match first
            if (jsonDict.TryGetValue(field.Name, out var value))
            {
                var fieldValue = DeserializeValue(value, field.FieldType);
                field.SetValue(obj, fieldValue);
            }
            // Try case-insensitive match
            else
            {
                var matchingKey = jsonDict.Keys.FirstOrDefault(k =>
                    string.Equals(k, field.Name, StringComparison.OrdinalIgnoreCase));
                if (matchingKey != null)
                {
                    var fieldValue = DeserializeValue(jsonDict[matchingKey], field.FieldType);
                    field.SetValue(obj, fieldValue);
                }
            }

        return obj;
    }

    private static T DeserializeArray<T>(JsonArray jsonArray)
    {
        var list = (List<JsonValue?>)jsonArray.GetValue();
        var elementType = typeof(T).GetElementType() ?? typeof(T).GetGenericArguments()[0];
        var array = Array.CreateInstance(elementType, list.Count);

        for (var i = 0; i < list.Count; i++) array.SetValue(DeserializeValue(list[i], elementType), i);

        if (typeof(T).IsArray)
            return (T)(object)array;

        var listType = typeof(List<>).MakeGenericType(elementType);
        var result = Activator.CreateInstance(listType);
        var addMethod = listType.GetMethod("Add");
        foreach (var item in array) addMethod.Invoke(result, new[] { item });
        return (T)result;
    }

    private static object? DeserializeValue(JsonValue? value, Type targetType)
    {
        if (value == null)
            return null;

        if (value is JsonObject jsonObject)
        {
            var method = typeof(JsonParser)
                .GetMethod("DeserializeObject", BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(targetType);
            return method?.Invoke(null, [jsonObject]);
        }

        if (value is JsonArray jsonArray)
        {
            var method = typeof(JsonParser)
                .GetMethod("DeserializeArray", BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(targetType);
            return method?.Invoke(null, [jsonArray]);
        }

        if (value is JsonString jsonString)
            return Convert.ChangeType(jsonString.GetValue(), targetType);
        if (value is JsonNumber jsonNumber)
            return Convert.ChangeType(jsonNumber.GetValue(), targetType);
        if (value is JsonBoolean jsonBoolean)
            return Convert.ChangeType(jsonBoolean.GetValue(), targetType);
        if (value is JsonNull)
            return null;

        throw new InvalidOperationException($"Cannot deserialize {value.GetType().Name} to {targetType.Name}");
    }
}