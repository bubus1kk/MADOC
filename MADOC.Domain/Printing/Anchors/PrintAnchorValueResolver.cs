using System.Globalization;
using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Printing.Anchors;

public sealed class PrintAnchorValueResolver<TDocument>
    where TDocument : class
{
    private readonly PrintAnchorProfile<TDocument> profile;

    public PrintAnchorValueResolver(PrintAnchorProfile<TDocument> profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        this.profile = profile;
    }

    public object? GetRawValue(
        TDocument document,
        string anchorKey)
    {
        return GetRawValue(document, new AnchorKey(anchorKey));
    }

    public object? GetRawValue(
        TDocument document,
        AnchorKey anchorKey)
    {
        ArgumentNullException.ThrowIfNull(document);

        var definition = profile.GetRequired(anchorKey);

        return definition.GetValue(document);
    }

    public string GetDisplayValue(
        TDocument document,
        string anchorKey)
    {
        return GetDisplayValue(document, new AnchorKey(anchorKey));
    }

    public string GetDisplayValue(
        TDocument document,
        AnchorKey anchorKey)
    {
        ArgumentNullException.ThrowIfNull(document);

        var definition = profile.GetRequired(anchorKey);
        var rawValue = definition.GetValue(document);

        return FormatDisplayValue(document, definition, rawValue);
    }

    private static string FormatDisplayValue(
        TDocument document,
        AnchorDefinition<TDocument> definition,
        object? rawValue)
    {
        if (rawValue is null)
        {
            return string.Empty;
        }

        return rawValue switch
        {
            string value => value,

            int value => value.ToString(CultureInfo.InvariantCulture),

            long value => value.ToString(CultureInfo.InvariantCulture),

            double value => value.ToString("0.##", CultureInfo.InvariantCulture),

            decimal value => value.ToString("0.##", CultureInfo.InvariantCulture),

            float value => value.ToString("0.##", CultureInfo.InvariantCulture),

            bool value => value ? "Да" : "Нет",

            DateOnly value => value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),

            TimeOnly value => value.ToString("HH:mm", CultureInfo.InvariantCulture),

            DateTime value => value.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),

            DateRange value => FormatDateRange(value),

            DateTimeRange value => FormatDateTimeRange(value),

            ListOptionKey value => FormatListOption(document, definition, value),

            IFormattable value => value.ToString(null, CultureInfo.InvariantCulture),

            _ => rawValue.ToString() ?? string.Empty
        };
    }

    private static string FormatDateRange(DateRange dateRange)
    {
        var from = dateRange.From.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        var to = dateRange.To.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

        return $"{from} — {to}";
    }

    private static string FormatDateTimeRange(DateTimeRange dateTimeRange)
    {
        var from = dateTimeRange.From.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        var to = dateTimeRange.To.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

        return $"{from} — {to}";
    }

    private static string FormatListOption(
        TDocument document,
        AnchorDefinition<TDocument> definition,
        ListOptionKey key)
    {
        if (document is not IListConfigurationProvider listConfigurationProvider)
        {
            throw new InvalidOperationException(
                $"Документ {typeof(TDocument).Name} содержит списочное значение, " +
                "но не реализует IListConfigurationProvider.");
        }

        var catalog = listConfigurationProvider.GetListCatalog();
        var list = catalog.GetList(definition.FieldName);

        if (!list.ContainsKey(key))
        {
            throw new InvalidOperationException(
                $"Ключ {key} не найден в списке поля {definition.FieldName}.");
        }

        return list.GetOption(key).DisplayName;
    }
}