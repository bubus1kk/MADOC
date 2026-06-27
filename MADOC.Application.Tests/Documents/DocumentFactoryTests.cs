using MADOC.Application.Documents;
using MADOC.Domain.Core.Documents;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentFactoryTests
{
    [TestMethod]
    public void Create_ReturnsDocumentInstanceForRegisteredKey()
    {
        var factory = new DocumentFactory(new DocumentTypeRegistry());

        var document = factory.Create("certificate_request");

        Assert.IsInstanceOfType(document, typeof(CertificateRequestDocument));
    }

    [TestMethod]
    public void Create_ThrowsForUnknownDocumentType()
    {
        var factory = new DocumentFactory(new DocumentTypeRegistry());

        Assert.ThrowsExactly<InvalidOperationException>(() => factory.Create("unknown_document"));
    }

    [TestMethod]
    public void Constructor_ThrowsForNullRegistry()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DocumentFactory(null!));
    }
}
