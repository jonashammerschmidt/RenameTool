using CaseExtensions;
using RenameTool.Tools;

namespace RenameTool
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                HelpCommand.WriteHelp();
                return;
            }

            string folder = Directory.GetCurrentDirectory();
            string findString = args[0];
            string replaceString = args[1];
            
            string[] findStrings;
            string[] replaceStrings;
            if (findString.ToCamelCase() == findString.ToKebabCase())
            {
                findStrings = new string[] { findString.ToPascalCase(), findString.ToCamelCase() };
                replaceStrings = new string[] { replaceString.ToPascalCase(), replaceString.ToCamelCase() };
            }
            else
            {
                findStrings = new string[] { findString.ToPascalCase(), findString.ToCamelCase(), findString.ToKebabCase() };
                replaceStrings = new string[] { replaceString.ToPascalCase(), replaceString.ToCamelCase(), replaceString.ToKebabCase() };
            }

            Console.WriteLine("Prepare rename for:");
            for (int i = 0; i < findStrings.Length; i++)
            {
                Console.WriteLine($" - {findStrings[i]}  ->  {replaceStrings[i]}");
            }

            if (ArgumentParser.HasArgument(args, "-c", "--custom"))
            {
                findStrings = new string[] { findString };
                replaceStrings = new string[] { replaceString };
            }

            Raname(folder, findStrings, replaceStrings);
        }

        private static void Raname(string directory, string[] findStrings, string[] replaceStrings)
        {
            var gitIgnoreTracker = new GitIgnoreTracker();

            FindAllFilesExcept(
                directory,
                findStrings,
                gitIgnoreTracker,
                out List<string> foundFiles,
                out List<string> foundFilteredFiles,
                out List<string> foundFilteredDirectories);

            FileContentRenamer.Rename(foundFiles.ToArray(), findStrings, replaceStrings);

            Console.WriteLine("Renaming files... [{0}]", foundFilteredFiles.Count);
            foreach (var foundFile in foundFilteredFiles)
            {
                FileRenamer.Rename(foundFile, findStrings, replaceStrings);
            }

            Console.WriteLine("Renaming directories... [{0}]", foundFilteredDirectories.Count);
            for (int i = 0; i < foundFilteredDirectories.Count; i++)
            {
                string oldDirectoryName = foundFilteredDirectories[i];
                string? newDirectoryName = DirectoryRenamer.Rename(oldDirectoryName, findStrings, replaceStrings);
                if (newDirectoryName is not null)
                {
                    foundFilteredDirectories = foundFilteredDirectories
                        .Select(dir => dir.Replace(oldDirectoryName, newDirectoryName))
                        .ToList();
                }
            }

            Console.WriteLine("Done.");
        }

        private static void FindAllFilesExcept(
            string rootDirectory,
            string[] findStrings,
            GitIgnoreTracker gitIgnoreTracker,
            out List<string> foundFiles,
            out List<string> foundFilteredFiles,
            out List<string> foundFilteredDirectories)
        {
            var pathsToSearch = new Queue<string>();
            foundFiles = new List<string>();
            foundFilteredFiles = new List<string>();
            foundFilteredDirectories = new List<string>();

            pathsToSearch.Enqueue(rootDirectory);

            int scannedDirectories = 0;
            while (pathsToSearch.Count > 0)
            {
                var currentDirectory = pathsToSearch.Dequeue();

                try
                {
                    var files = Directory.GetFiles(currentDirectory);
                    foreach (var file in Directory.GetFiles(currentDirectory))
                    {
                        if (!gitIgnoreTracker.IsFileIgnored(file))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (findStrings.Any(s => fileInfo.Name.Contains(s)))
                            {
                                foundFilteredFiles.Add(file);
                            }
                            foundFiles.Add(file);
                        }
                    }

                    foreach (var directory in Directory.GetDirectories(currentDirectory))
                    {
                        if (!gitIgnoreTracker.IsDirectoryIgnored(directory))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                            if (findStrings.Any(s => directoryInfo.Name.Contains(s)))
                            {
                                foundFilteredDirectories.Add(directory);
                            }
                            pathsToSearch.Enqueue(directory);
                            scannedDirectories++;
                            ConsoleHelper.Rewrite(77, "Scanning directory... [{0}]", scannedDirectories);
                        }
                    }
                }
                catch (Exception) { }
            }

            foundFilteredFiles.Sort((f1, f2) => f1.CompareTo(f2));
            foundFilteredDirectories.Sort((d1, d2) => d1.CompareTo(d2));
            ConsoleHelper.Rewrite("Scanning directory... [{0}]\n", scannedDirectories);
        }
    }
}