using MADOC.Domain.Ranges;
using System.ComponentModel.DataAnnotations;

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
                return new ValidationResult("Значение должно находиться в заданном числовом диапазоне");
            }

            if (range.From > range.To)
            {
                return new ValidationResult("Нижняя граница диапазона значений не может быть больше верхней");
            }

            if (!AllowFloats && (!IsWholeNumber(range.From) || !IsWholeNumber(range.To)))
            {
                return new ValidationResult("Числа с плавающей точкой запрещены");
            }

            return ValidationResult.Success;
        }

        private static bool IsWholeNumber(double value)
        {
            if (value % 1 == 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
    }
}
