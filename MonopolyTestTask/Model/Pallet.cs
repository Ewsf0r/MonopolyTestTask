using CommunityToolkit.Diagnostics;
using System.Collections.Immutable;

namespace MonopolyTestTask.Model
{
    public record Pallet
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
            ICollection<Box> boxes
        )
        {
            Guard.IsNotEmpty(boxes);
            Guard.IsGreaterThan(width, 0);
            Guard.IsGreaterThan(height, 0);
            Guard.IsGreaterThan(depth, 0);

            Boxes = boxes.ToImmutableList();
            Boxes
                .ForEach(_box =>
                    {
                        Guard.IsLessThanOrEqualTo(_box.Width, width);
                        Guard.IsLessThanOrEqualTo(_box.Depth, depth);
                    }
                );

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
