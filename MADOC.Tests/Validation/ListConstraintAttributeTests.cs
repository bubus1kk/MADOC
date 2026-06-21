using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;
using MADOC.Tests.Domain;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class ListConstraintAttributeTests
{
    private static readonly ListOption Student = new(new ListOptionKey("test.category.student"),"Студент");

    private static readonly ListOption GroupLeader = new(new ListOptionKey("test.category.group_leader"),"Староста");

    private static readonly ListOption Unknown = new( new ListOptionKey("test.category.unknown"),"Неизвестный");

    [TestMethod]
    public void Should_Pass_When_Key_Is_In_Catalog()
    {
        var model = new ListModel
        {
            Value = Student.Key
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Key_Is_Not_In_Catalog()
    {
        var model = new ListModel
        {
            Value = Unknown.Key
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new ListModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_String()
    {
        var model = new ObjectListModel
        {
            Value = Student.Key.Value
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Document_Does_Not_Provide_Configuration()
    {
        var model = new DocumentWithoutConfiguration
        {
            Value = Student.Key
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Field_Is_Not_In_Catalog()
    {
        var model = new DocumentWithoutListInCatalog
        {
            Value = Student.Key
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    private class ListModel : IListConfigurationProvider
    {
        [ListConstraint]
        public ListOptionKey? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(
                nameof(Value),
                Student,
                GroupLeader);

            return catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return new ListDependencySchema();
        }
    }

    private class ObjectListModel : IListConfigurationProvider
    {
        [ListConstraint]
        public object? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            var catalog = new DocumentListCatalog();

            catalog.AddList(
                nameof(Value),
                Student,
                GroupLeader);

            return catalog;
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return new ListDependencySchema();
        }
    }

    private class DocumentWithoutConfiguration
    {
        [ListConstraint]
        public ListOptionKey? Value { get; set; }
    }

    private class DocumentWithoutListInCatalog : IListConfigurationProvider
    {
        [ListConstraint]
        public ListOptionKey? Value { get; set; }

        public DocumentListCatalog GetListCatalog()
        {
            return new DocumentListCatalog();
        }

        public ListDependencySchema GetListDependencySchema()
        {
            return new ListDependencySchema();
        }
    }
}