using System.Collections.ObjectModel;

namespace MADOC.Domain.Printing.Anchors;

public abstract class PrintAnchorProfile<TDocument>
    where TDocument : class
{
    private readonly Lazy<IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>>> anchorsByKey;

    protected PrintAnchorProfile()
    {
        anchorsByKey = new Lazy<IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>>>(
            BuildAnchors);
    }

    public virtual string Prefix
    {
        get
        {
            return AnchorNameGenerator.CreateDocumentPrefix(typeof(TDocument));
        }
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

    protected abstract void Configure(AnchorProfileBuilder<TDocument> builder);

    private IReadOnlyDictionary<AnchorKey, AnchorDefinition<TDocument>> BuildAnchors()
    {
        if (string.IsNullOrWhiteSpace(Prefix))
        {
            throw new InvalidOperationException(
                $"Префикс профиля печатных якорей для документа {typeof(TDocument).Name} не может быть пустым.");
        }

        var builder = new AnchorProfileBuilder<TDocument>(Prefix);

        Configure(builder);

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
}