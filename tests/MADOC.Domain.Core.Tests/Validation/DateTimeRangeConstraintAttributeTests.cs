using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Attributes;
using MADOC.Tests.Domain;

namespace MADOC.Tests.Validation
{
    [TestClass]
    public class DateTimeRangeConstraintAttributeTests
    {
        private class DateTimeRangeModel
        {
            [DateTimeRangeConstraint]
            public DateTimeRange? Value { get; set; }
        }

        private class NullableDateTimeRangeModel
        {
            [DateTimeRangeConstraint]
            public DateTimeRange? Value { get; set; }
        }

        private class WrongTypeModel
        {
            [DateTimeRangeConstraint]
            public object? Value { get; set; }
        }

        [TestMethod]
        public void Should_Pass_When_Range_Is_Valid()
        {
            var model = new DateTimeRangeModel
            {
                Value = new DateTimeRange(new DateTime(2026, 6, 1, 10, 0, 0), new DateTime(2026, 6, 1, 12, 0, 0))
            };

            Assert.IsTrue(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_From_Is_Greater_Than_To()
        {
            var model = new DateTimeRangeModel
            {
                Value = new DateTimeRange(new DateTime(2026, 6, 1, 12, 0, 0), new DateTime(2026, 6, 1, 10, 0, 0))
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Pass_When_Value_Is_Null()
        {
            var model = new NullableDateTimeRangeModel
            {
                Value = null
            };

            Assert.IsTrue(ValidationTestHelper.IsValid(model));
        }

        [TestMethod]
        public void Should_Fail_When_Value_Is_Not_DateTimeRange()
        {
            var model = new WrongTypeModel
            {
                Value = "2026-06-01 10:00 - 2026-06-01 12:00"
            };

            Assert.IsFalse(ValidationTestHelper.IsValid(model));
        }
    }
}
