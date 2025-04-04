using CCWC.Domain.Models;

namespace CCWC.Application.Interfaces;

public interface ICountStrategy
{
    public void Count(string line, Counts counts);
}