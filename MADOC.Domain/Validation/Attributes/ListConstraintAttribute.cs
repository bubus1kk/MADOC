using System;
using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ListConstraintAttribute : ValidationAttribute
    {
        public ListConstraintAttribute(params string[] allowedValues)
        {
            ArgumentNullException.ThrowIfNull(allowedValues);

            AllowedValues = allowedValues;
        }

        public IReadOnlyList<string> AllowedValues { get; }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (AllowedValues.Count > 0)
            {
                var currentValue = value.ToString();

                if (string.IsNullOrWhiteSpace(currentValue))
                {
                    return ValidationResult.Success;
                }

                if (AllowedValues.Contains(currentValue))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(
                    $"Значение \"{currentValue}\" недопустимо. " +
                    $"Допустимые значения: {string.Join(", ", AllowedValues)}.");
            }

            if (!ListOptionKeyConverter.TryConvert(value, out var selectedKey))
            {
                return new ValidationResult(
                    $"Значение поля \"{validationContext.DisplayName}\" должно быть ключом варианта списка.");
            }

            var fieldName = validationContext.MemberName;

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return new ValidationResult(
                    "Не удалось определить имя поля выпадающего списка.");
            }

            var document = validationContext.ObjectInstance;

            if (document is not IListConfigurationProvider listConfigurationProvider)
            {
                return new ValidationResult(
                    $"Документ \"{document.GetType().Name}\" не предоставляет каталог списков.");
            }

            var catalog = listConfigurationProvider.GetListCatalog();

            if (catalog is null)
            {
                return new ValidationResult(
                    $"Документ \"{document.GetType().Name}\" вернул пустой каталог списков.");
            }

            if (!catalog.TryGetList(fieldName, out var listDefinition) ||
                listDefinition is null)
            {
                return new ValidationResult(
                    $"Для поля \"{fieldName}\" не найден список в каталоге документа.");
            }

            if (!listDefinition.ContainsKey(selectedKey))
            {
                return new ValidationResult(
                    $"Значение \"{selectedKey}\" недопустимо для поля \"{validationContext.DisplayName}\".");
            }

            return ValidationResult.Success;
        }
    }
}
