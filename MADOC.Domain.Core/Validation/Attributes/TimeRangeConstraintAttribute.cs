using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Core.Ranges;

namespace MADOC.Domain.Core.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TimeRangeConstraintAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not TimeRange range)
        {
            return new ValidationResult("Значение должно быть типа TimeRange.");
        }

        if (range.From > range.To)
        {
            return new ValidationResult("Время начала диапазона не должно быть позже времени окончания.");
        }

        return ValidationResult.Success;
    }
}