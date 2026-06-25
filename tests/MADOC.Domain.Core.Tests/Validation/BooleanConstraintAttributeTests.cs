using MADOC.Domain.Core.Validation.Attributes;

namespace MADOC.Tests.Domain.Attributes;

[TestClass]
public class BooleanConstraintAttributeTests
{
    [TestMethod]
    public void Should_Pass_When_Value_Is_True_And_No_Dependency_Is_Set()
    {
        var model = new SimpleBooleanModel
        {
            Value = true
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_False()
    {
        var model = new SimpleBooleanModel
        {
            Value = false
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Value_Is_Null()
    {
        var model = new NullableBooleanModel
        {
            Value = null
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Value_Is_Not_Bool()
    {
        var model = new WrongTypeModel
        {
            Value = "true"
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Current_Value_Is_True_And_Dependent_Field_Has_Forbidden_State()
    {
        var model = new DependentBooleanModel
        {
            IsArchived = true,
            CanEdit = true
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Current_Value_Is_True_And_Dependent_Field_Does_Not_Have_Forbidden_State()
    {
        var model = new DependentBooleanModel
        {
            IsArchived = false,
            CanEdit = true
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Pass_When_Current_Value_Is_False_Even_If_Dependent_Field_Has_Forbidden_State()
    {
        var model = new DependentBooleanModel
        {
            IsArchived = true,
            CanEdit = false
        };

        Assert.IsTrue(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Does_Not_Exist()
    {
        var model = new MissingDependentFieldModel
        {
            Value = true
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    [TestMethod]
    public void Should_Fail_When_Dependent_Field_Is_Not_Bool()
    {
        var model = new WrongDependentTypeModel
        {
            State = "true",
            Value = true
        };

        Assert.IsFalse(ValidationTestHelper.IsValid(model));
    }

    private class SimpleBooleanModel
    {
        [BooleanConstraint]
        public bool Value { get; set; }
    }

    private class NullableBooleanModel
    {
        [BooleanConstraint]
        public bool? Value { get; set; }
    }

    private class WrongTypeModel
    {
        [BooleanConstraint]
        public object? Value { get; set; }
    }

    private class DependentBooleanModel
    {
        public bool IsArchived { get; set; }

        [BooleanConstraint(
            DependsOnField = nameof(IsArchived),
            ForbiddenStateIfDependentIs = true)]
        public bool CanEdit { get; set; }
    }

    private class MissingDependentFieldModel
    {
        [BooleanConstraint(
            DependsOnField = "MissingField",
            ForbiddenStateIfDependentIs = true)]
        public bool Value { get; set;}
    }

    private class WrongDependentTypeModel
    {
        public string State { get; set; } = string.Empty;

        [BooleanConstraint(
            DependsOnField = nameof(State),
            ForbiddenStateIfDependentIs = true)]
        public bool Value { get; set; }
    }
}