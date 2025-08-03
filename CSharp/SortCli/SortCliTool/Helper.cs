namespace SortCliTool;

public class Helper
{
    public static List<string> RemoveDuplicates(List<string> list)
    {
        if (list.Count <= 1)
        {
            return list;
        }
        
        var sortedList = new List<string> { list[0] };

        for (var i = 1; i < list.Count; i++)
        {
            if (list[i] != list[i - 1])
            {
                sortedList.Add(list[i]);
            }
        }
        
        return sortedList;
    }
    
    public static async Task SortAndPrintAsync(string? inputFile, bool unique, string? algorithmName, CancellationToken token)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        var fullPath = Path.Combine(projectRoot, inputFile ?? "");

        if (!File.Exists(fullPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"File not found: {fullPath}");
            return;
        }
    
        var lines = await File.ReadAllLinesAsync(fullPath, token);
        var linesList = lines.ToList();
        var sortingAlgorithm = AlgorithmFactory.ReturnAlgorithmInstance(algorithmName);
    
        var sortedLines = sortingAlgorithm.Sort(linesList);

        if (sortedLines != null)
        {
            if (unique)
                sortedLines = Helper.RemoveDuplicates(sortedLines);
        
            foreach (var line in sortedLines)
                Console.WriteLine(line);   
        }
    }
}