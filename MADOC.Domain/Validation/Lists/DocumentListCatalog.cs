namespace MADOC.Domain.Validation.Lists
{
    public class DocumentListCatalog
    {
        private readonly Dictionary<string, ListDefinition> listsByFieldName = new();

        public IReadOnlyDictionary<string, ListDefinition> ListsByFieldName
        {
            get
            {
                return listsByFieldName;
            }
        }

        public void AddList(string fieldName, params ListOption[] options)
        {
            var listDefinition = new ListDefinition(fieldName, options);

            AddList(listDefinition);
        }

        public void AddList(ListDefinition listDefinition)
        {
            ArgumentNullException.ThrowIfNull(listDefinition);

            if (listsByFieldName.ContainsKey(listDefinition.FieldName))
            {
                throw new InvalidOperationException($"Список для поля {listDefinition.FieldName} уже существует");
            }

            listsByFieldName.Add(listDefinition.FieldName, listDefinition);
        }

        public bool CotainsList(string fieldName)
        {
            return listsByFieldName.ContainsKey(fieldName);
        }

        public bool TryGetList(string fieldName, out ListDefinition? listDefinition)
        {
            return listsByFieldName.TryGetValue(fieldName, out listDefinition);
        }

        public ListDefinition GetList(string fieldName)
        {
            if (!listsByFieldName.TryGetValue(fieldName, out var listDefinition))
            {
                throw new InvalidOperationException($"Список для поля {fieldName} не найден");
            }

            return listDefinition;
        }
    }
}
