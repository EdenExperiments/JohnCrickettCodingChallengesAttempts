using HuffmanEncoderDecoder.Handlers;
using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.Services;
using Microsoft.Extensions.DependencyInjection;

try
{
    var services = new ServiceCollection();

    services.AddSingleton<IEncodingService, EncodingService>();
    services.AddSingleton<IBinaryService, BinaryService>();
    services.AddSingleton<ArgumentHandler>();

    var serviceProvider = services.BuildServiceProvider();
    var argumentHandler = serviceProvider.GetRequiredService<ArgumentHandler>();

    argumentHandler?.HandleArguments(args);
}
catch (Exception ex)
{
    Console.WriteLine("Unknown Error Occured: {0}", ex);
}