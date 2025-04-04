using CCWC.Application.Interfaces;
using CCWC.Application.Strategies;

namespace CCWC.Application.Factories;

public static class CountStrategyFactory
{
    public static ICountStrategy GetStrategy(string flag)
    {
        return flag switch
        {
            "L" => new LineCountStrategy(),
            "W" => new WordCountStrategy(),
            "M" => new CharacterCountStrategy(),
            _ => throw new ArgumentException($"Unsupported flag: {flag}")
        };
    }
}