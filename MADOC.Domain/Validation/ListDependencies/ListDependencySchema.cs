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
            string childField,
            string parentField,
            string parentValue,
            params string[] allowedChildValues)
        {
            var rule = new ListDependencyRule(
                childField,
                parentField,
                parentValue,
                allowedChildValues);

            rules.Add(rule);
        }

        public bool HasRulesForConnection(string childField, string parentField)//проверяет, есть ли правила, связывающие childField и parentField
        {
            foreach (var rule in rules)
            {
                if (rule.ChildField == childField &&
                    rule.ParentField == parentField)
                {
                    return true;
                }
            }

            return false;
        }

        public IReadOnlyList<string> GetAllowedValues(string childField,IReadOnlyDictionary<string, string> parentValues)
        {
            if (string.IsNullOrWhiteSpace(childField))
            {
                throw new ArgumentException(
                    "Имя зависимого поля не может быть пустым.",
                    nameof(childField));
            }

            ArgumentNullException.ThrowIfNull(parentValues);

            if (parentValues.Count == 0)
            {
                return Array.Empty<string>();
            }

            List<string>? result = null;

            foreach (var parentValuePair in parentValues)
            {
                var parentField = parentValuePair.Key;
                var parentValue = parentValuePair.Value;

                var allowedForCurrentParent = GetAllowedValuesForOneParent(
                    childField,
                    parentField,
                    parentValue);

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
            string childField,
            string parentField,
            string parentValue)
        {
            var result = new List<string>();
            var uniqueValues = new HashSet<string>();

            foreach (var rule in rules)
            {
                if (rule.ChildField != childField)
                {
                    continue;
                }

                if (rule.ParentField != parentField)
                {
                    continue;
                }

                if (rule.ParentValue != parentValue)
                {
                    continue;
                }

                foreach (var allowedValue in rule.AllowedChildValues)
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