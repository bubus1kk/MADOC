using System;
using System.Collections.Generic;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListDependencyRule
    {
        public string ChildField { get; }
        public string ParentField { get; }
        public ListOptionKey ParentValueKey { get; }
        public IReadOnlyList<ListOptionKey> AllowedChildValueKeys { get; }

        public ListDependencyRule(
            string childField,
            string parentField,
            ListOptionKey parentValueKey,
            params ListOptionKey[] allowedChildValueKeys)
        {
            if (string.IsNullOrWhiteSpace(childField))
            {
                throw new ArgumentException(
                    "Имя зависимого поля не может быть пустым.",
                    nameof(childField));
            }

            if (string.IsNullOrWhiteSpace(parentField))
            {
                throw new ArgumentException(
                    "Имя родительского поля не может быть пустым.",
                    nameof(parentField));
            }

            if (string.IsNullOrWhiteSpace(parentValueKey.Value))
            {
                throw new ArgumentException(
                    "Ключ значения родительского поля не может быть пустым.",
                    nameof(parentValueKey));
            }

            if (allowedChildValueKeys is null || allowedChildValueKeys.Length == 0)
            {
                throw new ArgumentException(
                    "Список разрешённых ключей зависимого поля не может быть пустым.",
                    nameof(allowedChildValueKeys));
            }

            var uniqueKeys = new HashSet<ListOptionKey>();
            var copiedKeys = new List<ListOptionKey>();

            foreach (var allowedChildValueKey in allowedChildValueKeys)
            {
                if (string.IsNullOrWhiteSpace(allowedChildValueKey.Value))
                {
                    throw new ArgumentException(
                        "Ключ разрешённого значения зависимого поля не может быть пустым.",
                        nameof(allowedChildValueKeys));
                }

                if (!uniqueKeys.Add(allowedChildValueKey))
                {
                    throw new ArgumentException(
                        $"Ключ \"{allowedChildValueKey}\" указан несколько раз.",
                        nameof(allowedChildValueKeys));
                }

                copiedKeys.Add(allowedChildValueKey);
            }

            ChildField = childField;
            ParentField = parentField;
            ParentValueKey = parentValueKey;
            AllowedChildValueKeys = copiedKeys;
        }
    }
}