using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAB.DotIgnore;

namespace RenameTool
{
    public static class DirectoryAndFileRenamer
    {
        public static void RenameDirectoryTree(string path, string oldFileName, string newFileName, Dictionary<string, IgnoreList> ignores)
        {
            var di = new DirectoryInfo(path);
            RenameDirectoryTree(di, oldFileName, newFileName, ignores);
        }

        public static void RenameDirectoryTree(DirectoryInfo directory, string oldFileName, string newFileName, Dictionary<string, IgnoreList> ignores)
        {
            InternalRenameDirectoryTree(directory, oldFileName, newFileName, ignores);

            if (!ignores.Any(ignore =>
                    !string.IsNullOrWhiteSpace(directory.FullName.Replace(ignore.Key, "")) &&
                    ignore.Value.IsIgnored(directory.FullName.Replace(ignore.Key, ""), true)))
            {
                var currentName = directory.Name;
                var newName = currentName.Replace(oldFileName, newFileName);
                if (currentName != newName)
                {
                    var newDirname = Path.Combine(directory.Parent.FullName, newName);
                    directory.MoveTo(newDirname);
                }
            }
        }

        static void InternalRenameDirectoryTree(DirectoryInfo di, string oldFileName, string newFileName, Dictionary<string, IgnoreList> ignores)
        {
            foreach (var item in di.GetFileSystemInfos())
            {
                var subdir = item as DirectoryInfo;
                if (subdir != null &&
                    !ignores.Any(ignore =>
                        !string.IsNullOrWhiteSpace(subdir.FullName.Replace(ignore.Key, "")) &&
                        ignore.Value.IsIgnored(subdir.FullName.Replace(ignore.Key, ""), true)))
                {
                    InternalRenameDirectoryTree(subdir, oldFileName, newFileName, ignores);

                    var currentName = subdir.Name;
                    var newName = currentName.Replace(oldFileName, newFileName);
                    if (currentName != newName)
                    {
                        var newDirname = Path.Combine(subdir.Parent.FullName, newName);
                        subdir.MoveTo(newDirname);
                    }
                }

                var file = item as FileInfo;
                if (file != null && !ignores.Any(ignore => ignore.Value.IsIgnored(file.FullName.Replace(ignore.Key, ""), true)))
                {
                    var currentName = Path.GetFileNameWithoutExtension(file.Name);
                    var newName = currentName.Replace(oldFileName, newFileName);
                    if (currentName != newName)
                    {
                        var newFilename = Path.Combine(file.DirectoryName, newName + file.Extension);
                        file.MoveTo(newFilename);
                    }
                }
            }
        }
    }
}