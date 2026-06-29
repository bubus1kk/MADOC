using System.Text.RegularExpressions;

namespace MADOC.Application.Printing;

public sealed class FileSystemPrintTemplateProvider : IPrintTemplateProvider
{
    private static readonly Regex StylesheetLinkPattern = new(
        """<link\b(?=[^>]*\brel\s*=\s*["']stylesheet["'])(?=[^>]*\bhref\s*=\s*["'](?<href>[^"']+)["'])[^>]*>""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string templatesDirectory;

    public FileSystemPrintTemplateProvider(string templatesDirectory)
    {
        if (string.IsNullOrWhiteSpace(templatesDirectory))
        {
            throw new ArgumentException("Путь к каталогу шаблонов не может быть пустым", nameof(templatesDirectory));
        }

        this.templatesDirectory = templatesDirectory;
    }

    public static FileSystemPrintTemplateProvider FromApplicationBaseDirectory()
    {
        var templatesDirectory = Path.Combine(AppContext.BaseDirectory,"Printing","Templates");

        return new FileSystemPrintTemplateProvider(templatesDirectory);
    }

    public string GetTemplate(string templateFileName)
    {
        if (string.IsNullOrWhiteSpace(templateFileName))
        {
            throw new ArgumentException("Имя файла шаблона не может быть пустым", nameof(templateFileName));
        }

        var safeFileName = Path.GetFileName(templateFileName);

        if (!string.Equals(safeFileName, templateFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Имя шаблона не должно содержать путь к каталогу");
        }

        var templatePath = Path.Combine(templatesDirectory, safeFileName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"HTML-шаблон {safeFileName} не найден", templatePath);
        }

        return InlineLocalStyles(File.ReadAllText(templatePath));
    }

    private string InlineLocalStyles(string html)
    {
        var printingDirectory = Path.GetFullPath(Path.Combine(templatesDirectory, ".."));

        return StylesheetLinkPattern.Replace(html, match =>
        {
            var href = match.Groups["href"].Value;

            if (Uri.TryCreate(href, UriKind.Absolute, out _))
            {
                return match.Value;
            }

            var relativePath = href.Replace('/', Path.DirectorySeparatorChar);
            var stylesheetPath = Path.GetFullPath(Path.Combine(printingDirectory, relativePath));
            var pathInsidePrintingDirectory = Path.GetRelativePath(printingDirectory, stylesheetPath);

            if (pathInsidePrintingDirectory.StartsWith("..", StringComparison.Ordinal) ||
                !string.Equals(Path.GetExtension(stylesheetPath), ".css", StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(stylesheetPath))
            {
                return match.Value;
            }

            var stylesheet = File.ReadAllText(stylesheetPath);
            return $"<style data-madoc-source=\"{href}\">{Environment.NewLine}{stylesheet}{Environment.NewLine}</style>";
        });
    }
}
