using System.Linq;
using VContainer;

public class PlayerEquipDataAccess : IPlayerEquipRepository
{
    private readonly PlayerEquipDataBase playerEquipDataBase;
    private readonly IEquipRepository equipRepository;

    [Inject]
    public PlayerEquipDataAccess(PlayerEquipDataBase playerEquipDataBase
        , IEquipRepository equipRepository)
    {
        this.playerEquipDataBase = playerEquipDataBase;
        this.equipRepository = equipRepository;
    }

    public EquipData FindData(string equipId)
    {
        return playerEquipDataBase.equipData
            .Where(equip => equip.EquipId == equipId)
            .FirstOrDefault();
    }

    public void AddEquip(string equipId)
    {
        var equip = equipRepository.FindData(equipId);
        playerEquipDataBase.equipData.Add(equip);
    }

    public void RemoveEquip(string equipId)
    {
        var equip = equipRepository.FindData(equipId);
        playerEquipDataBase.equipData.Remove(equip);
    }

    public EquipId FindEquipId(string equipId)
    {

        var equip = playerEquipDataBase.equipData
        .Where(equip => equip.EquipId == equipId)
        .FirstOrDefault();

        if (equip == null)
        {
            return new EquipId(string.Empty);
        }

        return new EquipId(equip.EquipId);
    }

    public bool PlayerEquipCheck()
    {
        return playerEquipDataBase.equipData.Any();
    }

    public EquipData FistData()
    {
        return playerEquipDataBase.equipData.First();
    }
}
