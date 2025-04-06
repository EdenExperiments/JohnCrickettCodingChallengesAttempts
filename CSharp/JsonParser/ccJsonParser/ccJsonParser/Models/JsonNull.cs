namespace ccJsonParser.Models
{
    public class JsonNull : JsonValue
    {
        public override object? GetValue() => null;
    }
} 