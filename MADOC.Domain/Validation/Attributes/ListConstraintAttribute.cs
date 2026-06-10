using System.ComponentModel.DataAnnotations;

namespace MADOC.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ListConstraintAttribute : ValidationAttribute
{
    public IReadOnlyList<string> AllowedValues { get; }

    public ListConstraintAttribute(params string[] allowedValues)
    {
        if (allowedValues.Length == 0)
            throw new ArgumentException("Список допустимых значений не может быть пустым.", nameof(allowedValues));

        AllowedValues = allowedValues;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)// Проверяем, что значение является строкой и входит в список допустимых значений
    {
        if (value is null)
            return ValidationResult.Success;

        var text = value.ToString();// Преобразуем значение в строку для проверки

        if (string.IsNullOrWhiteSpace(text))
            return ValidationResult.Success;

        if (!AllowedValues.Contains(text))
        {
            return new ValidationResult(
                $"Значение '{text}' недопустимо. Допустимые значения: {string.Join(", ", AllowedValues)}.");
        }

        return ValidationResult.Success;
    }
}