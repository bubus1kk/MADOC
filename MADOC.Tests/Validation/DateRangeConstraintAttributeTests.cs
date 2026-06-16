using MADOC.Domain.Ranges;
using MADOC.Domain.Validation.Attributes;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class DateRangeConstraintAttributeTests
{
    [TestMethod]
    public void Should_Pass_When_Range_Is_Valid()
    {
        var model = new DateRangeModel
        {
            Value = new DateRange(
                new DateOnly(2026, 6, 1),
                new DateOnly(2026, 6, 15))
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_From_Is_Greater_Than_To()
    {
        var model = new DateRangeModel
        {
            Value = new DateRange(
                new DateOnly(2026, 6, 15),
                new DateOnly(2026, 6, 1))
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableDateRangeModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_DateRange()
    {
        var model = new WrongTypeModel
        {
            Value = "01.06.2026 - 15.06.2026"
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    private class DateRangeModel
    {
        [DateRangeConstraint]
        public DateRange? Value { get; set; }
    }

    private class NullableDateRangeModel
    {
        [DateRangeConstraint]
        public DateRange? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [DateRangeConstraint]
        public object? Value { get; set; }
    }
}