using System.Text.RegularExpressions;

namespace RenameTool
{
    public static class DirectoryRenamer
    {
        public static string? Rename(string directoryPath, string[] findStrings, string[] replaceStrings, bool useRegex)
        {
            var directory = new DirectoryInfo(directoryPath);

            var currentName = directory.Name;
            var newName = currentName;
            for (int findStringIndex = 0; findStringIndex < findStrings.Length; findStringIndex++)
            {
                if (useRegex)
                {
                    newName = Regex.Replace(newName, findStrings[findStringIndex], replaceStrings[findStringIndex]);
                }
                else
                {
                    newName = newName.Replace(findStrings[findStringIndex], replaceStrings[findStringIndex]);
                }
            }

            if (currentName != newName && directory.Parent is not null)
            {
                var oldDirname = Path.Combine(directory.Parent.FullName, directory.Name);
                var newDirname = Path.Combine(directory.Parent.FullName, newName);
                MoveDirectory(oldDirname, newDirname);
                return newDirname;
            }

            return null;
        }

        private static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));
            foreach (var folder in files)
            {
                string? targetFolder = folder.Key?.Replace(sourcePath, targetPath);
                if (targetFolder is not null)
                {
                    Directory.CreateDirectory(targetFolder);
                    foreach (var file in folder)
                    {
                        var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                        if (File.Exists(targetFile)) File.Delete(targetFile);
                        File.Move(file, targetFile);
                    }
                }
            }
            Directory.Delete(source, true);
        }
    }
}