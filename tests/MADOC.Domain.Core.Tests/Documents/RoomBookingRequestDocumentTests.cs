using MADOC.Domain.Core.Documents;
using MADOC.Domain.Core.Documents.ListConfigurations;
using MADOC.Domain.Core.Validation.ListDependencies;
using MADOC.Domain.Core.Validation.Lists;
using System.ComponentModel.DataAnnotations;

namespace MADOC.Tests.Domain.Documents;

[TestClass]
public class RoomBookingRequestDocumentTests
{
    [TestMethod]
    public void GraphValidator_Should_Return_No_Errors()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(RoomBookingRequestDocument));

        Assert.AreEqual(0, results.Count, JoinErrors(results));
    }

    [TestMethod]
    public void Document_Should_Be_Valid_When_List_Combination_Is_Allowed()
    {
        var document = CreateValidDocument();

        Assert.IsTrue(IsValid(document));
    }

    [TestMethod]
    public void Document_Should_Be_Invalid_When_Building_Is_Not_Allowed_For_EventFormat()
    {
        var document = CreateValidDocument();

        document.EventFormat = RoomBookingRequestConfiguration.EventFormat.Lecture.Key;
        document.Building = RoomBookingRequestConfiguration.Building.Laboratory.Key;

        Assert.IsFalse(IsValid(document));
    }

    [TestMethod]
    public void Resolver_Should_Return_Allowed_Buildings_For_Lecture()
    {
        var document = CreateValidDocument();
        document.EventFormat = RoomBookingRequestConfiguration.EventFormat.Lecture.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(RoomBookingRequestDocument.Building));

        AssertKeys(values, RoomBookingRequestConfiguration.Building.Main.Key);
    }

    [TestMethod]
    public void Resolver_Should_Return_Room_Intersection_For_Educational_ComputerClass()
    {
        var document = CreateValidDocument();
        document.Building = RoomBookingRequestConfiguration.Building.Educational.Key;
        document.RoomType = RoomBookingRequestConfiguration.RoomType.ComputerClass.Key;

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(document, nameof(RoomBookingRequestDocument.Room));

        AssertKeys(values, RoomBookingRequestConfiguration.Room.ComputerClass1.Key, RoomBookingRequestConfiguration.Room.ComputerClass2.Key);
    }

    [TestMethod]
    public void Computed_Properties_Should_Work()
    {
        var document = CreateValidDocument();

        Assert.AreEqual(2, document.DurationHours);
        Assert.IsFalse(document.IsLargeEvent);
        Assert.IsTrue(document.RequiresEquipment);
        Assert.AreEqual("Лекция: Аудитория 101", document.BookingSummary);
    }

    private static RoomBookingRequestDocument CreateValidDocument()
    {
        return new RoomBookingRequestDocument
        {
            OrganizerFullName = "Иванов Иван Иванович",
            OrganizerUnit = "ИС-21",
            ParticipantsCount = 30,
            EventDate = new DateOnly(2026, 3, 10),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(12, 0),
            EventDescription = "Проведение лекции",
            EventFormat = RoomBookingRequestConfiguration.EventFormat.Lecture.Key,
            Building = RoomBookingRequestConfiguration.Building.Main.Key,
            RoomType = RoomBookingRequestConfiguration.RoomType.LectureRoom.Key,
            Room = RoomBookingRequestConfiguration.Room.Auditorium101.Key,
            EquipmentSet = RoomBookingRequestConfiguration.EquipmentSet.Projector.Key
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
