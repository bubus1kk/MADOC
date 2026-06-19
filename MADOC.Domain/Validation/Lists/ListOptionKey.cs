namespace MADOC.Domain.Validation.Lists
{
    public readonly record struct ListOptionKey
    {
        public string Value { get; }

        public ListOptionKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("ключ варианта списка не может быть пустым", nameof(value));
            }

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
