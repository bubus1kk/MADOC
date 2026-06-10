using MADOC.Domain.Validation.Enums;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TimeConstraintAttribute : ValidationAttribute
    {
        public string MinTime { get; set; } = "";
        public string MaxTime { get; set; } = "";
        public string? DependsOnField { get; set; }
        public DependencyRule Dependency { get; set; } = DependencyRule.None;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is not TimeOnly)
            {
                return new ValidationResult("Значение должно иметь тип TimeOnly");
            }

            var currentTime = (TimeOnly)value;

            if (!string.IsNullOrWhiteSpace(MinTime))
            {
                var minTime = ParseTime(MinTime);

                if (currentTime < minTime)
                {
                    return new ValidationResult($"Время не должно быть раньше чем минимально возможное {MinTime}");
                }
            }

            if (!string.IsNullOrWhiteSpace(MaxTime))
            {
                var maxTime = ParseTime(MaxTime);

                if (currentTime > maxTime)
                {
                    return new ValidationResult($"Время не должно быть позже чем максимально возможное {MaxTime}");
                }
            }

            if (Dependency != DependencyRule.None)
            {
                var dependencyResult = ValidateDependency(currentTime, validationContext);

                if (dependencyResult is not null)
                {
                    return dependencyResult;
                }

                return ValidationResult.Success;
            }

            return ValidationResult.Success;
        }

        private ValidationResult? ValidateDependency(TimeOnly currentTime, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(DependsOnField))
            {
                return new ValidationResult("Не указано поле для сравнения");
            }

            var property = validationContext.ObjectInstance.GetType().GetProperty(DependsOnField, BindingFlags.Instance | BindingFlags.Public);

            if (property == null)
            {
                return new ValidationResult($"Сравниваемое поле {DependsOnField} не найдено");
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance);

            if (otherValue is not TimeOnly)
            {
                return new ValidationResult($"Значение сравниваемого поля {DependsOnField} должно быть типа TimeOnly");
            }

            var otherTime = (TimeOnly)otherValue;

            if (Dependency == DependencyRule.NotMoreThan && currentTime > otherTime) 
            {
                return new ValidationResult($"Время не должно быть позже, чем время в поле {DependsOnField}");          
            }

            if (Dependency == DependencyRule.NotLessThan && currentTime < otherTime)
            {
                return new ValidationResult($"Время не должно быть раньше, чем время в поле {DependsOnField}");
            }

            return null;
        }

        private static TimeOnly ParseTime(string value)
        {
            return TimeOnly.ParseExact(value,"HH:mm",CultureInfo.InvariantCulture);
        }

    }
}
