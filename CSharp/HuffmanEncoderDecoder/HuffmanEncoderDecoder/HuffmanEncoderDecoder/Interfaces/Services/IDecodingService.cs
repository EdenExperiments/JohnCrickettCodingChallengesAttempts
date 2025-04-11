namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IDecodingService
{
    public Dictionary<string, char> ParsePrefixTable(string header);

    public string FormBitString(Dictionary<string, char> prefixTable, string encodedString);

    public string DecodeBitString(string bitString, Dictionary<string, char> prefixTable);
}