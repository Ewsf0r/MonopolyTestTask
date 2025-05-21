using System.Collections.Immutable;

namespace MonopolyTestTask.Model
{
    public class Warehouse
    {
        public List<Pallet> Pallets { get; }

        public Warehouse(IEnumerable<Pallet> pallets)
        {
            Pallets = pallets.ToList();
        }

        public ImmutableList<Pallet> GetSortedPallets(
            out ImmutableList<Pallet> top3PalletsWithLongestExpirationDate)
        {
            var sortedPallets = Pallets
                .OrderBy(_pallet => _pallet.ExpirationDate)
                .GroupBy(_pallet => _pallet.ExpirationDate)
                .Select(_palletGroup => _palletGroup
                    .OrderBy(_pallet => _pallet.Weight))
                .SelectMany(_ => _)
                .ToImmutableList();

            top3PalletsWithLongestExpirationDate = sortedPallets
                .TakeLast(3)
                .OrderBy(_pallet => _pallet.Volume)
                .ToImmutableList();

            return sortedPallets;
        }

        public void AddPallet(Pallet pallet) => Pallets.Add(pallet);
        public void RemovePallet(Pallet pallet) => Pallets.Remove(pallet);
    }
}
