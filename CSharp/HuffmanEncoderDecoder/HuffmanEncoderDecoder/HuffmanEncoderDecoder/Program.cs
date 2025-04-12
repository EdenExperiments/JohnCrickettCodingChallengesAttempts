using System.CommandLine;
using HuffmanEncoderDecoder;
using HuffmanEncoderDecoder.Handlers;
using Microsoft.Extensions.DependencyInjection;

try
{
    var services = new ServiceCollection();
    services.ConfigureAll();
    var serviceProvider = services.BuildServiceProvider();

    var commandLineHandler = serviceProvider.GetRequiredService<CommandLineHandler>();
    await commandLineHandler.BuildRootCommand().InvokeAsync(args);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Unknown Error Occurred: {0}", ex);
}