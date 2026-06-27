using MADOC.Application.Lists;
using MADOC.Application.Tests.TestSupport;

namespace MADOC.Application.Tests.Lists;

[TestClass]
public sealed class DocumentFieldOptionsServiceTests
{
    [TestMethod]
    public void GetOptions_ReturnsOptionsForIndependentListField()
    {
        var service = ApplicationTestServices.CreateFieldOptionsService();
        var request = new DocumentFieldOptionsRequest("certificate_request", "CertificateType");

        var response = service.GetOptions(request);

        Assert.IsTrue(response.IsSuccess, string.Join(Environment.NewLine, response.Errors));
        Assert.IsTrue(response.Options.Count > 0);
        Assert.IsTrue(response.Options.All(option => !string.IsNullOrWhiteSpace(option.Value)));
        Assert.IsTrue(response.Options.All(option => !string.IsNullOrWhiteSpace(option.DisplayName)));
    }

    [TestMethod]
    public void GetOptions_UsesCurrentValuesForDependentListField()
    {
        var service = ApplicationTestServices.CreateFieldOptionsService();
        var certificateTypeResponse = service.GetOptions(new DocumentFieldOptionsRequest(
            "certificate_request",
            "CertificateType"));
        Assert.IsTrue(certificateTypeResponse.IsSuccess, string.Join(Environment.NewLine, certificateTypeResponse.Errors));

        DocumentFieldOptionsResponse? successfulDependentResponse = null;

        foreach (var certificateType in certificateTypeResponse.Options)
        {
            var dependentRequest = new DocumentFieldOptionsRequest(
                "certificate_request",
                "CertificatePurpose",
                new Dictionary<string, object?>
                {
                    ["CertificateType"] = certificateType.Value
                });

            var response = service.GetOptions(dependentRequest);

            if (response.IsSuccess && response.Options.Count > 0)
            {
                successfulDependentResponse = response;
                break;
            }
        }

        Assert.IsNotNull(successfulDependentResponse, "Не найдено ни одного значения CertificateType, для которого доступны варианты CertificatePurpose.");
    }

    [TestMethod]
    public void GetOptions_ReturnsFailedResponseForUnknownField()
    {
        var service = ApplicationTestServices.CreateFieldOptionsService();
        var request = new DocumentFieldOptionsRequest("certificate_request", "UnknownListField");

        var response = service.GetOptions(request);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsTrue(response.Errors.Count > 0);
    }

    [TestMethod]
    public void GetOptions_ReturnsFailedResponseWhenCurrentValuesCannotBeMapped()
    {
        var service = ApplicationTestServices.CreateFieldOptionsService();
        var request = new DocumentFieldOptionsRequest(
            "certificate_request",
            "CertificateType",
            new Dictionary<string, object?>
            {
                ["CopiesCount"] = "not-number"
            });

        var response = service.GetOptions(request);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsTrue(response.Errors.Any(error => error.Contains("CopiesCount", StringComparison.Ordinal)));
    }
}
