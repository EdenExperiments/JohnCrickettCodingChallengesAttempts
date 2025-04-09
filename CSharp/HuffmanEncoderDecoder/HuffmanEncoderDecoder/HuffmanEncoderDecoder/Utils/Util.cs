namespace HuffmanEncoderDecoder.Utils;

internal class Util
{
    internal static bool CheckFileExists(string filepath)
    {
        return File.Exists(filepath);
    }
}