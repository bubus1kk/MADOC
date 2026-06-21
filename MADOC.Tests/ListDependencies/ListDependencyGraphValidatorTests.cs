using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.ListDependencies;

[TestClass]
public class ListDependencyGraphValidatorTests
{
    private static readonly ListOption A = new(
        new ListOptionKey("test.a"),
        "A");

    private static readonly ListOption B = new(
        new ListOptionKey("test.b"),
        "B");

    private static readonly ListOption C = new(
        new ListOptionKey("test.c"),
        "C");

    private static readonly ListOption Missing = new(
        new ListOptionKey("test.missing"),
        "Missing");

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

        AssertHasError(results, "циклы");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Graph_Has_Three_Node_Cycle()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ThreeNodeCycleDocument));

        AssertHasError(results, "циклы");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Parent_Field_Does_Not_Exist()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(MissingParentDocument));

        AssertHasError(results, "не существует");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Child_Field_Does_Not_Have_ListConstraint()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ChildWithoutListConstraintDocument));

        AssertHasError(results, "не объявлено как выпадающий список");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Parent_Field_Does_Not_Have_ListConstraint()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ParentWithoutListConstraintDocument));

        AssertHasError(results, "не является выпадающим списком");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Document_Does_Not_Provide_Configuration()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(DocumentWithoutConfiguration));

        AssertHasError(results, "IListConfigurationProvider");
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

        AssertHasError(results, "не объявлена через ListDependencyAttribute");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Parent_Key_From_Rule_Is_Not_In_Catalog()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ParentKeyNotFoundDocument));

        AssertHasError(results, "родительского поля");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Child_Key_From_Rule_Is_Not_In_Catalog()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ChildKeyNotFoundDocument));

        AssertHasError(results, "среди вариантов поля");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Schema_Has_Duplicated_Rules()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(DuplicatedRulesDocument));

        AssertHasError(results, "повторяющееся правило");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_Catalog_Has_Missing_Field()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(CatalogWithMissingFieldDocument));

        AssertHasError(results, "несуществующее поле");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_List_Field_Is_Not_Described_In_Catalog()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(ListFieldWithoutCatalogDocument));

        AssertHasError(results, "нет описания в каталоге");
    }

    [TestMethod]
    public void Validate_Should_Return_Error_When_List_Field_Has_Wrong_Type()
    {
        var validator = new ListDependencyGraphValidator();

        var results = validator.Validate(typeof(WrongListFieldTypeDocument));

        AssertHasError(results, "ListOptionKey");
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

    private static void AssertHasError(
        IReadOnlyList<ValidationResult> results,
        string expectedText)
    {
        Assert.IsTrue(
            results.Any(result =>
                result.ErrorMessage is not null &&
                result.ErrorMessage.Contains(
                    expectedText,
                    StringComparison.OrdinalIgnoreCase)),
            $"Ожидалась ошибка, содержащая текст: {expectedText}");
    }

    private static DocumentListCatalog CreateParentChildCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(ValidDocument.Parent),
            A);

        catalog.AddList(
            nameof(ValidDocument.Child),
            B);

        return catalog;
    }

    private static ListDependencySchema CreateParentChildSchema()
    {
        var schema = new ListDependencySchema();

        schema.AddRule(
            nameof(ValidDocument.Child),
            nameof(ValidDocument.Parent),
            A,
            B);

        return schema;
    }

    public class ValidDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = CreateParentChildSchema();

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

    public class TwoNodeCycleDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

        [ListConstraint]
        [ListDependency(nameof(BField))]
        public ListOptionKey? AField { get; set; }

        [ListConstraint]
        [ListDependency(nameof(AField))]
        public ListOptionKey? BField { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(AField), A);
            catalog.AddList(nameof(BField), B);

            return catalog;
        }

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(AField), nameof(BField), B, A);
            schema.AddRule(nameof(BField), nameof(AField), A, B);

            return schema;
        }
    }

    public class ThreeNodeCycleDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

        [ListConstraint]
        [ListDependency(nameof(CField))]
        public ListOptionKey? AField { get; set; }

        [ListConstraint]
        [ListDependency(nameof(AField))]
        public ListOptionKey? BField { get; set; }

        [ListConstraint]
        [ListDependency(nameof(BField))]
        public ListOptionKey? CField { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(AField), A);
            catalog.AddList(nameof(BField), B);
            catalog.AddList(nameof(CField), C);

            return catalog;
        }

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(AField), nameof(CField), C, A);
            schema.AddRule(nameof(BField), nameof(AField), A, B);
            schema.AddRule(nameof(CField), nameof(BField), B, C);

            return schema;
        }
    }

    public class MissingParentDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        [ListDependency("MissingParent")]
        public ListOptionKey? Child { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(Child), B);

            return catalog;
        }
    }

    public class ChildWithoutListConstraintDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

        [ListConstraint]
        public ListOptionKey? Parent { get; set; }

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

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(Parent), A);
            catalog.AddList(nameof(Child), B);

            return catalog;
        }

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), A, B);

            return schema;
        }
    }

    public class ParentWithoutListConstraintDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

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

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(Parent), A);
            catalog.AddList(nameof(Child), B);

            return catalog;
        }

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), A, B);

            return schema;
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

    public class AttributeConnectionWithoutSchemaRuleDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = new();

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

    public class SchemaConnectionWithoutAttributeDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = CreateParentChildSchema();

        [ListConstraint]
        public ListOptionKey? Parent { get; set; }

        [ListConstraint]
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

    public class ParentKeyNotFoundDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

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

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), Missing, B);

            return schema;
        }
    }

    public class ChildKeyNotFoundDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

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

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), A, Missing);

            return schema;
        }
    }

    public class DuplicatedRulesDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateParentChildCatalog();
        private static readonly ListDependencySchema Schema = CreateSchema();

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

        private static ListDependencySchema CreateSchema()
        {
            var schema = new ListDependencySchema();

            schema.AddRule(nameof(Child), nameof(Parent), A, B);
            schema.AddRule(nameof(Child), nameof(Parent), A, B);

            return schema;
        }
    }

    public class CatalogWithMissingFieldDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public ListOptionKey? ExistingField { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList("MissingField", A);
            catalog.AddList(nameof(ExistingField), B);

            return catalog;
        }
    }

    public class ListFieldWithoutCatalogDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = new();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public ListOptionKey? Field { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    public class WrongListFieldTypeDocument : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public string Field { get; set; } = string.Empty;

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }

        private static DocumentListCatalog CreateCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(nameof(Field), A);

            return catalog;
        }
    }
}
