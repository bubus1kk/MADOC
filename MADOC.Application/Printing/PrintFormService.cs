using System.Reflection;
using MADOC.Application.Documents;
using MADOC.Printing.Html.Forms;

namespace MADOC.Application.Printing;

public sealed class PrintFormService
{
    private readonly IDocumentTypeRegistry registry;

    private readonly DocumentFactory documentFactory;

    private readonly DocumentValueMapper valueMapper;

    private readonly IPrintTemplateProvider templateProvider;

    public PrintFormService(
        IDocumentTypeRegistry registry,
        DocumentFactory documentFactory,
        DocumentValueMapper valueMapper,
        IPrintTemplateProvider templateProvider)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(documentFactory);
        ArgumentNullException.ThrowIfNull(valueMapper);
        ArgumentNullException.ThrowIfNull(templateProvider);

        this.registry = registry;
        this.documentFactory = documentFactory;
        this.valueMapper = valueMapper;
        this.templateProvider = templateProvider;
    }

    public PrintFormResponse Generate(PrintFormRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var descriptor = registry.GetRequired(request.DocumentType);
        var document = documentFactory.Create(request.DocumentType);
        var mappingResult = valueMapper.MapValues(document, request.Values);

        if (!mappingResult.IsSuccess)
        {
            return PrintFormResponse.Failed(
                request.DocumentType,
                string.Empty,
                mappingResult.Errors.Select(error => error.Message));
        }

        try
        {
            var templateFileName = string.IsNullOrWhiteSpace(request.TemplateFileName)
                ? descriptor.TemplateFileName
                : request.TemplateFileName!;

            var htmlTemplate = templateProvider.GetTemplate(templateFileName);
            var normalizedGeneratedFields = GeneratedFieldValueNormalizer.Normalize(request.GeneratedFields);
            var generationResult = GenerateHtml(
                descriptor.DocumentType,
                document,
                htmlTemplate,
                normalizedGeneratedFields,
                request.StrictMode,
                request.HtmlEncodeValues);

            if (!generationResult.IsSuccess)
            {
                return PrintFormResponse.Failed(
                    request.DocumentType,
                    generationResult.Content,
                    generationResult.Errors);
            }

            return PrintFormResponse.Success(request.DocumentType, generationResult.Content);
        }
        catch (Exception exception)
        {
            return PrintFormResponse.Failed(
                request.DocumentType,
                string.Empty,
                new[] { exception.Message });
        }
    }

    private static HtmlPrintFormGenerationResult GenerateHtml(
        Type documentType,
        object document,
        string htmlTemplate,
        IReadOnlyDictionary<string, string> generatedFields,
        bool strictMode,
        bool htmlEncodeValues)
    {
        var profileType = typeof(RuntimePrintAnchorProfile<>).MakeGenericType(documentType);
        var profile = Activator.CreateInstance(profileType)
            ?? throw new InvalidOperationException($"Не удалось создать профиль якорей для {documentType.Name}.");

        var generatorType = typeof(RuntimeHtmlPrintFormGenerator<>).MakeGenericType(documentType);
        var generator = Activator.CreateInstance(
            generatorType,
            profile,
            generatedFields,
            strictMode,
            htmlEncodeValues)
            ?? throw new InvalidOperationException($"Не удалось создать генератор печатной формы для {documentType.Name}.");

        var generateMethod = generatorType.GetMethod(
            "Generate",
            BindingFlags.Instance | BindingFlags.Public);

        if (generateMethod is null)
        {
            throw new InvalidOperationException($"Метод Generate не найден у генератора {generatorType.Name}.");
        }

        return (HtmlPrintFormGenerationResult)generateMethod.Invoke(
            generator,
            new[] { document, htmlTemplate })!;
    }
}
