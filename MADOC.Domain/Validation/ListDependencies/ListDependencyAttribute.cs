using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Domain.Validation.ListDependencies
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListDependencyAttribute : ValidationAttribute
    {
        public string DependsOnField { get; }
        public string ParentFieldValue { get; }
        public IReadOnlyList<string> AllowedFieldValues { get; }

        public ListDependencyAttribute(string dependsOnField, string parentFieldValue, params string[] allowedFieldValues)
        {
            if (string.IsNullOrWhiteSpace(dependsOnField))
            {
                throw new ArgumentException("Имя родительсокго поля не может быть пустым", nameof(dependsOnField));
            }

            if (string.IsNullOrWhiteSpace(parentFieldValue))
            {
                throw new ArgumentException("Значение родительского поля не может быть пустым", nameof(parentFieldValue));
            }

            if (allowedFieldValues.Length == 0)
            {
                throw new ArgumentException("Список допустимых значений не может быть пустым", nameof(allowedFieldValues));
            }

            this.DependsOnField = dependsOnField;
            this.ParentFieldValue = parentFieldValue;
            this.AllowedFieldValues = allowedFieldValues;
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

            var documentType = validationContext.ObjectInstance.GetType();

            var parentProperty = documentType.GetProperty(DependsOnField, BindingFlags.Instance | BindingFlags.Public);

            if (parentProperty is null)
            {
                return new ValidationResult($"Поле {DependsOnField} не найдено");
            }

            var parentFieldValue = parentProperty.GetValue(validationContext.ObjectInstance)?.ToString();

            if (string.IsNullOrWhiteSpace(parentFieldValue))
            {
                return ValidationResult.Success;
            }

            if (parentFieldValue != ParentFieldValue)
            {
                return ValidationResult.Success;
            }

            if (!AllowedFieldValues.Contains(currentValue))
            {
                var fieldName = validationContext.DisplayName;

                return new ValidationResult($"Значение {currentValue} недопустимо для поля {fieldName}");
            }

            return ValidationResult.Success;
        }
    }
}
