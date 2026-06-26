using System.Reflection;
using MADOC.Printing.Html.Anchors;

namespace MADOC.Application.Printing;

public sealed class RuntimePrintAnchorProfile<TDocument> : PrintAnchorProfile<TDocument>
    where TDocument : class
{
    public RuntimePrintAnchorProfile()
        : base()
    {
    }

    protected override IReadOnlyList<PropertyInfo> GetPropertiesForAnchors()
    {
        return typeof(TDocument)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is not null)
            .Where(property => property.GetMethod!.IsPublic)
            .Where(property => property.GetIndexParameters().Length == 0)
            .ToArray();
    }
}