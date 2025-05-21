using MonopolyTestTask.Model;

namespace MonopolyTestTaskTests
{
    public class PalletTests
    {
        [Fact]
        public void If_AnyMeasurementIsLessThanOrEqualToZero_ThenThrows()
        {
            var productionDate = DateOnly.MinValue;
            var testBox = new Box(0, 1, 1, 1, 1, productionDate);
            var testBoxes = new[] { testBox };

            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 0, 1, 1, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, -1, 1, 1, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 1, 0, 1, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 1, -1, 1, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 1, 1, 0, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 1, 1, -1, testBoxes));
        }


        [Fact]
        public void If_BoxWidthIsGreaterThanPalletWidth_Or_BoxDepthIsGreaterThanPalletDepth_ThenThrows_OtherwiseNotThrows()
        {
            var productionDate = DateOnly.MinValue;
            var testBox = new Box(0, 2, 1, 2, 1, productionDate);
            var testBoxes = new[] { testBox };
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 1, 1, 2, testBoxes));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pallet(0, 2, 1, 1, testBoxes));
            var testSubject = new Pallet(0, 2, 1, 2, testBoxes);
        }
    }
}
