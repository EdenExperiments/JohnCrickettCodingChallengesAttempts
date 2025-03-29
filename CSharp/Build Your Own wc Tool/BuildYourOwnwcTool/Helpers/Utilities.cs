
namespace BuildYourOwnwcTool.Helpers
{
    internal static class Utilities
    {
        internal static void PrintHelp()
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

    }
}
