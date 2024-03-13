
using MAB.DotIgnore;

namespace RenameTool.Tools
{
    public class GitIgnoreTracker
    {
        private readonly Dictionary<string, IgnoreList> ignores = new();

        public GitIgnoreTracker()
        {
            this.SetupInitialIgnoreLists();
            this.IncludeGitIgnoresFromSubFoldersIgnoreList();
        }

        public bool IsDirectoryIgnored(string directoryPath)
        {
            directoryPath = Path.GetFullPath(directoryPath);

            return ignores
                .Any(ignore =>
                    directoryPath.StartsWith(ignore.Key) &&
                    !string.IsNullOrWhiteSpace(directoryPath.Replace(ignore.Key, "")) &&
                    ignore.Value.IsIgnored(directoryPath.Replace(ignore.Key, ""), true));
        }

        public bool IsFileIgnored(string filePath)
        {
            filePath = Path.GetFullPath(filePath);

            return ignores
                .Any(ignore =>
                    filePath.StartsWith(ignore.Key) &&
                    ignore.Value.IsIgnored(filePath.Replace(ignore.Key, ""), false));
        }

        private void SetupInitialIgnoreLists()
        {
            string currentFolder = Program.CurrentDirectory;
            while (Directory.GetParent(currentFolder) != null)
            {
                if (Directory.GetFiles(currentFolder)
                    .Any(file => new FileInfo(file).Name == ".gitignore"))
                {
                    this.AddGitIgnoreFile(currentFolder);
                }

                if (Directory.GetDirectories(currentFolder)
                    .Any(dir => new DirectoryInfo(dir).Name == ".git"))
                {
                    break;
                }

                currentFolder = Directory.GetParent(currentFolder)!.FullName;
            }
        }

        private void IncludeGitIgnoresFromSubFoldersIgnoreList()
        {
            var directoryPath = Program.CurrentDirectory;
            this.IncludeGitIgnoresFromSubFoldersIgnoreListRek(directoryPath);
        }

        private void IncludeGitIgnoresFromSubFoldersIgnoreListRek(string directoryPath)
        {
            foreach (var subDirectoryPath in Directory.GetDirectories(directoryPath))
            {
                var subDirectory = new DirectoryInfo(subDirectoryPath);
                if (!this.IsDirectoryIgnored(subDirectory.FullName))
                {
                    if (subDirectory.GetFiles().Any(file => file.Name.Equals(".gitignore")))
                    {
                        AddGitIgnoreFile(subDirectory.FullName);
                    }

                    IncludeGitIgnoresFromSubFoldersIgnoreListRek(subDirectory.FullName);
                }
            }
        }

        private void AddGitIgnoreFile(string directoryPath)
        {
            var item = new IgnoreList(Path.Combine(directoryPath, ".gitignore"));
            item.AddRule(".git/");
            item.AddRule(".gitignore");
            ignores.Add(directoryPath, item);
        }
    }
}