using MADOC.Domain.Validation.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListDependencyGraphValidator
    {
        public IReadOnlyList<ValidationResult> Validate(Type documentType)
        {
            ArgumentNullException.ThrowIfNull(documentType);

            var validationResults = new List<ValidationResult>();

            var properties = documentType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            var propertiesByName = properties.ToDictionary(property => property.Name);

            var graph = new Dictionary<string, HashSet<string>>();

            foreach (var property in properties)
            {
                var dependencies = property.GetCustomAttributes<ListDependencyAttribute>().ToList();

                if (dependencies.Count == 0)
                {
                    continue;
                }

                var childListConstraint = property.GetCustomAttribute<ListConstraintAttribute>();

                if (childListConstraint is null)
                {
                    validationResults.Add(new ValidationResult($"Поле {property.Name} имеет  зависимость от другого списка, " +
                        $"но не объявлено как выпадающий список с набором значений"));
                }

                ValidateDuplicatedDependencyRules(property, dependencies, validationResults);

                foreach (var dependency in dependencies)
                {
                    if (!propertiesByName.TryGetValue(dependency.DependsOnField, out var parentProperty))
                    {
                        validationResults.Add(new ValidationResult($"Родительское поле {dependency.DependsOnField} для " +
                            $"поля {property.Name} не существует"));

                        continue;
                    }

                    var parentsListConstraint = parentProperty.GetCustomAttribute<ListConstraintAttribute>();

                    if (parentsListConstraint is null)
                    {
                        validationResults.Add(new ValidationResult($"Родительское поле {dependency.DependsOnField} для поля " +
                            $"{property.Name} не является выпадающим списком"));
                    }

                    else if (!parentsListConstraint.AllowedValues.Contains(dependency.ParentFieldValue))
                    {
                        validationResults.Add(new ValidationResult($"Значение {dependency.ParentFieldValue} не находится среди " +
                            $"допустимых значений поля родительского {dependency.DependsOnField}"));
                    }

                    if (childListConstraint is not null)
                    {
                        foreach (var value in dependency.AllowedFieldValues)
                        {
                            if (!childListConstraint.AllowedValues.Contains(value))
                            {
                                validationResults.Add(new ValidationResult($"Значение {value} поля {property.Name} не найдено " +
                                    $"в списке допустимых значений"));
                            }
                        }
                    }

                    if (!graph.ContainsKey(dependency.DependsOnField))
                    {
                        graph[dependency.DependsOnField] = new HashSet<string>();
                    }

                    graph[dependency.DependsOnField].Add(property.Name);
                }
            }

            if (ContainsCycle(graph))
            {
                validationResults.Add(new ValidationResult("Граф зависимостей не должен содержать циклы"));
            }

            return validationResults;
        }

        private static void ValidateDuplicatedDependencyRules(PropertyInfo property, IReadOnlyList<ListDependencyAttribute> dependencies, List<ValidationResult> validationResults)
        {
            var duplicatedRules = dependencies.GroupBy(depedency => new
            {
                depedency.DependsOnField,
                depedency.ParentFieldValue
            })
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var rule in duplicatedRules)
            {
                validationResults.Add(new ValidationResult($"Поле {property.Name} содержит несколько одинаковых правил " +
                    $"зависимости от поля {rule.Key.DependsOnField} и значения {rule.Key.ParentFieldValue}"));
            }
        }

        private static bool ContainsCycle(Dictionary<string, HashSet<string>> graph)
        {
            var visited = new HashSet<string>();
            var recursionStack = new HashSet<string>();

            foreach (var node in graph.Keys)
            {
                if (HasCycle(node, graph, visited, recursionStack))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasCycle(string node, Dictionary <string, HashSet<string>> graph, HashSet<string> visited,
            HashSet<string> recursionStack)
        {
            if (recursionStack.Contains(node))
            {
                return true;
            }

            if (visited.Contains(node))
            {
                return false;
            }

            visited.Add(node);
            recursionStack.Add(node);

            if (graph.TryGetValue(node, out var nextNodes))
            {
                foreach (var nextNode in nextNodes)
                {
                    if (HasCycle(nextNode, graph, visited, recursionStack))
                    {
                        return true;
                    }
                }
            }

            recursionStack.Remove(node);

            return false;
        }
    }
}
