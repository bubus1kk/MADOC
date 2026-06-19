//using MADOC.Domain.Validation.Attributes;
//using MADOC.Domain.Validation.ListDependencies;

//namespace MADOC.Tests.Domain.ListDependencies;

//[TestClass]
//public class ListFieldOptionResolverTests
//{
//    [TestMethod]
//    public void GetAvailableValues_Should_Return_All_Values_For_Independent_List_Field()
//    {
//        var document = new TestDocument();

//        var resolver = new ListFieldOptionResolver();

//        var values = resolver.GetAvailableValues(
//            document,
//            nameof(TestDocument.EventFormat));

//        CollectionAssert.AreEqual(
//            new List<string> { "Лекция", "Практика" },
//            values.ToList());
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Return_Values_By_One_Parent()
//    {
//        var document = new TestDocument
//        {
//            EventFormat = "Лекция"
//        };

//        var resolver = new ListFieldOptionResolver();

//        var values = resolver.GetAvailableValues(
//            document,
//            nameof(TestDocument.RoomType));

//        CollectionAssert.AreEqual(
//            new List<string> { "Лекционная", "Актовый зал" },
//            values.ToList());
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Return_Intersection_When_Field_Has_Two_Parents()
//    {
//        var document = new TestDocument
//        {
//            Building = "Учебный",
//            RoomType = "Компьютерный класс"
//        };

//        var resolver = new ListFieldOptionResolver();

//        var values = resolver.GetAvailableValues(
//            document,
//            nameof(TestDocument.Room));

//        CollectionAssert.AreEqual(
//            new List<string>
//            {
//                "Компьютерный класс 1",
//                "Компьютерный класс 2"
//            },
//            values.ToList());
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Return_Empty_List_When_Parent_Is_Not_Selected()
//    {
//        var document = new TestDocument
//        {
//            Building = "Учебный",
//            RoomType = ""
//        };

//        var resolver = new ListFieldOptionResolver();

//        var values = resolver.GetAvailableValues(
//            document,
//            nameof(TestDocument.Room));

//        Assert.AreEqual(0, values.Count);
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Return_Empty_List_When_Parents_Have_No_Intersection()
//    {
//        var document = new TestDocument
//        {
//            Building = "Главный",
//            RoomType = "Компьютерный класс"
//        };

//        var resolver = new ListFieldOptionResolver();

//        var values = resolver.GetAvailableValues(
//            document,
//            nameof(TestDocument.Room));

//        Assert.AreEqual(0, values.Count);
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Throw_When_Document_Is_Null()
//    {
//        var resolver = new ListFieldOptionResolver();

//        Assert.ThrowsExactly<ArgumentNullException>(() =>
//        {
//            resolver.GetAvailableValues(null!, "AnyField");
//        });
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Throw_When_Field_Name_Is_Empty()
//    {
//        var resolver = new ListFieldOptionResolver();

//        Assert.ThrowsExactly<ArgumentException>(() =>
//        {
//            resolver.GetAvailableValues(new TestDocument(), "");
//        });
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Throw_When_Field_Does_Not_Exist()
//    {
//        var resolver = new ListFieldOptionResolver();

//        Assert.ThrowsExactly<ArgumentException>(() =>
//        {
//            resolver.GetAvailableValues(new TestDocument(), "MissingField");
//        });
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Throw_When_Field_Is_Not_List()
//    {
//        var resolver = new ListFieldOptionResolver();

//        Assert.ThrowsExactly<InvalidOperationException>(() =>
//        {
//            resolver.GetAvailableValues(
//                new NonListDocument(),
//                nameof(NonListDocument.Name));
//        });
//    }

//    [TestMethod]
//    public void GetAvailableValues_Should_Throw_When_Dependent_Document_Does_Not_Provide_Schema()
//    {
//        var resolver = new ListFieldOptionResolver();

//        Assert.ThrowsExactly<InvalidOperationException>(() =>
//        {
//            resolver.GetAvailableValues(
//                new DocumentWithoutSchema(),
//                nameof(DocumentWithoutSchema.Child));
//        });
//    }

//    public class TestDocument : IListDependencySchemaProvider
//    {
//        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

//        [ListConstraint("Лекция", "Практика")]
//        public string EventFormat { get; set; } = string.Empty;

//        [ListConstraint("Главный", "Учебный")]
//        public string Building { get; set; } = string.Empty;

//        [ListConstraint("Лекционная", "Актовый зал", "Компьютерный класс", "Лаборатория")]
//        [ListDependency(nameof(EventFormat))]
//        public string RoomType { get; set; } = string.Empty;

//        [ListConstraint(
//            "Аудитория 101",
//            "Актовый зал 1",
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
//                nameof(RoomType),
//                nameof(EventFormat),
//                "Лекция",
//                "Лекционная",
//                "Актовый зал");

//            schema.AddRule(
//                nameof(RoomType),
//                nameof(EventFormat),
//                "Практика",
//                "Компьютерный класс",
//                "Лаборатория");

//            schema.AddRule(
//                nameof(Room),
//                nameof(Building),
//                "Главный",
//                "Аудитория 101",
//                "Актовый зал 1");

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

//    public class NonListDocument
//    {
//        public string Name { get; set; } = string.Empty;
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