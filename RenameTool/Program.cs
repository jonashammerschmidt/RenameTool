using CaseExtensions;
using RenameTool.Tools;
using System.Text.RegularExpressions;

namespace RenameTool
{
    public class Program
    {
        public static string CurrentDirectory = Directory.GetCurrentDirectory();

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                HelpCommand.WriteHelp();
                return;
            }

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

            if (ArgumentParser.HasArgument(args, "-c", "--custom"))
            {
                findStrings = new string[] { findString };
                replaceStrings = new string[] { replaceString };
            }

            bool useRegex = false;
            if (ArgumentParser.HasArgument(args, "-r", "--regex"))
            {
                findStrings = new string[] { findString };
                replaceStrings = new string[] { replaceString };
                useRegex = true;
            }

            Console.WriteLine("Prepare rename for:");
            for (int i = 0; i < findStrings.Length; i++)
            {
                Console.WriteLine($" - {findStrings[i]}  ->  {replaceStrings[i]}");
            }

            Raname(CurrentDirectory, findStrings, replaceStrings, useRegex);
        }

        private static void Raname(string directory, string[] findStrings, string[] replaceStrings, bool useRegex)
        {
            var gitIgnoreTracker = new GitIgnoreTracker();

            FindAllFilesExcept(
                directory,
                findStrings,
                gitIgnoreTracker,
                out List<string> foundFiles,
                out List<string> foundFilteredFiles,
                out List<string> foundFilteredDirectories,
                useRegex);

            FileContentRenamer.Rename(foundFiles.ToArray(), findStrings, replaceStrings, useRegex);

            Console.WriteLine("Renaming files... [{0}]", foundFilteredFiles.Count);
            foreach (var foundFile in foundFilteredFiles)
            {
                FileRenamer.Rename(foundFile, findStrings, replaceStrings, useRegex);
            }

            Console.WriteLine("Renaming directories... [{0}]", foundFilteredDirectories.Count);
            for (int i = 0; i < foundFilteredDirectories.Count; i++)
            {
                string oldDirectoryName = foundFilteredDirectories[i];
                string? newDirectoryName = DirectoryRenamer.Rename(oldDirectoryName, findStrings, replaceStrings, useRegex);
                if (newDirectoryName is not null)
                {
                    foundFilteredDirectories = foundFilteredDirectories
                        .Select(dir =>
                        {
                            if (useRegex)
                            {
                                return Regex.Replace(dir, oldDirectoryName, newDirectoryName);
                            }
                            else
                            {
                                return dir.Replace(oldDirectoryName, newDirectoryName);
                            }
                        })
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
            out List<string> foundFilteredDirectories,
            bool useRegex)
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
                    foreach (var file in Directory.GetFiles(currentDirectory))
                    {
                        if (!gitIgnoreTracker.IsFileIgnored(file))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (findStrings.Any(findString =>
                                {
                                    if (useRegex)
                                    {
                                        return Regex.IsMatch(fileInfo.Name, findString);
                                    }
                                    else
                                    {
                                        return fileInfo.Name.Contains(findString);
                                    }
                                }))
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
                            if (findStrings.Any(findString =>
                                {
                                    if (useRegex)
                                    {
                                        return Regex.IsMatch(directoryInfo.Name, findString);
                                    }
                                    else
                                    {
                                        return directoryInfo.Name.Contains(findString);
                                    }
                                }))
                            {
                                foundFilteredDirectories.Add(directory);
                            }

                            pathsToSearch.Enqueue(directory);
                            scannedDirectories++;
                            ConsoleHelper.Rewrite(77, "Scanning directory... [{0}]", scannedDirectories);
                        }
                    }
                }
                catch (Exception)
                {
                    /* Handle exceptions as needed */
                }
            }

            foundFilteredFiles.Sort((f1, f2) => f1.CompareTo(f2));
            foundFilteredDirectories.Sort((d1, d2) => d1.CompareTo(d2));
            ConsoleHelper.Rewrite("Scanning directory... [{0}]\n", scannedDirectories);
        }
    }
}