namespace MADOC.Domain.Validation.Lists
{
    public class ListOption
    {
        public ListOptionKey Key { get; }
        public string DisplayName { get; }

        public ListOption(ListOptionKey key, string displayName)
        {
            if (string.IsNullOrWhiteSpace(key.Value))
            {
                throw new ArgumentException("Ключ варианта списка не может быть пустым", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Отображаемое название варианта списка не может быть пустым", nameof(displayName));
            }

            this.Key = key;
            this.DisplayName = displayName;
        }
    }
}
