namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IEncodingService
{
    public Dictionary<char, int> BuildFrequencyMap(string fileText);
    public string EncodeTextToBitString(string fileText, Dictionary<char, string> prefixTable);

    public Dictionary<char, string> BuildPrefixTableRecursion(IHuffmanNode root);

    private void TraversePrefixTableRecursive(IHuffmanNode? node, string prefix, Dictionary<char, string> prefixTable)

    public Dictionary<char, string> BuildPrefixTableIterative(IHuffmanNode rootNode);

    public byte[] BitStringToBytes(string bitString);

    public byte[] PrefixTableToBytes(Dictionary<char, string> prefixTable);
}