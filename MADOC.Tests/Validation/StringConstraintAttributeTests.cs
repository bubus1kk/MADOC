using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Tests.Domain;


namespace MADOC.Tests.Validation;

[TestClass]
public class StringConstraintAttributeTests
{
    private class CyrillicDocument
    {
        [StringConstraint(
            MaxLength = 20,
            IsMultiline = false,
            Alphabet = AllowedAlphabet.CyrillicOnly,
            AllowSpecialChars = false)]
        public string Name { get; set; } = string.Empty;
    }

    private class LatinDocument
    {
        [StringConstraint(
            MaxLength = 20,
            IsMultiline = false,
            Alphabet = AllowedAlphabet.LatinOnly,
            AllowSpecialChars = false)]
        public string Login { get; set; } = string.Empty;
    }

    private class MultilineDocument
    {
        [StringConstraint(
            MaxLength = 100,
            IsMultiline = true,
            Alphabet = AllowedAlphabet.Any,
            AllowSpecialChars = true)]
        public string Text { get; set; } = string.Empty;
    }

    private class NotStringDocument
    {
        [StringConstraint]
        public object Value { get; set; } = new object();
    }

    [TestMethod]
    public void StringConstraint_Should_Pass_When_Cyrillic_Text_Is_Valid()
    {
        var document = new CyrillicDocument
        {
            Name = "Иванов Иван"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_CyrillicOnly_Contains_Latin()
    {
        var document = new CyrillicDocument
        {
            Name = "Ivan Иван"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_Text_Is_Too_Long()
    {
        var document = new CyrillicDocument
        {
            Name = "Иванов Иван Иванович Петрович"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_Special_Characters_Are_Not_Allowed()
    {
        var document = new CyrillicDocument
        {
            Name = "Иванов Иван!"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_Multiline_Is_Not_Allowed()
    {
        var document = new CyrillicDocument
        {
            Name = "Иванов\nИван"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Pass_When_Latin_Text_Is_Valid()
    {
        var document = new LatinDocument
        {
            Login = "Ivanov"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_LatinOnly_Contains_Cyrillic()
    {
        var document = new LatinDocument
        {
            Login = "Ivanов"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Pass_When_Multiline_Is_Allowed()
    {
        var document = new MultilineDocument
        {
            Text = "Первая строка\nВторая строка"
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Pass_When_Value_Is_Null()
    {
        var document = new CyrillicDocument
        {
            Name = null!
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void StringConstraint_Should_Fail_When_Value_Is_Not_String()
    {
        var document = new NotStringDocument
        {
            Value = 123
        };

        var results = ValidationTestHelper.ValidateObject(document);

        Assert.AreEqual(1, results.Count);
    }
}