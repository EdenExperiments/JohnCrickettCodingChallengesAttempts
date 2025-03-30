using CCWC.Helpers;

if (args.Length == 1 && args[0] == "--help")
{
    Utilities.PrintHelp();
}
else if (args.Length == 1 && File.Exists(args[0]))
{
    //Would be best if i could just read the file contents once and pass it through each one
    //rather than the file path
    var filePath = args[0];
    var numberOfBytes = FileInformationRetriever.GetByteCount(filePath);
    var numberOfLines = FileInformationRetriever.GetLineCount(filePath);
    var numberOfWords = FileInformationRetriever.GetWordCount(filePath);

    Console.WriteLine($"{numberOfBytes} {numberOfLines} {numberOfWords} {filePath}");
}
else if (args.Length == 2 && File.Exists(args[1]))
{
    var filePath = args[1];
    switch (args[0])
    {
        case "-c":
            var numberOfBytes = FileInformationRetriever.GetByteCount(filePath);
            Console.WriteLine($"{numberOfBytes} {filePath}");
            break;
        case "-l":
            var numberOfLines = FileInformationRetriever.GetLineCount(filePath);
            Console.WriteLine($"{numberOfLines} {filePath}");
            break;
        case "-w":
            var numberOfWords = FileInformationRetriever.GetWordCount(filePath);
            Console.WriteLine($"{numberOfWords} {filePath}");
            break;
        case "-m":
            var numberOfChars = FileInformationRetriever.GetCharacterCount(filePath);
            Console.WriteLine($"{numberOfChars} {filePath}");
            break;
        default:
            throw new ArgumentException("Invalid argument passed into ccwc");
    }
}
else
{
    Console.WriteLine("Incorrect number of arguments given, use ccwc --help for information");
}