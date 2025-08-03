using SortCliTool.SortingAlgorithms;

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
            _ => throw new Exception("Algorithm not found")
        };
    }
}