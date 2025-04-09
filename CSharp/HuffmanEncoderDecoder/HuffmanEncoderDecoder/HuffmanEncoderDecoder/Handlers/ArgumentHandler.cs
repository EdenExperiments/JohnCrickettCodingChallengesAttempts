using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.IO;
using HuffmanEncoderDecoder.Services;
using HuffmanEncoderDecoder.Utils;

namespace HuffmanEncoderDecoder.Handlers;

internal class ArgumentHandler(IEncodingService encodingService, IBinaryService binaryService)
{
    internal void HandleArguments(string[] args)
    {
        if (Util.CheckFileExists(args[0]) && !Util.CheckFileExists(args[1]))
        {
            var fileText = File.ReadAllText(args[0]);
            var frequencyMap = encodingService.BuildFrequencyMap(fileText);
            var binaryTree = binaryService.BuildBinaryTree(frequencyMap);
            var prefixTable = binaryService.BuildPrefixTable(binaryTree);
            var writer = new PrefixTableWriterReader(args[1]);
            writer.WritePrefixTableToFile(prefixTable);
            var bitString = encodingService.EncodeTextToBitString(fileText, prefixTable);
            writer.WriteEncodedTextWithlength(bitString);
        }

    }
}