namespace HuffmanEncoderDecoder.Interfaces.Handlers;

public interface IEncodingHandler
{
    public void Encode(string inputFilePath, string outputFilePath);
}