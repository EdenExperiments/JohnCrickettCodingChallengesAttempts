namespace HuffmanEncoderDecoder.IO;

internal class PrefixTableWriterReader(string filePath)
{
    public void WritePrefixTableToFile(Dictionary<char, string> prefixTable)
    {
        using var writer = new StreamWriter(filePath);
        foreach (var keyValuePair in prefixTable)
        {
            writer.Write(keyValuePair.Key);
            writer.Write(keyValuePair.Value);
        }
    }

    public void WriteEncodedTextWithlength(string bitString)
    {
        var numBytes = (bitString.Length + 7) / 8;
        var bytes = new byte[numBytes];

        for (var i = 0; i < bitString.Length; i++)
            if (bitString[i] == '1')
            {
                var byteIndex = i / 8;
                var bitIndex = 7 - i % 8; 
                bytes[byteIndex] |= (byte)(1 << bitIndex);
            }

        File.AppendAllText(filePath, bytes);
    }
}