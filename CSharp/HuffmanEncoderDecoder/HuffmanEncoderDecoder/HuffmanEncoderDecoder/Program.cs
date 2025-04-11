using System.CommandLine;
using HuffmanEncoderDecoder;
using HuffmanEncoderDecoder.Handlers;
using Microsoft.Extensions.DependencyInjection;

try
{
    var services = new ServiceCollection();
    services.ConfigureAll();
    var serviceProvider = services.BuildServiceProvider();

    var argumentHandler = serviceProvider.GetRequiredService<ArgumentHandler>();

    await argumentHandler.BuildRootCommand().InvokeAsync(args);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Unknown Error Occured: {0}", ex);
}