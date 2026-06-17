using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Tests.Domain;

namespace MADOC.Tests;

[TestClass]
public class DateConstraintAttributeTests
{
    private class DateRangeModel
    {
        [DateConstraint(MinDate = "01-01-2026", MaxDate = "31-12-2026")]
        public DateOnly Value { get; set; }
    }

    private class NullableDateModel
    {
        [DateConstraint]
        public DateOnly? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [DateConstraint]
        public object? Value { get; set; }
    }

    private class NotMoreThanDateModel
    {
        public DateOnly EndDate { get; set; }

        [DateConstraint(DependsOnField = nameof(EndDate), Dependency = DependencyRule.NotMoreThan)]
        public DateOnly StartDate { get; set; }
    }

    private class NotLessThanDateModel
    {
        public DateOnly StartDate { get; set; }

        [DateConstraint(DependsOnField = nameof(StartDate), Dependency = DependencyRule.NotLessThan)]
        public DateOnly EndDate { get; set; }
    }

    private class MissingDependsOnFieldModel
    {
        [DateConstraint(Dependency = DependencyRule.NotMoreThan)]
        public DateOnly Value { get; set; }
    }

    private class MissingDependentFieldModel
    {
        [DateConstraint(DependsOnField = "Отсутствующее поле", Dependency = DependencyRule.NotMoreThan)]
        public DateOnly Value { get; set; }
    }

    private class WrongDependentTypeModel
    {
        public string Other { get; set; } = string.Empty;

        [DateConstraint(DependsOnField = nameof(Other), Dependency = DependencyRule.NotMoreThan)]
        public DateOnly Value { get; set; }
    }

    [TestMethod]
    public void Should_Pass_When_Date_Is_In_Range()
    {
        var model = new DateRangeModel
        {
            Value = new DateOnly(2026, 6, 15)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Date_Is_Before_MinDate()
    {
        var model = new DateRangeModel
        {
            Value = new DateOnly(2025, 12, 31)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Date_Is_After_MaxDate()
    {
        var model = new DateRangeModel
        {
            Value = new DateOnly(2027, 1, 1)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableDateModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_DateOnly()
    {
        var model = new WrongTypeModel
        {
            Value = DateTime.Now
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Date_Is_Not_More_Than_Dependent_Date()
    {
        var model = new NotMoreThanDateModel
        {
            EndDate = new DateOnly(2026, 6, 20),
            StartDate = new DateOnly(2026, 6, 15)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Date_Is_More_Than_Dependent_Date()
    {
        var model = new NotMoreThanDateModel
        {
            EndDate = new DateOnly(2026, 6, 10),
            StartDate = new DateOnly(2026, 6, 15)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Date_Is_Not_Less_Than_Dependent_Date()
    {
        var model = new NotLessThanDateModel
        {
            StartDate = new DateOnly(2026, 6, 10),
            EndDate = new DateOnly(2026, 6, 15)
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Date_Is_Less_Than_Dependent_Date()
    {
        var model = new NotLessThanDateModel
        {
            StartDate = new DateOnly(2026, 6, 20),
            EndDate = new DateOnly(2026, 6, 15)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependency_Is_Set_But_DependsOnField_Is_Not_Set()
    {
        var model = new MissingDependsOnFieldModel
        {
            Value = new DateOnly(2026, 6, 15)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Does_Not_Exist()
    {
        var model = new MissingDependentFieldModel
        {
            Value = new DateOnly(2026, 6, 15)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Is_Not_DateOnly()
    {
        var model = new WrongDependentTypeModel
        {
            Other = "15-06-2026",
            Value = new DateOnly(2026, 6, 15)
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }
}