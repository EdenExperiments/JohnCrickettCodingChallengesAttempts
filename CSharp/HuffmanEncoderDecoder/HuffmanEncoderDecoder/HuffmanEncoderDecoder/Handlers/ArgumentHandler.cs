using HuffmanEncoderDecoder.Services;
using HuffmanEncoderDecoder.Utils;

namespace HuffmanEncoderDecoder.Handlers;

internal class ArgumentHandler(EncodingService encodingService)
{
    internal void HandleArguments(string[] args)
    {
        if (Util.CheckValidFile(args[0]))
        {
            var fileText = File.ReadAllText(args[0]);
            var frequencyMap = encodingService.BuildFrequencyMap(fileText);

            foreach (var variable in frequencyMap)
            {
                Console.WriteLine($"{variable.Key}:{variable.Value}");
            }

        }

    }
}