using HuffmanEncoderDecoder.Interfaces;

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
}