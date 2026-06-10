using MADOC.Domain.Ranges;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeRangeConstraintAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not DateTimeRange)
            {
                return new ValidationResult("Значение должно иметь тип DateTimeRange.");
            }

            var range = (DateTimeRange) value;

            if (range.From > range.To)
            {
                return new ValidationResult("Конец диапазона не должен быть раньше конца");
            }

            return ValidationResult.Success;
        }
    }
}
