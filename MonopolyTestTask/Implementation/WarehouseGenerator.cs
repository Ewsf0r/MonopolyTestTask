using CommunityToolkit.Diagnostics;
using MonopolyTestTask.Interface;
using MonopolyTestTask.Model;

namespace MonopolyTestTask.Implementation
{
    public class WarehouseGenerator : IWarehouseGenerator
    {
        public Warehouse Generate(int maxRandomNumber, int palletsCount, int minBoxesCountPerPallet, int maxBoxesCountPerPallet)
        {
            Guard.IsGreaterThan(palletsCount, 0);
            Guard.IsGreaterThan(minBoxesCountPerPallet, 0);
            Guard.IsGreaterThan(maxBoxesCountPerPallet, minBoxesCountPerPallet);

            var rnd = new Random();
            var boxesCount = 0;

            var pallets = Enumerable
                .Range(0, palletsCount)
                .Select(palletId =>
                    {
                        var width = rnd.Next(1, maxRandomNumber);
                        var height = rnd.Next(1, maxRandomNumber);
                        var depth = rnd.Next(1, maxRandomNumber);

                        var boxes = Enumerable
                            .Range(boxesCount, rnd.Next(minBoxesCountPerPallet, maxBoxesCountPerPallet))
                            .Select(boxId =>
                                {
                                    boxesCount++;
                                    var nullDateRandomizer = rnd.Next(maxRandomNumber);
                                    var isProductionOrExpiration = nullDateRandomizer % 2 == 0;
                                    var randomDate = DateOnly.MinValue.AddDays(rnd.Next(maxRandomNumber));

                                    return new Box(
                                        boxId,
                                        rnd.Next(1, width),
                                        rnd.Next(1, height),
                                        rnd.Next(1, depth),
                                        rnd.Next(1, maxRandomNumber),
                                        isProductionOrExpiration? randomDate:null,
                                        isProductionOrExpiration? null:randomDate
                                    );
                                }
                            );

                        return new Pallet(
                            palletId,
                            width,
                            height,
                            depth,
                            boxes);
                    }
                );

            return new Warehouse(pallets);
        }
    }
}
