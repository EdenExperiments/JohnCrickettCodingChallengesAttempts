using CCWC.Application.Interfaces;
using CCWC.Domain.Models;

namespace CCWC.Application.Strategies;

public class WordCountStrategy : ICountStrategy
{
    public void Count(string line, Count counts)
    {
        var wordsInFile = line.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        counts.W += wordsInFile.Length;
    }
}