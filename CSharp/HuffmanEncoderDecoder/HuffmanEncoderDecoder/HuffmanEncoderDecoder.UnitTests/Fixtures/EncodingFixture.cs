using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanEncoderDecoder.UnitTests.Fixtures
{
    public class EncodingFixture
    {
        public IServiceProvider ServiceProvider { get; } = TestStartup.ConfigureServices();
    }

}
