using HuffmanEncoderDecoder.Interfaces.Handlers;
using HuffmanEncoderDecoder.Interfaces.Services;
using HuffmanEncoderDecoder.IO;

namespace HuffmanEncoderDecoder.Handlers;

internal class EncodingHandler(IEncodingService encodingService, IBinaryService binaryService) : IEncodingHandler
{
    public void Encode(string inputFilePath, string outputFilePath, bool recursiveOpt)
    {
        if (!Utils.Utils.DoesFileExist(inputFilePath)) throw new ArgumentException("Input file doesn't exist.");
        if (Utils.Utils.DoesFileExist(outputFilePath)) throw new ArgumentException("Output file already exists.");

        var readerWriter = new FileReaderWriter(inputFilePath, outputFilePath);
        var fileText = readerWriter.ReadFile();

        var frequencyMap = encodingService.BuildFrequencyMap(fileText);
        var binaryTree = binaryService.BuildBinaryTree(frequencyMap);

        var prefixTable = recursiveOpt
            ? encodingService.BuildPrefixTableRecursion(binaryTree)
            : encodingService.BuildPrefixTableIterative(binaryTree);

        var bitString = encodingService.EncodeTextToBitString(fileText, prefixTable);
        var encodedText = encodingService.BitStringToBytes(bitString);
        var encodedHeader = encodingService.PrefixTableToBytes(prefixTable);

        readerWriter.WriteEncodedFile(encodedHeader, encodedText, bitString.Length);
    }
}