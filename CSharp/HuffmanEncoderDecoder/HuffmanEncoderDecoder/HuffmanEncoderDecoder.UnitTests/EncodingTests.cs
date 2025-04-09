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
        // Arrange
        var text = "Hello World";
        var frequencyMap = _encodingService.BuildFrequencyMap(text);
        var expectedTotalWeight = frequencyMap.Values.Sum();

        // Act
        var root = _binaryService.BuildBinaryTree(frequencyMap);

        // Assert
        Assert.NotNull(root);
        Assert.Equal(expectedTotalWeight, root.Weight());

        var leafNodes = GetLeafNodes(root).ToList();

        foreach (var leaf in leafNodes)
        {
            Assert.True(leaf.IsLeaf());
            Assert.True(leaf.Value().HasValue);
            Assert.Contains(leaf.Value().Value, frequencyMap.Keys);
        }

        Assert.Equal(frequencyMap.Count, leafNodes.Count);
    }

    [Fact]
    public void BuildBinaryTree_SingleCharacter_ReturnsSingleLeaf()
    {
        var map = new Dictionary<char, int> { ['A'] = 10 };
        var root = _binaryService.BuildBinaryTree(map);

        Assert.NotNull(root);
        Assert.True(root.IsLeaf());
        Assert.Equal('A', root.Value());
        Assert.Equal(10, root.Weight());
    }

    [Fact]
    public void BuildBinaryTree_TwoCharacters_ProducesSingleInternalRootWithTwoLeaves()
    {
        // Arrange
        var map = new Dictionary<char, int>
        {
            ['A'] = 3,
            ['B'] = 5
        };

        // Act
        var root = _binaryService.BuildBinaryTree(map);

        // Assert
        Assert.NotNull(root);
        Assert.False(root.IsLeaf());
        Assert.NotNull(root.Left);
        Assert.NotNull(root.Right);

        var leafValues = new[] { root.Left, root.Right }
            .Where(n => n != null && n.IsLeaf())
            .Select(n => n!.Value().Value)
            .ToHashSet();

        Assert.Contains('A', leafValues);
        Assert.Contains('B', leafValues);
        Assert.Equal(8, root.Weight()); // 3 + 5
    }

    [Fact]
    public void BuildBinaryTree_UnbalancedFrequencies_CIsCloserToRoot()
    {
        // Arrange
        var map = new Dictionary<char, int>
        {
            ['A'] = 1,
            ['B'] = 1,
            ['C'] = 50
        };

        var root = _binaryService.BuildBinaryTree(map);

        // Act
        var depthMap = new Dictionary<char, int>();
        BuildDepthMap(root, "", depthMap); // "recursive DFS" to find path length

        // Assert
        Assert.True(depthMap.ContainsKey('A'));
        Assert.True(depthMap.ContainsKey('B'));
        Assert.True(depthMap.ContainsKey('C'));

        // C should have a shorter or equal depth than A/B
        Assert.True(depthMap['C'] < depthMap['A']);
        Assert.True(depthMap['C'] < depthMap['B']);
    }

    // Helper: depth = length of prefix code
    private void BuildDepthMap(IHuffmanNode node, string path, Dictionary<char, int> map)
    {
        if (node.IsLeaf() && node.Value().HasValue)
        {
            map[node.Value().Value] = path.Length;
            return;
        }

        if (node.Left != null) BuildDepthMap(node.Left, path + "0", map);
        if (node.Right != null) BuildDepthMap(node.Right, path + "1", map);
    }



    private IEnumerable<IHuffmanNode> GetLeafNodes(IHuffmanNode? node)
    {
        if (node == null)
            yield break;

        if (node.IsLeaf())
        {
            yield return node;
        }
        else
        {
            foreach (var leaf in GetLeafNodes(node.Left))
                yield return leaf;

            foreach (var leaf in GetLeafNodes(node.Right))
                yield return leaf;
        }
    }
}