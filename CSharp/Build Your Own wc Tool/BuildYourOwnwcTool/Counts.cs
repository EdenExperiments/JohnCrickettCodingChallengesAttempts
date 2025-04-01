namespace CCWC;

internal class Counts(string filePath)
{
    internal long C { get; init; } = new FileInfo(filePath).Length;
    internal long L { get; set; } = 0;
    internal long M { get; set; } = 0;
    internal long W { get; set; } = 0;

    internal void AddToCount(string flag, long amount)
    {
        switch (flag)
        {
            case "L":
                L += amount;
                break;
            case "M":
                M += amount;
                break;
            case "W":
                W += amount;
                break;
            default:
                throw new ArgumentException("Invalid argument passed into ccwc");
        }
    }
}