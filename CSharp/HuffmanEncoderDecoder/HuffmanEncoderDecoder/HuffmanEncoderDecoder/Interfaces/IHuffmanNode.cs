namespace HuffmanEncoderDecoder.Interfaces;

public interface IHuffmanNode
{
    bool IsLeaf();

    int Weight();

    IHuffmanNode? Left { get; }
    IHuffmanNode? Right { get; }
    char? Value();
}