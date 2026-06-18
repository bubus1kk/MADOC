using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;

namespace MADOC.Tests.Domain.ListDependencies;

[TestClass]
public class ListDependencyGraphValidatorTests
{
    public class ValidDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");

            return schema;
        }
    }

    public class TwoNodeCycleDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        [ListDependency(nameof(B))]
        public string A { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(A))]
        public string B { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(A), nameof(B), "B", "A");
            schema.AddRule(nameof(B), nameof(A), "A", "B");

            return schema;
        }
    }

    public class ThreeNodeCycleDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        [ListDependency(nameof(C))]
        public string A { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(A))]
        public string B { get; set; } = string.Empty;

        [ListConstraint("C")]
        [ListDependency(nameof(B))]
        public string C { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(A), nameof(C), "C", "A");
            schema.AddRule(nameof(B), nameof(A), "A", "B");
            schema.AddRule(nameof(C), nameof(B), "B", "C");

            return schema;
        }
    }

    public class MissingParentDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("B")]
        [ListDependency("Пропущенный родитель")]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            return new ListDependencySchema();
        }
    }

    public class ChildWithoutListConstraintDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");

            return schema;
        }
    }

    public class ParentWithoutListConstraintDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");

            return schema;
        }
    }

    public class DocumentWithoutSchema
    {
        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;
    }

    public class AttributeConnectionWithoutSchemaRuleDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = new();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }
    }

    public class SchemaConnectionWithoutAttributeDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");

            return schema;
        }
    }

    public class ParentValueNotFoundDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "Пропущенное значение", "B");

            return schema;
        }
    }

    public class ChildValueNotFoundDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "Пропущенное значение ребенка");

            return schema;
        }
    }

    public class DuplicatedRulesDocument : IListDependencySchemaProvider
    {
        private static readonly ListDependencySchema DependencySchema = CreateDependencySchema();

        [ListConstraint("A")]
        public string Parent { get; set; } = string.Empty;

        [ListConstraint("B")]
        [ListDependency(nameof(Parent))]
        public string Child { get; set; } = string.Empty;

        public ListDependencySchema GetListDependencySchema()
        {
            return DependencySchema;
        }

        private static ListDependencySchema CreateDependencySchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");
            schema.AddRule(nameof(Child), nameof(Parent), "A", "B");

            return schema;
        }

        [TestMethod]
        public void Validate_Should_Return_No_Errors_When_Graph_Is_Valid()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ValidDocument));

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
        public void Validate_Should_Return_Error_When_Child_Field_Is_Not_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ChildWithoutListConstraintDocument));

            AssertHasError(results, "ListConstraintAttribute");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Parent_Field_Is_Not_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ParentWithoutListConstraintDocument));

            AssertHasError(results, "не является выпадающим списком");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Document_Does_Not_Provide_Schema()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(DocumentWithoutSchema));

            AssertHasError(results, "IListDependencySchemaProvider");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Attribute_Connection_Has_No_Schema_Rules()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(AttributeConnectionWithoutSchemaRuleDocument));

            AssertHasError(results, "нет правил");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Schema_Connection_Is_Not_Declared_By_Attribute()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(SchemaConnectionWithoutAttributeDocument));

            AssertHasError(results, "не объявлена");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Parent_Value_Does_Not_Exist_In_Parent_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ParentValueNotFoundDocument));

            AssertHasError(results, "не найдено среди допустимых значений");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Allowed_Child_Value_Does_Not_Exist_In_Child_List()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(ChildValueNotFoundDocument));

            AssertHasError(results, "не найдено среди допустимых значений поля");
        }

        [TestMethod]
        public void Validate_Should_Return_Error_When_Schema_Has_Duplicated_Rules()
        {
            var validator = new ListDependencyGraphValidator();

            var results = validator.Validate(typeof(DuplicatedRulesDocument));

            AssertHasError(results, "повторяющееся правило");
        }

        [TestMethod]
        public void Validate_Should_Throw_When_Document_Type_Is_Null()
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