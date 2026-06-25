using MADOC.Application.Documents;
using MADOC.Domain.Core.Validation.Lists;
using MADOC.Printing.Html.Anchors;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MADOC.Application.Forms;

public sealed class DocumentFormSchemaService
{
    private readonly IDocumentTypeRegistry registry;

    public DocumentFormSchemaService(IDocumentTypeRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        this.registry = registry;
    }

    public DocumentFormSchema GetSchema(DocumentTypeKey documentType)
    {
        var descriptor = registry.GetRequired(documentType);
        var document = CreateDocumentInstance(descriptor);
        var fields = BuildFields(descriptor, document);

        return new DocumentFormSchema(descriptor.Key, descriptor.DisplayName, fields);
    }

    private static object CreateDocumentInstance(DocumentTypeDescriptor descriptor)
    {
        return Activator.CreateInstance(descriptor.DocumentType)
            ?? throw new InvalidOperationException($"Не удалось создать документ типа {descriptor.DocumentType.Name}");
    }

    private static IReadOnlyList<DocumentFieldSchema> BuildFields(
        DocumentTypeDescriptor descriptor,
        object document)
    {
        var prefix = AnchorNameGenerator.CreateDocumentPrefix(descriptor.DocumentType);

        return descriptor.DocumentType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Where(property => property.GetMethod is not null && property.GetMethod.IsPublic)
            .Select(property => BuildField(prefix, property, document))
            .ToArray();
    }

    private static DocumentFieldSchema BuildField(string prefix, PropertyInfo property, object document)
    {
        var displayName = property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
        var type = DetermineFieldType(property);
        var isReadOnly = property.SetMethod is null || !property.SetMethod.IsPublic;
        var constraints = BuildConstraints(property);
        var dependency = BuildDependency(property);
        var options = BuildOptions(property, document, dependency);
        var anchorName = AnchorNameGenerator.Create(prefix, property).Value;

        return new DocumentFieldSchema(
            property.Name,
            displayName,
            type,
            isReadOnly,
            anchorName,
            constraints,
            options,
            dependency);
    }

    private static DocumentFieldType DetermineFieldType(PropertyInfo property)
    {
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (AttributeReader.HasAttribute(property, "ListConstraintAttribute") || propertyType == typeof(ListOptionKey))
        {
            return DocumentFieldType.List;
        }

        if (propertyType == typeof(string))
        {
            var stringConstraint = AttributeReader.FindAttribute(property, "StringConstraintAttribute");
            var isMultiline = AttributeReader.ReadProperty<bool?>(stringConstraint, "IsMultiline") == true;

            return isMultiline ? DocumentFieldType.MultilineText : DocumentFieldType.Text;
        }

        if (propertyType == typeof(int) || propertyType == typeof(long))
        {
            return DocumentFieldType.Integer;
        }

        if (propertyType == typeof(double) || propertyType == typeof(decimal) || propertyType == typeof(float))
        {
            return DocumentFieldType.Decimal;
        }

        if (propertyType == typeof(bool))
        {
            return DocumentFieldType.Boolean;
        }

        if (propertyType == typeof(DateOnly))
        {
            return DocumentFieldType.Date;
        }

        if (propertyType == typeof(TimeOnly))
        {
            return DocumentFieldType.Time;
        }

        if (propertyType == typeof(DateTime))
        {
            return DocumentFieldType.DateTime;
        }

        if (propertyType.Name == "DateRange")
        {
            return DocumentFieldType.DateRange;
        }

        if (propertyType.Name == "DateTimeRange")
        {
            return DocumentFieldType.DateTimeRange;
        }

        return DocumentFieldType.Text;
    }

    private static DocumentFieldConstraintDto BuildConstraints(PropertyInfo property)
    {
        var isRequired = property.GetCustomAttribute<RequiredAttribute>() is not null;
        var stringConstraint = AttributeReader.FindAttribute(property, "StringConstraintAttribute");
        var numberConstraint = AttributeReader.FindAttribute(property, "NumberConstraintAttribute");
        var dateConstraint = AttributeReader.FindAttribute(property, "DateConstraintAttribute");
        var timeConstraint = AttributeReader.FindAttribute(property, "TimeConstraintAttribute");

        return new DocumentFieldConstraintDto(
            isRequired,
            maxLength: AttributeReader.ReadProperty<int?>(stringConstraint, "MaxLength"),
            minLength: AttributeReader.ReadProperty<int?>(stringConstraint, "MinLength"),
            minNumber: AttributeReader.ReadProperty<double?>(numberConstraint, "MinValue"),
            maxNumber: AttributeReader.ReadProperty<double?>(numberConstraint, "MaxValue"),
            allowFloats: AttributeReader.ReadProperty<bool?>(numberConstraint, "AllowFloats"),
            minDate: AttributeReader.ReadAsString(dateConstraint, "MinDate"),
            maxDate: AttributeReader.ReadAsString(dateConstraint, "MaxDate"),
            minTime: AttributeReader.ReadAsString(timeConstraint, "MinTime"),
            maxTime: AttributeReader.ReadAsString(timeConstraint, "MaxTime"),
            alphabet: AttributeReader.ReadAsString(stringConstraint, "Alphabet"),
            allowSpecialChars: AttributeReader.ReadProperty<bool?>(stringConstraint, "AllowSpecialChars"),
            isMultiline: AttributeReader.ReadProperty<bool?>(stringConstraint, "IsMultiline"));
    }

    private static DocumentFieldDependencyDto? BuildDependency(PropertyInfo property)
    {
        var dependencyAttribute = AttributeReader.FindAttribute(property, "ListDependencyAttribute");

        if (dependencyAttribute is null)
        {
            return null;
        }

        var parentFields = AttributeReader.ReadProperty<IReadOnlyList<string>>(dependencyAttribute, "DependsOnFields");

        if (parentFields is null || parentFields.Count == 0)
        {
            return null;
        }

        return new DocumentFieldDependencyDto(parentFields);
    }

    private static IReadOnlyList<DocumentFieldOptionDto> BuildOptions(
        PropertyInfo property,
        object document,
        DocumentFieldDependencyDto? dependency)
    {
        if (!AttributeReader.HasAttribute(property, "ListConstraintAttribute"))
        {
            return Array.Empty<DocumentFieldOptionDto>();
        }

        if (dependency is not null)
        {
            return Array.Empty<DocumentFieldOptionDto>();
        }

        if (document is not IListConfigurationProvider listConfigurationProvider)
        {
            return Array.Empty<DocumentFieldOptionDto>();
        }

        var list = listConfigurationProvider.GetListCatalog().GetList(property.Name);

        return list.Options
            .Select(option => new DocumentFieldOptionDto(option.Key.Value, option.DisplayName)).ToArray();
    }
}
