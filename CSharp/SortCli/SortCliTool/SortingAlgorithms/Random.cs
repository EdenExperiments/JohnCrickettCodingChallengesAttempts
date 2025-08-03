using System.Collections;
using System.Security.Cryptography;

namespace SortCliTool.SortingAlgorithms;

public class Random : ISort
{
    public List<string>? Sort(List<string>? list)
    {
        if (list is null) return null;
        var rng = RandomNumberGenerator.Create();
        
        var sortedList = list
            .Select(x =>
            {
                var buffer = new byte[4];
                rng.GetBytes(buffer);
                var hash = BitConverter.ToInt32(buffer, 0);
                return (Value: x, Key: hash);
            })
            .OrderBy(x => x.Key)
            .Select(x => x.Value)
            .ToList();
        
        return sortedList;
    }
}