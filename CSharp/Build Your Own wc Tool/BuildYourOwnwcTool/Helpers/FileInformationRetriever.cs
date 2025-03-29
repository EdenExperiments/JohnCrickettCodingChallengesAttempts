namespace BuildYourOwnwcTool.Helpers;

internal static class FileInformationRetriever
{
    internal static int GetWordCount(string filePath)
    {
        var file = File.ReadAllText(filePath);
        var wordsInFile = file.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        return wordsInFile.Length;
    }

    internal static int GetByteCount(string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        return fileBytes.Length;
    }

    internal static int GetLineCount(string filePath)
    {
        var numberOfLines = File.ReadAllLines(filePath);
        return numberOfLines.Length;
    }

    internal static int GetCharacterCount(string filePath)
    {
        var numberOfChar = File.ReadAllText(filePath);
        return numberOfChar.Length;
    }
}