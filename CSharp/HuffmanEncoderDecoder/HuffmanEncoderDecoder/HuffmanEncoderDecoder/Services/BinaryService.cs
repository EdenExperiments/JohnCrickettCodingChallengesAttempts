using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.Models;

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
}