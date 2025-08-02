using System.CommandLine;

var inputFile = new Argument<string>("input file");
var unique = new Option<bool>("--unique", "-u")
{
    Description = "return unique words"
};

var rootCommand = new RootCommand("Sort Lines from a file.")
{
    inputFile,
    unique
};

rootCommand.SetAction((parseResult, token) =>
{
    var inputPath = parseResult.GetRequiredValue(inputFile);
    var isUnique = parseResult.GetValue(unique);

    return SortAndPrintAsync(inputPath, isUnique, token);
});


static async Task SortAndPrintAsync(string? inputFile, bool unique, CancellationToken token)
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
    var sorted = unique
        ? lines.Select(x => x.ToUpper()).Distinct().OrderBy(x => x)
        : lines.Select(x => x.ToUpper()).OrderBy(x => x);

    foreach (var line in sorted)
        Console.WriteLine(line);
}

return await new CommandLineConfiguration(rootCommand).InvokeAsync(args);