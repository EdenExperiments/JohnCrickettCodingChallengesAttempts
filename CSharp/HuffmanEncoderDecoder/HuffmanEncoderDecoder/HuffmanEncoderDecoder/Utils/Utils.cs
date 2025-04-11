namespace HuffmanEncoderDecoder.Utils;

internal class Utils
{
    internal static bool DoesFileExist(string filepath)
    {
        return File.Exists(filepath);
    }
}