using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Ranges;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberRangeConstraintAtribute : ValidationAttribute
    {
        public double MinValue { get; set; } = double.MinValue;
        public double MaxValue { get; set; } = double.MaxValue;
        public bool AllowFloats { get; set; } = true;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not NumberRange range)
            {
                return new ValidationResult("Значение должно быть типа NumberRange.");
            }

            if (range.From > range.To)
            {
                return new ValidationResult("Нижняя граница диапазона не может быть больше верхней.");
            }

            if (range.From < MinValue)
            {
                return new ValidationResult(
                    $"Нижняя граница диапазона не может быть меньше {MinValue}.");
            }

            if (range.To > MaxValue)
            {
                return new ValidationResult(
                    $"Верхняя граница диапазона не может быть больше {MaxValue}.");
            }

            if (!AllowFloats && (!IsWholeNumber(range.From) || !IsWholeNumber(range.To)))
            {
                return new ValidationResult("Числа с плавающей точкой запрещены.");
            }

            return ValidationResult.Success;
        }

        private static bool IsWholeNumber(double value)
        {
            return value % 1 == 0;
        }
    }
}