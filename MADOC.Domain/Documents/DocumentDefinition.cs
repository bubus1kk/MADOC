using MADOC.Domain.Fields;

namespace MADOC.Domain.Documents
{
    public class DocumentDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public IReadOnlyList<DocumentField> Fields { get; }

        public DocumentDefinition(
            string id,
            string name,
            IEnumerable<DocumentField> fields)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Document id cannot be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Document name cannot be empty.", nameof(name));

            var fieldList = fields.ToList();

            if (fieldList.Count == 0)
                throw new ArgumentException("Document must contain at least one field.", nameof(fields));

            var duplicatedFieldKeys = fieldList
                .GroupBy(field => field.Key)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatedFieldKeys.Count > 0)
            {
                throw new ArgumentException(
                    $"Document contains duplicated field keys: {string.Join(", ", duplicatedFieldKeys)}.",
                    nameof(fields));
            }

            this.Id = id;
            this.Name = name;
            this.Fields = fieldList;
        }

        public DocumentField? FindField(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Field key cannot be empty.", nameof(key));

            return Fields.FirstOrDefault(field => field.Key == key);
        }

        public bool ContainsField(string key)
        {
            return FindField(key) is not null;
        }
    }

}