using System.Text;
using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.Interfaces.Services;

namespace HuffmanEncoderDecoder.Services;

public class EncodingService : IEncodingService
{
    public Dictionary<char, int> BuildFrequencyMap(string fileText)
    {
        if (fileText is null)
            throw new ArgumentNullException(nameof(fileText));

        try
        {
            Dictionary<char, int> frequencyMap = new();
            foreach (var character in fileText)
                if (frequencyMap.TryGetValue(character, out var value))
                    frequencyMap[character] = ++value;
                else
                    frequencyMap[character] = 1;

            return frequencyMap;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred when building frequency map.", ex);
        }
    }

    public Dictionary<char, string> BuildPrefixTableRecursion(IHuffmanNode root)
    {
        try
        {
            var prefixTable = new Dictionary<char, string>();
            TraversePrefixTableRecursive(root, "", prefixTable);
            return prefixTable;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred when building prefix table via recursion.", ex);
        }
    }

    public Dictionary<char, string> BuildPrefixTableIterative(IHuffmanNode root)
    {
        try
        {
            var prefixTable = new Dictionary<char, string>();
            var prefixStack = new Stack<(IHuffmanNode node, string prefix)>();
            prefixStack.Push((root, ""));

            while (prefixStack.Count > 0)
            {
                var (node, prefix) = prefixStack.Pop();

                if (node.Value() is { } c)
                {
                    prefixTable[c] = prefix;
                }
                else
                {
                    prefixStack.Push((node.Right!, prefix + "1"));
                    prefixStack.Push((node.Left!, prefix + "0"));
                }
            }

            return prefixTable;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred building prefix table via iteration.", ex);
        }
    }

    public byte[] BitStringToBytes(string bitString)
    {
        try
        {
            var numBytes = (bitString.Length + 7) / 8;
            var bytes = new byte[numBytes];

            for (var i = 0; i < bitString.Length; i++)
                if (bitString[i] == '1')
                {
                    var byteIndex = i / 8;
                    var bitIndex = 7 - i % 8;
                    bytes[byteIndex] |= (byte)(1 << bitIndex);
                }

            return bytes;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred converting bit string to bytes.", ex);
        }
    }

    public byte[] PrefixTableToBytes(Dictionary<char, string> prefixTable)
    {
        try
        {
            var headerBuilder = new StringBuilder();
            var
                separator = '\u001E'; //todo: I need to make this whole seperator system better, although this is an uncommon character, it will still break if within a document

            foreach (var kvp in prefixTable)
            {
                var escapedChar = kvp.Key switch
                {
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    ' ' => "␣",
                    '\0' => "\\0",
                    _ => kvp.Key.ToString()
                };
                headerBuilder.AppendLine($"{escapedChar}{separator}{kvp.Value}");
            }

            return Encoding.UTF8.GetBytes(headerBuilder.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred converting prefix table to byte array.", ex);
        }
    }

    public string EncodeTextToBitString(string fileText, Dictionary<char, string> prefixTable)
    {
        try
        {
            var sb = new StringBuilder();

            foreach (var c in fileText)
            {
                if (!prefixTable.TryGetValue(c, out var code))
                    throw new Exception(
                        $"Character {c} not found in prefix table. Error in Encoding or Binary Service");

                sb.Append(code);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred when encoding fileText with prefix table.", ex);
        }
    }

    private void TraversePrefixTableRecursive(IHuffmanNode node, string prefix, Dictionary<char, string> prefixTable)
    {
        if (node.Value() is { } c)
        {
            prefixTable[c] = prefix;
            return;
        }

        TraversePrefixTableRecursive(node.Left!, prefix + "0", prefixTable);
        TraversePrefixTableRecursive(node.Right!, prefix + "1", prefixTable);
    }
}