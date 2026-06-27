using MADOC.Application.Documents;
using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Lists;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentValueMapperTests
{
    [TestMethod]
    public void MapValues_ConvertsSupportedValueTypes()
    {
        var mapper = new DocumentValueMapper();
        var document = new MapperTargetDocument();
        var values = new Dictionary<string, object?>
        {
            [nameof(MapperTargetDocument.Name)] = "Иванов Иван",
            [nameof(MapperTargetDocument.Count)] = "5",
            [nameof(MapperTargetDocument.Price)] = "12,5",
            [nameof(MapperTargetDocument.IsActive)] = "да",
            [nameof(MapperTargetDocument.Date)] = "2026-06-25",
            [nameof(MapperTargetDocument.Time)] = "08:30",
            [nameof(MapperTargetDocument.Option)] = "first_option",
            [nameof(MapperTargetDocument.Period)] = new Dictionary<string, object?>
            {
                ["from"] = "2026-06-01",
                ["to"] = "2026-06-03"
            }
        };

        var result = mapper.MapValues(document, values);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Иванов Иван", document.Name);
        Assert.AreEqual(5, document.Count);
        Assert.AreEqual(12.5m, document.Price);
        Assert.AreEqual(true, document.IsActive);
        Assert.AreEqual(new DateOnly(2026, 6, 25), document.Date);
        Assert.AreEqual(new TimeOnly(8, 30), document.Time);
        Assert.AreEqual(new ListOptionKey("first_option"), document.Option);
        Assert.IsNotNull(document.Period);
        Assert.AreEqual(new DateOnly(2026, 6, 1), document.Period.From);
        Assert.AreEqual(new DateOnly(2026, 6, 3), document.Period.To);
    }

    [TestMethod]
    public void MapValues_ReturnsErrorForUnknownFieldWhenRejectUnknownFieldsIsEnabled()
    {
        var mapper = new DocumentValueMapper();
        var document = new MapperTargetDocument();
        var values = new Dictionary<string, object?>
        {
            ["UnknownField"] = "value"
        };

        var result = mapper.MapValues(document, values);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("UnknownField", result.Errors.Single().FieldName);
    }

    [TestMethod]
    public void MapValues_IgnoresUnknownFieldWhenRejectUnknownFieldsIsDisabled()
    {
        var mapper = new DocumentValueMapper();
        var document = new MapperTargetDocument();
        var values = new Dictionary<string, object?>
        {
            ["UnknownField"] = "value"
        };

        var result = mapper.MapValues(document, values, rejectUnknownFields: false);

        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void MapValues_ReturnsErrorForInvalidConversion()
    {
        var mapper = new DocumentValueMapper();
        var document = new MapperTargetDocument();
        var values = new Dictionary<string, object?>
        {
            [nameof(MapperTargetDocument.Count)] = "not-number"
        };

        var result = mapper.MapValues(document, values);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(nameof(MapperTargetDocument.Count), result.Errors.Single().FieldName);
    }

    [TestMethod]
    public void MapValues_DoesNotWriteReadOnlyProperty()
    {
        var mapper = new DocumentValueMapper();
        var document = new MapperTargetDocument();
        var values = new Dictionary<string, object?>
        {
            [nameof(MapperTargetDocument.ReadOnlyText)] = "value"
        };

        var result = mapper.MapValues(document, values);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(nameof(MapperTargetDocument.ReadOnlyText), result.Errors.Single().FieldName);
    }

    private sealed class MapperTargetDocument
    {
        public string Name { get; set; } = string.Empty;

        public int? Count { get; set; }

        public decimal? Price { get; set; }

        public bool? IsActive { get; set; }

        public DateOnly? Date { get; set; }

        public TimeOnly? Time { get; set; }

        public ListOptionKey? Option { get; set; }

        public DateRange? Period { get; set; }

        public string ReadOnlyText => "read-only";
    }
}
