using CCWC.Application.Services;
using CCWC.Helpers;

namespace CCWC.CLIHandler;

public static class CommandLineHandler
{
    public static void HandleArguments(string[] args)
    {
        if (args.Length == 1 && args[0] == "--help")
            Utilities.WriteHelpText();
        else if (args.Length == 1 && File.Exists(args[0]))
            CcwcService.ProcessDefaultFlags(args[0]);
        else if (args.Length == 1 && !File.Exists(args[0]))
            Utilities.WriteFileNotExistError(args[0]);
        else if (args.Length == 1)
            Utilities.WriteSingleFlagError();
        else if (args.Length == 2 && Utilities.IsFlagValid(args[0]) && File.Exists(args[1]))
            CcwcService.ProcessSingle(Utilities.NormaliseFlag(args[0]), args[1]);
        else if (args.Length == 2 && !File.Exists(args[1]))
            Utilities.WriteFileNotExistError(args[1]);
        else
            Utilities.WriteGeneralError();
    }
}