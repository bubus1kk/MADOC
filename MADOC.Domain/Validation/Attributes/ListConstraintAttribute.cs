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

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var currentValue = value.ToString();

        if (string.IsNullOrWhiteSpace(currentValue))
        {
            return ValidationResult.Success;
        }

        if (!AllowedValues.Contains(currentValue))
        {
            return new ValidationResult(
                $"Значение \"{currentValue}\" недопустимо. " +
                $"Допустимые значения: {string.Join(", ", AllowedValues)}.");
        }

        return ValidationResult.Success;
    }
}