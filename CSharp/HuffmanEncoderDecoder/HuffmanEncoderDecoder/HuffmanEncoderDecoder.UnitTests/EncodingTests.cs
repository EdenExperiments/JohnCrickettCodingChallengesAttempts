using HuffmanEncoderDecoder.Interfaces;
using HuffmanEncoderDecoder.UnitTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace HuffmanEncoderDecoder.UnitTests;

public class EncodingTests(EncodingFixture fixture) : IClassFixture<EncodingFixture>
{
    private readonly IBinaryService _binaryService = fixture.ServiceProvider.GetRequiredService<IBinaryService>();
    private readonly IEncodingService _encodingService = fixture.ServiceProvider.GetRequiredService<IEncodingService>();

    [Fact]
    public void BuildFrequencyMap_HelloWorld_CorrectFrequencies()
    {
        //Arrange
        var text = "Hello World";

        //Act
        var frequencyMap = _encodingService.BuildFrequencyMap(text);

        //Assert
        Assert.Equal(1, frequencyMap['H']);
        Assert.Equal(3, frequencyMap['l']);
        Assert.Equal(1, frequencyMap[' ']);
        Assert.Equal(2, frequencyMap['o']);
    }

    [Fact]
    public void BuildFrequencyMap_EmptyString_ReturnsEmptyMap()
    {
        var text = "";
        var map = _encodingService.BuildFrequencyMap(text);

        Assert.Empty(map);
    }

    [Fact]
    public void BuildFrequencyMap_MixedCaseDigits_CorrectFrequencies()
    {
        var text = "aaaAAA111";
        var map = _encodingService.BuildFrequencyMap(text);

        Assert.Equal(3, map['a']);
        Assert.Equal(3, map['A']);
        Assert.Equal(3, map['1']);
    }

    [Fact]
    public void BuildFrequencyMap_WithSpacesAndPunctuation_CorrectFrequencies()
    {
        var text = "This is a test.";
        var map = _encodingService.BuildFrequencyMap(text);

        Assert.Equal(3, map[' ']);
        Assert.Equal(3, map['s']);
        Assert.Equal(2, map['t']);
        Assert.Equal(1, map['.']);
    }

    [Fact]
    public void BuildFrequencyMap_LargeInput_HandlesWithoutError()
    {
        var text = new string('x', 1_000_000);
        var map = _encodingService.BuildFrequencyMap(text);

        Assert.Single(map);
        Assert.Equal(1_000_000, map['x']);
    }

    [Fact]
    public void BuildFrequencyMap_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _encodingService.BuildFrequencyMap(null));
    }

    [Fact]
    public void BuildBinaryTree_HelloWorld_CorrectTree()
    {
        //Arrange
        var text = "Hello World";
        var frequencyMap = _encodingService.BuildFrequencyMap(text);
        var expectedTotalWeight = frequencyMap.Values.Sum();

        //Act
        var binaryTree = _binaryService.BuildBinaryTree(frequencyMap);

    }
}