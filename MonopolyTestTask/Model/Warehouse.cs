using System.Collections.Immutable;

namespace MonopolyTestTask.Model
{
    public class Warehouse
    {
        public ImmutableList<Pallet> Pallets { get; }
        public ImmutableList<Pallet> Top3PalletsWithLongestExpirationDate { get; }

        public Warehouse(IEnumerable<Pallet> pallets)
        {
            Pallets = pallets
                .OrderBy(_pallet => _pallet.ExpirationDate)
                .GroupBy(_pallet => _pallet.ExpirationDate)
                .Select(_palletGroup => _palletGroup
                    .OrderBy(_pallet => _pallet.Weight))
                .SelectMany(_ => _)
                .ToImmutableList();

            Top3PalletsWithLongestExpirationDate = Pallets
                .TakeLast(3)
                .OrderBy(_pallet => _pallet.Volume)
                .ToImmutableList();
        }
    }
}
