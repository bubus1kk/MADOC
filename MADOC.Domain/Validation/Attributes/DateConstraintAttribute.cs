using MADOC.Domain.Validation.Enums;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateConstraintAttribute : ValidationAttribute
    {
        public string MinDate { get; set; } = "";
        public string MaxDate { get; set; } = "";
        public string? DependsOnField { get; set; }
        public DependencyRule Dependency { get; set; } = DependencyRule.None;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not DateOnly)
            {
                return new ValidationResult("Значение должно быть типа DateOnly");
            }

            var currentDate = (DateOnly)value;

            if (!string.IsNullOrWhiteSpace(MinDate))
            {
                var minDate = ParseDate(MinDate);

                if (currentDate < minDate)
                {
                    return new ValidationResult($"Текущая дата не должна быть раньше {MinDate}");
                }
            }

            if (!string.IsNullOrWhiteSpace(MaxDate))
            {
                var maxDate = ParseDate(MaxDate);

                if (currentDate > maxDate)
                {
                    return new ValidationResult($"Текущая дата не должна быть позже {MaxDate}");
                }
            }

            if (Dependency != DependencyRule.None)
            {
                var dependencyResult = ValidateDependency(currentDate, validationContext);

                if (dependencyResult is not null)
                {
                    return dependencyResult;
                }
            }
            return ValidationResult.Success;
        }

        private ValidationResult? ValidateDependency(DateOnly currentDate, ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(DependsOnField))
            {
                return new ValidationResult("Должно быть указано зависимое поле");
            }

            var property = validationContext.ObjectInstance.GetType().GetProperty(DependsOnField, BindingFlags.Instance | BindingFlags.Public);

            if (property is null)
            {
                return new ValidationResult($"Зависимое поле {DependsOnField} не найдено");
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance);

            if (otherValue is not DateOnly)
            {
                return new ValidationResult($"Значение зависимого поля {DependsOnField} должно быть DateOnly.");
            }

            var otherDate = (DateOnly)otherValue;

            if (Dependency == DependencyRule.NotMoreThan && currentDate > otherDate)
            {
                return new ValidationResult($"Текущая дата не должна быть позже даты из поля {DependsOnField}");
            }

            if (Dependency == DependencyRule.NotLessThan && currentDate < otherDate)
            {
                return new ValidationResult($"Текущая дата не должна быть раньше даты из поя {DependsOnField}");
            }

            return null;
        }

        private static DateOnly ParseDate(string maybeDate)
        {
            return DateOnly.ParseExact(maybeDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}

