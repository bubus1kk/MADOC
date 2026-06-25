using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MADOC.Domain.Core.Documents;
using MADOC.Domain.Core.Documents.ListConfigurations;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Documents;

[TestClass]
public class CertificateRequestDocumentTests
{
    [TestMethod]
    public void GraphValidator_Should_Return_No_Errors()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(CertificateRequestDocument));

        Assert.AreEqual(0, results.Count, JoinErrors(results));
    }

    [TestMethod]
    public void Document_Should_Be_Valid_When_List_Combination_Is_Allowed()
    {
        var document = CreateValidDocument();

        Assert.IsTrue(IsValid(document));
    }

    [TestMethod]
    public void Document_Should_Be_Invalid_When_Purpose_Is_Not_Allowed_For_CertificateType()
    {
        var document = CreateValidDocument();

        document.CertificateType = CertificateRequestListConfiguration.CertificateType.Study.Key;
        document.CertificatePurpose = CertificateRequestListConfiguration.CertificatePurpose.Archive.Key;

        Assert.IsFalse(IsValid(document));
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_Purposes_For_Study_Certificate()
    {
        var document = CreateValidDocument();
        document.CertificateType = CertificateRequestListConfiguration.CertificateType.Study.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(CertificateRequestDocument.CertificatePurpose));

        AssertKeys(
            values,
            CertificateRequestListConfiguration.CertificatePurpose.AnyPlace.Key,
            CertificateRequestListConfiguration.CertificatePurpose.SocialProtection.Key,
            CertificateRequestListConfiguration.CertificatePurpose.Employer.Key);
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_ReceivePlaces_For_Electronic_Format()
    {
        var document = CreateValidDocument();
        document.CertificateFormat = CertificateRequestListConfiguration.CertificateFormat.Electronic.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(CertificateRequestDocument.ReceivePlace));

        AssertKeys(
            values,
            CertificateRequestListConfiguration.ReceivePlace.PersonalAccount.Key,
            CertificateRequestListConfiguration.ReceivePlace.Email.Key);
    }

    [TestMethod]
    public void Computed_Properties_Should_Work()
    {
        var document = CreateValidDocument();

        Assert.IsFalse(document.NeedStamp);
        Assert.IsTrue(document.IsElectronicCertificate);
        Assert.IsFalse(document.IsPaperCertificate);
        Assert.IsFalse(document.RequiresOfficeVisit);
        Assert.AreEqual("Справка об обучении: По месту требования", document.CertificateSummary);
    }

    private static CertificateRequestDocument CreateValidDocument()
    {
        return new CertificateRequestDocument
        {
            RequesterFullName = "Иванов Иван Иванович",
            RequesterGroup = "ИС-21",
            CopiesCount = 1,
            DesiredReceiveDate = new DateOnly(2026, 3, 10),
            DesiredReceiveTime = new TimeOnly(10, 0),
            OrganizationName = "Колледж",
            CertificateType = CertificateRequestListConfiguration.CertificateType.Study.Key,
            CertificatePurpose = CertificateRequestListConfiguration.CertificatePurpose.AnyPlace.Key,
            CertificateFormat = CertificateRequestListConfiguration.CertificateFormat.Electronic.Key,
            ReceivePlace = CertificateRequestListConfiguration.ReceivePlace.Email.Key
        };
    }

    private static bool IsValid(object document)
    {
        var results = new List<ValidationResult>();

        return Validator.TryValidateObject(
            document,
            new ValidationContext(document),
            results,
            true);
    }

    private static void AssertKeys(
        IReadOnlyList<ListOptionKey> actual,
        params ListOptionKey[] expected)
    {
        CollectionAssert.AreEqual(
            expected.ToList(),
            actual.ToList());
    }

    private static string JoinErrors(IReadOnlyList<ValidationResult> results)
    {
        return string.Join(
            "\n",
            results.Select(result => result.ErrorMessage));
    }
}
