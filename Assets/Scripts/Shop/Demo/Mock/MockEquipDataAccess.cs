using System.Linq;
using VContainer;

public class MockEquipDataAccess : IEquipRepository
{
    private EquipmentDataBase EquipmentDataBase;

    [Inject]
    public MockEquipDataAccess(EquipmentDataBase equipmentDataBase)
    {
        EquipmentDataBase = equipmentDataBase;
    }

    public EquipmentData FindData(string id)
    {
        return EquipmentDataBase
            .equipmentData
            .Where(equip => equip.EquipmentId == id)
            .FirstOrDefault();
    }

    public ShopServiceData FindServiceData(string id)
    {
        var shopdata = EquipmentDataBase
        .equipmentData
        .Where(equip => equip.EquipmentId == id)
        .FirstOrDefault();

        return new ShopServiceData(shopdata.EquipmentId, shopdata.EquipmentName,shopdata.EquipmentPrice);

    }
}
