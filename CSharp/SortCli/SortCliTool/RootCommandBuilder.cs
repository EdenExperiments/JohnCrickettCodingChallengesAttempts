using System.CommandLine;

namespace SortCliTool;

public class RootCommandBuilder
{
    public static RootCommand Build()
    {
        var inputFile = new Argument<string>("input file");
        var unique = new Option<bool>("--unique", "-u")
        {
            Description = "return unique words"
        };

        var algorithm = new Option<string>("--algorithm", "-a")
        {
            Description = "algorithm to use",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = parseResult => "quicksort"  
        };

        algorithm.AcceptOnlyFromAmong("quicksort", "radix", "mergesort", "random");


        var rootCommand = new RootCommand("Sort Lines from a file.")
        {
            inputFile,
            unique,
            algorithm
        };

        rootCommand.SetAction((parseResult, token) =>
        {
            var inputPath = parseResult.GetRequiredValue(inputFile);
            var isUnique = parseResult.GetValue(unique);
            var algorithmName = parseResult.GetValue(algorithm);

            return Helper.SortAndPrintAsync(inputPath, isUnique, algorithmName, token);
        });
        
        return rootCommand;
    }
}