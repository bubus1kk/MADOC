using System;
using System.Collections.Generic;

namespace MADOC.Domain.Validation.Lists
{
    public class ListDefinition
    {
        private readonly List<ListOption> options = new();

        public string FieldName { get; }

        public IReadOnlyList<ListOption> Options
        {
            get
            {
                return options;
            }
        }

        public ListDefinition(string fieldName, params ListOption[] options)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException(
                    "Имя поля списка не может быть пустым.",
                    nameof(fieldName));
            }

            FieldName = fieldName;

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach (var option in options)
            {
                AddOption(option);
            }
        }

        public void AddOption(ListOption option)
        {
            ArgumentNullException.ThrowIfNull(option);

            if (ContainsKey(option.Key))
            {
                throw new InvalidOperationException(
                    $"В списке \"{FieldName}\" уже есть вариант с ключом \"{option.Key}\".");
            }

            options.Add(option);
        }

        public bool ContainsKey(ListOptionKey key)
        {
            foreach (var option in options)
            {
                if (option.Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public ListOption GetOption(ListOptionKey key)
        {
            foreach (var option in options)
            {
                if (option.Key == key)
                {
                    return option;
                }
            }

            throw new InvalidOperationException(
                $"В списке \"{FieldName}\" не найден вариант с ключом \"{key}\".");
        }
    }
}