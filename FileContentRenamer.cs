using RenameTool.Tools;

namespace RenameTool
{
    public static class FileContentRenamer
    {
        public static void Rename(string[] files, string[] findStrings, string[] replaceStrings)
        {
            ConsoleHelper.Rewrite("Renaming file contents... [0]");
            int i = 0;
            foreach (var file in files)
            {
                var contents = File.ReadAllText(file);
                var newContent = contents;
                for (int findStringIndex = 0; findStringIndex < findStrings.Length; findStringIndex++)
                {
                    newContent = newContent.Replace(findStrings[findStringIndex], replaceStrings[findStringIndex]);
                }
                if (contents != newContent)
                {
                    File.WriteAllText(file, newContent);
                    ConsoleHelper.Rewrite(7, "Renaming file contents... [{0}]", ++i);
                }
            }
            ConsoleHelper.Rewrite("Renaming file contents... [{0}]\n", i);
        }
    }
}