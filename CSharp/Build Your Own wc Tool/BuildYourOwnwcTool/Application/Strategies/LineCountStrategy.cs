using CCWC.Application.Interfaces;
using CCWC.Domain.Models;

namespace CCWC.Application.Strategies;

public class LineCountStrategy : ICountStrategy
{
    public void Count(string line, Counts counts)
    {
        counts.L += 1;
    }
}