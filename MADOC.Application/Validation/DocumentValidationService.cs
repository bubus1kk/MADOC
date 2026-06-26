using System.ComponentModel.DataAnnotations;
using MADOC.Application.Documents;

namespace MADOC.Application.Validation;

public sealed class DocumentValidationService
{
    private const string DocumentLevelFieldName = "__document";

    private readonly DocumentFactory documentFactory;

    private readonly DocumentValueMapper valueMapper;

    public DocumentValidationService(DocumentFactory documentFactory, DocumentValueMapper valueMapper)
    {
        ArgumentNullException.ThrowIfNull(documentFactory);
        ArgumentNullException.ThrowIfNull(valueMapper);

        this.documentFactory = documentFactory;
        this.valueMapper = valueMapper;
    }

    public DocumentValidationResponse Validate(DocumentValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var document = documentFactory.Create(request.DocumentType);
        var mappingResult = valueMapper.MapValues(document, request.Values);

        if (!mappingResult.IsSuccess)
        {
            return new DocumentValidationResponse(request.DocumentType, isValid: false,
                mappingResult.Errors.Select(error => new DocumentValidationErrorDto(error.FieldName, error.Message)));
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(document);

        var isValid = Validator.TryValidateObject(document, validationContext, validationResults, validateAllProperties: true);

        var errors = ConvertErrors(validationResults);

        return new DocumentValidationResponse(request.DocumentType, isValid, errors);
    }

    private static IReadOnlyList<DocumentValidationErrorDto> ConvertErrors(IReadOnlyList<ValidationResult> validationResults)
    {
        var errors = new List<DocumentValidationErrorDto>();

        foreach (var validationResult in validationResults)
        {
            var message = validationResult.ErrorMessage ?? "Документ содержит некорректное значение";
            var memberNames = validationResult.MemberNames.ToArray();

            if (memberNames.Length == 0)
            {
                errors.Add(new DocumentValidationErrorDto(DocumentLevelFieldName, message));
                continue;
            }

            foreach (var memberName in memberNames)
            {
                errors.Add(new DocumentValidationErrorDto(memberName, message));
            }
        }

        return errors;
    }
}
