using HuffmanEncoderDecoder.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuffmanEncoderDecoder.Interfaces.Services;

namespace HuffmanEncoderDecoder.UnitTests
{
    public static class TestStartup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IEncodingService, EncodingService>();
            services.AddSingleton<IBinaryService, BinaryService>();

            return services.BuildServiceProvider();
        }
    }
}
