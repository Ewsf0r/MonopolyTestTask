using MonopolyTestTask.Model;

namespace MonopolyTestTask.Interface
{
    internal interface IWarehouseDBConnector
    {
        Task SaveWarehoseAsync(Warehouse _warehouse, CancellationToken cancel);
        Task<Warehouse?> LoadWarehoseAsync(CancellationToken cancel);
    }
}
