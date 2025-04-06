namespace ccJsonParser.Token
{
    public class JsonToken(JsonTokenType type, string value, int line, int column)
    {
        public JsonTokenType Type { get; } = type;
        public string Value { get; } = value;
        public int Line { get; } = line;
        public int Column { get; } = column;

        public override string ToString()
        {
            return $"{Type} ({Value}) at line {Line}, column {Column}";
        }
    }
} 