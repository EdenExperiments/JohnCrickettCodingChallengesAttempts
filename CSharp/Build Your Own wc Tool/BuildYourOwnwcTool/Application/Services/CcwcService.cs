using CCWC.Application.Factories;
using CCWC.Domain.Models;
using CCWC.Helpers;

namespace CCWC.Application.Services;

public static class CcwcService
{
    public static void ProcessDefaultFlags(string filePath)
    {
        var count = new Count(filePath);
        var lineStrategy = CountStrategyFactory.GetStrategy("L");
        var wordStrategy = CountStrategyFactory.GetStrategy("W");
        var charStrategy = CountStrategyFactory.GetStrategy("M");

        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lineStrategy.Count(line, count);
            wordStrategy.Count(line, count);
            charStrategy.Count(line, count);
        }

        Console.WriteLine($"{count.C} {count.L} {count.W} {filePath}");
    }

    public static void ProcessSingle(string flag, string filePath)
    {
        var counts = new Count(filePath);

        if (flag.ToUpper() == "C")
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