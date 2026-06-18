using System;
using System.Collections.Generic;
using System.Reflection;
using MADOC.Domain.Validation.Attributes;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListFieldOptionResolver
    {
        public IReadOnlyList<string> GetAvailableValues(object document,string fieldName)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException(
                    "Имя поля не может быть пустым.",
                    nameof(fieldName));
            }

            var documentType = document.GetType();

            var fieldProperty = documentType.GetProperty(fieldName,BindingFlags.Instance | BindingFlags.Public);

            if (fieldProperty is null)
            {
                throw new ArgumentException(
                    $"Поле \"{fieldName}\" не найдено в документе \"{documentType.Name}\".",
                    nameof(fieldName));
            }

            var listConstraint = fieldProperty.GetCustomAttribute<ListConstraintAttribute>();

            if (listConstraint is null)
            {
                throw new InvalidOperationException(
                    $"Поле \"{fieldName}\" не является выпадающим списком. " +
                    $"Для него должен быть указан ListConstraintAttribute.");
            }

            var allValues = new List<string>(listConstraint.AllowedValues);

            var dependency = fieldProperty.GetCustomAttribute<ListDependencyAttribute>();

            if (dependency is null)
            {
                return allValues;
            }

            if (document is not IListDependencySchemaProvider schemaProvider)
            {
                throw new InvalidOperationException(
                    $"Документ \"{documentType.Name}\" не предоставляет схему зависимостей списков.");
            }

            var parentValues = new Dictionary<string, string>();

            foreach (var parentFieldName in dependency.DependsOnFields)
            {
                var parentProperty = documentType.GetProperty(parentFieldName,BindingFlags.Instance | BindingFlags.Public);

                if (parentProperty is null)
                {
                    throw new InvalidOperationException(
                        $"Родительское поле \"{parentFieldName}\" не найдено.");
                }

                var parentValue = parentProperty
                    .GetValue(document)?
                    .ToString();

                if (string.IsNullOrWhiteSpace(parentValue))
                {
                    return Array.Empty<string>();
                }

                parentValues.Add(parentFieldName, parentValue);
            }

            var schema = schemaProvider.GetListDependencySchema();

            if (schema is null)
            {
                throw new InvalidOperationException(
                    $"Документ \"{documentType.Name}\" вернул пустую схему зависимостей списков.");
            }

            var allowedValues = schema.GetAllowedValues(fieldName,parentValues);

            return FilterByListConstraintOrder(
                allValues,
                allowedValues);
        }

        private static IReadOnlyList<string> FilterByListConstraintOrder(IReadOnlyList<string> allValues,
            IReadOnlyList<string> allowedValues)
        {
            var allowedValuesSet = new HashSet<string>(allowedValues);
            var result = new List<string>();

            foreach (var value in allValues)
            {
                if (allowedValuesSet.Contains(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }
}