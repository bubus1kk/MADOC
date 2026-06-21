using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.ListDependencies;

[TestClass]
public class ListDependencyAttributeTests
{
    private static readonly ListOption ParentA = new(new ListOptionKey("test.parent.a"), "A");

    private static readonly ListOption ParentB = new(new ListOptionKey("test.parent.b"), "B");

    private static readonly ListOption ChildA1 = new(new ListOptionKey("test.child.a1"), "A1");

    private static readonly ListOption ChildA2 = new(new ListOptionKey("test.child.a2"), "A2");

    private static readonly ListOption ChildB1 = new(new ListOptionKey("test.child.b1"), "B1");

    private static readonly ListOption MainBuilding = new(new ListOptionKey("test.building.main"), "Главный");

    private static readonly ListOption EducationalBuilding = new(new ListOptionKey("test.building.educational"), "Учебный");

    private static readonly ListOption LectureRoomType = new(new ListOptionKey("test.room_type.lecture"), "Лекционная");

    private static readonly ListOption ComputerRoomType = new(new ListOptionKey("test.room_type.computer"), "Компьютерный класс");

    private static readonly ListOption Auditorium = new(new ListOptionKey("test.room.auditorium"), "Аудитория 101");

    private static readonly ListOption ComputerRoom1 = new(new ListOptionKey("test.room.computer_1"), "Компьютерный класс 1");

    private static readonly ListOption ComputerRoom2 = new(new ListOptionKey("test.room.computer_2"), "Компьютерный класс 2");

    [TestMethod]
    public void Should_Pass_When_Value_Is_Allowed_By_One_Parent()
    {
        var model = new OneParentDocument
        {
            Parent = ParentA.Key,
            Child = ChildA1.Key
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_Allowed_By_One_Parent()
    {
        var model = new OneParentDocument
        {
            Parent = ParentA.Key,
            Child = ChildB1.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Child_Value_Is_Null()
    {
        var model = new OneParentDocument
        {
            Parent = ParentA.Key,
            Child = null
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Parent_Value_Is_Null()
    {
        var model = new OneParentDocument
        {
            Parent = null,
            Child = ChildA1.Key
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Allowed_By_Two_Parents()
    {
        var model = new TwoParentsDocument
        {
            Building = EducationalBuilding.Key,
            RoomType = ComputerRoomType.Key,
            Room = ComputerRoom1.Key
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_In_Intersection_Of_Two_Parents()
    {
        var model = new TwoParentsDocument
        {
            Building = MainBuilding.Key,
            RoomType = ComputerRoomType.Key,
            Room = ComputerRoom1.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Document_Does_Not_Provide_Dependency_Schema()
    {
        var model = new DocumentWithoutSchema
        {
            Parent = ParentA.Key,
            Child = ChildA1.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Parent_Field_Does_Not_Exist()
    {
        var model = new MissingParentDocument
        {
            Child = ChildA1.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    private static bool IsValid(object model)
    {
        var validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(model,new ValidationContext(model),validationResults,true);
    }

    private static DocumentListCatalog CreateOneParentCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(OneParentDocument.Parent),
            ParentA,
            ParentB);

        catalog.AddList(
            nameof(OneParentDocument.Child),
            ChildA1,
            ChildA2,
            ChildB1);

        return catalog;
    }

    private static ListDependencySchema CreateOneParentSchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(OneParentDocument.Child),
            nameof(OneParentDocument.Parent),
            ParentA,
            ChildA1,
            ChildA2);

        schema.AddRule(
            nameof(OneParentDocument.Child),
            nameof(OneParentDocument.Parent),
            ParentB,
            ChildB1);

        return schema;
    }

    private static DocumentListCatalog CreateTwoParentsCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(TwoParentsDocument.Building),
            MainBuilding,
            EducationalBuilding);

        catalog.AddList(
            nameof(TwoParentsDocument.RoomType),
            LectureRoomType,
            ComputerRoomType);

        catalog.AddList(
            nameof(TwoParentsDocument.Room),
            Auditorium,
            ComputerRoom1,
            ComputerRoom2);

        return catalog;
    }

    private static ListDependencySchema CreateTwoParentsSchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(TwoParentsDocument.Room),
            nameof(TwoParentsDocument.Building),
            MainBuilding,
            Auditorium);

        schema.AddRule(
            nameof(TwoParentsDocument.Room),
            nameof(TwoParentsDocument.Building),
            EducationalBuilding,
            ComputerRoom1,
            ComputerRoom2);

        schema.AddRule(
            nameof(TwoParentsDocument.Room),
            nameof(TwoParentsDocument.RoomType),
            LectureRoomType,
            Auditorium);

        schema.AddRule(
            nameof(TwoParentsDocument.Room),
            nameof(TwoParentsDocument.RoomType),
            ComputerRoomType,
            ComputerRoom1,
            ComputerRoom2);

        return schema;
    }

    public class OneParentDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateOneParentCatalog();
        private static readonly ListDependencySchema Schema = CreateOneParentSchema();

        [ListConstraint]
        public ListOptionKey? Parent { get; set; }

        [ListConstraint]
        [ListDependency(nameof(Parent))]
        public ListOptionKey? Child { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    public class TwoParentsDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateTwoParentsCatalog();
        private static readonly ListDependencySchema Schema = CreateTwoParentsSchema();

        [ListConstraint]
        public ListOptionKey? Building { get; set; }

        [ListConstraint]
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

    public class DocumentWithoutSchema
    {
        public ListOptionKey? Parent { get; set; }

        [ListDependency(nameof(Parent))]
        public ListOptionKey? Child { get; set; }
    }

    public class MissingParentDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateOneParentCatalog();
        private static readonly ListDependencySchema Schema = CreateOneParentSchema();

        [ListConstraint]
        [ListDependency("Пропущенный родитель")]
        public ListOptionKey? Child { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }
}
