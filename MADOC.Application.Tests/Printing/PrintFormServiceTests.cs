using MADOC.Application.Documents;
using MADOC.Application.Printing;
using MADOC.Application.Tests.TestSupport;

namespace MADOC.Application.Tests.Printing;

[TestClass]
public sealed class PrintFormServiceTests
{
    [TestMethod]
    public void Generate_ReturnsHtmlWithDocumentAndGeneratedValues()
    {
        var registry = TestDocumentTypeRegistryFactory.CreateRegistryForTestPrintDocument();
        var templateProvider = new InMemoryPrintTemplateProvider(new Dictionary<string, string>
        {
            ["test_print.html"] = "<main>{{ doc:test_print.text }}|{{ doc:test_print.number }}|{{ doc:test_print.computed_text }}|{{ generated_date }}</main>"
        });
        var service = ApplicationTestServices.CreatePrintFormService(registry, templateProvider);
        var request = new PrintFormRequest(
            "test_print",
            new Dictionary<string, object?>
            {
                ["Text"] = "Привет",
                ["Number"] = "7"
            },
            new Dictionary<string, object?>
            {
                ["generated_date"] = new DateOnly(2026, 6, 25)
            });

        var response = service.Generate(request);

        Assert.IsTrue(response.IsSuccess, string.Join(Environment.NewLine, response.Errors));
        StringAssert.Contains(response.HtmlContent, "Привет");
        StringAssert.Contains(response.HtmlContent, "7");
        StringAssert.Contains(response.HtmlContent, "Привет-7");
        StringAssert.Contains(response.HtmlContent, "25.06.2026");
        Assert.IsFalse(response.HtmlContent.Contains("{{", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generate_ReturnsFailedResponseWhenValuesCannotBeMapped()
    {
        var registry = TestDocumentTypeRegistryFactory.CreateRegistryForTestPrintDocument();
        var templateProvider = new InMemoryPrintTemplateProvider(new Dictionary<string, string>
        {
            ["test_print.html"] = "<main>{{ doc:test_print.text }}</main>"
        });
        var service = ApplicationTestServices.CreatePrintFormService(registry, templateProvider);
        var request = new PrintFormRequest(
            "test_print",
            new Dictionary<string, object?>
            {
                ["Number"] = "not-number"
            });

        var response = service.Generate(request);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsTrue(response.Errors.Any(error => error.Contains("Number", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Generate_ReturnsFailedResponseWhenTemplateIsMissing()
    {
        var registry = TestDocumentTypeRegistryFactory.CreateRegistryForTestPrintDocument("missing.html");
        var templateProvider = new InMemoryPrintTemplateProvider(new Dictionary<string, string>());
        var service = ApplicationTestServices.CreatePrintFormService(registry, templateProvider);
        var request = new PrintFormRequest(
            "test_print",
            new Dictionary<string, object?>
            {
                ["Text"] = "Привет"
            });

        var response = service.Generate(request);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsTrue(response.Errors.Count > 0);
    }

    [TestMethod]
    public void Generate_UsesTemplateNameFromRequestWhenItIsProvided()
    {
        var registry = TestDocumentTypeRegistryFactory.CreateRegistryForTestPrintDocument("default.html");
        var templateProvider = new InMemoryPrintTemplateProvider(new Dictionary<string, string>
        {
            ["custom.html"] = "<main>{{ doc:test_print.text }}</main>"
        });
        var service = ApplicationTestServices.CreatePrintFormService(registry, templateProvider);
        var request = new PrintFormRequest(
            "test_print",
            new Dictionary<string, object?>
            {
                ["Text"] = "Кастомный шаблон"
            },
            templateFileName: "custom.html");

        var response = service.Generate(request);

        Assert.IsTrue(response.IsSuccess, string.Join(Environment.NewLine, response.Errors));
        StringAssert.Contains(response.HtmlContent, "Кастомный шаблон");
    }
}
