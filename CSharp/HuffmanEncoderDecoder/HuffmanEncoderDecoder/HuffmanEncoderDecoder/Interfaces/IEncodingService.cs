using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanEncoderDecoder.Interfaces
{
    public interface IEncodingService
    {
        public Dictionary<char, int> BuildFrequencyMap(string fileText);
    }
}
