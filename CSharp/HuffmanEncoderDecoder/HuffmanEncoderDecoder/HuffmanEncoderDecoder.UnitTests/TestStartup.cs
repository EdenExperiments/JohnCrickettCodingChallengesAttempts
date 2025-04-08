using HuffmanEncoderDecoder.Services;
using HuffmanEncoderDecoder.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanEncoderDecoder.UnitTests
{
    public static class TestStartup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IEncodingService, EncodingService>();

            return services.BuildServiceProvider();
        }
    }
}
