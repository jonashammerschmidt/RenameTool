using RenameTool.Tools;

namespace RenameTool
{
    public static class DirectoryAndFileRenamer
    {
        public static void Rename(string path, string oldFileName, string newFileName, GitIgnoreTracker gitIgnoreTracker)
        {
            var di = new DirectoryInfo(path);
            RenameDirectoryTree(di, oldFileName, newFileName, gitIgnoreTracker);
        }

        public static void RenameDirectoryTree(DirectoryInfo directory, string oldFileName, string newFileName, GitIgnoreTracker gitIgnoreTracker)
        {
            InternalRenameDirectoryTree(directory, oldFileName, newFileName, gitIgnoreTracker);

            if (!gitIgnoreTracker.IsDirectoryIgnored(directory.FullName))
            {
                var currentName = directory.Name;
                var newName = currentName.Replace(oldFileName, newFileName);
                if (currentName != newName)
                {
                    var newDirname = Path.Combine(directory.Parent!.FullName, newName);
                    directory.MoveTo(newDirname);
                }
            }
        }

        static void InternalRenameDirectoryTree(DirectoryInfo di, string oldFileName, string newFileName, GitIgnoreTracker gitIgnoreTracker)
        {
            foreach (var item in di.GetFileSystemInfos())
            {
                var subdir = item as DirectoryInfo;
                if (subdir != null && !gitIgnoreTracker.IsDirectoryIgnored(subdir.FullName))
                {
                    InternalRenameDirectoryTree(subdir, oldFileName, newFileName, gitIgnoreTracker);

                    var currentName = subdir.Name;
                    var newName = currentName.Replace(oldFileName, newFileName);
                    if (currentName != newName && subdir.Parent is not null)
                    {
                        var newDirname = Path.Combine(subdir.Parent.FullName, newName);
                        subdir.MoveTo(newDirname);
                    }
                }

                var file = item as FileInfo;
                if (file != null && !gitIgnoreTracker.IsFileIgnored(file.FullName))
                {
                    var currentName = Path.GetFileNameWithoutExtension(file.Name);
                    var newName = currentName.Replace(oldFileName, newFileName);
                    if (currentName != newName && file.DirectoryName is not null)
                    {
                        var newFilename = Path.Combine(file.DirectoryName, newName + file.Extension);
                        file.MoveTo(newFilename);
                    }
                }
            }
        }
    }
}