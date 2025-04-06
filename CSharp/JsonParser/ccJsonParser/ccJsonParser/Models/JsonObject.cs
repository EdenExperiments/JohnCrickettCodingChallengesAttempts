using System.Collections.Generic;

namespace ccJsonParser.Models
{
    public class JsonObject : JsonValue
    {
        private readonly Dictionary<string, JsonValue> _properties = new();

        public void AddProperty(string name, JsonValue value)
        {
            _properties[name] = value;
        }

        public override object GetValue() => _properties;
    }
} 