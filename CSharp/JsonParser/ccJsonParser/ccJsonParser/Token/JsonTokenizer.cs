using System.Text;

namespace ccJsonParser.Token
{
    public class JsonTokenizer(string? input)
    {
        private int _position = 0;
        private int _line = 1;
        private int _column = 1;

        public IEnumerable<JsonToken> Tokenize()
        {
            while (_position < input.Length)
            {
                var current = input[_position];

                if (char.IsWhiteSpace(current))
                {
                    if (current == '\n')
                    {
                        _line++;
                        _column = 1;
                    }
                    else
                    {
                        _column++;
                    }
                    _position++;
                    continue;
                }

                yield return current switch
                {
                    '{' => CreateToken(JsonTokenType.LeftBrace, "{"),
                    '}' => CreateToken(JsonTokenType.RightBrace, "}"),
                    '[' => CreateToken(JsonTokenType.LeftBracket, "["),
                    ']' => CreateToken(JsonTokenType.RightBracket, "]"),
                    ':' => CreateToken(JsonTokenType.Colon, ":"),
                    ',' => CreateToken(JsonTokenType.Comma, ","),
                    '"' => TokenizeString(),
                    '-' or '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => TokenizeNumber(),
                    't' => TokenizeTrue(),
                    'f' => TokenizeFalse(),
                    'n' => TokenizeNull(),
                    _ => throw new Exception($"Unexpected character '{current}' at line {_line}, column {_column}")
                };
            }

            yield return new JsonToken(JsonTokenType.EndOfFile, "", _line, _column);
        }

        private JsonToken CreateToken(JsonTokenType type, string value)
        {
            var token = new JsonToken(type, value, _line, _column);
            _position++;
            _column++;
            return token;
        }

        private JsonToken TokenizeString()
        {
            _position++; // Skip opening quote
            _column++;
            var sb = new StringBuilder();

            while (_position < input.Length)
            {
                var current = input[_position];
                switch (current)
                {
                    case '"':
                        _position++;
                        _column++;
                        return new JsonToken(JsonTokenType.String, sb.ToString(), _line, _column - sb.Length - 1);
                    case '\\':
                    {
                        _position++;
                        _column++;
                        if (_position >= input.Length)
                            throw new Exception("Unterminated string literal");

                        current = input[_position];
                        switch (current)
                        {
                            case '"': sb.Append('"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u':
                                if (_position + 4 >= input.Length)
                                    throw new Exception("Invalid Unicode escape sequence");
                                string hex = input.Substring(_position + 1, 4);
                                sb.Append((char)Convert.ToInt32(hex, 16));
                                _position += 4;
                                _column += 4;
                                break;
                            default:
                                throw new Exception($"Invalid escape sequence '\\{current}'");
                        }

                        break;
                    }
                    default:
                        sb.Append(current);
                        break;
                }

                _position++;
                _column++;
            }

            throw new Exception("Unterminated string literal");
        }

        private JsonToken TokenizeNumber()
        {
            int start = _position;
            int startColumn = _column;

            if (input[_position] == '-')
            {
                _position++;
                _column++;
            }

            while (_position < input.Length && char.IsDigit(input[_position]))
            {
                _position++;
                _column++;
            }

            if (_position < input.Length && input[_position] == '.')
            {
                _position++;
                _column++;
                while (_position < input.Length && char.IsDigit(input[_position]))
                {
                    _position++;
                    _column++;
                }
            }

            if (_position < input.Length && (input[_position] == 'e' || input[_position] == 'E'))
            {
                _position++;
                _column++;
                if (_position < input.Length && (input[_position] == '+' || input[_position] == '-'))
                {
                    _position++;
                    _column++;
                }
                while (_position < input.Length && char.IsDigit(input[_position]))
                {
                    _position++;
                    _column++;
                }
            }

            string value = input.Substring(start, _position - start);
            return new JsonToken(JsonTokenType.Number, value, _line, startColumn);
        }

        private JsonToken TokenizeTrue()
        {
            if (_position + 3 < input.Length && input.Substring(_position, 4) == "true")
            {
                var token = new JsonToken(JsonTokenType.True, "true", _line, _column);
                _position += 4;
                _column += 4;
                return token;
            }
            throw new Exception($"Invalid token at line {_line}, column {_column}");
        }

        private JsonToken TokenizeFalse()
        {
            if (_position + 4 < input.Length && input.Substring(_position, 5) == "false")
            {
                var token = new JsonToken(JsonTokenType.False, "false", _line, _column);
                _position += 5;
                _column += 5;
                return token;
            }
            throw new Exception($"Invalid token at line {_line}, column {_column}");
        }

        private JsonToken TokenizeNull()
        {
            if (_position + 3 < input.Length && input.Substring(_position, 4) == "null")
            {
                var token = new JsonToken(JsonTokenType.Null, "null", _line, _column);
                _position += 4;
                _column += 4;
                return token;
            }
            throw new Exception($"Invalid token at line {_line}, column {_column}");
        }
    }
} 