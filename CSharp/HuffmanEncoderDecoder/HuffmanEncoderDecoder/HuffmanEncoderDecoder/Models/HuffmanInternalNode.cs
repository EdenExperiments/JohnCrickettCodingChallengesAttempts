using HuffmanEncoderDecoder.Interfaces;

namespace HuffmanEncoderDecoder.Models;

public class HuffmanInternalNode(IHuffmanNode left, IHuffmanNode right) : IHuffmanNode
{
    private readonly int _weight = left.Weight() + right.Weight();

    public int Weight()
    {
        return _weight;
    }

    public bool IsLeaf()
    {
        return false;
    }

    public char? Value()
    {
        return null;
    }

    public IHuffmanNode? Left => left;
    public IHuffmanNode? Right => right;
}