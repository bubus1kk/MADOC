using MADOC.Application.Printing;

namespace MADOC.Application.Tests.TestSupport;

internal sealed class InMemoryPrintTemplateProvider : IPrintTemplateProvider
{
    private readonly IReadOnlyDictionary<string, string> templates;

    public InMemoryPrintTemplateProvider(IReadOnlyDictionary<string, string> templates)
    {
        ArgumentNullException.ThrowIfNull(templates);
        this.templates = templates;
    }

    public string GetTemplate(string templateFileName)
    {
        if (!templates.TryGetValue(templateFileName, out var template))
        {
            throw new FileNotFoundException($"Тестовый шаблон {templateFileName} не найден");
        }

        return template;
    }
}
