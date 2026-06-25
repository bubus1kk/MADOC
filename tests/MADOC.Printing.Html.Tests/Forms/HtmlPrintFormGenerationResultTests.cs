using MADOC.Printing.Html.Forms;

namespace MADOC.Tests.Domain.Printing.Forms;

[TestClass]
public class HtmlPrintFormGenerationResultTests
{
    [TestMethod]
    public void Success_Should_Create_Result_With_Content_And_Empty_Errors()
    {
        var result = HtmlPrintFormGenerationResult.Success("<p>Готово</p>");

        Assert.AreEqual("<p>Готово</p>", result.Content);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Success_Should_Allow_Empty_Content()
    {
        var result = HtmlPrintFormGenerationResult.Success(string.Empty);

        Assert.AreEqual(string.Empty, result.Content);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.IsTrue(result.IsSuccess);
    }

    [TestMethod]
    public void Success_Should_Throw_When_Content_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            HtmlPrintFormGenerationResult.Success(null!);
        });
    }

    [TestMethod]
    public void Failed_Should_Create_Result_With_Content_And_Errors()
    {
        var result = HtmlPrintFormGenerationResult.Failed(
            "<p>{{ bad }}</p>",
            new[]
            {
                "Первая ошибка",
                "Вторая ошибка"
            });

        Assert.AreEqual("<p>{{ bad }}</p>", result.Content);
        Assert.AreEqual(2, result.Errors.Count);
        Assert.IsFalse(result.IsSuccess);
        CollectionAssert.Contains(result.Errors.ToList(), "Первая ошибка");
        CollectionAssert.Contains(result.Errors.ToList(), "Вторая ошибка");
    }

    [TestMethod]
    public void Failed_Should_Filter_Empty_And_Whitespace_Errors()
    {
        var result = HtmlPrintFormGenerationResult.Failed(
            "<p>HTML</p>",
            new[]
            {
                "",
                "   ",
                "Реальная ошибка"
            });

        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Реальная ошибка", result.Errors[0]);
        Assert.IsFalse(result.IsSuccess);
    }

    [TestMethod]
    public void Failed_Should_Preserve_Error_Order()
    {
        var result = HtmlPrintFormGenerationResult.Failed(
            "<p>HTML</p>",
            new[]
            {
                "Ошибка 1",
                "Ошибка 2",
                "Ошибка 3"
            });

        Assert.AreEqual("Ошибка 1", result.Errors[0]);
        Assert.AreEqual("Ошибка 2", result.Errors[1]);
        Assert.AreEqual("Ошибка 3", result.Errors[2]);
    }

    [TestMethod]
    public void Failed_Should_Throw_Exception_When_Content_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            HtmlPrintFormGenerationResult.Failed(
                null!,
                new[] { "Ошибка" });
        });
    }

    [TestMethod]
    public void Failed_Should_Throw_Exception_When_Errors_Is_Null()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            HtmlPrintFormGenerationResult.Failed("<p>HTML</p>", null!);
        });
    }

    [TestMethod]
    public void Failed_Should_Throw_Exception_When_Errors_Are_Empty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            HtmlPrintFormGenerationResult.Failed("<p>HTML</p>", Array.Empty<string>());
        });
    }

    [TestMethod]
    public void Failed_Should_Throw_Exception_When_All_Errors_Are_Empty_Or_Whitespace()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            HtmlPrintFormGenerationResult.Failed(
                "<p>HTML</p>",
                new[]
                {
                    "",
                    "   "
                });
        });
    }
}
