using MonopolyTestTask.Model;

namespace MonopolyTestTask.Interface
{
    public interface IWarehouseDBConnector
    {
        Task SaveWarehoseAsync(Warehouse _warehouse, CancellationToken cancel);
        Task<Warehouse?> LoadWarehoseAsync(CancellationToken cancel);
    }
}
