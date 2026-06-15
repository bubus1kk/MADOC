using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Validation.Enums;

namespace MADOC.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class StringConstraintAttribute : ValidationAttribute
{
    public int MaxLength { get; set; } = int.MaxValue;
    public bool IsMultiline { get; set; }
    public AllowedAlphabet Alphabet { get; set; } = AllowedAlphabet.Any;
    public bool AllowSpecialChars { get; set; } = true;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string text)
        {
            return new ValidationResult("Значение должно быть строкой.");
        }

        if (text.Length > MaxLength)
        {
            return new ValidationResult($"Длина строки не должна быть больше {MaxLength} символов.");
        }

        if (!IsMultiline && (text.Contains('\n') || text.Contains('\r')))
        {
            return new ValidationResult("Многострочный ввод запрещён.");
        }

        if (!AllowSpecialChars && text.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
        {
            return new ValidationResult("Специальные символы запрещены.");
        }

        if (Alphabet == AllowedAlphabet.LatinOnly && text.Any(ch => char.IsLetter(ch) && !IsLatinLetter(ch)))
        {
            return new ValidationResult("Разрешены только латинские буквы.");
        }

        if (Alphabet == AllowedAlphabet.CyrillicOnly && text.Any(ch => char.IsLetter(ch) && !IsCyrillicLetter(ch)))
        {
            return new ValidationResult("Разрешены только кириллические буквы.");
        }

        return ValidationResult.Success;
    }

    private static bool IsLatinLetter(char ch)
    {
        return ch is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
    }

    private static bool IsCyrillicLetter(char ch)
    {
        return ch is >= 'А' and <= 'я' or 'Ё' or 'ё';
    }
}