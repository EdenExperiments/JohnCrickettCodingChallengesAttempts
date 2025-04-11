using System.Text;
using HuffmanEncoderDecoder.Interfaces.Services;

namespace HuffmanEncoderDecoder.Services;

public class EncodingService : IEncodingService
{
    public Dictionary<char, int> BuildFrequencyMap(string fileText)
    {
        if (fileText is null)
            throw new ArgumentNullException($"fileText parameter passed to BuildFrequencyMap is null");

        Dictionary<char, int> frequencyMap = new();
        foreach (var character in fileText)
            if (frequencyMap.TryGetValue(character, out var value))
                frequencyMap[character] = ++value;
            else
                frequencyMap[character] = 1;

        return frequencyMap;
    }

    public string EncodeTextToBitString(string fileText, Dictionary<char, string> prefixTable)
    {
        var sb = new StringBuilder();

        foreach (char c in fileText)
        {
            if (!prefixTable.TryGetValue(c, out var code))
            {
                throw new Exception($"Character {c} not found in prefix table. Error in Encoding or Binary Service");
            }

            sb.Append(code);
        }

        return sb.ToString();
    }
}