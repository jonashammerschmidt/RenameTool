using CaseExtensions;
using RenameTool.Tools;

namespace RenameTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                HelpCommand.WriteHelp();
                return;
            }

            string folder = Directory.GetCurrentDirectory();
            string oldFileName = args[0];
            string newFileName = args[1];

            if (ArgumentParser.HasArgument(args, "-c", "--custom"))
            {
                Rename(folder, oldFileName, newFileName);
            }
            else
            {
                Rename(folder, oldFileName.ToPascalCase(), newFileName.ToPascalCase());
                Rename(folder, oldFileName.ToCamelCase(), newFileName.ToCamelCase());
                Rename(folder, oldFileName.ToKebabCase(), newFileName.ToKebabCase());
            }
        }

        private static void Rename(string folder, string oldFileName, string newFileName)
        {
            var gitIgnoreTracker = new GitIgnoreTracker();
            FileContentRenamer.Rename(folder, oldFileName, newFileName, gitIgnoreTracker);
            DirectoryAndFileRenamer.Rename(folder, oldFileName, newFileName, gitIgnoreTracker);
        }
    }
}