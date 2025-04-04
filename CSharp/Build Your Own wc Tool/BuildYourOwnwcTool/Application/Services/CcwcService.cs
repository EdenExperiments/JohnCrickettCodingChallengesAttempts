using CCWC.Application.Factories;
using CCWC.Application.Strategies;
using CCWC.Domain.Models;
using CCWC.Helpers;

namespace CCWC.Application.Services;

public static class CcwcService
{
    public static void ProcessDefaultFlags(string filePath)
    {
        var counts = new Counts(filePath);
        var lineStrategy = new LineCountStrategy();
        var wordStrategy = new WordCountStrategy();
        var charStrategy = new CharacterCountStrategy();

        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lineStrategy.Count(line, counts);
            wordStrategy.Count(line, counts);
            charStrategy.Count(line, counts);
        }

        Console.WriteLine($"{counts.C} {counts.L} {counts.W} {filePath}");
    }

    public static void ProcessSingle(string flag, string filePath)
    {
        var counts = new Counts(filePath);

        if (flag == "C")
        {
            Utilities.WriteResult(counts, flag, filePath);
            return;
        }

        var strategy = CountStrategyFactory.GetStrategy(flag);

        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = reader.ReadLine()) != null) strategy.Count(line, counts);

        Utilities.WriteResult(counts, flag, filePath);
    }
}