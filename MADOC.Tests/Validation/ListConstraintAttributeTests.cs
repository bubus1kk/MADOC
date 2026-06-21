//using MADOC.Domain.Validation.Attributes;
//using System;
//namespace MADOC.Tests.Domain.Attributes;

//[TestClass]
//public class ListConstraintAttributeTests
//{
//    [TestMethod]
//    public void Should_Pass_When_Value_Is_In_Allowed_Values()
//    {
//        var model = new ListModel
//        {
//            Value = "Студент"
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Fail_When_Value_Is_Not_In_Allowed_Values()
//    {
//        var model = new ListModel
//        {
//            Value = "Преподаватель"
//        };

//        Assert.IsFalse(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Pass_When_Value_Is_Null()
//    {
//        var model = new NullableListModel
//        {
//            Value = null
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Pass_When_Value_Is_Empty_String()
//    {
//        var model = new ListModel
//        {
//            Value = ""
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Pass_When_Value_Is_Whitespace()
//    {
//        var model = new ListModel
//        {
//            Value = "   "
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    [TestMethod]
//    public void Should_Use_ToString_For_Value_Checking()
//    {
//        var model = new ObjectListModel
//        {
//            Value = 123
//        };

//        Assert.IsTrue(ValidationTestHelper.IsValid(model));
//    }

//    private class ListModel
//    {
//        [ListConstraint("Студент", "Староста", "Выпускник")]
//        public string Value { get; set; } = string.Empty;
//    }

//    private class NullableListModel
//    {
//        [ListConstraint("Студент", "Староста", "Выпускник")]
//        public string? Value { get; set; }
//    }

//    private class ObjectListModel
//    {
//        [ListConstraint("123")]
//        public object? Value { get; set; }
//    }
//}