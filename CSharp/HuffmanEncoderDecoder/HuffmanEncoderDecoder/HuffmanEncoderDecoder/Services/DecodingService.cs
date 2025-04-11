using System.Text;
using HuffmanEncoderDecoder.Interfaces.Services;

namespace HuffmanEncoderDecoder.Services;

public class DecodingService : IDecodingService
{
    public Dictionary<string, char> ParsePrefixTable(string header)
    {
        var prefixTable = new Dictionary<string, char>();
        char separator = '\u001E'; // ← safest weird char

        foreach (var line in header.Split('\n'))
        {
            var cleanLine = line.Trim('\r', '\n').Trim(); // strips weird line endings
            if (string.IsNullOrWhiteSpace(cleanLine)) continue;
            
            var parts = cleanLine.Split(separator);
            if (parts.Length != 2) throw new Exception($"Invalid prefix table entry: {line}");
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

    public string FormBitString(Dictionary<string, char> prefixTable, string encodedString)
    {
        throw new NotImplementedException();
    }

    public string DecodeBitString(string bitString, Dictionary<string, char> prefixTable)
    {
        var result = new StringBuilder();
        int start = 0;
        int maxCodeLength = prefixTable.Keys.Max(k => k.Length); // Safety limit

        while (start < bitString.Length)
        {
            bool matchFound = false;
            int length = 1;

            while (length <= maxCodeLength && (start + length) <= bitString.Length)
            {
                var slice = bitString.Substring(start, length);
                if (prefixTable.TryGetValue(slice, out char ch))
                {
                    result.Append(ch);
                    start += length;
                    matchFound = true;
                    break;
                }

                length++;
            }

            if (!matchFound)
            {
                Console.WriteLine($"[ERROR] No match found for bitstream starting at index {start}. Aborting.");
                break;
            }
        }

        Console.WriteLine($"[INFO] Total decoded characters: {result.Length}");

        return result.ToString();
    }


}