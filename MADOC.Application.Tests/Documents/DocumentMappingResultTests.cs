using MADOC.Application.Documents;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentMappingResultTests
{
    [TestMethod]
    public void Success_ReturnsSuccessfulResultWithoutErrors()
    {
        var result = DocumentMappingResult.Success();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Failed_ReturnsFailedResultWithErrors()
    {
        var result = DocumentMappingResult.Failed(new[]
        {
            new DocumentMappingError("Name", "Некорректное значение")
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(1, result.Errors.Count);
    }

    [TestMethod]
    public void Failed_ThrowsWhenErrorListIsEmpty()
    {
        Assert.ThrowsExactly<ArgumentException>(() => DocumentMappingResult.Failed(Array.Empty<DocumentMappingError>()));
    }
}
