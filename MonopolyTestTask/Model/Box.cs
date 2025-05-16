namespace MonopolyTestTask.Model
{
    public record Box
    {
        private const int expirationDurationDays = 100;
        public int Id { get; }
        public double Width { get; }
        public double Height { get; }
        public double Depth { get; }
        public double Weight { get; }
        public double Volume { get; }
        public DateOnly? ProductionDate { get; }
        public DateOnly ExpirationDate { get; }
        public Box(
            int id,
            double width,
            double height,
            double depth,
            double weight,
            DateOnly? productionDate = null,
            DateOnly? expirationDate = null
            )
        {
            if (productionDate == null && expirationDate == null)
                throw new ArgumentNullException();
            else if (productionDate != null) 
            {
                ProductionDate = productionDate;
                ExpirationDate = productionDate.Value.AddDays(expirationDurationDays);
            }
            else
                ExpirationDate = expirationDate!.Value;
            Id = id;
            Width = width;
            Height = height;
            Depth = depth;
            Weight = weight;
            Volume = Width * Height * Depth;
        }
    }
}
