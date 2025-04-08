using HuffmanEncoderDecoder.Interfaces;

namespace HuffmanEncoderDecoder.Models;

public class HuffmanLeafNode(char element, int weight) : IHuffmanNode
{

    public int Weight()
    {
        return weight;
    }

    public bool IsLeaf()
    {
        return true;
    }

    public char? Value()
    {
        return element;
    }

    public IHuffmanNode? Left => null;

    public IHuffmanNode? Right => null;
}