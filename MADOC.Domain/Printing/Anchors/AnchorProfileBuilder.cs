using System.Linq.Expressions;
using System.Reflection;

namespace MADOC.Domain.Printing.Anchors;

public sealed class AnchorProfileBuilder<TDocument>
    where TDocument : class
{
    private readonly string prefix;

    private readonly List<AnchorDefinition<TDocument>> anchors = new();

    private bool isBuilt;

    public AnchorProfileBuilder(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException(
                "Префикс якорей не может быть пустым.",
                nameof(prefix));
        }

        this.prefix = prefix;
    }

    public AnchorProfileBuilder<TDocument> Map<TValue>(
        Expression<Func<TDocument, TValue>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        EnsureCanAddAnchor();

        var propertyInfo = GetPropertyInfo(propertyExpression);
        var anchorKey = AnchorNameGenerator.Create(prefix, propertyInfo);

        EnsureUniqueAnchor(anchorKey, propertyInfo.Name);

        var compiledGetter = propertyExpression.Compile();

        object? GetValueFromDocument(TDocument document)
        {
            return compiledGetter(document);
        }

        AddAnchor(anchorKey, propertyInfo.Name, GetValueFromDocument);

        return this;
    }

    internal AnchorProfileBuilder<TDocument> Map(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        EnsureCanAddAnchor();
        ValidateProperty(propertyInfo);

        var anchorKey = AnchorNameGenerator.Create(prefix, propertyInfo);

        EnsureUniqueAnchor(anchorKey, propertyInfo.Name);

        object? GetValueFromDocument(TDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            return propertyInfo.GetValue(document);
        }

        AddAnchor(anchorKey, propertyInfo.Name, GetValueFromDocument);

        return this;
    }

    internal IReadOnlyList<AnchorDefinition<TDocument>> Build()
    {
        isBuilt = true;

        return anchors.ToArray();
    }

    private void AddAnchor(
        AnchorKey key,
        string fieldName,
        Func<TDocument, object?> getValueFromDocument)
    {
        var definition = new AnchorDefinition<TDocument>(
            key,
            fieldName,
            getValueFromDocument);

        anchors.Add(definition);
    }

    private void EnsureCanAddAnchor()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "Нельзя добавлять якоря после завершения сборки профиля.");
        }
    }

    private void EnsureUniqueAnchor(
        AnchorKey key,
        string fieldName)
    {
        foreach (var anchor in anchors)
        {
            if (anchor.Key == key)
            {
                throw new InvalidOperationException(
                    $"Якорь {key} уже зарегистрирован.");
            }

            if (anchor.FieldName == fieldName)
            {
                throw new InvalidOperationException(
                    $"Поле {fieldName} уже зарегистрировано как печатный якорь.");
            }
        }
    }

    private static PropertyInfo GetPropertyInfo<TValue>(
        Expression<Func<TDocument, TValue>> propertyExpression)
    {
        var body = RemoveConvertExpression(propertyExpression.Body);

        if (body is not MemberExpression memberExpression)
        {
            throw new ArgumentException(
                "Выражение должно указывать напрямую на свойство документа.",
                nameof(propertyExpression));
        }

        if (memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException(
                "Выражение должно указывать на свойство, а не на поле или метод.",
                nameof(propertyExpression));
        }

        var sourceExpression = RemoveConvertExpression(memberExpression.Expression);

        if (sourceExpression is not ParameterExpression)
        {
            throw new ArgumentException(
                "Выражение должно указывать только на прямое свойство документа. " +
                "Вложенные свойства и вычисления нужно передавать в HTML-генератор как сгенерированные поля.",
                nameof(propertyExpression));
        }

        ValidateProperty(propertyInfo);

        return propertyInfo;
    }

    private static void ValidateProperty(PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetMethod is null)
        {
            throw new ArgumentException(
                $"Свойство {propertyInfo.Name} не имеет getter.",
                nameof(propertyInfo));
        }

        if (propertyInfo.GetIndexParameters().Length > 0)
        {
            throw new ArgumentException(
                $"Индексируемое свойство {propertyInfo.Name} нельзя использовать как печатный якорь.",
                nameof(propertyInfo));
        }

        if (propertyInfo.DeclaringType is not null &&
            !propertyInfo.DeclaringType.IsAssignableFrom(typeof(TDocument)))
        {
            throw new ArgumentException(
                $"Свойство {propertyInfo.Name} не относится к документу {typeof(TDocument).Name}.",
                nameof(propertyInfo));
        }
    }

    private static Expression? RemoveConvertExpression(Expression? expression)
    {
        while (expression is UnaryExpression unaryExpression &&
               (unaryExpression.NodeType == ExpressionType.Convert ||
                unaryExpression.NodeType == ExpressionType.ConvertChecked))
        {
            expression = unaryExpression.Operand;
        }

        return expression;
    }
}
