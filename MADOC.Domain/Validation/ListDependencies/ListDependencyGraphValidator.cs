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

            var properties = documentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var propertiesByName = new Dictionary<string, PropertyInfo>();

            foreach (var property in properties)
            {
                propertiesByName.Add(property.Name, property);
            }

            var graph = new Dictionary<string, HashSet<string>>();
            var attributeConnections = new HashSet<string>();
            var documentHasDependencies = false;

            foreach (var property in properties)
            {
                var dependency = property.GetCustomAttribute<ListDependencyAttribute>();

                if (dependency is null)
                {
                    continue;
                }

                documentHasDependencies = true;

                ValidateChildFieldName(property, validationResults);

                foreach (var ParentFieldNameName in dependency.DependsOnFields)
                {
                    var connectionKey = CreateConnectionKey(property.Name, ParentFieldNameName);

                    attributeConnections.Add(connectionKey);

                    ValidateParentFieldName(property.Name, ParentFieldNameName, propertiesByName, validationResults);

                    AddGraphEdge(graph, ParentFieldNameName, property.Name);
                }
            }

            if (documentHasDependencies)
            {
                var schema = TryGetSchema(documentType, validationResults);

                if (schema is not null)
                {
                    ValidateSchemaRules(schema, propertiesByName, attributeConnections, validationResults);

                    ValidateAttributeConnectionsHaveSchemaRules(schema, attributeConnections, validationResults);
                }
            }

            if (ContainsCycle(graph))
            {
                validationResults.Add(new ValidationResult("граф зависимостей списков не должен содержать циклы"));
            }

            return validationResults;
        }

        private static void ValidateChildFieldName(PropertyInfo childProperty, List<ValidationResult> validationResults)
        {
            var childListConstraint = childProperty.GetCustomAttribute<ListConstraintAttribute>();

            if (childListConstraint is null)
            {
                validationResults.Add(new ValidationResult($"поле {childProperty.Name} имеет зависимость от другого списка, " +
                        "но не объявлено как выпадающий список"));
            }
        }

        private static void ValidateParentFieldName(string ChildFieldNameName, string ParentFieldNameName, Dictionary<string,
            PropertyInfo> propertiesByName, List<ValidationResult> validationResults)
        {
            if (!propertiesByName.TryGetValue(ParentFieldNameName, out var parentProperty))
            {
                validationResults.Add(new ValidationResult($"Родительское поле {ParentFieldNameName} для поля {ChildFieldNameName} не существует"));

                return;
            }

            var parentListConstraint = parentProperty.GetCustomAttribute<ListConstraintAttribute>();

            if (parentListConstraint is null)
            {
                validationResults.Add(new ValidationResult($"родительское поле {ParentFieldNameName} для поля {ChildFieldNameName} " +
                    "не является выпадающим списком"));
            }
        }

        private static ListDependencySchema? TryGetSchema(Type documentType, List<ValidationResult> validationResults)
        {
            if (!typeof(IListDependencySchemaProvider).IsAssignableFrom(documentType))
            {
                validationResults.Add(new ValidationResult($"документ {documentType.Name} содержит зависимости списков, " +
                        "но не реализует IListDependencySchemaProvider."));

                return null;
            }

            if (documentType.IsAbstract)
            {
                validationResults.Add(new ValidationResult($"нельзя создать экземпляр абстрактного документа {documentType.Name}" +
                        "для получения схемы зависимостей"));

                return null;
            }

            object? documentInstance;

            try
            {
                documentInstance = Activator.CreateInstance(documentType);
            }
            catch (Exception exception)
            {
                validationResults.Add(new ValidationResult($"Не удалось создать экземпляр документа {documentType.Name} " +
                        $"для получения схемы зависимостей. Ошибка: {exception.Message}"));

                return null;
            }

            if (documentInstance is not IListDependencySchemaProvider schemaProvider)
            {
                validationResults.Add(new ValidationResult($"Документ {documentType.Name} не предоставляет схему " +
                    $"зависимостей списков"));

                return null;
            }

            var schema = schemaProvider.GetListDependencySchema();

            if (schema is null)
            {
                validationResults.Add(new ValidationResult($"Документ {documentType.Name} вернул пустую схему " +
                    $"зависимостей списков."));

                return null;
            }

            return schema;
        }

        private static void ValidateSchemaRules(ListDependencySchema schema, Dictionary<string, PropertyInfo> propertiesByName,
            HashSet<string> attributeConnections, List<ValidationResult> validationResults)
        {
            var ruleKeys = new HashSet<string>();

            foreach (var rule in schema.Rules)
            {
                var ruleKey = CreateRuleKey(rule.ChildFieldName, rule.ParentFieldName, rule.ParentFieldValue);

                if (!ruleKeys.Add(ruleKey))
                {
                    validationResults.Add(new ValidationResult(
                            $"В схеме зависимостей найдено повторяющееся правило: поле {rule.ChildFieldName} зависит от поля " +
                            $"{rule.ParentFieldName} при значении {rule.ParentFieldValue}"));

                    continue;
                }

                ValidateSchemaConnectionExistsInAttributes(rule, attributeConnections, validationResults);

                if (!propertiesByName.TryGetValue(rule.ChildFieldName, out var childProperty))
                {
                    validationResults.Add(new ValidationResult($"в схеме зависимостей указано несуществующее зависимое поле {rule.ChildFieldName}"));

                    continue;
                }

                if (!propertiesByName.TryGetValue(rule.ParentFieldName, out var parentProperty))
                {
                    validationResults.Add(new ValidationResult($"в схеме зависимостей указано несуществующее родительское поле {rule.ParentFieldName}"));

                    continue;
                }

                var childListConstraint = childProperty.GetCustomAttribute<ListConstraintAttribute>();
                var parentListConstraint = parentProperty.GetCustomAttribute<ListConstraintAttribute>();

                if (childListConstraint is null)
                {
                    validationResults.Add(new ValidationResult($"зависимое поле {rule.ChildFieldName} из схемы не имеет " +
                        $"ListConstraintAttribute"));

                    continue;
                }

                if (parentListConstraint is null)
                {
                    validationResults.Add(new ValidationResult($"родительское поле {rule.ParentFieldName} из схемы не имеет " +
                        $"ListConstraintAttribute"));

                    continue;
                }

                ValidateParentFieldNameValue(rule, parentListConstraint, validationResults);

                ValidateAllowedChildValues(rule, childListConstraint, validationResults);
            }
        }

        private static void ValidateSchemaConnectionExistsInAttributes(ListDependencyRule rule, HashSet<string> attributeConnections,
            List<ValidationResult> validationResults)
        {
            var connectionKey = CreateConnectionKey(rule.ChildFieldName, rule.ParentFieldName);

            if (!attributeConnections.Contains(connectionKey))
            {
                validationResults.Add(new ValidationResult($"в схеме есть связь {rule.ParentFieldName} → {rule.ChildFieldName}, " +
                        "но она не объявлена через ListDependencyAttribute"));
            }
        }

        private static void ValidateAttributeConnectionsHaveSchemaRules(ListDependencySchema schema, HashSet<string> attributeConnections,
            List<ValidationResult> validationResults)
        {
            foreach (var connectionKey in attributeConnections)
            {
                var parts = connectionKey.Split('|');

                var ChildFieldName = parts[0];
                var ParentFieldName = parts[1];

                if (!schema.HasRulesForConnection(ChildFieldName, ParentFieldName))
                {
                    validationResults.Add(new ValidationResult($"поле {ChildFieldName} объявило зависимость от поля {ParentFieldName} " +
                            "через ListDependencyAttribute, но в схеме зависимостей нет правил для этой связи"));
                }
            }
        }

        private static void ValidateParentFieldNameValue(ListDependencyRule rule, ListConstraintAttribute parentListConstraint,
            List<ValidationResult> validationResults)
        {
            if (!ContainsValue(parentListConstraint.AllowedValues, rule.ParentFieldValue))
            {
                validationResults.Add(new ValidationResult($"значение {rule.ParentFieldValue} не найдено среди допустимых значений " +
                        $"родительского поля {rule.ParentFieldName}"));
            }
        }

        private static void ValidateAllowedChildValues(ListDependencyRule rule, ListConstraintAttribute childListConstraint,
            List<ValidationResult> validationResults)
        {
            foreach (var allowedChildValue in rule.AllowedChildFieldValues)
            {
                if (!ContainsValue(childListConstraint.AllowedValues, allowedChildValue))
                {
                    validationResults.Add(new ValidationResult($"значение {allowedChildValue} из правила зависимости не найдено " +
                            $"среди допустимых значений поля {rule.ChildFieldName}"));
                }
            }
        }

        private static bool ContainsValue(IReadOnlyList<string> values, string value)
        {
            foreach (var currentValue in values)
            {
                if (currentValue == value)
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddGraphEdge(Dictionary<string, HashSet<string>> graph, string ParentFieldName, string ChildFieldName)
        {
            if (!graph.ContainsKey(ParentFieldName))
            {
                graph[ParentFieldName] = new HashSet<string>();
            }

            graph[ParentFieldName].Add(ChildFieldName);
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

        private static bool HasCycle(string node, Dictionary<string, HashSet<string>> graph, HashSet<string> visited, HashSet<string> recursionStack)
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

        private static string CreateConnectionKey(string ChildFieldName, string ParentFieldName)
        {
            return $"{ChildFieldName}|{ParentFieldName}";
        }

        private static string CreateRuleKey(string ChildFieldName, string ParentFieldName, string ParentFieldNameValue)
        {
            return $"{ChildFieldName}|{ParentFieldName}|{ParentFieldNameValue}";
        }
    }
}