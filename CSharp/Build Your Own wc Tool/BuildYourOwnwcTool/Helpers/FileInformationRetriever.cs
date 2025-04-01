namespace CCWC.Helpers;

internal static class FileInformationRetriever
{
    internal static long GetWordCount(string fileText)
    {
        var wordsInFile = fileText.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        return wordsInFile.Length;
    }

    internal static long GetCharacterCount(string fileText)
    {
        return fileText.Length;
    }
}