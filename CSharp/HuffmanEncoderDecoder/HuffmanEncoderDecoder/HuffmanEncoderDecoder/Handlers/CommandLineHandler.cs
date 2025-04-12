using System.CommandLine;
using HuffmanEncoderDecoder.Interfaces.Handlers;

namespace HuffmanEncoderDecoder.Handlers;

internal class CommandLineHandler(IEncodingHandler encodingHandler, IDecodingHandler decodingHandler)
{
    public RootCommand BuildRootCommand()
    {
        var inputArg = new Argument<string>("input", "Input File Location");
        var outputArg = new Argument<string>("output", "Output File Location");
        var recursiveOpt =
            new Option<bool>("--recursive",
                "Use recursive traversal during encoding"); //included just because i began with a recursive method and later added iterative.

        var encodeCommand = new Command("encode",
            "Compress a file at the input location and place the encoded version at the output location")
        {
            inputArg,
            outputArg,
            recursiveOpt
        };

        encodeCommand.SetHandler(encodingHandler.Encode, inputArg, outputArg, recursiveOpt);

        var decodeCommand = new Command("decode",
            "Decompress a file at the input location and place the decoded version at the output location")
        {
            inputArg,
            outputArg
        };

        decodeCommand.SetHandler(decodingHandler.DecodeFile, inputArg, outputArg);

        var root = new RootCommand("Huffman Encoder/Decoder CLI Tool");
        root.AddCommand(encodeCommand);
        root.AddCommand(decodeCommand);
        return root;
    }
}