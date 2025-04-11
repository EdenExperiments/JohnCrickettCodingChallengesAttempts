using System.Text;

namespace HuffmanEncoderDecoder.IO;

internal class FileReaderWriter(string inputPath, string outputPath)
{
    public void WriteEncodedFile(byte[] encodedHeader, byte[] encodedText, int bitLength)
    {
        using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(stream);

        writer.Write(encodedHeader.Length);
        writer.Write(encodedHeader);
        writer.Write(bitLength);
        writer.Write(encodedText);
    }

    public string ReadFile()
    {
        using var stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public (string, int, byte[], int) ReadEncodedFile()
    {
        using var stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);

        var headerLength = reader.ReadInt32();
        var header = reader.ReadBytes(headerLength);
        var headerText = Encoding.UTF8.GetString(header);

        var bitLength = reader.ReadInt32();
        var encodedText = reader.ReadBytes((bitLength + 7)/8);

        return (headerText, headerLength, encodedText, bitLength);
    }
} 