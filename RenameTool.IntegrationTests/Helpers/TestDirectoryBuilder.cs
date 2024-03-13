namespace RenameTool.IntegrationTests.Helpers;

public class TestDirectoryBuilder
{
    private readonly string basePath;

    public TestDirectoryBuilder(string basePath)
    {
        this.basePath = basePath;
        Directory.CreateDirectory(basePath); // Stellt sicher, dass das Basisverzeichnis existiert
    }

    public TestDirectoryBuilder AddFile(string relativePath, string content = "Test content")
    {
        var fullPath = Path.Combine(basePath, relativePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(fullPath, content);
        return this;
    }

    public TestDirectoryBuilder AddDirectory(string relativePath)
    {
        var fullPath = Path.Combine(basePath, relativePath);
        Directory.CreateDirectory(fullPath);
        return this;
    }

    public string Build()
    {
        return basePath;
    }
}
