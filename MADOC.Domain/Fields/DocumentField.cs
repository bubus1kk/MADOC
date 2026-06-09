namespace MADOC.Domain.Fields
{
    public class DocumentField
    {
        public string Key { get; }
        public string DisplayName { get; }
        public bool IsRequired { get; }

        public DocumentField(string key, string displayName, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Ключ поля не может быть пустым", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Отображаемое имя не может быть пустым", nameof(displayName));
            }

            this.Key = key;
            this.DisplayName = displayName;
            this.IsRequired = isRequired;
        }
    }
}
