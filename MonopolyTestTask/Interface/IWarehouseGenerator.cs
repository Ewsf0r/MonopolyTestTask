using MonopolyTestTask.Model;

namespace MonopolyTestTask.Interface
{
    public interface IWarehouseGenerator
    {
        Warehouse Generate(int maxRandomNumber, int palletsCount, int minBoxesCountPerPallet, int maxBoxesCountPerPallet);
    }
}
