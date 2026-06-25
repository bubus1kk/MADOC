using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;
using MADOC.Tests.Domain;

namespace MADOC.Tests.Validation
{
    [TestClass]
    public class NumberRangeConstraintAttributeTests
    {
        private class NumberRangeModel
        {
            [NumberRangeConstraint]
            public NumberRange? Value { get; set; }
        }

        private class NullableNumberRangeModel
        {
            [NumberRangeConstraint]
            public NumberRange? Value { get; set; }
        }

        private class WrongTypeModel
        {
            [NumberRangeConstraint]
            public object? Value { get; set; }
        }

        private class WholeNumberRangeModel
        {
            [NumberRangeConstraint(AllowFloats = false)]
            public NumberRange? Value { get; set; }
        }

        [TestMethod]
        public void Should_Pass_When_Range_Is_Valid()
        {
            var model = new NumberRangeModel
            {
                Value = new NumberRange(10, 20)
            };

            Assert.IsTrue(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_From_Is_Greater_Than_To()
        {
            var model = new NumberRangeModel
            {
                Value = new NumberRange(20, 10)
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Pass_When_Value_Is_Null()
        {
            var model = new NullableNumberRangeModel
            {
                Value = null
            };

            Assert.IsTrue(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_Value_Is_Not_NumberRange()
        {
            var model = new WrongTypeModel
            {
                Value = "10-20"
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Pass_When_Floats_Are_Not_Allowed_But_Bounds_Are_Whole()
        {
            var model = new WholeNumberRangeModel
            {
                Value = new NumberRange(10, 20)
            };

            Assert.IsTrue(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_Floats_Are_Not_Allowed_And_From_Is_Float()
        {
            var model = new WholeNumberRangeModel
            {
                Value = new NumberRange(10.5, 20)
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_Floats_Are_Not_Allowed_And_To_Is_Float()
        {
            var model = new WholeNumberRangeModel
            {
                Value = new NumberRange(10, 20.5)
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }
    }
}
