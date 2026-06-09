using MADOC.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace MADOC.Domain.Documents
{
    public class DocumentInstance
    {
        public string DocumentDefenitionId { get; }

        private readonly Dictionary<string, FieldValue> _values = new();

        public DocumentInstance(string? documentDefinitionId)
        {
            if (string.IsNullOrWhiteSpace(documentDefinitionId))
                throw new ArgumentException("id документа не может быть пустым.", nameof(documentDefinitionId));
            DocumentDefenitionId = documentDefinitionId;
        }

        public void SetValue(string fieldKey, object? value)
        {
           var fieldValue = new FieldValue(fieldKey,value);

           _values[fieldKey] = fieldValue;
        }

        public object? GetValue(string fieldKey)
        {
            if (string.IsNullOrWhiteSpace(fieldKey))
                throw new ArgumentException("Ключ поле не может быть пустым.", nameof(fieldKey));

            if (_values.TryGetValue(fieldKey, out var fieldValue))
            {
                return fieldValue;
            }

            return null;
        }

        public bool HasValue(string fieldKey)
        {
            if (string.IsNullOrWhiteSpace(fieldKey))
                throw new ArgumentException("Ключ поле не может быть пустым.", nameof(fieldKey));
            return _values.ContainsKey(fieldKey);
        }
        public IReadOnlyCollection<FieldValue> Values
        {
            get
            {
                return _values.Values;
            }
        }
    }
}
