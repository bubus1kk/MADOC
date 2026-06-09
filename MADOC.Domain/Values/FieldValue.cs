using System;
using System.Collections.Generic;
using System.Text;

namespace MADOC.Domain.Values
{
    public class FieldValue
    {
        public string FieldKey { get; }
        public object? Value { get; }

        public FieldValue(string fieldKey, object? value)
        {
            if (string.IsNullOrWhiteSpace(fieldKey))
                throw new ArgumentException("Ключ поле не может быть пустым.", nameof(fieldKey));
            FieldKey = fieldKey;
            Value = value;
        }
    }
}
