using System;
using System.Collections.Generic;
using System.Linq;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.ListDependencies;

[TestClass]
public class ListFieldOptionResolverTests
{
    private static readonly ListOption Lecture = new(
        new ListOptionKey("test.event_format.lecture"),
        "Лекция");

    private static readonly ListOption Practice = new(
        new ListOptionKey("test.event_format.practice"),
        "Практика");

    private static readonly ListOption MainBuilding = new(
        new ListOptionKey("test.building.main"),
        "Главный");

    private static readonly ListOption EducationalBuilding = new(
        new ListOptionKey("test.building.educational"),
        "Учебный");

    private static readonly ListOption LectureRoomType = new(
        new ListOptionKey("test.room_type.lecture"),
        "Лекционная");

    private static readonly ListOption AssemblyHallRoomType = new(
        new ListOptionKey("test.room_type.assembly_hall"),
        "Актовый зал");

    private static readonly ListOption ComputerRoomType = new(
        new ListOptionKey("test.room_type.computer"),
        "Компьютерный класс");

    private static readonly ListOption LaboratoryRoomType = new(
        new ListOptionKey("test.room_type.laboratory"),
        "Лаборатория");

    private static readonly ListOption Auditorium = new(
        new ListOptionKey("test.room.auditorium"),
        "Аудитория 101");

    private static readonly ListOption AssemblyHall = new(
        new ListOptionKey("test.room.assembly_hall"),
        "Актовый зал 1");

    private static readonly ListOption ComputerRoom1 = new(
        new ListOptionKey("test.room.computer_1"),
        "Компьютерный класс 1");

    private static readonly ListOption ComputerRoom2 = new(
        new ListOptionKey("test.room.computer_2"),
        "Компьютерный класс 2");

    private static readonly ListOption Laboratory = new(
        new ListOptionKey("test.room.laboratory"),
        "Лаборатория 1");

    [TestMethod]
    public void GetAvailableValues_Should_Return_All_Values_For_Independent_List_Field()
    {
        var document = new TestDocument();

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(TestDocument.EventFormat));

        AssertKeys(
            values,
            Lecture.Key,
            Practice.Key);
    }

    [TestMethod]
    public void GetAvailableValues_Should_Return_Values_By_One_Parent()
    {
        var document = new TestDocument
        {
            EventFormat = Lecture.Key
        };

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(TestDocument.RoomType));

        AssertKeys(
            values,
            LectureRoomType.Key,
            AssemblyHallRoomType.Key);
    }

    [TestMethod]
    public void GetAvailableValues_Should_Return_Intersection_When_Field_Has_Two_Parents()
    {
        var document = new TestDocument
        {
            Building = EducationalBuilding.Key,
            RoomType = ComputerRoomType.Key
        };

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(TestDocument.Room));

        AssertKeys(
            values,
            ComputerRoom1.Key,
            ComputerRoom2.Key);
    }

    [TestMethod]
    public void GetAvailableValues_Should_Return_Empty_List_When_Parent_Is_Not_Selected()
    {
        var document = new TestDocument
        {
            Building = EducationalBuilding.Key,
            RoomType = null
        };

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(TestDocument.Room));

        Assert.AreEqual(0, values.Count);
    }

    [TestMethod]
    public void GetAvailableValues_Should_Return_Empty_List_When_Parents_Have_No_Intersection()
    {
        var document = new TestDocument
        {
            Building = MainBuilding.Key,
            RoomType = ComputerRoomType.Key
        };

        var resolver = new ListFieldOptionResolver();

        var values = resolver.GetAvailableValues(
            document,
            nameof(TestDocument.Room));

        Assert.AreEqual(0, values.Count);
    }

    [TestMethod]
    public void GetAvailableValues_Should_Throw_When_Document_Is_Null()
    {
        var resolver = new ListFieldOptionResolver();

        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            resolver.GetAvailableValues(null!, "AnyField");
        });
    }

    [TestMethod]
    public void GetAvailableValues_Should_Throw_When_Field_Name_Is_Empty()
    {
        var resolver = new ListFieldOptionResolver();

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            resolver.GetAvailableValues(new TestDocument(), "");
        });
    }

    [TestMethod]
    public void GetAvailableValues_Should_Throw_When_Field_Does_Not_Exist()
    {
        var resolver = new ListFieldOptionResolver();

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            resolver.GetAvailableValues(new TestDocument(), "MissingField");
        });
    }

    [TestMethod]
    public void GetAvailableValues_Should_Throw_When_Field_Is_Not_List()
    {
        var resolver = new ListFieldOptionResolver();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            resolver.GetAvailableValues(
                new NonListDocument(),
                nameof(NonListDocument.Name));
        });
    }

    [TestMethod]
    public void GetAvailableValues_Should_Throw_When_Document_Does_Not_Provide_Configuration()
    {
        var resolver = new ListFieldOptionResolver();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
        {
            resolver.GetAvailableValues(
                new DocumentWithoutConfiguration(),
                nameof(DocumentWithoutConfiguration.Child));
        });
    }

    private static void AssertKeys(
        IReadOnlyList<ListOptionKey> actualValues,
        params ListOptionKey[] expectedValues)
    {
        CollectionAssert.AreEqual(
            expectedValues.ToList(),
            actualValues.ToList());
    }

    private static DocumentListCatalog CreateCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(TestDocument.EventFormat),
            Lecture,
            Practice);

        catalog.AddList(
            nameof(TestDocument.Building),
            MainBuilding,
            EducationalBuilding);

        catalog.AddList(
            nameof(TestDocument.RoomType),
            LectureRoomType,
            AssemblyHallRoomType,
            ComputerRoomType,
            LaboratoryRoomType);

        catalog.AddList(
            nameof(TestDocument.Room),
            Auditorium,
            AssemblyHall,
            ComputerRoom1,
            ComputerRoom2,
            Laboratory);

        return catalog;
    }

    private static ListDependencySchema CreateDependencySchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(TestDocument.RoomType),
            nameof(TestDocument.EventFormat),
            Lecture,
            LectureRoomType,
            AssemblyHallRoomType);

        schema.AddRule(
            nameof(TestDocument.RoomType),
            nameof(TestDocument.EventFormat),
            Practice,
            ComputerRoomType,
            LaboratoryRoomType);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.Building),
            MainBuilding,
            Auditorium,
            AssemblyHall);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.Building),
            EducationalBuilding,
            ComputerRoom1,
            ComputerRoom2,
            Laboratory);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.RoomType),
            LectureRoomType,
            Auditorium);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.RoomType),
            AssemblyHallRoomType,
            AssemblyHall);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.RoomType),
            ComputerRoomType,
            ComputerRoom1,
            ComputerRoom2);

        schema.AddRule(
            nameof(TestDocument.Room),
            nameof(TestDocument.RoomType),
            LaboratoryRoomType,
            Laboratory);

        return schema;
    }

    public class TestDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();

        private static readonly ListDependencySchema Schema = CreateDependencySchema();

        [ListConstraint]
        public ListOptionKey? EventFormat { get; set; }

        [ListConstraint]
        public ListOptionKey? Building { get; set; }

        [ListConstraint]
        [ListDependency(nameof(EventFormat))]
        public ListOptionKey? RoomType { get; set; }

        [ListConstraint]
        [ListDependency(nameof(Building), nameof(RoomType))]
        public ListOptionKey? Room { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    public class NonListDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = new();

        private static readonly ListDependencySchema Schema = new();

        public string Name { get; set; } = string.Empty;

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    public class DocumentWithoutConfiguration
    {
        [ListConstraint]
        public ListOptionKey? Parent { get; set; }

        [ListConstraint]
        [ListDependency(nameof(Parent))]
        public ListOptionKey? Child { get; set; }
    }
}