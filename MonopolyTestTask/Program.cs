using MonopolyTestTask.Implementation;

//var db = new WarehousesDBConnector();
var generator = new WarehouseGenerator();
var warehouse = generator.Generate(10, 10, 3, 10);
Console.WriteLine("Unsorted pallets:");
foreach (var palet in warehouse.Pallets)
    Console.WriteLine(palet);

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