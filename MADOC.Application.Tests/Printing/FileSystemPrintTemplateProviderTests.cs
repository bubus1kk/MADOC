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
    public void GetTemplate_InlinesLocalStylesheet()
    {
        var printingDirectory = CreateTemporaryDirectory();
        var templatesDirectory = Path.Combine(printingDirectory, "Templates");
        var stylesDirectory = Path.Combine(printingDirectory, "Styles");
        Directory.CreateDirectory(templatesDirectory);
        Directory.CreateDirectory(stylesDirectory);
        File.WriteAllText(
            Path.Combine(templatesDirectory, "template.html"),
            """<html><head><link rel="stylesheet" href="Styles/template.css"></head></html>""");
        File.WriteAllText(Path.Combine(stylesDirectory, "template.css"), "body { color: #000; }");
        var provider = new FileSystemPrintTemplateProvider(templatesDirectory);

        var template = provider.GetTemplate("template.html");

        StringAssert.Contains(template, """<style data-madoc-source="Styles/template.css">""");
        StringAssert.Contains(template, "body { color: #000; }");
        Assert.IsFalse(template.Contains("<link", StringComparison.OrdinalIgnoreCase));
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
