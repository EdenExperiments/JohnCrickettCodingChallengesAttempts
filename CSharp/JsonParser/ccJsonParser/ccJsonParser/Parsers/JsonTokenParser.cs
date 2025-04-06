using ccJsonParser.Models;
using ccJsonParser.Token;

namespace ccJsonParser.Parsers;

public class JsonTokenParser
{
    private readonly IEnumerator<JsonToken> _tokens;
    private JsonToken _currentToken;

    public JsonTokenParser(IEnumerable<JsonToken> tokens)
    {
        _tokens = tokens.GetEnumerator();
        MoveNext();
    }

    public JsonValue Parse()
    {
        return ParseValue();
    }

    private JsonValue ParseValue()
    {
        switch (_currentToken.Type)
        {
            case JsonTokenType.LeftBrace:
                return ParseObject();
            case JsonTokenType.LeftBracket:
                return ParseArray();
            case JsonTokenType.String:
                return ParseString();
            case JsonTokenType.Number:
                return ParseNumber();
            case JsonTokenType.True:
            case JsonTokenType.False:
                return ParseBoolean();
            case JsonTokenType.Null:
                return ParseNull();
            default:
                throw new Exception(
                    $"Unexpected token {_currentToken.Type} at line {_currentToken.Line}, column {_currentToken.Column}");
        }
    }

    private JsonObject ParseObject()
    {
        var obj = new JsonObject();
        MoveNext(); // Skip {

        while (_currentToken.Type != JsonTokenType.RightBrace)
        {
            if (_currentToken.Type != JsonTokenType.String)
                throw new Exception($"Expected string key at line {_currentToken.Line}, column {_currentToken.Column}");

            var key = _currentToken.Value;
            MoveNext();

            if (_currentToken.Type != JsonTokenType.Colon)
                throw new Exception($"Expected colon at line {_currentToken.Line}, column {_currentToken.Column}");

            MoveNext();
            var value = ParseValue();
            obj.AddProperty(key, value);

            if (_currentToken.Type == JsonTokenType.Comma)
            {
                MoveNext();
                if (_currentToken.Type == JsonTokenType.RightBrace)
                    throw new Exception($"Trailing comma at line {_currentToken.Line}, column {_currentToken.Column}");
            }
            else if (_currentToken.Type != JsonTokenType.RightBrace)
            {
                throw new Exception(
                    $"Expected comma or right brace at line {_currentToken.Line}, column {_currentToken.Column}");
            }
        }

        MoveNext(); // Skip }
        return obj;
    }

    private JsonArray ParseArray()
    {
        var array = new JsonArray();
        MoveNext(); // Skip [

        while (_currentToken.Type != JsonTokenType.RightBracket)
        {
            var value = ParseValue();
            array.Add(value);

            if (_currentToken.Type == JsonTokenType.Comma)
            {
                MoveNext();
                if (_currentToken.Type == JsonTokenType.RightBracket)
                    throw new Exception($"Trailing comma at line {_currentToken.Line}, column {_currentToken.Column}");
            }
            else if (_currentToken.Type != JsonTokenType.RightBracket)
            {
                throw new Exception(
                    $"Expected comma or right bracket at line {_currentToken.Line}, column {_currentToken.Column}");
            }
        }

        MoveNext(); // Skip ]
        return array;
    }

    private JsonString ParseString()
    {
        var value = new JsonString(_currentToken.Value);
        MoveNext();
        return value;
    }

    private JsonNumber ParseNumber()
    {
        if (!double.TryParse(_currentToken.Value, out var number))
            throw new Exception($"Invalid number format at line {_currentToken.Line}, column {_currentToken.Column}");

        var value = new JsonNumber(number);
        MoveNext();
        return value;
    }

    private JsonBoolean ParseBoolean()
    {
        var value = new JsonBoolean(_currentToken.Type == JsonTokenType.True);
        MoveNext();
        return value;
    }

    private JsonNull ParseNull()
    {
        var value = new JsonNull();
        MoveNext();
        return value;
    }

    private void MoveNext()
    {
        if (!_tokens.MoveNext())
            throw new Exception("Unexpected end of input");

        _currentToken = _tokens.Current;
    }
}