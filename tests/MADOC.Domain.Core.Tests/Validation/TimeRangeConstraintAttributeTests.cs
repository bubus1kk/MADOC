using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class TimeRangeConstraintAttributeTests
{
    [TestMethod]
    public void Should_Pass_When_Range_Is_Valid()
    {
        var model = new TimeRangeModel
        {
            Value = new TimeRange(
                new TimeOnly(10, 0),
                new TimeOnly(12, 0))
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_From_Is_Greater_Than_To()
    {
        var model = new TimeRangeModel
        {
            Value = new TimeRange(
                new TimeOnly(12, 0),
                new TimeOnly(10, 0))
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableTimeRangeModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_TimeRange()
    {
        var model = new WrongTypeModel
        {
            Value = "10:00 - 12:00"
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    private class TimeRangeModel
    {
        [TimeRangeConstraint]
        public TimeRange? Value { get; set; }
    }

    private class NullableTimeRangeModel
    {
        [TimeRangeConstraint]
        public TimeRange? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [TimeRangeConstraint]
        public object? Value { get; set; }
    }
}