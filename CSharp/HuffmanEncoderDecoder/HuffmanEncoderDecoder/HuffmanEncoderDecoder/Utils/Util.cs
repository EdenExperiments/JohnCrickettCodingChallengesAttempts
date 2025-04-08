namespace HuffmanEncoderDecoder.Utils;

internal class Util
{
    internal static bool CheckValidFile(string filepath)
    {
        return File.Exists(filepath);
    }
}