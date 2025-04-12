using System.Text;
using HuffmanEncoderDecoder.Interfaces.Services;

namespace HuffmanEncoderDecoder.Services;

public class DecodingService : IDecodingService
{
    public Dictionary<string, char> ParsePrefixTable(string header)
    {
        try
        {
            var prefixTable = new Dictionary<string, char>();
            var separator = '\u001E'; 

            foreach (var line in header.Split('\n'))
            {
                var cleanLine = line.Trim('\r', '\n').Trim();
                if (string.IsNullOrWhiteSpace(cleanLine)) continue;
            
                var parts = cleanLine.Split(separator);
                if (parts.Length != 2) throw new Exception($"Invalid prefix table entry: {line}. Likely \u001E is within the source text.");
                var character = parts[0];
                var code = parts[1].Trim();

                // Convert the escaped character back
                var decodedChar = character switch
                {
                    "\\n" => '\n',
                    "\\r" => '\r',
                    "\\t" => '\t',
                    "␣" => ' ',
                    "\\0" => '\0',
                    _ => character[0]
                };

                prefixTable[code] = decodedChar;
            }

            return prefixTable;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred when parsing the prefix table from file.", ex);
        }
    }

    public string DecodeBitString(string bitString, Dictionary<string, char> prefixTable)
    {
        try
        {
            var result = new StringBuilder();
            var start = 0;
            var maxCodeLength = prefixTable.Keys.Max(k => k.Length); 

            while (start < bitString.Length)
            {
                var matchFound = false;
                var length = 1;

                while (length <= maxCodeLength && (start + length) <= bitString.Length)
                {
                    var slice = bitString.Substring(start, length);
                    if (prefixTable.TryGetValue(slice, out var ch))
                    {
                        result.Append(ch);
                        start += length;
                        matchFound = true;
                        break;
                    }

                    length++;
                }

                if (matchFound) continue;
                throw new Exception($"No match found for bit stream starting at index {start}.");
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred when decoding the bit string with the prefix table.", ex);
        }
    }

    public string BytesToBitString(byte[] bytes, int bitLength)
    {
        try
        {
            var sb = new StringBuilder(bitLength);

            for (var i = 0; i < bitLength; i++)
            {
                var byteIndex = i / 8;
                var bitIndex = 7 - i % 8;

                var bit = (bytes[byteIndex] & (1 << bitIndex)) != 0;
                sb.Append(bit ? '1' : '0');
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred converting bytes to a bit string.", ex);
        }
    }
}