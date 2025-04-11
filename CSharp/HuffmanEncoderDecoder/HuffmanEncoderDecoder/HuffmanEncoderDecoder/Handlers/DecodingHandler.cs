using HuffmanEncoderDecoder.Interfaces.Handlers;
using HuffmanEncoderDecoder.Interfaces.Services;
using HuffmanEncoderDecoder.IO;

namespace HuffmanEncoderDecoder.Handlers;

internal class DecodingHandler(IDecodingService decodingService, IBinaryService binaryService) : IDecodingHandler
{
    public void DecodeFile(string inputFilePath, string outputFilePath)
    {
        if (!Utils.Utils.DoesFileExist(inputFilePath))
        {
            throw new ArgumentException("Input file doesn't exist.");
        }
        if (Utils.Utils.DoesFileExist(outputFilePath))
        {
            throw new ArgumentException("Output file already exists.");
        }

        var writerReader = new FileReaderWriter(inputFilePath, outputFilePath);
        var (encodedHeader, headerLength, encodedText, textLength) = writerReader.ReadEncodedFile();
        var prefixTable = decodingService.ParsePrefixTable(encodedHeader);
        var bitString = binaryService.BytesToBitString(encodedText, textLength);
        var decodedText = decodingService.DecodeBitString(bitString, prefixTable);

        File.WriteAllText(outputFilePath, decodedText);

    }
}