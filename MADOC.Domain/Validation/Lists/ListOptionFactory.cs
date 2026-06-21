namespace MADOC.Domain.Validation.Lists
{
    public class ListOptionFactory
    {
        private readonly string prefix;

        public ListOptionFactory(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Префикс ключей списка не может быть пустым", nameof(prefix));
            }

            this.prefix = prefix;
        }

        public ListOption Create(string keyPart, string displayName)
        {
            if (string.IsNullOrWhiteSpace(keyPart))
            {
                throw new ArgumentException("Часть ключа варианта списка не может быть пустой", nameof(keyPart));
            }

            var fullKey = $"{prefix}.{keyPart}";

            return new ListOption(new ListOptionKey(fullKey), displayName);
        }
    }
}
