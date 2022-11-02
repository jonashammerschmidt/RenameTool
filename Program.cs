using CaseExtensions;
using MAB.DotIgnore;
using RenameTool.Tools;

namespace RenameTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            string folder = Directory.GetCurrentDirectory();
            string oldFileName = args[0];
            string newFileName = args[1];

            var gitIgnoreTracker = new GitIgnoreTracker();

            Rename(folder, oldFileName.ToPascalCase(), newFileName.ToPascalCase(), gitIgnoreTracker);
            Rename(folder, oldFileName.ToCamelCase(), newFileName.ToCamelCase(), gitIgnoreTracker);
            Rename(folder, oldFileName.ToKebabCase(), newFileName.ToKebabCase(), gitIgnoreTracker);
        }

        private static void Rename(string folder, string oldFileName, string newFileName, GitIgnoreTracker gitIgnoreTracker)
        {
            DirectoryAndFileRenamer.RenameDirectoryTree(folder, oldFileName, newFileName, gitIgnoreTracker);

            foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                if (!gitIgnoreTracker.IsFileIgnored(file))
                {
                    var contents = File.ReadAllText(file);
                    var newContent = contents.Replace(oldFileName, newFileName);
                    if (contents != newContent)
                    {
                        File.WriteAllText(file, newContent);
                    }
                }
            }
        }
    }
}