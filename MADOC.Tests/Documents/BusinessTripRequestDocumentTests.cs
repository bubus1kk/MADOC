using MADOC.Domain.Documents;
using MADOC.Domain.Documents.ListConfigurations;
using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Tests.Domain.Documents;

[TestClass]
public class BusinessTripRequestDocumentTests
{
    [TestMethod]
    public void GraphValidator_Should_Return_No_Errors()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(BusinessTripRequestDocument));

        Assert.AreEqual(0, results.Count, JoinErrors(results));
    }

    [TestMethod]
    public void Document_Should_Be_Valid_When_List_Combination_Is_Allowed()
    {
        var document = CreateValidDocument();

        Assert.IsTrue(IsValid(document));
    }

    [TestMethod]
    public void Document_Should_Be_Invalid_When_Purpose_Is_Not_Allowed_For_Department()
    {
        var document = CreateValidDocument();

        document.Department = BusinessTripRequestConfiguration.Department.Administration.Key;
        document.Purpose = BusinessTripRequestConfiguration.Purpose.Training.Key;

        Assert.IsFalse(IsValid(document));
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_Purposes_For_ItDepartment()
    {
        var document = CreateValidDocument();
        document.Department = BusinessTripRequestConfiguration.Department.ItDepartment.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(BusinessTripRequestDocument.Purpose));

        AssertKeys(
            values,
            BusinessTripRequestConfiguration.Purpose.SystemImplementation.Key,
            BusinessTripRequestConfiguration.Purpose.ReportMeeting.Key,
            BusinessTripRequestConfiguration.Purpose.Audit.Key);
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_TransportTypes_For_Federal_Destination()
    {
        var document = CreateValidDocument();
        document.Destination = BusinessTripRequestConfiguration.Destination.Federal.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(BusinessTripRequestDocument.TransportType));

        AssertKeys(
            values,
            BusinessTripRequestConfiguration.TransportType.Train.Key,
            BusinessTripRequestConfiguration.TransportType.Plane.Key);
    }

    [TestMethod]
    public void Computed_Properties_Should_Work()
    {
        var document = CreateValidDocument();

        Assert.AreEqual(3, document.TripDays);
        Assert.IsFalse(document.IsInternationalTrip);
        Assert.IsFalse(document.RequiresAccommodation);
        Assert.AreEqual("Совещание: Местное", document.TripSummary);
    }

    private static BusinessTripRequestDocument CreateValidDocument()
    {
        return new BusinessTripRequestDocument
        {
            EmployeeFullName = "Иванов Иван Иванович",
            Position = "Методист",
            TripPeriod = new DateRange(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 3)),
            DestinationCity = "Нижний Новгород",
            Reason = "Рабочая поездка",
            Department = BusinessTripRequestConfiguration.Department.Administration.Key,
            Purpose = BusinessTripRequestConfiguration.Purpose.Meeting.Key,
            Destination = BusinessTripRequestConfiguration.Destination.Local.Key,
            TransportType = BusinessTripRequestConfiguration.TransportType.PublicTransport.Key,
            Accommodation = BusinessTripRequestConfiguration.Accommodation.NotRequired.Key
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
