using System.Collections.Immutable;

namespace MonopolyTestTask.Model
{
    public class Pallet
    {
        private const double emptyWeight = 30;
        public int Id { get; }
        public double Width { get; }
        public double Height { get; }
        public double Depth { get; }
        public double Weight { get; }
        public double Volume { get; }
        public DateOnly ExpirationDate { get; }
        public ImmutableList<Box> Boxes { get; }
        public Pallet(
            int id,
            double width,
            double height,
            double depth,
            IEnumerable<Box> boxes
            )
        {
            //Assuming empty pallet is an error
            if (boxes?.Any(_box => _box != null) == false)
                throw new ArgumentNullException("Pallet must contain some boxes");
            Boxes = boxes!.ToImmutableList();
            var largeBoxes = Boxes
                .Where(_box => _box.Width > width || _box.Depth > depth)
                .Select(_box => _box.Id.ToString());
            if (largeBoxes.Any())
                throw new ArgumentException($"Boxes {String.Join(", ", largeBoxes)} are too large for pallet {id}");
            Id = id;
            Width = width;
            Height = height;
            Depth = depth;
            Weight = Boxes.Sum(_=>_.Weight) + emptyWeight;
            Volume = Boxes.Sum(_ => _.Volume) + Width * Height * Depth;
            ExpirationDate = Boxes.Max(_box => _box.ExpirationDate);
        }
    }
}
