using MonopolyTestTask.Model;

namespace MonopolyTestTaskTests
{
    public class WarehouseTests
    {
        [Fact]
        public void If_PalletsAreNull_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Warehouse(null));
        }

        [Fact]
        public void If_LessThen3Pallets_ThenDoesNotThrow()
        {
            var box1 = new Box(0, 1, 1, 1, 2, expirationDate: DateOnly.MinValue.AddDays(1));
            var boxes1 = new[] { box1 };
            var pallet1 = new Pallet(0, 1, 1, 1, boxes1);

            var box2 = new Box(1, 2, 1, 1, 1, expirationDate: DateOnly.MinValue);
            var boxes2 = new[] { box2 };
            var pallet2 = new Pallet(1, 2, 1, 1, boxes2);

            var pallets = new[] { pallet1, pallet2 };
            var warehouse = new Warehouse(pallets: pallets);
            var sorted = warehouse.GetSortedPallets(out var top3);

            Assert.Equal(2, sorted.Count);
            var actualPallet1 = sorted[0];
            Assert.Equal(pallet2, actualPallet1);
            var actualPallet2 = sorted[1];
            Assert.Equal(pallet1, actualPallet2);

            Assert.Equal(2, top3.Count);
            actualPallet1 = top3[0];
            Assert.Equal(pallet1, actualPallet1);
            actualPallet2 = top3[1];
            Assert.Equal(pallet2, actualPallet2);
        }
    }
}
