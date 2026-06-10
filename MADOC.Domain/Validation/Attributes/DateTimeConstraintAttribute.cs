using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using MADOC.Domain.Validation.Enums;

namespace MADOC.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DateTimeConstraintAttribute : ValidationAttribute
{
    public string Min { get; set; } = "";
    public string Max { get; set; } = "";

    public string? DependsOnField { get; set; }
    public DependencyRule Dependency { get; set; } = DependencyRule.None;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not DateTime currentDateTime)
            return new ValidationResult("Значение должно быть типа DateTime.");

        if (!string.IsNullOrWhiteSpace(Min))
        {
            var minDateTime = ParseDateTime(Min);

            if (currentDateTime < minDateTime)
                return new ValidationResult($"Дата и время не должны быть раньше {Min}.");
        }

        if (!string.IsNullOrWhiteSpace(Max))
        {
            var maxDateTime = ParseDateTime(Max);

            if (currentDateTime > maxDateTime)
                return new ValidationResult($"Дата и время не должны быть позже {Max}.");
        }

        if (Dependency != DependencyRule.None)
        {
            var dependencyResult = ValidateDependency(currentDateTime, validationContext);

            if (dependencyResult is not null)
                return dependencyResult;
        }

        return ValidationResult.Success;
    }

    private ValidationResult? ValidateDependency(DateTime currentDateTime, ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(DependsOnField))
            return new ValidationResult("Поле DependsOnField должно быть указано, если используется зависимость.");

        var property = validationContext.ObjectInstance
            .GetType()
            .GetProperty(DependsOnField, BindingFlags.Instance | BindingFlags.Public);

        if (property is null)
            return new ValidationResult($"Зависимое поле '{DependsOnField}' не найдено.");

        var otherValue = property.GetValue(validationContext.ObjectInstance);

        if (otherValue is not DateTime otherDateTime)
            return new ValidationResult($"Зависимое поле '{DependsOnField}' должно быть типа DateTime.");

        if (Dependency == DependencyRule.NotMoreThan && currentDateTime > otherDateTime)
            return new ValidationResult($"Дата и время не должны быть позже поля '{DependsOnField}'.");

        if (Dependency == DependencyRule.NotLessThan && currentDateTime < otherDateTime)
            return new Valid    ationResult($"Дата и время не должны быть раньше поля '{DependsOnField}'.");

        return null;
    }

    private static DateTime ParseDateTime(string value)
    {
        return DateTime.ParseExact(
            value,
            "yyyy-MM-dd HH:mm",
            CultureInfo.InvariantCulture);
    }
}