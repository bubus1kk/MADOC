using MADOC.Application.Documents;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentTypeKeyTests
{
    [TestMethod]
    public void Constructor_TrimsValue()
    {
        var key = new DocumentTypeKey("  certificate_request  ");

        Assert.AreEqual("certificate_request", key.Value);
        Assert.AreEqual("certificate_request", key.ToString());
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_ThrowsForEmptyValue(string? value)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new DocumentTypeKey(value!));
    }

    [TestMethod]
    public void ImplicitConversion_CreatesKeyFromString()
    {
        DocumentTypeKey key = "student_application";

        Assert.AreEqual("student_application", key.Value);
    }
}
