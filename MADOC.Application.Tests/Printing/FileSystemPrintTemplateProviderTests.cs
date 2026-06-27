using MADOC.Application.Printing;

namespace MADOC.Application.Tests.Printing;

[TestClass]
public sealed class FileSystemPrintTemplateProviderTests
{
    [TestMethod]
    public void GetTemplate_ReturnsFileContent()
    {
        var directory = CreateTemporaryDirectory();
        var templatePath = Path.Combine(directory, "template.html");
        File.WriteAllText(templatePath, "<html>template</html>");
        var provider = new FileSystemPrintTemplateProvider(directory);

        var template = provider.GetTemplate("template.html");

        Assert.AreEqual("<html>template</html>", template);
    }

    [TestMethod]
    public void GetTemplate_ThrowsWhenTemplateDoesNotExist()
    {
        var directory = CreateTemporaryDirectory();
        var provider = new FileSystemPrintTemplateProvider(directory);

        Assert.ThrowsExactly<FileNotFoundException>(() => provider.GetTemplate("missing.html"));
    }

    [TestMethod]
    public void GetTemplate_ThrowsWhenTemplateNameContainsDirectoryPath()
    {
        var directory = CreateTemporaryDirectory();
        var provider = new FileSystemPrintTemplateProvider(directory);

        Assert.ThrowsExactly<InvalidOperationException>(() => provider.GetTemplate("../template.html"));
    }

    [TestMethod]
    public void Constructor_ThrowsForEmptyDirectory()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FileSystemPrintTemplateProvider(""));
    }

    private static string CreateTemporaryDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"madoc-template-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        return directory;
    }
}
