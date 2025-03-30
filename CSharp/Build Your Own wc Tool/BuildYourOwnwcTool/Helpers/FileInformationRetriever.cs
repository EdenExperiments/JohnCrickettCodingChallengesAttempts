namespace CCWC.Helpers;

internal static class FileInformationRetriever
{
    internal static int GetWordCount(string fileText)
    {
        var wordsInFile = fileText.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);
        return wordsInFile.Length;
    }

    internal static int GetByteCount(byte[] fileBytes)
    {
        return fileBytes.Length;
    }

    internal static int GetLineCount(string fileText)
    {
        var lines = fileText.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        lines = lines.Where(l => l.Length > 0).ToArray();
        return lines.Length;
    }

    internal static int GetCharacterCount(string fileText)
    {
        return fileText.Length;
    }
}