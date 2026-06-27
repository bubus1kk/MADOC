using MADOC.Application.Lists;

namespace MADOC.Application.Tests.Lists;

[TestClass]
public sealed class DocumentFieldOptionsRequestTests
{
    [TestMethod]
    public void Constructor_StoresDocumentTypeFieldNameAndValues()
    {
        var values = new Dictionary<string, object?>
        {
            ["CertificateType"] = "education"
        };

        var request = new DocumentFieldOptionsRequest("certificate_request", "CertificatePurpose", values);

        Assert.AreEqual("certificate_request", request.DocumentType.Value);
        Assert.AreEqual("CertificatePurpose", request.FieldName);
        Assert.AreSame(values, request.Values);
    }

    [TestMethod]
    public void Constructor_UsesEmptyValuesWhenValuesAreNotProvided()
    {
        var request = new DocumentFieldOptionsRequest("certificate_request", "CertificateType");

        Assert.AreEqual(0, request.Values.Count);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_ThrowsForEmptyFieldName(string? fieldName)
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentFieldOptionsRequest("certificate_request", fieldName!));
    }
}
