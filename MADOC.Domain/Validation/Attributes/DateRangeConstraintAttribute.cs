using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Ranges;

namespace MADOC.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DateRangeConstraintAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not DateRange range)
        {
            return new ValidationResult("Значение должно быть типа DateRange.");
        }

        if (range.From > range.To)
        {
            return new ValidationResult("Дата начала диапазона не должна быть позже даты окончания.");
        }

        return ValidationResult.Success;
    }
}