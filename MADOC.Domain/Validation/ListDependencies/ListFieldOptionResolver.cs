using System.Reflection;
using MADOC.Domain.Validation.Attributes;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListFieldOptionResolver
    {
        public IReadOnlyList<string> GetAvailableValues(object document, string fieldName)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException(
                    "Имя поля не может быть пустым.",
                    nameof(fieldName));
            }

            var documentType = document.GetType();// Получаем тип поля, для которого нужно определить доступные значения

            var fieldProperty = documentType.GetProperty(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public);// Ищем свойство с указанным именем

            if (fieldProperty is null)
            {
                throw new ArgumentException(
                    $"Поле \"{fieldName}\" не найдено в документе \"{documentType.Name}\".",
                    nameof(fieldName));
            }

            var listConstraint = fieldProperty.GetCustomAttribute<ListConstraintAttribute>();// Получаем атрибут ListConstraintAttribute, если он есть

            if (listConstraint is null)
            {
                throw new InvalidOperationException(
                    $"Поле \"{fieldName}\" не является выпадающим списком. " +
                    $"Для него должен быть указан ListConstraintAttribute.");
            }

            var allValues = listConstraint.AllowedValues.ToList();// Получаем все возможные значения из атрибута ListConstraintAttribute

            var dependencies = fieldProperty
                .GetCustomAttributes<ListDependencyAttribute>()
                .ToList();// Получаем все атрибуты ListDependencyAttribute, которые определяют зависимости для данного поля

            if (dependencies.Count == 0)
            {
                return allValues;
            }// Если зависимостей нет, возвращаем все возможные значения

            var allowedByParentGroups = new List<HashSet<string>>();// Создаем список, который будет содержать множества допустимых значений для каждого родительского поля

            var dependenciesGroupedByParent = dependencies
                .GroupBy(dependency => dependency.DependsOnField);// Группируем зависимости по родительскому полю

            foreach (var parentGroup in dependenciesGroupedByParent)// Проходим по каждой группе зависимостей, сгруппированных по родительскому полю
            {
                var parentFieldName = parentGroup.Key;

                var parentProperty = documentType.GetProperty(
                    parentFieldName,
                    BindingFlags.Instance | BindingFlags.Public);

                if (parentProperty is null)
                {
                    throw new InvalidOperationException(
                        $"Родительское поле \"{parentFieldName}\" не найдено.");
                }

                var parentValue = parentProperty
                    .GetValue(document)?
                    .ToString();

                if (string.IsNullOrWhiteSpace(parentValue))// Если значение родительского поля пустое, возвращаем пустой список доступных значений
                {
                    return Array.Empty<string>();
                }

                var matchingRules = parentGroup
                    .Where(rule => rule.ParentFieldValue == parentValue)
                    .ToList();// Находим все правила, которые соответствуют текущему значению родительского поля

                if (matchingRules.Count == 0)
                {
                    return Array.Empty<string>();
                }

                var allowedForCurrentParent = matchingRules
                    .SelectMany(rule => rule.AllowedFieldValues)
                    .ToHashSet();// Получаем множество допустимых значений для текущего родительского поля

                allowedByParentGroups.Add(allowedForCurrentParent);// Добавляем множество допустимых значений в список
            }

            var availableValues = allValues
                .Where(value => allowedByParentGroups.All(group => group.Contains(value)))
                .ToList();// Находим значения, которые присутствуют во всех множествах допустимых значений для каждого родительского поля

            return availableValues;// Возвращаем итоговый список доступных значений для данного поля, учитывая все зависимости
        }
    }
}