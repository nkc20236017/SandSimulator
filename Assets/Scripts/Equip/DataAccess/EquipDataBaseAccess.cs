using System.Linq;
using VContainer;

public class EquipDataBaseAccess : IEquipRepository
{
    private EquipDataBase EquipDataBase;

    [Inject]
    public EquipDataBaseAccess(EquipDataBase equipDataBase)
    {
        this.EquipDataBase = equipDataBase;
    }

    public EquipData FindData(string equipId)
    {
        var equip = EquipDataBase.equipmentData
            .Where(equip => equip.EquipId == equipId)
            .FirstOrDefault();
        return equip;
    }

    public EquipId FindId(string equipId)
    {
        var equip = EquipDataBase.equipmentData
    .Where(equip => equip.EquipId == equipId)
    .FirstOrDefault();

        return new EquipId(equip.EquipId);
    }
}
