namespace SortCliTool.SortingAlgorithms;

public class Merge : ISort
{
    public Merge()
    {
    }

    public List<string> Sort(List<string>? list)
    {
        if (list is { Count: <= 1 })
        {
            return list;
        }
        
        var left = list?.GetRange(0, list.Count / 2);
        var right = list?.GetRange(list.Count / 2, list.Count - list.Count / 2);

        left = Sort(left);
        right = Sort(right);

        return MergeLists(left, right);
    }

    private static List<string> MergeLists(List<string> left, List<string> right)
    {
        var result = new List<string>();
        int leftIndex = 0, rightIndex = 0;

        while (leftIndex < left.Count && rightIndex < right.Count)
        {
            if (string.CompareOrdinal(left[leftIndex], right[rightIndex]) <= 0)
            {
                result.Add(left[leftIndex]);
                leftIndex++;
            }
            else
            {
                result.Add(right[rightIndex]);
                rightIndex++;
            }
        }

        while (leftIndex < left.Count)
        {
            result.Add(left[leftIndex]);
            leftIndex++;
        }

        while (rightIndex < right.Count)
        {
            result.Add(right[rightIndex]);
            rightIndex++;
        }

        return result;
    }
}