using HuffmanEncoderDecoder.Interfaces;

namespace HuffmanEncoderDecoder.Models;

public class HuffmanTree
{
    private readonly IHuffmanNode? _root;

    public HuffmanTree(char element, int weight)
    {
        _root = new HuffmanLeafNode(element, weight);
    }

    public HuffmanTree(IHuffmanNode left, IHuffmanNode right)
    {
        _root = new HuffmanInternalNode(left, right);
    }

    public IHuffmanNode? Root()
    {
        return _root;
    }

    public int Weight()
    {
        return _root!.Weight();
    }
}