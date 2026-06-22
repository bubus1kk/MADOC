using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MADOC.Domain.Documents;
using MADOC.Domain.Documents.ListConfigurations;
using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Documents;

[TestClass]
public class ServiceContractDocumentTests
{
    [TestMethod]
    public void GraphValidator_Should_Return_No_Errors()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ServiceContractDocument));

        Assert.AreEqual(0, results.Count, JoinErrors(results));
    }

    [TestMethod]
    public void Document_Should_Be_Valid_When_List_Combination_Is_Allowed()
    {
        var document = CreateValidDocument();

        Assert.IsTrue(IsValid(document));
    }

    [TestMethod]
    public void Document_Should_Be_Invalid_When_ContractType_Is_Not_Allowed_For_ContractorType()
    {
        var document = CreateValidDocument();

        document.ContractorType = ServiceContractConfiguration.ContractorType.Individual.Key;
        document.ContractType = ServiceContractConfiguration.ContractType.SupplyContract.Key;

        Assert.IsFalse(IsValid(document));
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_ContractTypes_For_Individual()
    {
        var document = CreateValidDocument();
        document.ContractorType = ServiceContractConfiguration.ContractorType.Individual.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(ServiceContractDocument.ContractType));

        AssertKeys(
            values,
            ServiceContractConfiguration.ContractType.ServiceContract.Key,
            ServiceContractConfiguration.ContractType.CivilLawContract.Key,
            ServiceContractConfiguration.ContractType.LeaseContract.Key);
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_PaymentSchedules_For_ServiceContract()
    {
        var document = CreateValidDocument();
        document.ContractType = ServiceContractConfiguration.ContractType.ServiceContract.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(ServiceContractDocument.PaymentSchedule));

        AssertKeys(
            values,
            ServiceContractConfiguration.PaymentSchedule.OneTimePayment.Key,
            ServiceContractConfiguration.PaymentSchedule.MonthlyPayment.Key,
            ServiceContractConfiguration.PaymentSchedule.StagedPayment.Key);
    }

    [TestMethod]
    public void Computed_Properties_Should_Work()
    {
        var document = CreateValidDocument();

        Assert.IsFalse(document.IsFreeContract);
        Assert.IsFalse(document.UsesElectronicSignature);
        Assert.AreEqual(31, document.ContractDurationDays);
        Assert.AreEqual("Договор оказания услуг: Физическое лицо", document.ContractSummary);
    }

    private static ServiceContractDocument CreateValidDocument()
    {
        return new ServiceContractDocument
        {
            ResponsibleFullName = "Иванов Иван Иванович",
            ContractorName = "Петров Петр Петрович",
            ContractSubject = "Оказание консультационных услуг",
            ContractAmount = 15000,
            ContractPeriod = new DateRange(
                new DateOnly(2026, 3, 1),
                new DateOnly(2026, 3, 31)),
            ContractorType = ServiceContractConfiguration.ContractorType.Individual.Key,
            ContractType = ServiceContractConfiguration.ContractType.ServiceContract.Key,
            PaymentSchedule = ServiceContractConfiguration.PaymentSchedule.OneTimePayment.Key,
            SigningMethod = ServiceContractConfiguration.SigningMethod.PaperSigning.Key
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
