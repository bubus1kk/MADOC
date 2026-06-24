using MADOC.Domain.Printing.Anchors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Printing.Anchors;

[TestClass]
public class AnchorDefinitionTests
{
    private sealed class TestDocument
    {
        public string Name { get; set; } = string.Empty;

        public string? OptionalText { get; set; }
    }

    [TestMethod]
    public void Constructor_Should_Save_Key_And_Field_Name()
    {
        var key = new AnchorKey("test_document.name");

        var definition = new AnchorDefinition<TestDocument>(key,nameof(TestDocument.Name), document => document.Name);

        Assert.AreEqual(key, definition.Key);
        Assert.AreEqual(nameof(TestDocument.Name), definition.FieldName);
    }

    [TestMethod]
    public void Constructor_Should_Throw_When_Field_Name_Is_Empty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorDefinition<TestDocument>(new AnchorKey("test_document.name"),string.Empty,document => document.Name);
        });
    }

    [TestMethod]
    public void Constructor_Should_Throw_When_Value_Getter_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new AnchorDefinition<TestDocument>(new AnchorKey("test_document.name"), nameof(TestDocument.Name),null!);
        });
    }

    [TestMethod]
    public void GetValue_Should_Return_Value_From_Document()
    {
        var document = new TestDocument
        {
            Name = "Иванов Иван Иванович"
        };

        var definition = new AnchorDefinition<TestDocument>(new AnchorKey("test_document.name"), nameof(TestDocument.Name), source => source.Name);

        var value = definition.GetValue(document);

        Assert.AreEqual("Иванов Иван Иванович", value);
    }

    [TestMethod]
    public void GetValue_Should_Return_Null_When_Getter_Returns_Null()
    {
        var document = new TestDocument();

        var definition = new AnchorDefinition<TestDocument>(new AnchorKey("test_document.optional_text"), nameof(TestDocument.OptionalText), source => source.OptionalText);

        var value = definition.GetValue(document);

        Assert.IsNull(value);
    }

    [TestMethod]
    public void GetValue_Should_Throw_When_Document_Is_Null()
    {
        var definition = new AnchorDefinition<TestDocument>(new AnchorKey("test_document.name"), nameof(TestDocument.Name), source => source.Name);

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            definition.GetValue(null!);
        });
    }
}
