using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class CatalogListConstraintAttributeTests
{
    private static readonly ListOption Student = new(
        new ListOptionKey("test.student"),
        "Студент");

    private static readonly ListOption GroupLeader = new(
        new ListOptionKey("test.group_leader"),
        "Староста");

    private static readonly ListOption Graduate = new(
        new ListOptionKey("test.graduate"),
        "Выпускник");

    [TestMethod]
    public void Should_Pass_When_Key_Is_In_Catalog()
    {
        var model = new ListModel
        {
            Value = Student.Key
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Key_Is_Not_In_Catalog()
    {
        var model = new ListModel
        {
            Value = new ListOptionKey("test.teacher")
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new ListModel
        {
            Value = null
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_String_Key_From_Catalog()
    {
        var model = new StringListModel
        {
            Value = Student.Key.Value
        };

        Assert.IsTrue(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Cannot_Be_Converted_To_Key()
    {
        var model = new ObjectListModel
        {
            Value = 123
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Document_Does_Not_Provide_List_Configuration()
    {
        var model = new DocumentWithoutConfiguration
        {
            Value = Student.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Field_Is_Not_Described_In_Catalog()
    {
        var model = new FieldWithoutCatalogListModel
        {
            Value = Student.Key
        };

        Assert.IsFalse(IsValid(model));
    }

    private static bool IsValid(object model)
    {
        var validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            validationResults,
            true);
    }

    private static DocumentListCatalog CreateCatalog()
    {
        var catalog = new DocumentListCatalog();

        catalog.AddList(
            nameof(ListModel.Value),
            Student,
            GroupLeader,
            Graduate);

        return catalog;
    }

    private class ListModel : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public ListOptionKey? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    private class StringListModel : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public string? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    private class ObjectListModel : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = CreateCatalog();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public object? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    private class FieldWithoutCatalogListModel : IListConfigurationProvider
    {
        private static readonly DocumentListCatalog Catalog = new();
        private static readonly ListDependencySchema Schema = new();

        [ListConstraint]
        public ListOptionKey? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return Catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return Schema;
        }
    }

    private class DocumentWithoutConfiguration
    {
        [ListConstraint]
        public ListOptionKey? Value { get; set; }
    }
}
