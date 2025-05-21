using CommunityToolkit.Diagnostics;

namespace MonopolyTestTask.Model
{
    public record Pallet
    {
        private const double emptyWeight = 30;
        public int Id { get; }
        public double Width { get; }
        public double Height { get; }
        public double Depth { get; }
        public double Weight { get; private set; }
        public double Volume { get; private set; }
        public DateOnly ExpirationDate { get; private set; }
        public List<Box> Boxes { get; }
        public Pallet(
            int id,
            double width,
            double height,
            double depth,
            IEnumerable<Box> boxes
        )
        {
            Guard.IsGreaterThan(width, 0);
            Guard.IsGreaterThan(height, 0);
            Guard.IsGreaterThan(depth, 0);

            Boxes = boxes.ToList();
            if (Boxes.Count > 0)
            {
                Boxes
                    .ForEach(_box =>
                        {
                            Guard.IsLessThanOrEqualTo(_box.Width, width);
                            Guard.IsLessThanOrEqualTo(_box.Depth, depth);
                        }
                    );
            }

            Id = id;
            Width = width;
            Height = height;
            Depth = depth;

            Weight = Boxes.Sum(_=>_.Weight) + emptyWeight;
            Volume = Boxes.Sum(_ => _.Volume) + Width * Height * Depth;
            ExpirationDate = Boxes.Min(_box => _box.ExpirationDate);
        }

        public void AddBox(Box box)
        {
            Guard.IsLessThanOrEqualTo(box.Width, Width);
            Guard.IsLessThanOrEqualTo(box.Depth, Depth);
            Boxes.Add(box);
            if (box.ExpirationDate < ExpirationDate)
                ExpirationDate = box.ExpirationDate;
            Weight += box.Weight;
            Volume += box.Volume;
        }

        public void AddManyBoxes(IEnumerable<Box> boxes)
        {
            foreach(var box in boxes)
                AddBox(box);
        }
    }
}
