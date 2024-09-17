public class EquipService : IEquip
{
    private IEquipRepository EquipRepository;
    private IModifiers modifiers;

    public EquipService(IEquipRepository equipRepository, IModifiers modifiers)
    {
        EquipRepository = equipRepository;
        this.modifiers = modifiers;
    }

    public void Equip(string id)
    {
        var oldEquip = string.Empty;
        var newEquip = EquipRepository.FindData(id);
        if (newEquip.EquipId == id)
        {
            oldEquip = newEquip.EquipId;
        }

        if (oldEquip != string.Empty)
        {
            UnEquip(oldEquip);
        }
        EquipRepository.AddEquip(id);
        modifiers.AddModifiers(id);

    }



    public void UnEquip(string id)
    {
        EquipRepository.RemoveEquip(id);
        modifiers.RemoveModifiers(id);
    }
}
