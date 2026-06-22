using MADOC.Domain.Documents;
using MADOC.Domain.Documents.ListConfigurations;
using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Tests.Domain.Documents;

[TestClass]
public class StudentApplicationDocumentTests
{
    [TestMethod]
    public void GraphValidator_Should_Return_No_Errors()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(StudentApplicationDocument));

        Assert.AreEqual(0, results.Count, JoinErrors(results));
    }

    [TestMethod]
    public void Document_Should_Be_Valid_When_List_Combination_Is_Allowed()
    {
        var document = CreateValidDocument();

        Assert.IsTrue(IsValid(document));
    }

    [TestMethod]
    public void Document_Should_Be_Invalid_When_ApplicationType_Is_Not_Allowed_For_ApplicantCategory()
    {
        var document = CreateValidDocument();

        document.ApplicantCategory = StudentApplicationListConfiguration.ApplicantCategory.Student.Key;
        document.ApplicationType = StudentApplicationListConfiguration.ApplicationType.GroupCertificate.Key;

        Assert.IsFalse(IsValid(document));
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_ApplicationTypes_For_Student()
    {
        var document = CreateValidDocument();
        document.ApplicantCategory = StudentApplicationListConfiguration.ApplicantCategory.Student.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(StudentApplicationDocument.ApplicationType));

        AssertKeys(
            values,
            StudentApplicationListConfiguration.ApplicationType.Certificate.Key,
            StudentApplicationListConfiguration.ApplicationType.AcademicLeave.Key,
            StudentApplicationListConfiguration.ApplicationType.FinancialAid.Key,
            StudentApplicationListConfiguration.ApplicationType.Dormitory.Key);
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_ReceiveMethods_For_Study_Subtype()
    {
        var document = CreateValidDocument();
        document.ApplicationSubtype = StudentApplicationListConfiguration.ApplicationSubtype.Study.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(StudentApplicationDocument.ReceiveMethod));

        AssertKeys(
            values,
            StudentApplicationListConfiguration.ReceiveMethod.Electronic.Key,
            StudentApplicationListConfiguration.ReceiveMethod.Paper.Key);
    }

    [TestMethod]
    public void Computed_Properties_Should_Work()
    {
        var document = CreateValidDocument();

        Assert.AreEqual(3, document.AbsenceDays);
        Assert.IsFalse(document.IsLongAbsence);
        Assert.IsFalse(document.RequiresPaperProcessing);
        Assert.AreEqual("Заявление на справку: Об обучении", document.ApplicationSummary);
    }

    private static StudentApplicationDocument CreateValidDocument()
    {
        return new StudentApplicationDocument
        {
            FullName = "Иванов Иван Иванович",
            GroupName = "ИС-21",
            Age = 18,
            BirthDate = new DateOnly(2006, 5, 10),
            AbsencePeriod = new DateRange(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 3)),
            NeedPaperCopy = false,
            Comment = "Тестовое заявление",
            ApplicantCategory = StudentApplicationListConfiguration.ApplicantCategory.Student.Key,
            ApplicationType = StudentApplicationListConfiguration.ApplicationType.Certificate.Key,
            ApplicationSubtype = StudentApplicationListConfiguration.ApplicationSubtype.Study.Key,
            ReceiveMethod = StudentApplicationListConfiguration.ReceiveMethod.Electronic.Key
        };
    }

    private static bool IsValid(object document)
    {
        var results = new List<ValidationResult>();

        return Validator.TryValidateObject(document, new ValidationContext(document), results, true);
    }

    private static void AssertKeys(IReadOnlyList<ListOptionKey> actual, params ListOptionKey[] expected)
    {
        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    private static string JoinErrors(IReadOnlyList<ValidationResult> results)
    {
        return string.Join("\n", results.Select(result => result.ErrorMessage));
    }
}
