namespace ccJsonParser.Models
{
    public class JsonNumber(double value) : JsonValue
    {
        public override object GetValue() => value;
    }
} 