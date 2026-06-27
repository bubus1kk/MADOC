using MADOC.Application.Printing;

namespace MADOC.Application.Tests.Printing;

[TestClass]
public sealed class PrintFormRequestResponseTests
{
    [TestMethod]
    public void PrintFormRequest_StoresValuesAndUsesSafeDefaults()
    {
        var values = new Dictionary<string, object?>
        {
            ["RequesterFullName"] = "Иванов Иван"
        };

        var request = new PrintFormRequest("certificate_request", values);

        Assert.AreEqual("certificate_request", request.DocumentType.Value);
        Assert.AreSame(values, request.Values);
        Assert.AreEqual(0, request.GeneratedFields.Count);
        Assert.IsNull(request.TemplateFileName);
        Assert.IsTrue(request.StrictMode);
        Assert.IsTrue(request.HtmlEncodeValues);
    }

    [TestMethod]
    public void PrintFormRequest_StoresCustomSettings()
    {
        var values = new Dictionary<string, object?>();
        var generatedFields = new Dictionary<string, object?>
        {
            ["generated_date"] = new DateOnly(2026, 6, 25)
        };

        var request = new PrintFormRequest(
            "certificate_request",
            values,
            generatedFields,
            "custom.html",
            strictMode: false,
            htmlEncodeValues: false);

        Assert.AreSame(generatedFields, request.GeneratedFields);
        Assert.AreEqual("custom.html", request.TemplateFileName);
        Assert.IsFalse(request.StrictMode);
        Assert.IsFalse(request.HtmlEncodeValues);
    }

    [TestMethod]
    public void PrintFormRequest_ThrowsForNullValues()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new PrintFormRequest("certificate_request", null!));
    }

    [TestMethod]
    public void PrintFormResponseSuccess_ReturnsSuccessfulResponse()
    {
        var response = PrintFormResponse.Success("certificate_request", "<html></html>");

        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual("certificate_request", response.DocumentType.Value);
        Assert.AreEqual("<html></html>", response.HtmlContent);
        Assert.AreEqual(0, response.Errors.Count);
    }

    [TestMethod]
    public void PrintFormResponseFailed_ReturnsFailedResponseWithErrors()
    {
        var response = PrintFormResponse.Failed(
            "certificate_request",
            "",
            new[] { "Ошибка генерации" });

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual("", response.HtmlContent);
        Assert.AreEqual(1, response.Errors.Count);
    }

    [TestMethod]
    public void PrintFormResponseFailed_ThrowsWhenErrorsAreEmpty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            PrintFormResponse.Failed("certificate_request", "", Array.Empty<string>()));
    }
}
