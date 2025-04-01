using System.Reflection;

namespace CCWC.Helpers;

internal static class Utilities
{
    internal static void WriteHelpText()
    {
        Console.WriteLine(@"
Usage: ccwc [option] <file>

Options:
  -c        Print the byte count
  -l        Print the line count
  -w        Print the word count
  -m        Print the character count

When no option is provided, ccwc prints -c -l -w <file>
");
    }

    internal static bool IsFlagValid(string flag)
    {
        var type = typeof(Counts);

        var fieldInfo = type.GetProperty(flag.TrimStart('-').ToUpper(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return fieldInfo != null;
    }

    internal static void WriteResult(Counts counts, string key, string filePath)
    {
        var property = counts.GetType().GetProperty(key,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null) throw new ArgumentException($"Property '{key}' not found on Counts");

        var value = property.GetValue(counts);
        Console.WriteLine($"{value} {filePath}");
    }
}