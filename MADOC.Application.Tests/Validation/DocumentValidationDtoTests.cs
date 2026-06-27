using MADOC.Application.Validation;

namespace MADOC.Application.Tests.Validation;

[TestClass]
public sealed class DocumentValidationDtoTests
{
    [TestMethod]
    public void DocumentValidationErrorDto_StoresFieldNameAndMessage()
    {
        var error = new DocumentValidationErrorDto("RequesterFullName", "Поле обязательно");

        Assert.AreEqual("RequesterFullName", error.FieldName);
        Assert.AreEqual("Поле обязательно", error.Message);
    }

    [TestMethod]
    public void DocumentValidationErrorDto_ThrowsForEmptyFieldName()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentValidationErrorDto("", "Ошибка"));
    }

    [TestMethod]
    public void DocumentValidationErrorDto_ThrowsForEmptyMessage()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentValidationErrorDto("Field", ""));
    }

    [TestMethod]
    public void DocumentValidationRequest_StoresDocumentTypeAndValues()
    {
        var values = new Dictionary<string, object?>
        {
            ["RequesterFullName"] = "Иванов Иван"
        };

        var request = new DocumentValidationRequest("certificate_request", values);

        Assert.AreEqual("certificate_request", request.DocumentType.Value);
        Assert.AreSame(values, request.Values);
    }

    [TestMethod]
    public void DocumentValidationRequest_ThrowsForNullValues()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new DocumentValidationRequest("certificate_request", null!));
    }

    [TestMethod]
    public void DocumentValidationResponse_StoresInvalidResultWithErrors()
    {
        var errors = new[]
        {
            new DocumentValidationErrorDto("RequesterFullName", "Поле обязательно")
        };

        var response = new DocumentValidationResponse("certificate_request", isValid: false, errors);

        Assert.AreEqual("certificate_request", response.DocumentType.Value);
        Assert.IsFalse(response.IsValid);
        Assert.AreEqual(1, response.Errors.Count);
    }

    [TestMethod]
    public void DocumentValidationResponse_ThrowsWhenValidResultContainsErrors()
    {
        var errors = new[]
        {
            new DocumentValidationErrorDto("RequesterFullName", "Поле обязательно")
        };

        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentValidationResponse("certificate_request", isValid: true, errors));
    }
}
