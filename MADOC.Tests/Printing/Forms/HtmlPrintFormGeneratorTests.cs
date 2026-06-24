using MADOC.Domain.Printing.Anchors;
using MADOC.Domain.Printing.Forms;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Printing.Forms;

[TestClass]
public class HtmlPrintFormGeneratorTests
{
    [TestMethod]
    public void Constructor_Should_Throw_When_Profile_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            _ = new HtmlPrintFormGenerator<GeneratorDocument>(null!);
        });
    }

    [TestMethod]
    public void Generate_Should_Throw_When_Document_Is_Null()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            generator.Generate(null!, "<p>template</p>");
        });
    }

    [TestMethod]
    public void Generate_Should_Throw_When_Template_Is_Null()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            generator.Generate(CreateDocument(), null!);
        });
    }

    [TestMethod]
    public void Generate_Should_Return_Original_Html_When_Template_Has_No_Anchors()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>();

        var result = generator.Generate(
            CreateDocument(),
            "<p>Обычный HTML без якорей</p>");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("<p>Обычный HTML без якорей</p>", result.Content);
    }

    [TestMethod]
    public void Generate_Should_Replace_Document_String_Anchor()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc:generator.name }}</p>");

        AssertSuccessContent(result, "<p>Иванов Иван Иванович</p>");
    }

    [TestMethod]
    public void Generate_Should_Trim_Anchor_Content()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{    doc:generator.name    }}</p>");

        AssertSuccessContent(result, "<p>Иванов Иван Иванович</p>");
    }

    [TestMethod]
    public void Generate_Should_Allow_Spaces_Around_Doc_Colon()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc   :   generator.name }}</p>");

        AssertSuccessContent(result, "<p>Иванов Иван Иванович</p>");
    }

    [TestMethod]
    public void Generate_Should_Replace_Multiple_Document_Anchors()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "ФИО: {{ doc:generator.name }}, группа: {{ doc:generator.group_name }}, копий: {{ doc:generator.copies_count }}");

        AssertSuccessContent(
            result,
            "ФИО: Иванов Иван Иванович, группа: ИС-21, копий: 2");
    }

    [TestMethod]
    public void Generate_Should_Replace_Repeated_Document_Anchors()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "{{ doc:generator.group_name }} / {{ doc:generator.group_name }}");

        AssertSuccessContent(result, "ИС-21 / ИС-21");
    }

    [TestMethod]
    public void Generate_Should_Replace_Computed_Document_Property()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc:generator.summary }}</p>");

        AssertSuccessContent(
            result,
            "<p>Иванов Иван Иванович / ИС-21 / 2</p>");
    }

    [TestMethod]
    public void Generate_Should_Replace_Null_Document_Value_With_Empty_String()
    {
        var document = CreateDocument();
        document.OptionalComment = null;

        var result = CreateGenerator().Generate(
            document,
            "<p>{{ doc:generator.optional_comment }}</p>");

        AssertSuccessContent(result, "<p></p>");
    }

    [TestMethod]
    public void Generate_Should_Replace_ListOptionKey_With_DisplayName()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc:generator.status }}</p>");

        AssertSuccessContent(result, "<p>Одобрено</p>");
    }

    [TestMethod]
    public void Generate_Should_Replace_Generated_Field()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["print_date"] = "24.06.2026"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>Дата печати: {{ print_date }}</p>",
            generatedFields);

        AssertSuccessContent(result, "<p>Дата печати: 24.06.2026</p>");
    }

    [TestMethod]
    public void Generate_Should_Replace_Document_And_Generated_Fields_Together()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["print_date"] = "24.06.2026",
            ["document_number"] = "CR-0001"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>ФИО: {{ doc:generator.name }}</p><p>Дата: {{ print_date }}</p><p>Номер: {{ document_number }}</p>",
            generatedFields);

        AssertSuccessContent(
            result,
            "<p>ФИО: Иванов Иван Иванович</p><p>Дата: 24.06.2026</p><p>Номер: CR-0001</p>");
    }

    [TestMethod]
    public void Generate_Should_Use_Case_Sensitive_Generated_Field_Names()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["printDate"] = "24.06.2026"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ printdate }}</p>",
            generatedFields);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("<p>{{ printdate }}</p>", result.Content);
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_When_Generated_Field_Has_Cyrillic_Name()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["ДатаПечати"] = "24.06.2026"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ ДатаПечати }}</p>",
            generatedFields);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("<p>{{ ДатаПечати }}</p>", result.Content);
        Assert.HasCount(1, result.Errors);
        StringAssert.Contains(result.Errors[0], "некорректный формат");
    }

    [TestMethod]
    public void Generate_Should_Html_Encode_Document_Value_By_Default()
    {
        var document = CreateDocument();
        document.Name = "Иванов <script>";

        var result = CreateGenerator().Generate(
            document,
            "<p>{{ doc:generator.name }}</p>");

        AssertSuccessContent(result, "<p>Иванов &lt;script&gt;</p>");
    }

    [TestMethod]
    public void Generate_Should_Html_Encode_Generated_Value_By_Default()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["value"] = "<b>текст</b>"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ value }}</p>",
            generatedFields);

        AssertSuccessContent(result, "<p>&lt;b&gt;текст&lt;/b&gt;</p>");
    }

    [TestMethod]
    public void Generate_Should_Not_Html_Encode_When_Encoding_Is_Disabled()
    {
        var document = CreateDocument();
        document.Name = "Иванов <b>";

        var generator = new HtmlPrintFormGenerator<GeneratorDocument>(
            htmlEncodeValues: false);

        var result = generator.Generate(
            document,
            "<p>{{ doc:generator.name }}</p>");

        AssertSuccessContent(result, "<p>Иванов <b></p>");
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_When_Document_Anchor_Is_Unknown_In_Strict_Mode()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc:generator.fake_field }}</p>");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("<p>{{ doc:generator.fake_field }}</p>", result.Content);
        StringAssert.Contains(result.Errors[0], "Не удалось обработать якорь");
    }

    [TestMethod]
    public void Generate_Should_Replace_Unknown_Document_Anchor_With_Empty_String_In_Non_Strict_Mode()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>(
            strictMode: false);

        var result = generator.Generate(
            CreateDocument(),
            "<p>{{ doc:generator.fake_field }}</p>");

        AssertSuccessContent(result, "<p></p>");
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_When_Generated_Field_Is_Missing_In_Strict_Mode()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ print_date }}</p>");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("<p>{{ print_date }}</p>", result.Content);
        StringAssert.Contains(result.Errors[0], "Сгенерированное поле");
    }

    [TestMethod]
    public void Generate_Should_Replace_Missing_Generated_Field_With_Empty_String_In_Non_Strict_Mode()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>(
            strictMode: false);

        var result = generator.Generate(
            CreateDocument(),
            "<p>{{ print_date }}</p>");

        AssertSuccessContent(result, "<p></p>");
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_When_Anchor_Is_Empty_In_Strict_Mode()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ }}</p>");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("<p>{{ }}</p>", result.Content);
        StringAssert.Contains(result.Errors[0], "некорректный формат");
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_When_Anchor_Has_Invalid_Characters_In_Strict_Mode()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ invalid anchor }}</p>");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("<p>{{ invalid anchor }}</p>", result.Content);
        StringAssert.Contains(result.Errors[0], "некорректный формат");
    }

    [TestMethod]
    public void Generate_Should_Replace_Invalid_Anchor_With_Empty_String_In_Non_Strict_Mode()
    {
        var generator = new HtmlPrintFormGenerator<GeneratorDocument>(
            strictMode: false);

        var result = generator.Generate(
            CreateDocument(),
            "<p>{{ invalid anchor }}</p>");

        AssertSuccessContent(result, "<p></p>");
    }

    [TestMethod]
    public void Generate_Should_Return_Failed_For_Doc_Prefix_With_Empty_Name()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc: }}</p>");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("<p>{{ doc: }}</p>", result.Content);
    }

    [TestMethod]
    public void Generate_Should_Treat_DocWithoutColon_As_Generated_Field()
    {
        var generatedFields = new Dictionary<string, string>
        {
            ["doc"] = "Значение обычного якоря"
        };

        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc }}</p>",
            generatedFields);

        AssertSuccessContent(result, "<p>Значение обычного якоря</p>");
    }

    [TestMethod]
    public void Generate_Should_Leave_Unclosed_Anchor_As_Text()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "<p>{{ doc:generator.name</p>");

        AssertSuccessContent(result, "<p>{{ doc:generator.name</p>");
    }

    [TestMethod]
    public void Generate_Should_Process_Text_After_Last_Anchor()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "Начало {{ doc:generator.name }} конец");

        AssertSuccessContent(result, "Начало Иванов Иван Иванович конец");
    }

    [TestMethod]
    public void Generate_Should_Collect_Multiple_Errors_In_Strict_Mode()
    {
        var result = CreateGenerator().Generate(
            CreateDocument(),
            "{{ doc:generator.fake_field }} / {{ missing_generated }} / {{ invalid anchor }}");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(3, result.Errors.Count);
        StringAssert.Contains(result.Content, "{{ doc:generator.fake_field }}");
        StringAssert.Contains(result.Content, "{{ missing_generated }}");
        StringAssert.Contains(result.Content, "{{ invalid anchor }}");
    }

    private static HtmlPrintFormGenerator<GeneratorDocument> CreateGenerator()
    {
        return new HtmlPrintFormGenerator<GeneratorDocument>();
    }

    private static GeneratorDocument CreateDocument()
    {
        return new GeneratorDocument
        {
            Name = "Иванов Иван Иванович",
            GroupName = "ИС-21",
            CopiesCount = 2,
            OptionalComment = "Комментарий",
            Status = new ListOptionKey("approved")
        };
    }

    private static void AssertSuccessContent(
        HtmlPrintFormGenerationResult result,
        string expectedContent)
    {
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.AreEqual(expectedContent, result.Content);
    }

    private sealed class GeneratorDocument : IListConfigurationProvider
    {
        public string Name { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public int CopiesCount { get; set; }

        public string? OptionalComment { get; set; }

        public ListOptionKey Status { get; set; }

        public string Summary
        {
            get
            {
                return $"{Name} / {GroupName} / {CopiesCount}";
            }
        }

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
}
