namespace MonopolyTestTask;

using Implementation;
using Model;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Welcome to warehouse controller. Please select your option:");
        var generator = new WarehouseGenerator();
        var dbConnector = new WarehousesDBConnector();
        while (true)
        {
            var source = GetWarehouseSourceIndex();
            Warehouse? warehouse = null;
            var cancellationTokenSource = new CancellationTokenSource();
            switch (source)
            {
                case 1:
                    {
                        warehouse = generator.Generate(10, 10, 3, 10);
                        break;
                    }
                case 2:
                    {
                        warehouse = GenerateWarehouseSemiAutomaticly(generator);
                        break;
                    }
                case 3:
                    {
                        warehouse = GenerateWarehouseManualy();
                        if (warehouse == null)
                            Console.WriteLine("Manual input was terminated");
                        break;
                    }
                case 4:
                    {
                        warehouse = await dbConnector.LoadWarehoseAsync(cancellationTokenSource.Token);
                        if (warehouse == null)
                            Console.WriteLine("There is now saved data in the database");
                        break;
                    }
                case 0:
                    return;
            }
            if (warehouse == null)
                continue;

            Console.WriteLine();
            Console.WriteLine("Warehouse pallets:");
            foreach (var palet in warehouse.Pallets)
                Console.WriteLine(palet);

            Console.WriteLine();
            Console.WriteLine($"Would you like to save warehouse to database [y/n]"); 
            while (true)
            {
                var response = Console.ReadLine();
                if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
                    break;
                if (string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
                {
                    await dbConnector.SaveWarehoseAsync(warehouse, cancellationTokenSource.Token);
                    break;
                }
                Console.WriteLine("Please enter valid choice");
            }

            Console.WriteLine();
            Console.WriteLine();
            var sortedPallets = warehouse.GetSortedPallets(out var best);
            Console.WriteLine("Sorted pallets:");
            foreach (var palet in sortedPallets)
                Console.WriteLine(palet);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Top 3 pallets sorted by volume:");
            foreach (var palet in best)
                Console.WriteLine(palet);

            Console.WriteLine();
            Console.WriteLine($"Would you like to try new warehouse [y/n]");
            while (true)
            {
                var response = Console.ReadLine();
                if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
                    return;
                if (string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
                    break;
                Console.WriteLine("Please enter valid choice");
            }
            Console.WriteLine();
        }
    }

    private static int GetWarehouseSourceIndex()
    {
        Console.WriteLine("1 generate warehouse automaticly");
        Console.WriteLine("2 generate warehouse semi-automaticly");
        Console.WriteLine("3 generate warehouse manualy");
        Console.WriteLine("4 load warehose from database");
        Console.WriteLine("0 exit program");
        Console.WriteLine();

        while (true)
        {
            if (!int.TryParse(Console.ReadLine(), out var source))
            {
                Console.WriteLine("Please enter valid choice");
                continue;
            }
            switch (source)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    return source;
                default:
                    {
                        Console.WriteLine("Please enter valid choice");
                        continue;
                    }
            }
        }
    }

    private static Warehouse GenerateWarehouseSemiAutomaticly(WarehouseGenerator generator)
    {
        int maxRandomNumber, palletsCount, minBoxesCountPerPallet, maxBoxesCountPerPallet;
        Console.WriteLine("How many pallets should be in the warehouse?");
        Console.WriteLine("Minimum 1");
        Console.WriteLine("Default 10");
        if (!int.TryParse(Console.ReadLine(), out palletsCount) || palletsCount < 1)
            palletsCount = 10;

        Console.WriteLine("What is minimum amount of boxes on one pallet?");
        Console.WriteLine("Minimum 1");
        Console.WriteLine("Default 3");
        if (!int.TryParse(Console.ReadLine(), out minBoxesCountPerPallet) || minBoxesCountPerPallet < 1)
            minBoxesCountPerPallet = 3;

        Console.WriteLine("What is maximum amount of boxes on one pallet?");
        Console.WriteLine($"Minimum {minBoxesCountPerPallet}");
        Console.WriteLine("Default 3");
        if (!int.TryParse(Console.ReadLine(), out maxBoxesCountPerPallet)
            || maxBoxesCountPerPallet <= minBoxesCountPerPallet)
            maxBoxesCountPerPallet = 10;

        Console.WriteLine("What is the maximum value of other parameters?");
        Console.WriteLine($"Minimum 1");
        Console.WriteLine("Default 10");
        if (!int.TryParse(Console.ReadLine(), out maxRandomNumber) || maxRandomNumber < 1)
            maxRandomNumber = 10;

        var warehouse = generator.Generate(
            maxRandomNumber,
            palletsCount,
            minBoxesCountPerPallet,
            maxBoxesCountPerPallet
        );
        return warehouse;
    }

    private static Warehouse? GenerateWarehouseManualy()
    {
        var pallets = new List<Pallet>();
        var palletId = 0;
        var boxId = 0;
        Console.WriteLine($"Add pallet [y/n]");
        while (true)
        {
            var response = Console.ReadLine();
            if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
                break;
            if (!string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Please enter valid choice");
                continue;
            }

            double? palletWidth, palletHeight, palletDepth;
            palletWidth = GetDoubleParameter("pallet width");
            if (!palletWidth.HasValue)
                break;
            palletHeight = GetDoubleParameter("pallet height");
            if (!palletHeight.HasValue)
                break;
            palletDepth = GetDoubleParameter("pallet depth");
            if (!palletDepth.HasValue)
                break;

            var pallet = new Pallet(
                palletId,
                palletWidth.Value,
                palletHeight.Value,
                palletDepth.Value,
                Enumerable.Empty<Box>()
            );
            AddBoxes(pallet, ref boxId);
            pallets.Add(pallet);
            palletId++;
        }

        if (pallets.Count == 0)
            return null;
        var warehouse = new Warehouse(pallets);
        return warehouse;
    }

    private static void AddBoxes(Pallet pallet, ref int boxId)
    {
        while (true)
        {
            Console.WriteLine($"Add box [y/n]");
            var response = Console.ReadLine();
            if (string.Equals(response, "n", StringComparison.InvariantCultureIgnoreCase))
                break;
            if (!string.Equals(response, "y", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Please enter valid choice");
                continue;
            }
            double? boxWidth, boxHeight, boxDepth, boxWeight;
            DateOnly? productionDate, expirationDate;
            boxWidth = GetDoubleParameter("box width");
            if (!boxWidth.HasValue)
                break;
            boxHeight = GetDoubleParameter("box height");
            if (!boxHeight.HasValue)
                break;
            boxDepth = GetDoubleParameter("box depth");
            if (!boxDepth.HasValue)
                break;
            boxWeight = GetDoubleParameter("box weight");
            if (!boxWeight.HasValue)
                break;
            productionDate = GetDateOnlyParameter("production date");
            expirationDate = GetDateOnlyParameter("expiration date");
            var box = new Box(
                boxId,
                boxWidth.Value,
                boxHeight.Value,
                boxDepth.Value,
                boxWeight.Value,
                productionDate,
                expirationDate
            );
            pallet.AddBox(box);
            boxId++;
        }
    }

    private static DateOnly? GetDateOnlyParameter(string parameterName)
    {
        DateOnly result;
        Console.WriteLine($"Please enter {parameterName} or empty line");
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                return null;
            if(DateOnly.TryParse(line, out result))
                return result;
            Console.WriteLine("Please enter valid date");
        }
    }

    private static double? GetDoubleParameter(string parameterName)
    {
        double result;
        Console.WriteLine($"Please enter {parameterName}, or 0 to stop manual input");
        while (!double.TryParse(
                Console.ReadLine()?.Replace(',', '.'),
                out result)
            || result < 0)
        {
            if (result == 0)
                return null;
            Console.WriteLine("Please enter real positive number");
        }
        return result;
    }
}