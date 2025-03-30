using CCWC.Helpers;

if (args.Length == 1 && args[0] == "--help")
{
    Utilities.WriteHelpText();
}
else if (args.Length == 1 && File.Exists(args[0]))
{
    var filePath = args[0];
    var fileBytes = File.ReadAllBytes(filePath);
    var fileText = System.Text.Encoding.UTF8.GetString(fileBytes);
    var numberOfBytes = FileInformationRetriever.GetByteCount(fileBytes);
    var numberOfLines = FileInformationRetriever.GetLineCount(fileText);
    var numberOfWords = FileInformationRetriever.GetWordCount(fileText);

    Console.WriteLine($"{numberOfBytes} {numberOfLines} {numberOfWords} {filePath}");
}
else if (args.Length == 2 && File.Exists(args[1]))
{
    var filePath = args[1];
    var fileBytes = File.ReadAllBytes(filePath);
    var fileText = System.Text.Encoding.UTF8.GetString(fileBytes);

    switch (args[0])
    {
        case "-c":
            Console.WriteLine($"{FileInformationRetriever.GetByteCount(fileBytes)} {filePath}");
            break;
        case "-l":
            Console.WriteLine($"{FileInformationRetriever.GetLineCount(fileText)} {filePath}");
            break;
        case "-w":
            Console.WriteLine($"{FileInformationRetriever.GetWordCount(fileText)} {filePath}");
            break;
        case "-m":
            Console.WriteLine($"{FileInformationRetriever.GetCharacterCount(fileText)} {filePath}");
            break;
        default:
            throw new ArgumentException("Invalid argument passed into ccwc");
    }
}
else
{
    Console.WriteLine("Incorrect number of arguments given, use ccwc --help for information");
}
