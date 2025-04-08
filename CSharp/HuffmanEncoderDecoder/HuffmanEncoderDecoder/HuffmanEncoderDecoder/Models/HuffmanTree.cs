using HuffmanEncoderDecoder.Interfaces;

namespace HuffmanEncoderDecoder.Models;

public class HuffmanTree : IComparable
{
    private IHuffmanNode? _root;

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

    public int CompareTo(object? obj)
    {
        HuffmanTree? tree = obj as HuffmanTree;

        if (_root!.Weight() < tree!.Weight())
            return -1;
        return _root.Weight() == tree.Weight() ? 0 : 1;
    }
}