namespace MADOC.Application.Printing;

public sealed class FileSystemPrintTemplateProvider : IPrintTemplateProvider
{
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

        return File.ReadAllText(templatePath);
    }
}
