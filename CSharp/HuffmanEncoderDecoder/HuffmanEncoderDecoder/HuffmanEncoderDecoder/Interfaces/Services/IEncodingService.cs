namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IEncodingService
{
    public Dictionary<char, int> BuildFrequencyMap(string fileText);
    public string EncodeTextToBitString(string fileText, Dictionary<char, string> prefixTable);
}