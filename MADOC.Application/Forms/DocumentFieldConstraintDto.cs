namespace MADOC.Application.Forms;

public sealed class DocumentFieldConstraintDto
{
    public bool IsRequired { get; }

    public int? MaxLength { get; }

    public int? MinLength { get; }

    public double? MinNumber { get; }

    public double? MaxNumber { get; }

    public bool? AllowFloats { get; }

    public string? MinDate { get; }

    public string? MaxDate { get; }

    public string? MinTime { get; }

    public string? MaxTime { get; }

    public string? Alphabet { get; }

    public bool? AllowSpecialChars { get; }

    public bool? IsMultiline { get; }

    public DocumentFieldConstraintDto(
        bool isRequired,
        int? maxLength = null,
        int? minLength = null,
        double? minNumber = null,
        double? maxNumber = null,
        bool? allowFloats = null,
        string? minDate = null,
        string? maxDate = null,
        string? minTime = null,
        string? maxTime = null,
        string? alphabet = null,
        bool? allowSpecialChars = null,
        bool? isMultiline = null)
    {
        IsRequired = isRequired;
        MaxLength = maxLength;
        MinLength = minLength;
        MinNumber = minNumber;
        MaxNumber = maxNumber;
        AllowFloats = allowFloats;
        MinDate = minDate;
        MaxDate = maxDate;
        MinTime = minTime;
        MaxTime = maxTime;
        Alphabet = alphabet;
        AllowSpecialChars = allowSpecialChars;
        IsMultiline = isMultiline;
    }
}
