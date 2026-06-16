using MADOC.Domain.Validation.Attributes;
using MADOC.Domain.Validation.Enums;
using MADOC.Tests.Domain;

namespace MADOC.Tests.Validation;

[TestClass]
public class NumberConstraintAttributeTests
{

    private class RangeModel
    {
        [NumberConstraint(MinValue = 10, MaxValue = 20)]
        public double Value { get; set; }
    }

    private class NullableNumberModel
    {
        [NumberConstraint]
        public double? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [NumberConstraint]
        public object? Value { get; set; }
    }

    private class FloatAllowedModel
    {
        [NumberConstraint(AllowFloats = true)]
        public double Value { get; set; }
    }

    private class WholeNumberOnlyModel
    {
        [NumberConstraint(AllowFloats = false)]
        public double Value { get; set; }
    }

    private class NotMoreThanModel
    {
        public double Limit { get; set; }

        [NumberConstraint(DependsOnField = nameof(Limit), Dependency = DependencyRule.NotMoreThan)]
        public double Current { get; set; }
    }

    private class NotLessThanModel
    {
        public double Minimum { get; set; }

        [NumberConstraint(DependsOnField = nameof(Minimum), Dependency = DependencyRule.NotLessThan)]
        public double Current { get; set; }
    }

    private class MissingDependsOnFieldModel
    {
        [NumberConstraint(Dependency = DependencyRule.NotMoreThan)]
        public double Current { get; set; }
    }

    private class NotExistingDependentFieldModel
    {
        [NumberConstraint(DependsOnField = "Несуществующее поле", Dependency = DependencyRule.NotMoreThan)]
        public double Current { get; set; }
    }

    private class DependentFieldWrongTypeModel
    {
        public string Limit { get; set; } = string.Empty;

        [NumberConstraint(DependsOnField = nameof(Limit), Dependency = DependencyRule.NotMoreThan)]
        public double Current { get; set; }
    }

    [TestMethod]
    public void Should_Pass_When_Number_Is_In_Range()
    {
        var model = new RangeModel
        {
            Value = 15
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Number_Is_Less_Than_MinValue()
    {
        var model = new RangeModel
        {
            Value = 5
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Number_Is_Greater_Than_MaxValue()
    {
        var model = new RangeModel
        {
            Value = 25
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableNumberModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Cannot_Be_Converted_To_Number()
    {
        var model = new WrongTypeModel
        {
            Value = "абвгд"
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Float_Is_Allowed()
    {
        var model = new FloatAllowedModel
        {
            Value = 0.5
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Float_Is_Not_Allowed()
    {
        var model = new WholeNumberOnlyModel
        {
            Value = 0.5
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Whole_Number_Is_Required_And_Value_Is_Whole()
    {
        var model = new WholeNumberOnlyModel
        {
            Value = 1000
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Not_More_Than_Dependent_Field()
    {
        var model = new NotMoreThanModel
        {
            Limit = 10050,
            Current = 90
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_More_Than_Dependent_Field()
    {
        var model = new NotMoreThanModel
        {
            Limit = 10,
            Current = 120
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Not_Less_Than_Dependent_Field()
    {
        var model = new NotLessThanModel
        {
            Minimum = 50,
            Current = 60
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Less_Than_Dependent_Field()
    {
        var model = new NotLessThanModel
        {
            Minimum = 50,
            Current = 40
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependency_Is_Set_But_DependsOnField_Is_Not_Set()
    {
        var model = new MissingDependsOnFieldModel
        {
            Current = 10
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Does_Not_Exist()
    {
        var model = new NotExistingDependentFieldModel
        {
            Current = 10
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Is_Not_Number()
    {
        var model = new DependentFieldWrongTypeModel
        {
            Limit = "авбае",
            Current = 10
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }
}

