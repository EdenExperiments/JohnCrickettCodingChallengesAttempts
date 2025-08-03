namespace SortCliTool.SortingAlgorithms;

public class Radix : ISort
{
    public List<string>? Sort(List<string>? list)
    {
        if (list is not { Count: > 1 })
        {
            return list;
        }
        
        var maxLength = list.Max(x => x.Length);
        var workingList = list.
            Select(x => x.PadRight(maxLength, '\0')).
            ToList();

        for (var pos = maxLength - 1; pos >= 0; pos--)
        {
            workingList = CountingSortByChar(workingList, pos);
        }
        
        return workingList.Select(x => x.TrimEnd('\0')).ToList();
    }

    private List<string> CountingSortByChar(List<string> workingList, int pos)
    {
        const int range = char.MaxValue + 1;
        var count = new int[range];
        var output = new string[workingList.Count];

        foreach (var s in workingList)
        {
            var c = s[pos];
            count[c]++;
        }

        for (var i = 1; i < range; i++)
        {
            count[i] += count[i - 1];
        }

        for (var i = workingList.Count - 1; i >= 0; i--)
        {
            var c = workingList[i][pos];
            output[--count[c]] = workingList[i];
        }
        
        return output.ToList();
    }
}