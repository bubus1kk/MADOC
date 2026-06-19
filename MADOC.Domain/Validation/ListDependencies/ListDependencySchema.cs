using System;
using System.Collections.Generic;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListDependencySchema
    {
        private readonly List<ListDependencyRule> rules = new();

        public IReadOnlyList<ListDependencyRule> Rules
        {
            get
            {
                return rules;
            }
        }

        public void AddRule(
            string childField,
            string parentField,
            ListOptionKey parentValueKey,
            params ListOptionKey[] allowedChildValueKeys)
        {
            var rule = new ListDependencyRule(
                childField,
                parentField,
                parentValueKey,
                allowedChildValueKeys);

            rules.Add(rule);
        }

        public bool HasRulesForConnection(string childField, string parentField)
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

        public IReadOnlyList<ListOptionKey> GetAllowedValues(
            string childField,
            IReadOnlyDictionary<string, ListOptionKey> parentValues)
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
                return Array.Empty<ListOptionKey>();
            }

            List<ListOptionKey>? result = null;

            foreach (var parentValuePair in parentValues)
            {
                var parentField = parentValuePair.Key;
                var parentValueKey = parentValuePair.Value;

                var allowedForCurrentParent = GetAllowedValuesForOneParent(
                    childField,
                    parentField,
                    parentValueKey);

                if (allowedForCurrentParent.Count == 0)
                {
                    return Array.Empty<ListOptionKey>();
                }

                if (result is null)
                {
                    result = new List<ListOptionKey>(allowedForCurrentParent);
                }
                else
                {
                    result = IntersectPreservingOrder(result, allowedForCurrentParent);
                }
            }

            if (result is null)
            {
                return Array.Empty<ListOptionKey>();
            }

            return result;
        }

        private List<ListOptionKey> GetAllowedValuesForOneParent(
            string childField,
            string parentField,
            ListOptionKey parentValueKey)
        {
            var result = new List<ListOptionKey>();
            var uniqueValues = new HashSet<ListOptionKey>();

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

                if (rule.ParentValueKey != parentValueKey)
                {
                    continue;
                }

                foreach (var allowedValueKey in rule.AllowedChildValueKeys)
                {
                    if (uniqueValues.Add(allowedValueKey))
                    {
                        result.Add(allowedValueKey);
                    }
                }
            }

            return result;
        }

        private static List<ListOptionKey> IntersectPreservingOrder(
            IReadOnlyList<ListOptionKey> firstValues,
            IReadOnlyList<ListOptionKey> secondValues)
        {
            var secondValuesSet = new HashSet<ListOptionKey>(secondValues);
            var result = new List<ListOptionKey>();

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