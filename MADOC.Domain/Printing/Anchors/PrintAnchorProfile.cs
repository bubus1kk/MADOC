using System.Collections.ObjectModel;
using System.Reflection;

namespace MADOC.Domain.Printing.Anchors;

public class PrintAnchorProfile<TDocument>
    where TDocument : class
{
    private readonly Lazy<IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>>> anchorsByKey;

    public string Prefix { get; }

    public PrintAnchorProfile()
        : this(AnchorNameGenerator.CreateDocumentPrefix(typeof(TDocument)))
    {
    }

    public PrintAnchorProfile(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException(
                $"Префикс профиля печатных якорей для документа {typeof(TDocument).Name} не может быть пустым.",
                nameof(prefix));
        }

        Prefix = prefix;

        anchorsByKey = new Lazy<IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>>>(
            BuildAnchors);
    }

    public IReadOnlyCollection<AnchorDefinition<TDocument>> Anchors
    {
        get
        {
            return anchorsByKey.Value.Values.ToArray();
        }
    }

    public bool Contains(AnchorKey key)
    {
        return anchorsByKey.Value.ContainsKey(key);
    }

    public bool Contains(string key)
    {
        return Contains(new AnchorKey(key));
    }

    public bool TryGet(
        AnchorKey key,
        out AnchorDefinition<TDocument>? definition)
    {
        return anchorsByKey.Value.TryGetValue(key, out definition);
    }

    public AnchorDefinition<TDocument> GetRequired(AnchorKey key)
    {
        if (!anchorsByKey.Value.TryGetValue(key, out var definition))
        {
            throw new InvalidOperationException(
                $"Якорь {key} не зарегистрирован в профиле печатной формы для документа {typeof(TDocument).Name}.");
        }

        return definition;
    }

    public AnchorDefinition<TDocument> GetRequired(string key)
    {
        return GetRequired(new AnchorKey(key));
    }

    protected virtual IReadOnlyList<PropertyInfo> GetPropertiesForAnchors()
    {
        var properties = typeof(TDocument).GetProperties(
            BindingFlags.Instance | BindingFlags.Public);

        var result = new List<PropertyInfo>();

        foreach (var property in properties)
        {
            if (CanUsePropertyAsAnchor(property))
            {
                result.Add(property);
            }
        }

        return result;
    }

    private IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>> BuildAnchors()
    {
        var builder = new AnchorProfileBuilder<TDocument>(Prefix);
        var properties = GetPropertiesForAnchors();

        foreach (var property in properties)
        {
            builder.Map(property);
        }

        var anchors = builder.Build();

        if (anchors.Count == 0)
        {
            throw new InvalidOperationException(
                $"Профиль печатных якорей для документа {typeof(TDocument).Name} не содержит ни одного якоря.");
        }

        var dictionary = new Dictionary<AnchorKey, AnchorDefinition<TDocument>>();

        foreach (var anchor in anchors)
        {
            if (!dictionary.TryAdd(anchor.Key, anchor))
            {
                throw new InvalidOperationException(
                    $"Якорь {anchor.Key} повторно зарегистрирован в профиле документа {typeof(TDocument).Name}.");
            }
        }

        return new ReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>>(dictionary);
    }

    private static bool CanUsePropertyAsAnchor(PropertyInfo property)
    {
        if (property.GetMethod is null)
        {
            return false;
        }

        if (!property.GetMethod.IsPublic)
        {
            return false;
        }

        if (property.GetIndexParameters().Length > 0)
        {
            return false;
        }

        return true;
    }
}
