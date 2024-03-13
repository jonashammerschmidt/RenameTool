using RenameTool;
using RenameTool.IntegrationTests.Helpers;

[TestClass]
public class RenameToolTests
{
    private string testDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    [TestInitialize]
    public void Initialize()
    {
        Program.CurrentDirectory = testDirectoryPath;
    }

    [TestCleanup]
    public void Cleanup()
    {
        Directory.Delete(testDirectoryPath, true);
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldBeRenamedCorrectly_WhenOldNameIsPresent_PascalCase()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("testDirOldName")
            .AddFile("testDirOldName/testFileOldName.txt", "Das ist ein OldName Text.");

        // Act
        Program.Main([ "OldName", "NewName" ]);

        // Assert
        string newDirPath = Path.Combine(testDirectoryPath, "testDirNewName");
        string newFilePath = Path.Combine(newDirPath, "testFileNewName.txt");
        Assert.IsTrue(Directory.Exists(newDirPath), "Directory was not correctly renamed.");
        Assert.IsTrue(File.Exists(newFilePath), "File was not correctly renamed.");
        Assert.AreEqual("Das ist ein NewName Text.", File.ReadAllText(newFilePath), "File content was not correctly updated.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldBeRenamedCorrectly_WhenOldNameIsPresent_CamelCase()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("testDiroldName")
            .AddFile("testDiroldName/testFileoldName.txt", "Das ist ein oldName Text.");

        // Act
        Program.Main([ "OldName", "NewName" ]);

        // Assert
        string newDirPath = Path.Combine(testDirectoryPath, "testDirnewName");
        string newFilePath = Path.Combine(newDirPath, "testFilenewName.txt");
        Assert.IsTrue(Directory.Exists(newDirPath), "Directory was not correctly renamed.");
        Assert.IsTrue(File.Exists(newFilePath), "File was not correctly renamed.");
        Assert.AreEqual("Das ist ein newName Text.", File.ReadAllText(newFilePath), "File content was not correctly updated.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldBeRenamedCorrectly_WhenOldNameIsPresent_KebabCase()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("testDirold-name")
            .AddFile("testDirold-name/testFileold-name.txt", "Das ist ein old-name Text.");

        // Act
        Program.Main([ "OldName", "NewName" ]);

        // Assert
        string newDirPath = Path.Combine(testDirectoryPath, "testDirnew-name");
        string newFilePath = Path.Combine(newDirPath, "testFilenew-name.txt");
        Assert.IsTrue(Directory.Exists(newDirPath), "Directory was not correctly renamed.");
        Assert.IsTrue(File.Exists(newFilePath), "File was not correctly renamed.");
        Assert.AreEqual("Das ist ein new-name Text.", File.ReadAllText(newFilePath), "File content was not correctly updated.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldRemainUnchanged_WhenOldNameIsNotPresent()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("someOtherDir")
            .AddFile("someOtherDir/someOtherFile.txt", "Das ist ein some other Text.");

        // Act
        Program.Main([ "OldName", "NewName" ]);

        // Assert
        string unchangedDirPath = Path.Combine(testDirectoryPath, "someOtherDir");
        string unchangedFilePath = Path.Combine(unchangedDirPath, "someOtherFile.txt");
        Assert.IsTrue(Directory.Exists(unchangedDirPath), "Directory was renamed, which shouldn't happen.");
        Assert.IsTrue(File.Exists(unchangedFilePath), "File was renamed, which shouldn't happen.");
        Assert.AreEqual("Das ist ein some other Text.", File.ReadAllText(unchangedFilePath), "File content was changed, which shouldn't happen.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldBeRenamedCorrectly_UsingComplexRegex()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("regexDir")
            .AddFile("regexDir/regexFile.txt", "regexContent ist das hier mit regexContent");

        // Act
        Program.Main(["^regex(.*)$", "renamed$1", "-r"]);

        // Assert
        string newDirPath = Path.Combine(testDirectoryPath, "renamedDir");
        string newFilePath = Path.Combine(newDirPath, "renamedFile.txt");
        Assert.IsTrue(Directory.Exists(newDirPath), "Directory was not correctly renamed.");
        Assert.IsTrue(File.Exists(newFilePath), "File was not correctly renamed.");
        Assert.AreEqual("renamedContent ist das hier mit regexContent", File.ReadAllText(newFilePath), "File content was not correctly updated.");
    }

    [TestMethod]
    public void UnmatchedFiles_ShouldRemainUnchanged_UsingComplexRegex()
    {
        // Arrange
        string directoryName = "complexRegexTestDir";
        string testDirectoryName = Path.Combine(testDirectoryPath, directoryName);
        Directory.CreateDirectory(testDirectoryName);
        string unmatchedFilePath = Path.Combine(testDirectoryName, "ignoreThisFile.txt");
        File.WriteAllText(unmatchedFilePath, "This file should not be renamed by the complex regex.");

        // Act
        Program.Main(["(complexFile)(123)(\\.txt)$", "$1Renamed$3", "-r"]);

        // Assert
        Assert.IsTrue(File.Exists(unmatchedFilePath), "Unmatched file was incorrectly renamed or deleted.");
        Assert.AreEqual("This file should not be renamed by the complex regex.", File.ReadAllText(unmatchedFilePath), "File content of the unmatched file was altered.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldBeRenamedCorrectly_UsingCustom()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("testDirOldName")
            .AddFile("testDirOldName/testFileOldName.txt", "Das ist ein OldName Text.");

        // Act
        Program.Main([ "OldName", "NewName", "-c" ]);

        // Assert
        string newDirPath = Path.Combine(testDirectoryPath, "testDirNewName");
        string newFilePath = Path.Combine(newDirPath, "testFileNewName.txt");
        Assert.IsTrue(Directory.Exists(newDirPath), "Directory was not correctly renamed.");
        Assert.IsTrue(File.Exists(newFilePath), "File was not correctly renamed.");
        Assert.AreEqual("Das ist ein NewName Text.", File.ReadAllText(newFilePath), "File content was not correctly updated.");
    }

    [TestMethod]
    public void DirectoryAndFiles_ShouldRemainUnchanged_UsingCustom()
    {
        // Arrange
        var builder = new TestDirectoryBuilder(testDirectoryPath)
            .AddDirectory(".git")
            .AddFile(".gitignore", "log")
            .AddDirectory("testDirOldName")
            .AddFile("testDirOldName/testFileOldName.txt", "Das ist ein OldName Text.");

        // Act
        Program.Main([ "oldname", "newname", "-c" ]);

        // Assert
        string unchangedDirPath = Path.Combine(testDirectoryPath, "testDirOldName");
        string unchangedFilePath = Path.Combine(unchangedDirPath, "testFileOldName.txt");
        Assert.IsTrue(Directory.Exists(unchangedDirPath), "Directory was renamed, which shouldn't happen.");
        Assert.IsTrue(File.Exists(unchangedFilePath), "File was renamed, which shouldn't happen.");
        Assert.AreEqual("Das ist ein OldName Text.", File.ReadAllText(unchangedFilePath), "File content was changed, which shouldn't happen.");
    }
}