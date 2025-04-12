namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IDecodingService
{
    public Dictionary<string, char> ParsePrefixTable(string header);
    public string DecodeBitString(string bitString, Dictionary<string, char> prefixTable);

    public string BytesToBitString(byte[] bytes, int bitLength);
}