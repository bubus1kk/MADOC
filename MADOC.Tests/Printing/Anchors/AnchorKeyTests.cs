using MADOC.Domain.Printing.Anchors;

namespace MADOC.Tests.Domain.Printing.Anchors;

[TestClass]
public class AnchorKeyTests
{
    [TestMethod]
    public void Constructor_Should_Save_Value()
    {
        var key = new AnchorKey("certificate_request.requester_full_name");

        Assert.AreEqual("certificate_request.requester_full_name", key.Value);
    }

    [TestMethod]
    public void ToString_Should_Return_Value()
    {
        var key = new AnchorKey("certificate_request.requester_full_name");

        Assert.AreEqual("certificate_request.requester_full_name", key.ToString());
    }

    [TestMethod]
    public void Keys_With_Same_Value_Should_Be_Equal()
    {
        var first = new AnchorKey("certificate_request.requester_full_name");
        var second = new AnchorKey("certificate_request.requester_full_name");

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    public void Constructor_Should_Allow_Latin_Letters_Digits_Dot_Dash_And_Underscore()
    {
        var key = new AnchorKey("document_1.field-name.value_2");

        Assert.AreEqual("document_1.field-name.value_2", key.Value);
    }

    [TestMethod]
    public void Constructor_Should_Throw_Exception_When_Value_Is_Empty()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorKey(string.Empty);
        });
    }

    [TestMethod]
    public void Constructor_Should_Throw_Exception_When_Value_Is_Whitespace()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorKey("   ");
        });
    }

    [TestMethod]
    public void Constructor_Should_Throw_Exception_When_Value_Contains_Spaces()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorKey("certificate request.requester full name");
        });
    }

    [TestMethod]
    public void Constructor_Should_Throw_Exception_When_Value_Contains_Cyrillic_Letters()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorKey("заявка.фио");
        });
    }

    [TestMethod]
    public void Constructor_Should_Throw_Exception_When_Value_Contains_Brackets()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new AnchorKey("certificate_request.requester_full_name()");
        });
    }
}
