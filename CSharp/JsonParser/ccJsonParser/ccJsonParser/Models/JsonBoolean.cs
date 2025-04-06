namespace ccJsonParser.Models
{
    public class JsonBoolean(bool value) : JsonValue
    {
        public override object GetValue() => value;
    }
} 