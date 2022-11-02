using RenameTool.Tools;

namespace RenameTool
{
    public static class FileContentRenamer
    {
        public static void Rename(string folder, string oldFileName, string newFileName, GitIgnoreTracker gitIgnoreTracker)
        {
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