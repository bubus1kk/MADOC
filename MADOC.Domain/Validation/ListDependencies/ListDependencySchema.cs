using System;
using System.Collections.Generic;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListDependencySchema
    {
        private readonly List<ListDependencyRule> rules = new();

        public IReadOnlyList<ListDependencyRule> Rules//для чтения, чтобы не давать возможность изменять правила извне
        {
            get
            {
                return rules;
            }
        }

        public void AddRule(
            string childFieldName,
            string parentFieldName,
            string parentFieldValue,
            params string[] allowedChildValues)
        {
            var rule = new ListDependencyRule(
                childFieldName,
                parentFieldName,
                parentFieldValue,
                allowedChildValues);

            rules.Add(rule);
        }

        public bool HasRulesForConnection(string childFieldName, string parentFieldName)//проверяет, есть ли правила, связывающие childFieldName и parentFieldName
        {
            foreach (var rule in rules)
            {
                if (rule.ChildFieldName == childFieldName &&
                    rule.ParentFieldName == parentFieldName)
                {
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyList<string> GetAllowedValues(string childFieldName,IReadOnlyDictionary<string, string> parentFieldValues)
        {
            if (string.IsNullOrWhiteSpace(childFieldName))
            {
                throw new ArgumentException(
                    "Имя зависимого поля не может быть пустым.",
                    nameof(childFieldName));
            }

            ArgumentNullException.ThrowIfNull(parentFieldValues);

            if (parentFieldValues.Count == 0)
            {
                return Array.Empty<string>();
            }

            List<string>? result = null;

            foreach (var parentFieldValuePair in parentFieldValues)
            {
                var parentFieldName = parentFieldValuePair.Key;
                var parentFieldValue = parentFieldValuePair.Value;

                var allowedForCurrentParent = GetAllowedValuesForOneParent(
                    childFieldName,
                    parentFieldName,
                    parentFieldValue);

                if (allowedForCurrentParent.Count == 0)
                {
                    return Array.Empty<string>();
                }

                if (result is null)
                {
                    result = new List<string>(allowedForCurrentParent);
                }
                else
                {
                    result = IntersectPreservingOrder(result, allowedForCurrentParent);
                }
            }

            if (result is null)
            {
                return Array.Empty<string>();
            }

            return result;
        }

        private List<string> GetAllowedValuesForOneParent(
            string childFieldName,
            string parentFieldName,
            string parentFieldValue)
        {
            var result = new List<string>();
            var uniqueValues = new HashSet<string>();

            foreach (var rule in rules)
            {
                if (rule.ChildFieldName != childFieldName)
                {
                    continue;
                }

                if (rule.ParentFieldName != parentFieldName)
                {
                    continue;
                }

                if (rule.ParentFieldValue != parentFieldValue)
                {
                    continue;
                }

                foreach (var allowedValue in rule.AllowedChildFieldValues)
                {
                    if (uniqueValues.Add(allowedValue))
                    {
                        result.Add(allowedValue);
                    }
                }
            }

            return result;
        }

        private static List<string> IntersectPreservingOrder(
            IReadOnlyList<string> firstValues,
            IReadOnlyList<string> secondValues)
        {
            var secondValuesSet = new HashSet<string>(secondValues);
            var result = new List<string>();

            foreach (var value in firstValues)
            {
                if (secondValuesSet.Contains(value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }
}