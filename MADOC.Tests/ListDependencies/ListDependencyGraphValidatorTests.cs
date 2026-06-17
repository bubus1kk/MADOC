using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using System.ComponentModel.DataAnnotations;


namespace MADOC.Tests.ListDependencies
{
    [TestClass]
    public class ListDependencyGraphValidatorTests
    {
        private class ValidDagDocument
        {
            [ListConstraint("A1", "A2")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1", "B2")]
            [ListDependency(nameof(A), "A1", "B1")]
            [ListDependency(nameof(A), "A2", "B2")]
            public string B { get; set; } = string.Empty;

            [ListConstraint("C1", "C2")]
            [ListDependency(nameof(B), "B1", "C1")]
            [ListDependency(nameof(B), "B2", "C2")]
            public string C { get; set; } = string.Empty;
        }

        private class TwoNodeCycleDocument
        {
            [ListConstraint("A1")]
            [ListDependency(nameof(B), "B1", "A1")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1")]
            [ListDependency(nameof(A), "A1", "B1")]
            public string B { get; set; } = string.Empty;
        }

        private class ThreeNodeCycleDocument
        {
            [ListConstraint("A1")]
            [ListDependency(nameof(C), "C1", "A1")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1")]
            [ListDependency(nameof(A), "A1", "B1")]
            public string B { get; set; } = string.Empty;

            [ListConstraint("C1")]
            [ListDependency(nameof(B), "B1", "C1")]
            public string C { get; set; } = string.Empty;
        }

        private class MissingParentDocument
        {
            [ListConstraint("B1")]
            [ListDependency("Пропущенное поле", "A1", "B1")]
            public string B { get; set; } = string.Empty;
        }

        private class ParentWithoutListConstraintDocument
        {
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1")]
            [ListDependency(nameof(A), "A1", "B1")]
            public string B { get; set; } = string.Empty;
        }

        private class ChildWithoutListConstraintDocument
        {
            [ListConstraint("A1")]
            public string A { get; set; } = string.Empty;

            [ListDependency(nameof(A), "A1", "B1")]
            public string B { get; set; } = string.Empty;
        }

        private class ParentValueNotFoundDocument
        {
            [ListConstraint("A1")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1")]
            [ListDependency(nameof(A), "Пропущенное значение родителя", "B1")]
            public string B { get; set; } = string.Empty;
        }

        private class ChildAllowedValueNotFoundDocument
        {
            [ListConstraint("A1")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1")]
            [ListDependency(nameof(A), "A1", "B2")]
            public string B { get; set; } = string.Empty;
        }

        private class DuplicatedRulesDocument
        {
            [ListConstraint("A1")]
            public string A { get; set; } = string.Empty;

            [ListConstraint("B1", "B2")]
            [ListDependency(nameof(A), "A1", "B1")]
            [ListDependency(nameof(A), "A1", "B2")]
            public string B { get; set; } = string.Empty;
        }


        [TestMethod]
        public void Validate_Should_Return_No_Errors_When_Graph_Is_Valid_Dag()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ValidDagDocument));

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Graph_Has_Two_Node_Cycle()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(TwoNodeCycleDocument));

            AssertHasError(results, "цикл");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Graph_Has_Three_Node_Cycle()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ThreeNodeCycleDocument));

            AssertHasError(results, "цикл");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Parent_Field_Does_Not_Exist()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(MissingParentDocument));

            AssertHasError(results, "не существует");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Parent_Field_Is_Not_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ParentWithoutListConstraintDocument));

            AssertHasError(results, "не является выпадающим списком");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Child_Field_Has_Dependency_But_Is_Not_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ChildWithoutListConstraintDocument));

            AssertHasError(results, "не объявлено как выпадающий список с набором значений");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_ParentValue_Does_Not_Exist_In_Parent_ListConstraint()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ParentValueNotFoundDocument));

            AssertHasError(results, "не находится среди допустимых значений");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_AllowedValue_Does_Not_Exist_In_Child_ListConstraint()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ChildAllowedValueNotFoundDocument));

            AssertHasError(results, "не найдено в списке допустимых значений");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Document_Has_Duplicated_Dependency_Rules()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(DuplicatedRulesDocument));

            AssertHasError(results, "несколько одинаковых правил зависимости");
        }

        [TestMethod]
        public void Validate_Should_Throw_When_DocumentType_Is_Null()
        {
            var validator = new ListDependencyGraphValidator();

            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                validator.Validate(null!);
            });
        }

        private static void AssertHasError(IReadOnlyList<ValidationResult> results, string expectedText)
        {
            Assert.IsTrue(results.Any(result =>
                    result.ErrorMessage is not null &&
                    result.ErrorMessage.Contains(expectedText, StringComparison.OrdinalIgnoreCase)),
                $"Ожидалась ошибка, содержащая текст: {expectedText}");
        }
    }
}