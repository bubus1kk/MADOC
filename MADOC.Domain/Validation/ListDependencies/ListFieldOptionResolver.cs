using System;
using System.Collections.Generic;
using System.Reflection;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListFieldOptionResolver
    {
        public IReadOnlyList<ListOptionKey> GetAvailableValues(object document, string fieldName)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException(
                    "Имя поля не может быть пустым.",
                    nameof(fieldName));
            }

            var documentType = document.GetType();
            var fieldProperty = documentType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);

            if (fieldProperty is null)
            {
                throw new ArgumentException(
                    $"Поле \"{fieldName}\" не найдено в документе \"{documentType.Name}\".",
                    nameof(fieldName));
            }

            if (fieldProperty.GetCustomAttribute<ListConstraintAttribute>() is null)
            {
                throw new InvalidOperationException(
                    $"Поле \"{fieldName}\" не является выпадающим списком.");
            }

            if (document is not IListConfigurationProvider configurationProvider)
            {
                throw new InvalidOperationException(
                    $"Документ \"{documentType.Name}\" не предоставляет каталог списков.");
            }

            var catalog = configurationProvider.GetListCatalog();
            var listDefinition = catalog.GetList(fieldName);
            var allValues = GetOptionKeys(listDefinition);

            var dependency = fieldProperty.GetCustomAttribute<ListDependencyAttribute>();

            if (dependency is null)
            {
                return allValues;
            }

            var parentValues = new Dictionary<string, ListOptionKey>();

            foreach (var parentFieldName in dependency.DependsOnFields)
            {
                var parentProperty = documentType.GetProperty(parentFieldName, BindingFlags.Instance | BindingFlags.Public);

                if (parentProperty is null)
                {
                    throw new InvalidOperationException(
                        $"Родительское поле \"{parentFieldName}\" не найдено.");
                }

                var parentValue = parentProperty.GetValue(document);

                if (parentValue is null)
                {
                    return Array.Empty<ListOptionKey>();
                }

                if (!ListOptionKeyConverter.TryConvert(parentValue, out var parentValueKey))
                {
                    throw new InvalidOperationException(
                        $"Значение родительского поля \"{parentFieldName}\" должно быть ListOptionKey.");
                }

                parentValues.Add(parentFieldName, parentValueKey);
            }

            var schema = configurationProvider.GetListDependencySchema();
            var allowedValues = schema.GetAllowedValues(fieldName, parentValues);

            return FilterByListConstraintOrder(allValues, allowedValues);
        }

        private static IReadOnlyList<ListOptionKey> GetOptionKeys(ListDefinition listDefinition)
        {
            var result = new List<ListOptionKey>();

            foreach (var option in listDefinition.Options)
            {
                result.Add(option.Key);
            }

            return result;
        }

        private static IReadOnlyList<ListOptionKey> FilterByListConstraintOrder(
            IReadOnlyList<ListOptionKey> allValues,
            IReadOnlyList<ListOptionKey> allowedValues)
        {
            var allowedValuesSet = new HashSet<ListOptionKey>(allowedValues);
            var result = new List<ListOptionKey>();

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