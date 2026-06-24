using MADOC.Domain.Printing.Anchors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Printing.Anchors;

[TestClass]
public class AnchorNameGeneratorTests
{
    [TestMethod]
    public void ToSnakeCase_Should_Convert_PascalCase_To_SnakeCase()
    {
        var result = AnchorNameGenerator.ToSnakeCase("RequesterFullName");

        Assert.AreEqual("requester_full_name", result);
    }

    [TestMethod]
    public void ToSnakeCase_Should_Keep_Existing_Underscore()
    {
        var result = AnchorNameGenerator.ToSnakeCase("Requester_FullName");

        Assert.AreEqual("requester_full_name", result);
    }

    [TestMethod]
    public void ToSnakeCase_Should_Handle_Acronyms()
    {
        var result = AnchorNameGenerator.ToSnakeCase("HTMLTemplateName");

        Assert.AreEqual("html_template_name", result);
    }

    [TestMethod]
    public void ToSnakeCase_Should_Handle_Digits()
    {
        var result = AnchorNameGenerator.ToSnakeCase("Field2Value");

        Assert.AreEqual("field2_value", result);
    }


    [TestMethod]
    public void ToSnakeCase_Should_Throw_When_Value_Is_Empty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            AnchorNameGenerator.ToSnakeCase(string.Empty);
        });
    }

    [TestMethod]
    public void CreateDocumentPrefix_Should_Remove_Document_Suffix()
    {
        var result = AnchorNameGenerator.CreateDocumentPrefix(
            typeof(CertificateRequestDocument));

        Assert.AreEqual("certificate_request", result);
    }

    [TestMethod]
    public void CreateDocumentPrefix_Should_Work_When_Type_Name_Has_No_Document_Suffix()
    {
        var result = AnchorNameGenerator.CreateDocumentPrefix(
            typeof(StudentApplication));

        Assert.AreEqual("student_application", result);
    }

    [TestMethod]
    public void CreateDocumentPrefix_Should_Throw_When_Document_Name_Becomes_Empty()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            AnchorNameGenerator.CreateDocumentPrefix(typeof(Document));
        });
    }

    [TestMethod]
    public void Create_Should_Create_Key_From_Prefix_And_Property()
    {
        var property = typeof(CertificateRequestDocument).GetProperty(
            nameof(CertificateRequestDocument.RequesterFullName))!;

        var key = AnchorNameGenerator.Create("certificate_request", property);

        Assert.AreEqual("certificate_request.requester_full_name", key.Value);
    }

    [TestMethod]
    public void Create_Should_Throw_When_Prefix_Is_Empty()
    {
        var property = typeof(CertificateRequestDocument).GetProperty(
            nameof(CertificateRequestDocument.RequesterFullName))!;

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            AnchorNameGenerator.Create(string.Empty, property);
        });
    }

    [TestMethod]
    public void Create_Should_Throw_When_Prefix_Ends_With_Dot()
    {
        var property = typeof(CertificateRequestDocument).GetProperty(
            nameof(CertificateRequestDocument.RequesterFullName))!;

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            AnchorNameGenerator.Create("certificate_request.", property);
        });
    }

    [TestMethod]
    public void Create_Should_Throw_When_Property_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            AnchorNameGenerator.Create("certificate_request", null!);
        });
    }

    private sealed class CertificateRequestDocument
    {
        public string RequesterFullName { get; set; } = string.Empty;
    }

    private sealed class StudentApplication
    {
    }

    private sealed class Document
    {
    }
}
