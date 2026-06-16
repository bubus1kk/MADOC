using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Tests.Domain;

namespace MADOC.Tests;

[TestClass]
public class TimeConstraintAttributeTests
{
    private class TimeRangeModel
    {
        [TimeConstraint(MinTime = "08:00", MaxTime = "18:00")]
        public TimeOnly Value { get; set; }
    }

    private class NullableTimeModel
    {
        [TimeConstraint]
        public TimeOnly? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [TimeConstraint]
        public object? Value { get; set; }
    }

    private class NotMoreThanTimeModel
    {
        public TimeOnly EndTime { get; set; }

        [TimeConstraint(DependsOnField = nameof(EndTime), Dependency = DependencyRule.NotMoreThan)]
        public TimeOnly StartTime { get; set; }
    }

    private class NotLessThanTimeModel
    {
        public TimeOnly StartTime { get; set; }

        [TimeConstraint(DependsOnField = nameof(StartTime), Dependency = DependencyRule.NotLessThan)]
        public TimeOnly EndTime { get; set; }
    }

    private class MissingDependsOnFieldModel
    {
        [TimeConstraint(Dependency = DependencyRule.NotMoreThan)]
        public TimeOnly Value { get; set; }
    }

    private class MissingDependentFieldModel
    {
        [TimeConstraint(DependsOnField = "Пропущенное поле", Dependency = DependencyRule.NotMoreThan)]
        public TimeOnly Value { get; set; }
    }

    private class WrongDependentTypeModel
    {
        public string Other { get; set; } = string.Empty;

        [TimeConstraint(DependsOnField = nameof(Other), Dependency = DependencyRule.NotMoreThan)]
        public TimeOnly Value { get; set; }
    }

    [TestMethod]
    public void Should_Pass_When_Time_Is_In_Range()
    {
        var model = new TimeRangeModel
        {
            Value = new TimeOnly(12, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Time_Is_Before_MinTime()
    {
        var model = new TimeRangeModel
        {
            Value = new TimeOnly(7, 59)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Time_Is_After_MaxTime()
    {
        var model = new TimeRangeModel
        {
            Value = new TimeOnly(18, 1)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableTimeModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_TimeOnly()
    {
        var model = new WrongTypeModel
        {
            Value = DateTime.Now
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Time_Is_Not_More_Than_Dependent_Time()
    {
        var model = new NotMoreThanTimeModel
        {
            EndTime = new TimeOnly(18, 0),
            StartTime = new TimeOnly(17, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Time_Is_More_Than_Dependent_Time()
    {
        var model = new NotMoreThanTimeModel
        {
            EndTime = new TimeOnly(16, 0),
            StartTime = new TimeOnly(17, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Time_Is_Not_Less_Than_Dependent_Time()
    {
        var model = new NotLessThanTimeModel
        {
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Time_Is_Less_Than_Dependent_Time()
    {
        var model = new NotLessThanTimeModel
        {
            StartTime = new TimeOnly(12, 0),
            EndTime = new TimeOnly(11, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependency_Is_Set_But_DependsOnField_Is_Not_Set()
    {
        var model = new MissingDependsOnFieldModel
        {
            Value = new TimeOnly(12, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Does_Not_Exist()
    {
        var model = new MissingDependentFieldModel
        {
            Value = new TimeOnly(12, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Is_Not_TimeOnly()
    {
        var model = new WrongDependentTypeModel
        {
            Other = "12:00",
            Value = new TimeOnly(12, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }
}
