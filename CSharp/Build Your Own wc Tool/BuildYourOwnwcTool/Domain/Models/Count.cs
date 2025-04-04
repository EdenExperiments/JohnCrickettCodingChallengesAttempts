namespace CCWC.Domain.Models;

public class Count(string filePath)
{
    internal long C { get; init; } = new FileInfo(filePath).Length;
    internal long L { get; set; } = 0;
    internal long M { get; set; } = 0;
    internal long W { get; set; } = 0;
}