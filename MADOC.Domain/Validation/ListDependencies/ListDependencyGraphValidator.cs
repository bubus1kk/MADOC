using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Lists;
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

            var documentHasListFields = false;
            var documentHasDependencies = false;

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<ListConstraintAttribute>() is not null)
                {
                    documentHasListFields = true;
                }

                var dependency = property.GetCustomAttribute<ListDependencyAttribute>();

                if (dependency is null)
                {
                    continue;
                }

                documentHasDependencies = true;

                ValidateChildField(property, validationResults);

                foreach (var parentFieldName in dependency.DependsOnFields)
                {
                    var connectionKey = CreateConnectionKey(property.Name, parentFieldName);

                    attributeConnections.Add(connectionKey);

                    ValidateParentField(property.Name, parentFieldName, propertiesByName, validationResults);

                    AddGraphEdge(graph, parentFieldName, property.Name);
                }
            }

            var documentProvidesConfiguration = typeof(IListConfigurationProvider).IsAssignableFrom(documentType);

            if (documentHasListFields || documentHasDependencies || documentProvidesConfiguration)
            {
                var configurationProvider = TryGetConfigurationProvider(documentType, validationResults);

                if (configurationProvider is not null)
                {
                    var catalog = configurationProvider.GetListCatalog();
                    var schema = configurationProvider.GetListDependencySchema();

                    if (catalog is null)
                    {
                        validationResults.Add(new ValidationResult($"Документ {documentType.Name} вернул пустой каталог списков"));
                    }
                    else
                    {
                        ValidateCatalog(catalog, propertiesByName, validationResults);
                    }

                    if (schema is null)
                    {
                        validationResults.Add(new ValidationResult($"Документ {documentType.Name} вернул пустую схему зависимостей списков"));
                    }
                    else if (catalog is not null)
                    {
                        ValidateSchemaRules(schema, catalog, propertiesByName, attributeConnections, validationResults);

                        ValidateAttributeConnectionsHaveSchemaRules(schema, attributeConnections, validationResults);
                    }
                }
            }

            if (ContainsCycle(graph))
            {
                validationResults.Add(new ValidationResult("Граф зависимостей списков не должен содержать циклы"));
            }

            return validationResults;
        }

        private static void ValidateChildField(PropertyInfo childProperty, List<ValidationResult> validationResults)
        {
            var childListConstraint = childProperty.GetCustomAttribute<ListConstraintAttribute>();

            if (childListConstraint is null)
            {
                validationResults.Add(new ValidationResult($"Поле {childProperty.Name} имеет зависимость от другого списка, " +
                        "но не объявлено как выпадающий список через"));
            }

            if (!IsListOptionKeyProperty(childProperty))
            {
                validationResults.Add(new ValidationResult($"Поле {childProperty.Name} должно иметь тип ListOptionKey? для работы со списками"));
            }
        }

        private static void ValidateParentField(string childFieldName, string parentFieldName, Dictionary<string, PropertyInfo> propertiesByName,
            List<ValidationResult> validationResults)
        {
            if (!propertiesByName.TryGetValue(parentFieldName, out var parentProperty))
            {
                validationResults.Add(new ValidationResult($"Родительское поле {parentFieldName} для поля {childFieldName} не существует"));

                return;
            }

            var parentListConstraint = parentProperty.GetCustomAttribute<ListConstraintAttribute>();

            if (parentListConstraint is null)
            {
                validationResults.Add(new ValidationResult($"Родительское поле {parentFieldName} для поля {childFieldName} " +
                        "не является выпадающим списком"));
            }

            if (!IsListOptionKeyProperty(parentProperty))
            {
                validationResults.Add(new ValidationResult($"Родительское поле {parentFieldName} должно иметь тип ListOptionKey?"));
            }
        }

        private static IListConfigurationProvider? TryGetConfigurationProvider(Type documentType, List<ValidationResult> validationResults)
        {
            if (!typeof(IListConfigurationProvider).IsAssignableFrom(documentType))
            {
                validationResults.Add(new ValidationResult($"Документ {documentType.Name} содержит списки или зависимости списков, " +
                        "но не реализует IListConfigurationProvider"));

                return null;
            }

            if (documentType.IsAbstract)
            {
                validationResults.Add(new ValidationResult($"Нельзя создать экземпляр абстрактного документа {documentType.Name}."));

                return null;
            }

            object? documentInstance;

            try
            {
                documentInstance = Activator.CreateInstance(documentType);
            }
            catch (Exception exception)
            {
                validationResults.Add(new ValidationResult($"Не удалось создать экземпляр документа {documentType.Name}. " +
                    $"Ошибка: {exception.Message}"));

                return null;
            }

            if (documentInstance is not IListConfigurationProvider configurationProvider)
            {
                validationResults.Add(new ValidationResult($"Документ {documentType.Name} не предоставляет конфигурацию списков"));

                return null;
            }

            return configurationProvider;
        }

        private static void ValidateCatalog(DocumentListCatalog catalog, Dictionary<string, PropertyInfo> propertiesByName, List<ValidationResult> validationResults)
        {
            foreach (var listPair in catalog.ListsByFieldName)
            {
                var fieldName = listPair.Key;
                var listDefinition = listPair.Value;

                if (!propertiesByName.TryGetValue(fieldName, out var property))
                {
                    validationResults.Add(new ValidationResult($"В каталоге списков указано несуществующее поле {fieldName}"));

                    continue;
                }

                if (property.GetCustomAttribute<ListConstraintAttribute>() is null)
                {
                    validationResults.Add(new ValidationResult($"Поле {fieldName} есть в каталоге списков, " +
                            "но не имеет ListConstraintAttribute"));
                }

                if (!IsListOptionKeyProperty(property))
                {
                    validationResults.Add(new ValidationResult($"Поле {fieldName} должно иметь тип ListOptionKey?"));
                }

                if (listDefinition.Options.Count == 0)
                {
                    validationResults.Add(new ValidationResult($"список для поля {fieldName} не содержит вариантов"));
                }
            }

            foreach (var propertyPair in propertiesByName)
            {
                var property = propertyPair.Value;

                if (property.GetCustomAttribute<ListConstraintAttribute>() is null)
                {
                    continue;
                }

                if (!catalog.CotainsList(property.Name))
                {
                    validationResults.Add(new ValidationResult($"поле {property.Name} объявлено как список, но для него нет " +
                        $"описания в каталоге"));
                }
            }
        }

        private static void ValidateSchemaRules(ListDependencySchema schema, DocumentListCatalog catalog, Dictionary<string,
            PropertyInfo> propertiesByName, HashSet<string> attributeConnections, List<ValidationResult> validationResults)
        {
            var ruleKeys = new HashSet<string>();

            foreach (var rule in schema.Rules)
            {
                var ruleKey = CreateRuleKey(
                    rule.ChildField,
                    rule.ParentField,
                    rule.ParentValueKey);

                if (!ruleKeys.Add(ruleKey))
                {
                    validationResults.Add(new ValidationResult("В схеме зависимостей найдено повторяющееся правило"));

                    continue;
                }

                ValidateSchemaConnectionExistsInAttributes(rule, attributeConnections, validationResults);

                if (!propertiesByName.ContainsKey(rule.ChildField))
                {
                    validationResults.Add(new ValidationResult($"В схеме зависимостей указано несуществующее зависимое поле{rule.ChildField}"));

                    continue;
                }

                if (!propertiesByName.ContainsKey(rule.ParentField))
                {
                    validationResults.Add(new ValidationResult($"В схеме зависимостей указано несуществующее родительское " +
                        $"поле {rule.ParentField}"));

                    continue;
                }

                if (!catalog.TryGetList(rule.ChildField, out var childList) || childList is null)
                {
                    validationResults.Add(new ValidationResult($"Для зависимого поля {rule.ChildField} нет списка в каталоге"));

                    continue;
                }

                if (!catalog.TryGetList(rule.ParentField, out var parentList) || parentList is null)
                {
                    validationResults.Add(new ValidationResult($"Для родительского поля {rule.ParentField} нет списка в каталоге"));

                    continue;
                }

                if (!parentList.ContainsKey(rule.ParentValueKey))
                {
                    validationResults.Add(new ValidationResult($"Ключ {rule.ParentValueKey} не найден среди вариантов " +
                            $"родительского поля {rule.ParentField}"));
                }

                foreach (var allowedChildValueKey in rule.AllowedChildValueKeys)
                {
                    if (!childList.ContainsKey(allowedChildValueKey))
                    {
                        validationResults.Add(new ValidationResult($"Ключ {allowedChildValueKey} из правила зависимости не найден " +
                                $"среди вариантов поля {rule.ChildField}"));
                    }
                }
            }
        }

        private static void ValidateSchemaConnectionExistsInAttributes(ListDependencyRule rule, HashSet<string> attributeConnections,
            List<ValidationResult> validationResults)
        {
            var connectionKey = CreateConnectionKey(rule.ChildField, rule.ParentField);

            if (!attributeConnections.Contains(connectionKey))
            {
                validationResults.Add(new ValidationResult($"В схеме есть связь {rule.ParentField} → {rule.ChildField}, " +
                        "но она не объявлена через ListDependencyAttribute."));
            }
        }

        private static void ValidateAttributeConnectionsHaveSchemaRules(ListDependencySchema schema, HashSet<string> attributeConnections,
            List<ValidationResult> validationResults)
        {
            foreach (var connectionKey in attributeConnections)
            {
                var parts = connectionKey.Split('|');

                var childField = parts[0];
                var parentField = parts[1];

                if (!schema.HasRulesForConnection(childField, parentField))
                {
                    validationResults.Add(new ValidationResult($"Поле {childField} объявило зависимость от поля {parentField}, " +
                            "но в схеме зависимостей нет правил для этой связи"));
                }
            }
        }

        private static bool IsListOptionKeyProperty(PropertyInfo property)
        {
            if (property.PropertyType == typeof(ListOptionKey))
            {
                return true;
            }

            var nullableType = Nullable.GetUnderlyingType(property.PropertyType);

            return nullableType == typeof(ListOptionKey);
        }

        private static void AddGraphEdge(Dictionary<string, HashSet<string>> graph, string parentField, string childField)
        {
            if (!graph.ContainsKey(parentField))
            {
                graph[parentField] = new HashSet<string>();
            }

            graph[parentField].Add(childField);
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

        private static string CreateConnectionKey(string childField, string parentField)
        {
            return $"{childField}|{parentField}";
        }

        private static string CreateRuleKey(string childField, string parentField, ListOptionKey parentValueKey)
        {
            return $"{childField}|{parentField}|{parentValueKey}";
        }
    }
}