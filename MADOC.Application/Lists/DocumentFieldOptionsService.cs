using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Application.Lists;

public sealed class DocumentFieldOptionsService
{
    private readonly DocumentFactory documentFactory;

    private readonly DocumentValueMapper valueMapper;

    private readonly ListFieldOptionResolver optionResolver;

    public DocumentFieldOptionsService(
        DocumentFactory documentFactory,
        DocumentValueMapper valueMapper,
        ListFieldOptionResolver? optionResolver = null)
    {
        ArgumentNullException.ThrowIfNull(documentFactory);
        ArgumentNullException.ThrowIfNull(valueMapper);

        this.documentFactory = documentFactory;
        this.valueMapper = valueMapper;
        this.optionResolver = optionResolver ?? new ListFieldOptionResolver();
    }

    public DocumentFieldOptionsResponse GetOptions(DocumentFieldOptionsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var document = documentFactory.Create(request.DocumentType);
        var mappingResult = valueMapper.MapValues(document, request.Values, rejectUnknownFields: false);

        if (!mappingResult.IsSuccess)
        {
            return DocumentFieldOptionsResponse.Failed(
                mappingResult.Errors.Select(error => error.Message));
        }

        try
        {
            var availableKeys = optionResolver.GetAvailableValues(document, request.FieldName);
            var options = CreateOptions(document, request.FieldName, availableKeys);

            return DocumentFieldOptionsResponse.Success(options);
        }
        catch (Exception exception)
        {
            return DocumentFieldOptionsResponse.Failed(new[] { exception.Message });
        }
    }

    private static IReadOnlyList<DocumentFieldOptionDto> CreateOptions(
        object document,
        string fieldName,
        IReadOnlyList<ListOptionKey> availableKeys)
    {
        if (document is not IListConfigurationProvider listConfigurationProvider)
        {
            throw new InvalidOperationException($"Документ {document.GetType().Name} не предоставляет каталог списков.");
        }

        var list = listConfigurationProvider.GetListCatalog().GetList(fieldName);
        var options = new List<DocumentFieldOptionDto>();

        foreach (var key in availableKeys)
        {
            var option = list.GetOption(key);
            options.Add(new DocumentFieldOptionDto(option.Key.Value, option.DisplayName));
        }

        return options;
    }
}
