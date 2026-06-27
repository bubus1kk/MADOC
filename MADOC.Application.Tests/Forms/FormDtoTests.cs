using MADOC.Application.Forms;

namespace MADOC.Application.Tests.Forms;

[TestClass]
public sealed class FormDtoTests
{
    [TestMethod]
    public void DocumentFieldOptionDto_StoresValueAndDisplayName()
    {
        var option = new DocumentFieldOptionDto("education", "Справка об обучении");

        Assert.AreEqual("education", option.Value);
        Assert.AreEqual("Справка об обучении", option.DisplayName);
    }

    [TestMethod]
    public void DocumentFieldOptionDto_ThrowsForEmptyValue()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new DocumentFieldOptionDto("", "Название"));
    }

    [TestMethod]
    public void DocumentFieldDependencyDto_RemovesEmptyAndDuplicateParentFields()
    {
        var dependency = new DocumentFieldDependencyDto(new[]
        {
            "ParentField",
            "",
            "ParentField",
            "SecondParent"
        });

        CollectionAssert.AreEqual(
            new[] { "ParentField", "SecondParent" },
            dependency.ParentFieldNames.ToArray());
    }

    [TestMethod]
    public void DocumentFieldDependencyDto_ThrowsWhenNoValidParentFieldsExist()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new DocumentFieldDependencyDto(new[] { "", "   " }));
    }

    [TestMethod]
    public void DocumentFieldSchema_StoresFieldDescription()
    {
        var constraints = new DocumentFieldConstraintDto(isRequired: true, maxLength: 100);
        var options = new[]
        {
            new DocumentFieldOptionDto("value", "Название")
        };
        var dependency = new DocumentFieldDependencyDto(new[] { "Parent" });

        var field = new DocumentFieldSchema(
            "Name",
            "Название",
            DocumentFieldType.Text,
            isReadOnly: false,
            "test.name",
            constraints,
            options,
            dependency);

        Assert.AreEqual("Name", field.Name);
        Assert.AreEqual("Название", field.DisplayName);
        Assert.AreEqual(DocumentFieldType.Text, field.Type);
        Assert.IsFalse(field.IsReadOnly);
        Assert.AreEqual("test.name", field.AnchorName);
        Assert.AreSame(constraints, field.Constraints);
        Assert.AreEqual(1, field.Options.Count);
        Assert.AreSame(dependency, field.Dependency);
    }

    [TestMethod]
    public void DocumentFormSchema_ThrowsWhenFieldsAreEmpty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentFormSchema("test", "Тестовая форма", Array.Empty<DocumentFieldSchema>()));
    }
}
