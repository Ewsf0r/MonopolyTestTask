using Microsoft.Data.Sqlite;
using MonopolyTestTask.Interface;
using System.Text;

namespace MonopolyTestTask.Model
{
    public class WarehousesDBConnector: IDisposable, IWarehouseDBConnector
    {
        private static readonly string DbPath = AppContext.BaseDirectory + "Data/Warehouses.db";

        private static readonly string PalletTableName = nameof(Pallet).ToLower();
        private static readonly string PalletId = nameof(Pallet.Id).ToLower();
        private static readonly string PalletWidth = nameof(Pallet.Width).ToLower();
        private static readonly string PalletHeight = nameof(Pallet.Height).ToLower();
        private static readonly string PalletDepth = nameof(Pallet.Depth).ToLower();

        private static readonly string BoxTableName = nameof(Box).ToLower();
        private static readonly string BoxId = nameof(Box.Id).ToLower();
        private static readonly string BoxWidth = nameof(Box.Width).ToLower();
        private static readonly string BoxHeight = nameof(Box.Height).ToLower();
        private static readonly string BoxDepth = nameof(Box.Depth).ToLower();
        private static readonly string BoxWeight = nameof(Box.Weight).ToLower();
        private static readonly string BoxProductionDate = nameof(Box.ProductionDate).ToLower();
        private static readonly string BoxExpirationDate = nameof(Box.ExpirationDate).ToLower();
        private static readonly string BoxForeignKey = PalletTableName + PalletId;

        private readonly SqliteConnection _connection;
        public WarehousesDBConnector()
        {
            var isInitNeeded = false;
            var file = new FileInfo(DbPath);

            if (!file.Directory!.Exists)
                file.Directory.Create();
            if (!file.Exists)
            {
                using var stream = file.Create();
                isInitNeeded = true;
            }

            _connection = new SqliteConnection($"Data Source={DbPath}");
            _connection.Open();
            if(isInitNeeded)
                InitDbAsync(CancellationToken.None).Wait();
        }

        private async Task InitDbAsync(CancellationToken cancel)
        {
            if(await IsDbExistAsync(cancel))
                return;
            using var command = _connection.CreateCommand();

            var executionStr = 
                $"CREATE TABLE IF NOT EXISTS {PalletTableName} (" +
                $"{PalletId} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $"{PalletWidth} REAL, " +
                $"{PalletHeight} REAL, " +
                $"{PalletDepth} REAL" +
                $");";

            command.CommandText = executionStr;
            await command.ExecuteNonQueryAsync(cancel);

            executionStr =
                $"CREATE TABLE IF NOT EXISTS {BoxTableName} (" +
                $"{BoxId} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $"{BoxWidth} REAL, " +
                $"{BoxHeight} REAL, " +
                $"{BoxDepth} REAL, " +
                $"{BoxWeight} REAL, " +
                $"{BoxProductionDate} TEXT, " +
                $"{BoxExpirationDate} TEXT, " +
                $"{BoxForeignKey} INTEGER, " +
                $"FOREIGN KEY ({BoxForeignKey}) REFERENCES {PalletTableName}({PalletId})" +
                $");";

            command.CommandText = executionStr;
            await command.ExecuteNonQueryAsync(cancel);
        }

        private async Task ClearDbAsync(CancellationToken cancel)
        {
            var executionStr = 
                $"DELETE FROM {BoxTableName}; " +
                $"DELETE FROM {PalletTableName};";
            using var command = _connection.CreateCommand();
            command.CommandText = executionStr;
            await command.ExecuteNonQueryAsync(cancel);
        }

        public async Task SaveWarehoseAsync(Warehouse _warehouse, CancellationToken cancel) 
        {
            await ClearDbAsync(cancel);

            var executionStrBuilder = new StringBuilder();
            executionStrBuilder.Append(
                $"INSERT INTO {PalletTableName} (" +
                $"{PalletId}, " +
                $"{PalletWidth}, " +
                $"{PalletHeight}, " +
                $"{PalletDepth}, " +
                $") VALUES ");
            var pallets = _warehouse.Pallets;
            foreach (var pallet in pallets)
            {
                executionStrBuilder.Append(
                    $"(" +
                    $"{pallet.Id}" +
                    $"{pallet.Width}" +
                    $"{pallet.Height}" +
                    $"{pallet.Depth}" +
                    $"), ");
            }
            executionStrBuilder.Remove(executionStrBuilder.Length - 2, 2);
            executionStrBuilder.Append(";");
            using var command = _connection.CreateCommand();
            command.CommandText = executionStrBuilder.ToString();
            await command.ExecuteNonQueryAsync(cancel);
            executionStrBuilder.Clear();

            foreach (var pallet in pallets)
            {
                executionStrBuilder.Append(
                    $"INSERT INTO {BoxTableName} (" +
                    $"{BoxId}, " +
                    $"{BoxWidth}, " +
                    $"{BoxHeight}, " +
                    $"{BoxDepth}, " +
                    $"{BoxWeight}, " +
                    $"{BoxProductionDate}, " +
                    $"{BoxExpirationDate}, " +
                    $") VALUES ");

                var boxes = pallet.Boxes;
                foreach (var box in boxes)
                {
                    executionStrBuilder.Append(
                        $"(" +
                        $"{box.Id}" +
                        $"{box.Width}" +
                        $"{box.Height}" +
                        $"{box.Depth}" +
                        $"{box.Weight}" +
                        $"{box.ProductionDate}" +
                        $"{box.ExpirationDate}" +
                        $"), ");
                }
                executionStrBuilder.Remove(executionStrBuilder.Length - 2, 2);
                executionStrBuilder.Append(";");

                command.CommandText = executionStrBuilder.ToString();
                await command.ExecuteNonQueryAsync(cancel);
                executionStrBuilder.Clear();
            }
        }

        public async Task<Warehouse?> LoadWarehoseAsync(CancellationToken cancel)
        {
            var executionStr = 
                $"SELECT (" +
                $"{PalletId}, " +
                $"{PalletWidth}, " +
                $"{PalletHeight}, " +
                $"{PalletDepth}) " +
                $"FROM {PalletTableName};";

            using var command = _connection.CreateCommand();
            command.CommandText = executionStr;
            var reader = await command.ExecuteReaderAsync(cancel);

            if (!reader.HasRows)
            {
                Console.WriteLine($"No warehouse");
                await _connection.CloseAsync();
                return null;
            }

            var pallets = new List<Pallet>();
            while (await reader.ReadAsync(cancel))
            {
                var pallet = new Pallet(
                    id: (int)reader[PalletId],
                    width: (double)reader[PalletWidth],
                    height: (double)reader[PalletWidth],
                    depth: (double)reader[PalletDepth],
                    boxes: new List<Box>());
            }
            reader.Dispose();

            foreach (var pallet in pallets) 
            {
                executionStr =
                    $"SELECT (" +
                    $"{BoxId}, " +
                    $"{BoxWidth}, " +
                    $"{BoxHeight}, " +
                    $"{BoxDepth}, " +
                    $"{BoxWeight}, " +
                    $"{BoxProductionDate}, " +
                    $"{BoxExpirationDate}, " +
                    $") " +
                    $"FROM {BoxTableName} " +
                    $"WHERE {BoxForeignKey} = {pallet.Id};";

                command.CommandText = executionStr;
                reader = await command.ExecuteReaderAsync(cancel);

                if (!reader.HasRows)
                {
                    Console.WriteLine($"No warehouse");
                    await _connection.CloseAsync();
                    return null;
                }

                var boxes = new List<Box>();
                while (await reader.ReadAsync(cancel))
                {
                    var box = new Box(
                        id: (int)reader[BoxId],
                        width: (double)reader[BoxWidth],
                        height: (double)reader[BoxHeight],
                        depth: (double)reader[BoxDepth],
                        weight: (double)reader[BoxWeight],
                        productionDate: GetNullableDateOnlyFromReader(reader[BoxProductionDate]),
                        expirationDate: GetNullableDateOnlyFromReader(reader[BoxExpirationDate]));
                    boxes.Add(box);
                }
                pallet.AddManyBoxes(boxes);
            }

            var warehose = new Warehouse(pallets);
            return warehose;
        }

        private DateOnly? GetNullableDateOnlyFromReader(object readerField)
        {
            var str = (string?)readerField;
            DateOnly? result = string.IsNullOrEmpty(str) ? null : DateOnly.Parse(str);
            return result;
        }

        private async Task<bool> IsDbExistAsync(CancellationToken cancel = default!)
        {
            var executionStr = "SELECT count(*) FROM sqlite_master WHERE type='table';";
            using var command = _connection.CreateCommand();
            command.CommandText = executionStr;
            using var reader = await command.ExecuteReaderAsync(cancel);
            if(!reader.HasRows || !await reader.ReadAsync(cancel))
                return false;
            return reader.GetInt32(0) > 0;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
