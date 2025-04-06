namespace ccJsonParser.Models;

public class JsonArray : JsonValue
{
    private readonly List<JsonValue> _values = [];

    public void Add(JsonValue value)
    {
        _values.Add(value);
    }

    public override object GetValue()
    {
        return _values;
    }
}