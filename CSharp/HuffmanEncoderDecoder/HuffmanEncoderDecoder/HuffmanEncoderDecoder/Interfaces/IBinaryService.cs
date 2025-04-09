using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanEncoderDecoder.Interfaces
{
    public interface IBinaryService
    {
        public IHuffmanNode BuildBinaryTree(Dictionary<char, int> frequencyMap);
        public Dictionary<char, string> BuildPrefixTable(IHuffmanNode rootNode);
    }
}
