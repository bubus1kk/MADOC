//using MADOC.Domain.Validation.Attributes;
//using MADOC.Domain.Validation.ListDependencies;
//using MADOC.Tests.Domain;

//namespace MADOC.Tests.Domain.ListDependencies;

//[TestClass]
//public class ListDependencyAttributeTests
//{
//    //[TestMethod]
//    //public void Should_Pass_When_Value_Is_Allowed_By_One_Parent()
//    //{
//    //    var model = new OneParentDocument
//    //    {
//    //        Parent = "A",
//    //        Child = "A1"
//    //    };

//    //    Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    //}

//    //[TestMethod]
//    //public void Should_Fail_When_Value_Is_Not_Allowed_By_One_Parent()
//    //{
//    //    var model = new OneParentDocument
//    //    {
//    //        Parent = "A",
//    //        Child = "B1"
//    //    };

//    //    Assert.IsFalse(ValidationTestHelper.IsValid(model));
//    //}

//    [TestMethod]
//    public void Should_Pass_When_Parent_Value_Is_Empty()
//    {
//        var model = new OneParentDocument
//        {
//            Parent = "",
//            Child = "B1"
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Pass_When_Current_Value_Is_Empty()
//    {
//        var model = new OneParentDocument
//        {
//            Parent = "A",
//            Child = ""
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    //[TestMethod]
//    //public void Should_Pass_When_Value_Is_Allowed_By_Two_Parents_Intersection()
//    //{
//    //    var model = new TwoParentsDocument
//    //    {
//    //        Building = "Учебный",
//    //        RoomType = "Компьютерный класс",
//    //        Room = "Компьютерный класс 1"
//    //    };

//    //    Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    //}

//    //[TestMethod]
//    //public void Should_Fail_When_Value_Is_Allowed_By_First_Parent_But_Not_By_Second_Parent()
//    //{
//    //    var model = new TwoParentsDocument
//    //    {
//    //        Building = "Учебный",
//    //        RoomType = "Компьютерный класс",
//    //        Room = "Лаборатория 1"
//    //    };

//    //    Assert.IsFalse(ValidationTestHelper.IsValid(model));
//    //}

//    [TestMethod]
//    public void Should_Fail_When_Document_Does_Not_Provide_Dependency_Schema()
//    {
//        var model = new DocumentWithoutSchema
//        {
//            Parent = "A",
//            Child = "A1"
//        };

//        Assert.IsFalse(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Constructor_Should_Throw_When_Parents_Are_Not_Provided()
//    {
//        Assert.ThrowsExactly<ArgumentException>(() =>
//        {
//            _ = new ListDependencyAttribute();
//        });
//    }

//    [TestMethod]
//    public void Constructor_Should_Throw_When_Parent_Name_Is_Empty()
//    {
//        Assert.ThrowsExactly<ArgumentException>(() =>
//        {
//            _ = new ListDependencyAttribute("");
//        });
//    }

//    [TestMethod]
//    public void Constructor_Should_Throw_When_Parent_Name_Is_Duplicated()
//    {
//        Assert.ThrowsExactly<ArgumentException>(() =>
//        {
//            _ = new ListDependencyAttribute("Parent", "Parent");
//        });
//    }

//    public class OneParentDocument : IListDependencySchemaProvider
//    {
//        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

//        [ListConstraint("A", "B")]
//        public string Parent { get; set; } = string.Empty;

//        [ListConstraint("A1", "A2", "B1", "B2")]
//        [ListDependency(nameof(Parent))]
//        public string Child { get; set; } = string.Empty;

//        public ListDependencySchema GetListDependencySchema()
//        {
//            return DependencySchema;
//        }

//        private static ListDependencySchema CreateDependencySchema()
//        {
//            var schema = new ListDependencySchema();

//            schema.AddRule(
//                nameof(Child),
//                nameof(Parent),
//                "A",
//                "A1",
//                "A2");

//            schema.AddRule(
//                nameof(Child),
//                nameof(Parent),
//                "B",
//                "B1",
//                "B2");

//            return schema;
//        }
//    }

//    public class TwoParentsDocument : IListDependencySchemaProvider
//    {
//        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

//        [ListConstraint("Главный", "Учебный")]
//        public string Building { get; set; } = string.Empty;

//        [ListConstraint("Компьютерный класс", "Лаборатория")]
//        public string RoomType { get; set; } = string.Empty;

//        [ListConstraint(
//            "Компьютерный класс 1",
//            "Компьютерный класс 2",
//            "Лаборатория 1")]
//        [ListDependency(nameof(Building), nameof(RoomType))]
//        public string Room { get; set; } = string.Empty;

//        public ListDependencySchema GetListDependencySchema()
//        {
//            return DependencySchema;
//        }

//        private static ListDependencySchema CreateDependencySchema()
//        {
//            var schema = new ListDependencySchema();

//            schema.AddRule(
//                nameof(Room),
//                nameof(Building),
//                "Учебный",
//                "Компьютерный класс 1",
//                "Компьютерный класс 2",
//                "Лаборатория 1");

//            schema.AddRule(
//                nameof(Room),
//                nameof(RoomType),
//                "Компьютерный класс",
//                "Компьютерный класс 1",
//                "Компьютерный класс 2");

//            schema.AddRule(
//                nameof(Room),
//                nameof(RoomType),
//                "Лаборатория",
//                "Лаборатория 1");

//            return schema;
//        }
//    }

//    public class DocumentWithoutSchema
//    {
//        [ListConstraint("A")]
//        public string Parent { get; set; } = string.Empty;

//        [ListConstraint("A1")]
//        [ListDependency(nameof(Parent))]
//        public string Child { get; set; } = string.Empty;
//    }
//}