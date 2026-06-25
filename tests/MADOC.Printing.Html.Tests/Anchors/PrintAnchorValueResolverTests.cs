using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;
using MADOC.Printing.Html.Anchors;


namespace MADOC.Tests.Domain.Printing.Anchors;

[TestClass]
public class PrintAnchorValueResolverTests
{
    [TestMethod]
    public void GetRawValue_Should_Return_Unformatted_Value()
    {
        var document = CreateDocument();
        var resolver = CreateResolver();

        var value = resolver.GetRawValue(
            document,
            "value_document.number");

        Assert.AreEqual(15, value);
    }

    [TestMethod]
    public void GetRawValue_Should_Throw_When_Document_Is_Null()
    {
        var resolver = CreateResolver();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            resolver.GetRawValue(null!, "value_document.number");
        });
    }

    [TestMethod]
    public void GetDisplayValue_Should_Return_Empty_String_For_Null()
    {
        var document = CreateDocument();
        document.OptionalText = null;

        var value = CreateResolver().GetDisplayValue(
            document,
            "value_document.optional_text");

        Assert.AreEqual(string.Empty, value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_String()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.text");

        Assert.AreEqual("Текст", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_Int_Long_Double_Decimal_And_Float()
    {
        var document = CreateDocument();
        var resolver = CreateResolver();

        Assert.AreEqual("15", resolver.GetDisplayValue(document, "value_document.number"));
        Assert.AreEqual("123456789", resolver.GetDisplayValue(document, "value_document.long_number"));
        Assert.AreEqual("12.35", resolver.GetDisplayValue(document, "value_document.double_number"));
        Assert.AreEqual("77.89", resolver.GetDisplayValue(document, "value_document.decimal_number"));
        Assert.AreEqual("1.5", resolver.GetDisplayValue(document, "value_document.float_number"));
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_Nullable_Value_Types_When_They_Have_Value()
    {
        var document = CreateDocument();
        var resolver = CreateResolver();

        Assert.AreEqual("22", resolver.GetDisplayValue(document, "value_document.nullable_number"));
        Assert.AreEqual("30.06.2026", resolver.GetDisplayValue(document, "value_document.nullable_date"));
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_Bool_As_Russian_Text()
    {
        var document = CreateDocument();
        var resolver = CreateResolver();

        Assert.AreEqual("Да", resolver.GetDisplayValue(document, "value_document.true_value"));
        Assert.AreEqual("Нет", resolver.GetDisplayValue(document, "value_document.false_value"));
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_DateOnly()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.date");

        Assert.AreEqual("30.06.2026", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_TimeOnly()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.time");

        Assert.AreEqual("14:30", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_DateTime()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.date_time");

        Assert.AreEqual("24.06.2026 09:10", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_DateRange()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.date_range");

        Assert.AreEqual("01.06.2026 — 30.06.2026", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_DateTimeRange()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.date_time_range");

        Assert.AreEqual("01.06.2026 10:00 — 30.06.2026 18:00", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Format_ListOptionKey_As_DisplayName()
    {
        var value = CreateResolver().GetDisplayValue(
            CreateDocument(),
            "value_document.status");

        Assert.AreEqual("Одобрено", value);
    }

    [TestMethod]
    public void GetDisplayValue_Should_Throw_When_ListOptionKey_Document_Does_Not_Provide_Catalog()
    {
        var document = new DocumentWithoutListCatalog
        {
            Status = new ListOptionKey("approved")
        };

        var resolver = new PrintAnchorValueResolver<DocumentWithoutListCatalog>(
            new PrintAnchorProfile<DocumentWithoutListCatalog>());

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            resolver.GetDisplayValue(
                document,
                "document_without_list_catalog.status");
        });
    }

    [TestMethod]
    public void GetDisplayValue_Should_Throw_When_ListOptionKey_Is_Not_In_Catalog()
    {
        var document = CreateDocument();
        document.Status = new ListOptionKey("missing");

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            CreateResolver().GetDisplayValue(
                document,
                "value_document.status");
        });
    }

    [TestMethod]
    public void GetDisplayValue_Should_Throw_When_Anchor_Is_Unknown()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            CreateResolver().GetDisplayValue(
                CreateDocument(),
                "value_document.unknown");
        });
    }

    private static PrintAnchorValueResolver<ValueDocument> CreateResolver()
    {
        return new PrintAnchorValueResolver<ValueDocument>(
            new PrintAnchorProfile<ValueDocument>("value_document"));
    }

    private static ValueDocument CreateDocument()
    {
        return new ValueDocument
        {
            Text = "Текст",
            OptionalText = "Комментарий",
            Number = 15,
            NullableNumber = 22,
            LongNumber = 123456789,
            DoubleNumber = 12.345,
            DecimalNumber = 77.891m,
            FloatNumber = 1.5f,
            TrueValue = true,
            FalseValue = false,
            Date = new DateOnly(2026, 6, 30),
            NullableDate = new DateOnly(2026, 6, 30),
            Time = new TimeOnly(14, 30),
            DateTime = new DateTime(2026, 6, 24, 9, 10, 0),
            DateRange = new DateRange(
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 30)),
            DateTimeRange = new DateTimeRange(
                new DateTime(2026, 6, 1, 10, 0, 0),
                new DateTime(2026, 6, 30, 18, 0, 0)),
            Status = new ListOptionKey("approved")
        };
    }

    private sealed class ValueDocument : IListConfigurationProvider
    {
        public string Text { get; set; } = string.Empty;

        public string? OptionalText { get; set; }

        public int Number { get; set; }

        public int? NullableNumber { get; set; }

        public long LongNumber { get; set; }

        public double DoubleNumber { get; set; }

        public decimal DecimalNumber { get; set; }

        public float FloatNumber { get; set; }

        public bool TrueValue { get; set; }

        public bool FalseValue { get; set; }

        public DateOnly Date { get; set; }

        public DateOnly? NullableDate { get; set; }

        public TimeOnly Time { get; set; }

        public DateTime DateTime { get; set; }

        public DateRange DateRange { get; set; }

        public DateTimeRange DateTimeRange { get; set; }

        public ListOptionKey Status { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(
                nameof(Status),
                new ListOption(
                    new ListOptionKey("approved"),
                    "Одобрено"),
                new ListOption(
                    new ListOptionKey("rejected"),
                    "Отклонено"));

            return catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return new ListDependencySchema();
        }
    }

    private sealed class DocumentWithoutListCatalog
    {
        public ListOptionKey Status { get; set; }
    }
}
