using HuffmanEncoderDecoder.Handlers;
using HuffmanEncoderDecoder.Interfaces.Handlers;
using HuffmanEncoderDecoder.Interfaces.Services;
using HuffmanEncoderDecoder.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HuffmanEncoderDecoder;

public static class ServiceConfiguration
{
    public static void ConfigureAll(this IServiceCollection services)
    {
        //may split up later into seperate methods
        services.AddSingleton<IEncodingHandler, EncodingHandler>();
        services.AddSingleton<IDecodingHandler, DecodingHandler>();
        services.AddSingleton<IEncodingService, EncodingService>();
        services.AddSingleton<IDecodingService, DecodingService>();
        services.AddSingleton<IBinaryService, BinaryService>();
        services.AddSingleton<EncodingHandler>();
        services.AddSingleton<DecodingHandler>();
        services.AddSingleton<ArgumentHandler>();
    }
}