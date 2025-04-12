using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.Interfaces.Services;
using HuffmanEncoderDecoder.Models;
using System.Text;

namespace HuffmanEncoderDecoder.Services;

public class BinaryService : IBinaryService
{
    public IHuffmanNode BuildBinaryTree(Dictionary<char, int> frequencyMap)
    {
        var heap = new PriorityQueue<HuffmanTree, int>();

        foreach (var (ch, freq) in frequencyMap)
            heap.Enqueue(new HuffmanTree(ch, freq), freq);

        while (heap.Count > 1)
        {
            var tree1 = heap.Dequeue();
            var tree2 = heap.Dequeue();
            var merged = new HuffmanTree(tree1.Root(), tree2.Root());

            heap.Enqueue(merged, merged.Weight());
        }

        return heap.Dequeue().Root()!;
    }

    public Dictionary<char, string> BuildPrefixTable(IHuffmanNode root)
    {
        var prefixTable = new Dictionary<char, string>();
        TraversePrefixTableRecursive(root, "", prefixTable);
        return prefixTable;
    }

    private void TraversePrefixTableRecursive(IHuffmanNode? node, string prefix, Dictionary<char, string> prefixTable)
    {
        if (node == null) return;

        // If it's a leaf node
        if (node.Value() is { } c)
        {
            prefixTable[c] = prefix;
            return;
        }

        TraversePrefixTableRecursive(node.Left, prefix + "0", prefixTable);
        TraversePrefixTableRecursive(node.Right, prefix + "1", prefixTable);
    }

    public byte[] BitStringToBytes(string bitString)
    {
        var numBytes = (bitString.Length + 7) / 8;
        var bytes = new byte[numBytes];

        for (var i = 0; i < bitString.Length; i++)
            if (bitString[i] == '1')
            {
                var byteIndex = i / 8;
                var bitIndex = 7 - i % 8;
                bytes[byteIndex] |= (byte)(1 << bitIndex);
            }

        return bytes;
    }

    public string BytesToBitString(byte[] bytes, int bitLength)
    {
        var sb = new StringBuilder(bitLength);

        for (int i = 0; i < bitLength; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = 7 - (i % 8); // MSB first

            bool bit = (bytes[byteIndex] & (1 << bitIndex)) != 0;
            sb.Append(bit ? '1' : '0');
        }

        return sb.ToString();
    }


    public byte[] PrefixTableToBytes(Dictionary<char, string> prefixTable)
    {
        var headerBuilder = new StringBuilder();
        char separator = '\u001E'; //todo: I need to make this whole seperator system better, although this is an extremely uncommon character, it will still break if within a document

        foreach (var kvp in prefixTable)
        {
            var escapedChar = kvp.Key switch
            {
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                ' ' => "␣",
                '\0' => "\\0",
                _ => kvp.Key.ToString()
            };
            headerBuilder.AppendLine($"{escapedChar}{separator}{kvp.Value}");
        }

        return Encoding.UTF8.GetBytes(headerBuilder.ToString());
    }
}