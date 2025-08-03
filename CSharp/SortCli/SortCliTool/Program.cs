using System.CommandLine;
using SortCliTool;

var rootCommand = RootCommandBuilder.Build();
return await new CommandLineConfiguration(rootCommand).InvokeAsync(args);

