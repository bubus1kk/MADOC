namespace MADOC.Domain.Validation.ListDependencies
{
    public class ListDependencyRule
    {
        public string ChildFieldName { get; }
        public string ParentFieldName { get; }
        public string ParentFieldValue { get; }
        public IReadOnlyList<string> AllowedChildFieldValues { get; }

        public ListDependencyRule(string childFieldName, string parentFieldName, string ParentFieldValue,
            params string[] allowedChildFieldValues)
        {
            if (string.IsNullOrWhiteSpace(childFieldName))
            {
                throw new ArgumentException("Имя зависимого поля не может быть пустым", nameof(childFieldName));
            }

            if (string.IsNullOrWhiteSpace(parentFieldName))
            {
                throw new ArgumentException("Имя родительсого поля не может быть пустым", nameof(parentFieldName));
            }

            if (string.IsNullOrWhiteSpace(ParentFieldValue))
            {
                throw new ArgumentException("Значение родительсого поля не может быть пустым", nameof(ParentFieldValue));
            }

            if (allowedChildFieldValues.Length == 0 || allowedChildFieldValues is null)
            {
                throw new ArgumentException("Список значений зависимого поля не может быть " +
                    "пустым", nameof(allowedChildFieldValues));
            }

            var uniqueChildFieldAllowedValues = new HashSet<string>();

            foreach (var value in allowedChildFieldValues)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Значение не может быть пустым", nameof(value));
                }

                if (!uniqueChildFieldAllowedValues.Add(value))
                {
                    throw new ArgumentException($"Значение {value} указано несколько раз", nameof(value));
                }
            }

            this.ChildFieldName = childFieldName;
            this.ParentFieldName = parentFieldName;
            this.ParentFieldValue = ParentFieldValue;
            this.AllowedChildFieldValues = allowedChildFieldValues;
        }
    }
}
