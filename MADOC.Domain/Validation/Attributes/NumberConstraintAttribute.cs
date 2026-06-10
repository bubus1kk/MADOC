using MADOC.Domain.Validation.Enums;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberConstraintAttribute : ValidationAttribute
    {
        public double MinValue { get; set; } = double.MinValue;
        public double MaxValue { get; set; } = double.MaxValue;
        public bool AllowFloats { get; set; } = true;
        public string? DependsOnField { get; set; }
        public DependencyRule Dependency { get; set; } = DependencyRule.None;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (!TryConvertToDouble(value, out var number))
            {
                return new ValidationResult("Значение должно быть числом");
            }

            if (number < MinValue)
            {
                return new ValidationResult($"Число не может быть меньше минимального значения, равного {MinValue}");
            }

            if (number > MaxValue)
            {
                return new ValidationResult($"Число не может быть больше максимального значения, равного {MaxValue}");
            }

            if (!AllowFloats && !IsWholeNumber(number))
            {
                return new ValidationResult("Числа с плавающей точкой запрещены");
            }

            if (Dependency != DependencyRule.None)
            {
                var dependencyResult = ValidateDependency(number, validationContext);

                if (dependencyResult != null)
                {
                    return dependencyResult;
                }
            }

            return ValidationResult.Success;
        }

        private static bool TryConvertToDouble(object? value, out double result)
        {
            result = 0;

            if (value is null)
            {
                return false;
            }

            try
            {
                result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
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

        private ValidationResult? ValidateDependency(double currentValue, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(DependsOnField))
            {
                return new ValidationResult("Необходимо указать имя зависимого поля");
            }

            var objectType = validationContext.ObjectInstance.GetType();

            var dependentProperty = objectType.GetProperty(DependsOnField, BindingFlags.Instance | BindingFlags.Public);

            if (dependentProperty == null)
            {
                return new ValidationResult($"Зависисмое поле {DependsOnField} не найдено");
            }

            var dependentVaule = dependentProperty.GetValue(validationContext.ObjectInstance);

            if (!TryConvertToDouble(dependentVaule, out var otherValue))
            {
                return new ValidationResult($"Значаение зависимого поля {DependsOnField} должно быть числом");
            }

            if (Dependency == DependencyRule.NotMoreThan && currentValue > otherValue)
            {
                return new ValidationResult($"Текущее знаение должно быть не больше, чем  значение {DependsOnField}");
            }

            if (Dependency == DependencyRule.NotLessThan && currentValue < otherValue)
            {
                return new ValidationResult($"Текущее знаение должно быть не меньше, чем значение {DependsOnField}");
            }

            return null;
        }
    }
}
