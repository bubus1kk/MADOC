using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Domain.Validation.ListDependencies
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ListDependencyAttribute : ValidationAttribute
    {
        public IReadOnlyList<string> DependsOnFields { get; }

        public ListDependencyAttribute(params string[] dependsOnFields)
        {
            if (dependsOnFields is null || dependsOnFields.Length == 0)
            {
                throw new ArgumentException("должно быть указано хотя бы одно родительское поле", nameof(dependsOnFields));
            }

            var uniqueFields = new HashSet<string>();

            foreach (var dependsOnField in dependsOnFields)
            {
                if (string.IsNullOrWhiteSpace(dependsOnField))
                {
                    throw new ArgumentException("имя родительского поля не может быть пустым", nameof(dependsOnFields));
                }

                if (!uniqueFields.Add(dependsOnField))
                {
                    throw new ArgumentException($"родительское поле {dependsOnField} указано несколько раз", nameof(dependsOnFields));
                }
            }

            DependsOnFields = dependsOnFields;
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

            var childFieldName = validationContext.MemberName;

            if (string.IsNullOrWhiteSpace(childFieldName))
            {
                return new ValidationResult("Не удалось определить имя зависимого поля.");
            }

            var document = validationContext.ObjectInstance;

            if (document is not IListDependencySchemaProvider schemaProvider)
            {
                return new ValidationResult($"документ {document.GetType().Name} не предоставляет схему зависимостей списков");
            }

            var documentType = document.GetType();

            var parentValues = new Dictionary<string, string>();

            foreach (var parentFieldName in DependsOnFields)
            {
                var parentProperty = documentType.GetProperty(parentFieldName, BindingFlags.Instance | BindingFlags.Public);

                if (parentProperty is null)
                {
                    return new ValidationResult($"родительское поле {parentFieldName} не найдено");
                }

                var parentValue = parentProperty.GetValue(document)?.ToString();

                if (string.IsNullOrWhiteSpace(parentValue))
                {
                    return ValidationResult.Success;
                }

                parentValues.Add(parentFieldName, parentValue);
            }

            var schema = schemaProvider.GetListDependencySchema();

            if (schema is null)
            {
                return new ValidationResult($"документ {documentType.Name} вернул пустую схему зависимостей списков");
            }

            var allowedValues = schema.GetAllowedValues(childFieldName, parentValues);

            foreach (var allowedValue in allowedValues)
            {
                if (allowedValue == currentValue)
                {
                    return ValidationResult.Success;
                }
            }

            var fieldName = validationContext.DisplayName;

            return new ValidationResult($"значение {currentValue}недопустимо для поля {fieldName} при текущих значениях родительских полей");
        }
    }
}