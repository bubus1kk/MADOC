using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class DateTimeConstraintAttributeTests
{
    [TestMethod]
    public void Should_Pass_When_DateTime_Is_In_Range()
    {
        var model = new DateTimeRangeModel
        {
            Value = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_DateTime_Is_Before_Min()
    {
        var model = new DateTimeRangeModel
        {
            Value = new DateTime(2025, 12, 31, 23, 59, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_DateTime_Is_After_Max()
    {
        var model = new DateTimeRangeModel
        {
            Value = new DateTime(2027, 1, 1, 0, 1, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableDateTimeModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_DateTime()
    {
        var model = new WrongTypeModel
        {
            Value = DateOnly.FromDateTime(DateTime.Now)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_DateTime_Is_Not_More_Than_Dependent_DateTime()
    {
        var model = new NotMoreThanDateTimeModel
        {
            EndAt = new DateTime(2026, 6, 15, 18, 0, 0),
            StartAt = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_DateTime_Is_More_Than_Dependent_DateTime()
    {
        var model = new NotMoreThanDateTimeModel
        {
            EndAt = new DateTime(2026, 6, 15, 10, 0, 0),
            StartAt = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_DateTime_Is_Not_Less_Than_Dependent_DateTime()
    {
        var model = new NotLessThanDateTimeModel
        {
            StartAt = new DateTime(2026, 6, 15, 10, 0, 0),
            EndAt = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_DateTime_Is_Less_Than_Dependent_DateTime()
    {
        var model = new NotLessThanDateTimeModel
        {
            StartAt = new DateTime(2026, 6, 15, 14, 0, 0),
            EndAt = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependency_Is_Set_But_DependsOnField_Is_Not_Set()
    {
        var model = new MissingDependsOnFieldModel
        {
            Value = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Does_Not_Exist()
    {
        var model = new MissingDependentFieldModel
        {
            Value = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Is_Not_DateTime()
    {
        var model = new WrongDependentTypeModel
        {
            Other = "2026-06-15 12:00",
            Value = new DateTime(2026, 6, 15, 12, 0, 0)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    private class DateTimeRangeModel
    {
        [DateTimeConstraint(
            Min = "2026-01-01 00:00",
            Max = "2026-12-31 23:59")]
        public DateTime Value { get; set; }
    }

    private class NullableDateTimeModel
    {
        [DateTimeConstraint]
        public DateTime? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [DateTimeConstraint]
        public object? Value { get; set; }
    }

    private class NotMoreThanDateTimeModel
    {
        public DateTime EndAt { get; set; }

        [DateTimeConstraint(
            DependsOnField = nameof(EndAt),
            Dependency = DependencyRule.NotMoreThan)]
        public DateTime StartAt { get; set; }
    }

    private class NotLessThanDateTimeModel
    {
        public DateTime StartAt { get; set; }

        [DateTimeConstraint(
            DependsOnField = nameof(StartAt),
            Dependency = DependencyRule.NotLessThan)]
        public DateTime EndAt { get; set; }
    }

    private class MissingDependsOnFieldModel
    {
        [DateTimeConstraint(Dependency = DependencyRule.NotMoreThan)]
        public DateTime Value { get; set; }
    }

    private class MissingDependentFieldModel
    {
        [DateTimeConstraint(
            DependsOnField = "MissingField",
            Dependency = DependencyRule.NotMoreThan)]
        public DateTime Value { get; set; }
    }

    private class WrongDependentTypeModel
    {
        public string Other { get; set; } = string.Empty;

        [DateTimeConstraint(
            DependsOnField = nameof(Other),
            Dependency = DependencyRule.NotMoreThan)]
        public DateTime Value { get; set; }
    }
}