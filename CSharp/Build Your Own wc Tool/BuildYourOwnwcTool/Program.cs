using CCWC;
using CCWC.Helpers;

if (args.Length == 1 && args[0] == "--help")
{
    Utilities.WriteHelpText();
}
else if (args.Length == 1 && File.Exists(args[0]))
{
    var filePath = args[0];
    var lineCount = 0L;
    var wordCount = 0L;
    var byteCount = new FileInfo(filePath).Length;

    using (var reader = new StreamReader(filePath))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            lineCount++;
            wordCount += FileInformationRetriever.GetWordCount(line);
        }
    }

    Console.WriteLine($"{byteCount} {lineCount} {wordCount} {filePath}");
}
else if (args.Length == 1 && !File.Exists(args[0]))
{
    Console.WriteLine("When using 1 argument, only --help or <filepath> is a valid use of ccwc");
}
else if (args.Length == 2 && Utilities.IsFlagValid(args[0]) && File.Exists(args[1]))
{
    var flag = args[0].TrimStart('-').ToUpper();
    var filePath = args[1];
    var counts = new Counts(filePath);

    if (flag == "C")
        Utilities.WriteResult(counts, flag, filePath);
    else
    {
        using var reader = new StreamReader(filePath);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            switch (flag)
            {
                case "L":
                    counts.AddToCount(flag, 1);
                    break;
                case "W":
                    counts.AddToCount(flag, FileInformationRetriever.GetWordCount(line));
                    break;
                case "M":
                    counts.AddToCount(flag, FileInformationRetriever.GetCharacterCount(line) + Environment.NewLine.Length);
                    break;
                default:
                    throw new ArgumentException("Invalid argument passed into ccwc");
            }
        }

        Utilities.WriteResult(counts, flag, filePath);
    }
}
else if (args.Length == 2 && !File.Exists(args[1]))
{
    Console.WriteLine($"File at {args[1]} does not exist");
}
else 
{
    Console.WriteLine("Incorrect number of arguments given, or incorrect use of flags, use ccwc --help for information");
}
