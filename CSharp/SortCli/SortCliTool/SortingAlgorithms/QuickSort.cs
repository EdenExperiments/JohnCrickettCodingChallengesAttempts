namespace SortCliTool.SortingAlgorithms;

public class QuickSort : ISort
{
    public List<string>? Sort(List<string>? list)
    {
        if (list is not { Count: > 1 })
            return list;
        
        var stack = new Stack<(int left, int right)>();
        stack.Push((0, list.Count - 1));
        
        while (stack.Count > 0)
        {
            var (left, right) = stack.Pop();

            if (left >= right) continue;
            
            var pivot = Partition(list, left, right);

            if (pivot - 1 - left > right - (pivot - 1))
            {
                stack.Push((left, pivot - 1));
                stack.Push((pivot + 1, right));
            }
            else
            {
                stack.Push((pivot + 1, right));
                stack.Push((left, pivot - 1));
            }
        }

        return list;
    }

    private static int Partition(List<string> list, int left, int right)
    {
        var pivot = list[right];
        var i = left - 1;

        for (var j = left; j < right; j++)
        {
            if (string.CompareOrdinal(list[j], pivot) > 0) continue;
            i++;
            (list[i], list[j]) = (list[j], list[i]);
        }
        
        (list[i + 1], list[right]) = (list[right], list[i + 1]);
        return i + 1;
    }
        
}