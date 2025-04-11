using HuffmanEncoderDecoder.Interfaces.Handlers;
using HuffmanEncoderDecoder.Interfaces.Services;
using HuffmanEncoderDecoder.IO;
using HuffmanEncoderDecoder.Utils;

namespace HuffmanEncoderDecoder.Handlers;

internal class EncodingHandler(IEncodingService encodingService, IBinaryService binaryService) : IEncodingHandler
{
    public void Encode(string inputFilePath, string outputFilePath)
    {
        if (!Utils.Utils.DoesFileExist(inputFilePath))
        {
            throw new ArgumentException("Input file doesn't exist.");
        }
        if (Utils.Utils.DoesFileExist(outputFilePath))
        {
            throw new ArgumentException("Output file already exists.");
        }
        
        var fileText = File.ReadAllText(inputFilePath);
        var frequencyMap = encodingService.BuildFrequencyMap(fileText);
        var binaryTree = binaryService.BuildBinaryTree(frequencyMap);
        var prefixTable = binaryService.BuildPrefixTable(binaryTree);
        var bitString = encodingService.EncodeTextToBitString(fileText, prefixTable);
        var bitLength = bitString.Length;
        var encodedText = binaryService.BitStringToBytes(bitString);
        var encodedHeader = binaryService.PrefixTableToBytes(prefixTable);
        var writer = new FileReaderWriter(inputFilePath, outputFilePath);
        writer.WriteEncodedFile(encodedHeader, encodedText, bitLength);
    }
}