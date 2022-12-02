namespace RenameTool
{
    public static class FileRenamer
    {
        public static void Rename(string filePath, string[] findStrings, string[] replaceStrings)
        {
            var file = new FileInfo(filePath);
            var currentName = Path.GetFileNameWithoutExtension(file.Name);
            var newName = currentName;
            for (int findStringIndex = 0; findStringIndex < findStrings.Length; findStringIndex++)
            {
                newName = newName.Replace(findStrings[findStringIndex], replaceStrings[findStringIndex]);
            }

            if (currentName != newName && file.DirectoryName is not null)
            {
                var newFilename = Path.Combine(file.DirectoryName, newName + file.Extension);
                file.MoveTo(newFilename);
            }
        }
    }
}