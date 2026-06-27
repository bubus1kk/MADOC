using MADOC.Application.Forms;
using MADOC.Application.Lists;

namespace MADOC.Application.Tests.Lists;

[TestClass]
public sealed class DocumentFieldOptionsResponseTests
{
    [TestMethod]
    public void Success_ReturnsOptionsWithoutErrors()
    {
        var options = new[]
        {
            new DocumentFieldOptionDto("first", "Первый"),
            new DocumentFieldOptionDto("second", "Второй")
        };

        var response = DocumentFieldOptionsResponse.Success(options);

        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(2, response.Options.Count);
        Assert.AreEqual(0, response.Errors.Count);
    }

    [TestMethod]
    public void Failed_ReturnsErrorsWithoutOptions()
    {
        var response = DocumentFieldOptionsResponse.Failed(new[] { "Ошибка получения вариантов" });

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(0, response.Options.Count);
        Assert.AreEqual(1, response.Errors.Count);
    }

    [TestMethod]
    public void Failed_FiltersEmptyErrors()
    {
        var response = DocumentFieldOptionsResponse.Failed(new[] { "", "Ошибка", "   " });

        Assert.AreEqual(1, response.Errors.Count);
        Assert.AreEqual("Ошибка", response.Errors.Single());
    }

    [TestMethod]
    public void Failed_ThrowsWhenErrorListIsEmpty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            DocumentFieldOptionsResponse.Failed(Array.Empty<string>()));
    }
}
