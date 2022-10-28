using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAB.DotIgnore;

namespace Contract.Renamer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string folder = Directory.GetCurrentDirectory();
            string oldFileName = CaseChanger.ToPascalCase(args[0]);
            string newFileName = CaseChanger.ToPascalCase(args[1]);
            Dictionary<string, IgnoreList> ignores = GetIgnoreLists();

            Rename(folder, oldFileName, newFileName, ignores);
            Rename(folder, oldFileName.LowerFirstChar(), newFileName.LowerFirstChar(), ignores);
            Rename(folder, oldFileName.PascalToKebabCase(), newFileName.PascalToKebabCase(), ignores);
        }

        private static void Rename(string folder, string oldFileName, string newFileName, Dictionary<string, IgnoreList> ignores)
        {
            DirectoryAndFileRenamer.RenameDirectoryTree(folder, oldFileName, newFileName, ignores);

            foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                if (!ignores.Any(ignore => ignore.Value.IsIgnored(file.Replace(ignore.Key, ""), false)))
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

        private static Dictionary<string, IgnoreList> GetIgnoreLists()
        {
            var ignores = new Dictionary<string, IgnoreList>();
            string currentFolder = Directory.GetCurrentDirectory();
            while (Directory.GetParent(currentFolder) != null)
            {
                if (Directory.GetFiles(currentFolder)
                    .Any(file => new FileInfo(file).Name == ".gitignore"))
                {
                    var item = new IgnoreList(Path.Combine(currentFolder, ".gitignore"));
                    item.AddRule(".git/");
                    item.AddRule(".gitignore");
                    ignores.Add(currentFolder, item);
                }

                if (Directory.GetDirectories(currentFolder)
                    .Any(dir => new DirectoryInfo(dir).Name == ".git"))
                {
                    break;
                }

                currentFolder = Directory.GetParent(currentFolder)!.FullName;
            }

            return ignores;
        }
    }
}