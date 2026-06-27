using MADOC.Application.Validation;
using MADOC.Application.Tests.TestSupport;

namespace MADOC.Application.Tests.Validation;

[TestClass]
public sealed class DocumentValidationServiceTests
{
    [TestMethod]
    public void Validate_ReturnsInvalidResponseForEmptyRequiredFields()
    {
        var service = ApplicationTestServices.CreateValidationService();
        var request = new DocumentValidationRequest(
            "certificate_request",
            new Dictionary<string, object?>());

        var response = service.Validate(request);

        Assert.IsFalse(response.IsValid);
        Assert.IsTrue(response.Errors.Count > 0);
        Assert.IsTrue(response.Errors.Any(error => error.FieldName == "RequesterFullName"));
    }

    [TestMethod]
    public void Validate_ReturnsMappingErrorsWhenValuesCannotBeConverted()
    {
        var service = ApplicationTestServices.CreateValidationService();
        var request = new DocumentValidationRequest(
            "certificate_request",
            new Dictionary<string, object?>
            {
                ["CopiesCount"] = "not-number"
            });

        var response = service.Validate(request);

        Assert.IsFalse(response.IsValid);
        Assert.IsTrue(response.Errors.Any(error => error.FieldName == "CopiesCount"));
    }

    [TestMethod]
    public void Validate_ReturnsMappingErrorForUnknownField()
    {
        var service = ApplicationTestServices.CreateValidationService();
        var request = new DocumentValidationRequest(
            "certificate_request",
            new Dictionary<string, object?>
            {
                ["UnknownField"] = "value"
            });

        var response = service.Validate(request);

        Assert.IsFalse(response.IsValid);
        Assert.IsTrue(response.Errors.Any(error => error.FieldName == "UnknownField"));
    }

    [TestMethod]
    public void Validate_ThrowsForNullRequest()
    {
        var service = ApplicationTestServices.CreateValidationService();

        Assert.ThrowsExactly<ArgumentNullException>(() => service.Validate(null!));
    }
}
