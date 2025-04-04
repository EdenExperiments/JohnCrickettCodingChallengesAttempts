using CCWC.Application.Interfaces;
using CCWC.Domain.Models;

namespace CCWC.Application.Strategies;

public class CharacterCountStrategy : ICountStrategy
{
    public void Count(string line, Count counts)
    {
        counts.M += line.Length;
    }
}