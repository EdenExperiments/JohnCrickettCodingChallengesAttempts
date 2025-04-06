namespace ccJsonParser.Models
{
    public class JsonString(string value) : JsonValue
    {
        public override object GetValue() => value;
    }
} 