using MADOC.Domain.Core.Validation.Lists;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Domain.Core.Validation.ListDependencies
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

            if (!ListOptionKeyConverter.TryConvert(value, out var currentValueKey))
            {
                return new ValidationResult($"значение поля {validationContext.DisplayName} должно быть ключом варианта списка");
            }

            var childFieldName = validationContext.MemberName;

            if (string.IsNullOrWhiteSpace(childFieldName))
            {
                return new ValidationResult("не удалось определить имя зависимого поля");
            }

            var document = validationContext.ObjectInstance;

            if (document is not IListDependencySchemaProvider schemaProvider)
            {
                return new ValidationResult($"документ {document.GetType().Name} не предоставляет схему зависимостей списков");
            }

            var documentType = document.GetType();
            var parentValues = new Dictionary<string, ListOptionKey>();
            
            foreach (var parentFieldName in DependsOnFields)
            {
                var parentProperty = documentType.GetProperty(parentFieldName, BindingFlags.Instance | BindingFlags.Public);

                if (parentProperty is null)
                {
                    return new ValidationResult($"родительское поле {parentFieldName} не найдено");
                }

                var parentValue = parentProperty.GetValue(document);

                if (parentValue is null)
                {
                    return ValidationResult.Success;
                }

                if (!ListOptionKeyConverter.TryConvert(parentValue, out var parentValueKey))
                {
                    return new ValidationResult($"значение родительского поля {parentFieldName} должно быть ключом варианта списка");
                }

                parentValues.Add(parentFieldName, parentValueKey);
            }

            var schema = schemaProvider.GetListDependencySchema();

            if (schema is null)
            {
                return new ValidationResult($"документ {documentType.Name} вернул пустую схему зависимостей списков");
            }

            var allowedValues = schema.GetAllowedValues(childFieldName, parentValues);

            foreach (var allowedValue in allowedValues)
            {
                if (allowedValue == currentValueKey)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"значение {currentValueKey} недопустимо для поля {validationContext.DisplayName} " +
                $"при текущих значениях родительских полей");
        }
    }
}