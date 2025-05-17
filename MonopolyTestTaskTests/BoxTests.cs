using MonopolyTestTask.Model;

namespace MonopolyTestTaskTests
{
    public class BoxTests
    {
        [Fact]
        public void If_ProductionDateIsNull_And_ExpirationDateIsNull_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Box(0, 0, 0, 0, 0));
        }

        [Fact]
        public void If_AnyMeasurementIsLessThanOrEqualToZero_ThenThrows()
        {
            var productionDate = DateOnly.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 0, 1, 1, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, -1, 1, 1, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, 0, 1, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, -1, 1, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, 1, 0, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, 1, -1, 1, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, 1, 1, 0, productionDate));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Box(0, 1, 1, 1, -1, productionDate));
        }

        [Fact]
        public void If_ProductionDateIsNotNull_And_ExpirationDateIsNotNull_ThenCalculatesExpirationDate()
        {
            var productionDate = DateOnly.MinValue;
            var expirationDate = DateOnly.MaxValue;
            var expectedExpirationDate = productionDate.AddDays(100);

            var testSubject = new Box(0, 1, 1, 1, 1, productionDate, expirationDate);
            var actualExpirationDate = testSubject.ExpirationDate;

            Assert.Equal(expectedExpirationDate, actualExpirationDate);
        }

        [Fact]
        public void If_ProductionDateIsNull_And_ExpirationDateIsNotNull_ThenKeepsExpirationDate()
        {
            var expirationDate = DateOnly.MaxValue;
            var expectedExpirationDate = expirationDate;
            
            var testSubject = new Box(0, 1, 1, 1, 1, expirationDate: expirationDate);
            var actualExpirationDate = testSubject.ExpirationDate;

            Assert.Equal(expectedExpirationDate, actualExpirationDate);
        }
    }
}