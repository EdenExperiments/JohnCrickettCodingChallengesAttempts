namespace HuffmanEncoderDecoder.Interfaces.Services;

public interface IBinaryService
{
    public IHuffmanNode BuildBinaryTree(Dictionary<char, int> frequencyMap);
}