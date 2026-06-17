using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;

namespace MADOC.Tests.ListDependencies
{
    [TestClass]
    public class ListFieldOptionResolverTests
    {
        private class TestRoomBookingDocument
        {
            [ListConstraint("Лекция", "Практика", "Конференция")]
            public string EventFormat { get; set; } = string.Empty;

            [ListConstraint("Главный", "Учебный")]
            public string Building { get; set; } = string.Empty;

            [ListConstraint("Лекционная", "Актовый зал", "Компьютерный класс", "Учебная")]
            [ListDependency(nameof(EventFormat), "Лекция", "Лекционная", "Актовый зал")]
            [ListDependency(nameof(EventFormat), "Практика", "Компьютерный класс", "Учебная")]
            public string RoomType { get; set; } = string.Empty;

            [ListConstraint("Аудитория 101", "Аудитория 201", "Актовый зал 1", "Компьютерный класс 210", "Компьютерный класс 212")]
            [ListDependency(nameof(Building), "Главный", "Аудитория 101", "Аудитория 201", "Актовый зал 1")]
            [ListDependency(nameof(Building), "Учебный", "Компьютерный класс 210", "Компьютерный класс 212")]
            [ListDependency(nameof(RoomType), "Лекционная", "Аудитория 101", "Аудитория 201")]
            [ListDependency(nameof(RoomType), "Актовый зал", "Актовый зал 1")]
            [ListDependency(nameof(RoomType), "Компьютерный класс", "Компьютерный класс 210", "Компьютерный класс 212")]
            public string Room { get; set; } = string.Empty;
        }

        private class NonListFieldDocument
        {
            public string Name { get; set; } = string.Empty;
        }

        private class OrderDocument
        {
            [ListConstraint("A")]
            public string Parent { get; set; } = string.Empty;

            [ListConstraint("A", "B", "C", "D")]
            [ListDependency(nameof(Parent), "A", "C", "B")]
            public string Child { get; set; } = string.Empty;
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_All_Values_For_Independent_List_Field()
        {
            var document = new TestRoomBookingDocument();

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.EventFormat));

            CollectionAssert.AreEqual(new List<string> { "Лекция", "Практика", "Конференция" }, values.ToList());
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_Values_By_Parent_Field()
        {
            var document = new TestRoomBookingDocument
            {
                EventFormat = "Лекция"
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.RoomType));

            CollectionAssert.AreEqual(new List<string> { "Лекционная", "Актовый зал" }, values.ToList());
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_Empty_List_When_Parent_Value_Is_Empty()
        {
            var document = new TestRoomBookingDocument
            {
                EventFormat = ""
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.RoomType));

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_Empty_List_When_No_Rule_Matches_Parent_Value()
        {
            var document = new TestRoomBookingDocument
            {
                EventFormat = "Конференция"
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.RoomType));

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_Intersection_When_Field_Has_Two_Parents()
        {
            var document = new TestRoomBookingDocument
            {
                Building = "Учебный",
                RoomType = "Компьютерный класс"
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.Room));

            CollectionAssert.AreEqual(new List<string> { "Компьютерный класс 210", "Компьютерный класс 212" }, values.ToList());
        }

        [TestMethod]
        public void GetAvailableValues_Should_Return_Empty_List_When_Two_Parents_Have_No_Intersection()
        {
            var document = new TestRoomBookingDocument
            {
                Building = "Главный",
                RoomType = "Компьютерный класс"
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(TestRoomBookingDocument.Room));

            Assert.AreEqual(0, values.Count);
        }

        [TestMethod]
        public void GetAvailableValues_Should_Throw_When_Field_Does_Not_Exist()
        {
            var document = new TestRoomBookingDocument();

            var resolver = new ListFieldOptionResolver();

            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                resolver.GetAvailableValues(document, "Пропущенное поле");
            });
        }

        [TestMethod]
        public void GetAvailableValues_Should_Throw_When_Field_Is_Not_List()
        {
            var document = new NonListFieldDocument();

            var resolver = new ListFieldOptionResolver();

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                resolver.GetAvailableValues(
                    document,
                    nameof(NonListFieldDocument.Name));
            });
        }

        [TestMethod]
        public void GetAvailableValues_Should_Preserve_Order_From_ListConstraint()
        {
            var document = new OrderDocument
            {
                Parent = "A"
            };

            var resolver = new ListFieldOptionResolver();

            var values = resolver.GetAvailableValues(document, nameof(OrderDocument.Child));

            CollectionAssert.AreEqual(new List<string> { "B", "C" }, values.ToList());
        }

        [TestMethod]
        public void GetAvailableValues_Should_Throw_When_Document_Is_Null()
        {
            var resolver = new ListFieldOptionResolver();

            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                resolver.GetAvailableValues(null!, "любое поле");
            });
        }

        [TestMethod]
        public void GetAvailableValues_Should_Throw_When_FieldName_Is_Empty()
        {
            var document = new TestRoomBookingDocument();

            var resolver = new ListFieldOptionResolver();

            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                resolver.GetAvailableValues(document, "");
            });
        }
    }
}
