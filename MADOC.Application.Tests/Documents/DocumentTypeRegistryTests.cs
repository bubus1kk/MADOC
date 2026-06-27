using MADOC.Application.Documents;
using MADOC.Domain.Core.Documents;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentTypeRegistryTests
{
    [TestMethod]
    public void DefaultRegistry_ContainsAllSupportedDocuments()
    {
        var registry = new DocumentTypeRegistry();

        var descriptors = registry.GetAll();

        Assert.AreEqual(5, descriptors.Count);
        Assert.IsTrue(descriptors.Any(descriptor => descriptor.Key.Value == "certificate_request"));
        Assert.IsTrue(descriptors.Any(descriptor => descriptor.Key.Value == "student_application"));
        Assert.IsTrue(descriptors.Any(descriptor => descriptor.Key.Value == "service_contract"));
        Assert.IsTrue(descriptors.Any(descriptor => descriptor.Key.Value == "business_trip_request"));
        Assert.IsTrue(descriptors.Any(descriptor => descriptor.Key.Value == "room_booking_request"));
    }

    [TestMethod]
    public void GetRequired_ReturnsDescriptorForRegisteredDocument()
    {
        var registry = new DocumentTypeRegistry();

        var descriptor = registry.GetRequired("certificate_request");

        Assert.AreEqual("certificate_request", descriptor.Key.Value);
        Assert.AreEqual("Заявка на справку", descriptor.DisplayName);
        Assert.AreEqual(typeof(CertificateRequestDocument), descriptor.DocumentType);
        Assert.AreEqual("certificate_request.html", descriptor.TemplateFileName);
    }

    [TestMethod]
    public void GetRequired_ThrowsForUnknownDocument()
    {
        var registry = new DocumentTypeRegistry();

        Assert.ThrowsExactly<InvalidOperationException>(() => registry.GetRequired("unknown_document"));
    }

    [TestMethod]
    public void Constructor_ThrowsForDuplicateKeys()
    {
        var descriptors = new[]
        {
            new DocumentTypeDescriptor("test_document", "Тестовый документ", typeof(CertificateRequestDocument), "test.html"),
            new DocumentTypeDescriptor("test_document", "Дубликат", typeof(StudentApplicationDocument), "duplicate.html")
        };

        Assert.ThrowsExactly<InvalidOperationException>(() => new DocumentTypeRegistry(descriptors));
    }

    [TestMethod]
    public void TryGet_ReturnsFalseForUnknownDocument()
    {
        var registry = new DocumentTypeRegistry();

        var found = registry.TryGet("missing_document", out var descriptor);

        Assert.IsFalse(found);
        Assert.IsNull(descriptor);
    }
}
