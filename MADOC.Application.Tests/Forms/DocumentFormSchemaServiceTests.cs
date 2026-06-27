using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.Application.Tests.TestSupport;

namespace MADOC.Application.Tests.Forms;

[TestClass]
public sealed class DocumentFormSchemaServiceTests
{
    [TestMethod]
    public void GetSchema_ReturnsSchemaForRegisteredDocument()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        var schema = service.GetSchema("certificate_request");

        Assert.AreEqual(new DocumentTypeKey("certificate_request"), schema.DocumentType);
        Assert.AreEqual("Заявка на справку", schema.DisplayName);
        Assert.IsTrue(schema.Fields.Count > 0);
    }

    [TestMethod]
    public void GetSchema_MapsStringPropertyToTextFieldWithConstraints()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        var schema = service.GetSchema("certificate_request");
        var field = schema.Fields.Single(field => field.Name == "RequesterFullName");

        Assert.AreEqual("ФИО заявителя", field.DisplayName);
        Assert.AreEqual(DocumentFieldType.Text, field.Type);
        Assert.IsFalse(field.IsReadOnly);
        Assert.AreEqual("certificate_request.requester_full_name", field.AnchorName);
        Assert.IsTrue(field.Constraints.IsRequired);
        Assert.AreEqual(150, field.Constraints.MaxLength);
    }

    [TestMethod]
    public void GetSchema_MapsIndependentListFieldWithOptions()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        var schema = service.GetSchema("certificate_request");
        var field = schema.Fields.Single(field => field.Name == "CertificateType");

        Assert.AreEqual(DocumentFieldType.List, field.Type);
        Assert.IsTrue(field.Constraints.IsRequired);
        Assert.IsNull(field.Dependency);
        Assert.IsTrue(field.Options.Count > 0);
        Assert.IsTrue(field.Options.All(option => !string.IsNullOrWhiteSpace(option.Value)));
        Assert.IsTrue(field.Options.All(option => !string.IsNullOrWhiteSpace(option.DisplayName)));
    }

    [TestMethod]
    public void GetSchema_MapsDependentListFieldWithoutInitialOptions()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        var schema = service.GetSchema("certificate_request");
        var field = schema.Fields.Single(field => field.Name == "CertificatePurpose");

        Assert.AreEqual(DocumentFieldType.List, field.Type);
        Assert.IsNotNull(field.Dependency);
        CollectionAssert.Contains(field.Dependency.ParentFieldNames.ToList(), "CertificateType");
        Assert.AreEqual(0, field.Options.Count);
    }

    [TestMethod]
    public void GetSchema_MapsComputedPropertyAsReadOnlyField()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        var schema = service.GetSchema("certificate_request");
        var field = schema.Fields.Single(field => field.Name == "NeedStamp");

        Assert.AreEqual(DocumentFieldType.Boolean, field.Type);
        Assert.IsTrue(field.IsReadOnly);
        Assert.AreEqual("certificate_request.need_stamp", field.AnchorName);
    }

    [TestMethod]
    public void GetSchema_ThrowsForUnknownDocumentType()
    {
        var service = ApplicationTestServices.CreateFormSchemaService();

        Assert.ThrowsExactly<InvalidOperationException>(() => service.GetSchema("unknown_document"));
    }
}
