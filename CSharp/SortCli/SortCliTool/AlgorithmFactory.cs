using SortCliTool.SortingAlgorithms;
using Random = SortCliTool.SortingAlgorithms.Random;

namespace SortCliTool;

public static class AlgorithmFactory
{
    public static ISort ReturnAlgorithmInstance(string? algorithm)
    {
        return algorithm?.ToLowerInvariant() switch
        {
            "quicksort" => new QuickSort(),
            "radix" => new Radix(),
            "mergesort" => new Merge(),
            "random" => new Random(),
            _ => throw new Exception("Algorithm not found")
        };
    }
}