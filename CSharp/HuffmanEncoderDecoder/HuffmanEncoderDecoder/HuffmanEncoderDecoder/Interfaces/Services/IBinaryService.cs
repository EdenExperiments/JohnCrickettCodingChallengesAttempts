namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IBinaryService
{
    public IHuffmanNode BuildBinaryTree(Dictionary<char, int> frequencyMap);
    public Dictionary<char, string> BuildPrefixTable(IHuffmanNode rootNode);

    public byte[] BitStringToBytes(string bitString);

    public string BytesToBitString(byte[] bytes, int bitLength);

    public byte[] PrefixTableToBytes(Dictionary<char, string> prefixTable);
}