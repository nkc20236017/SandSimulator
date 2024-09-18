using System.Collections.Generic;

public class EquipService : IEquip
{
    private IModifiers modifiers;
    private IPlayerEquipRepository playerEquipRepository;

    public EquipService( IModifiers modifiers,
        IPlayerEquipRepository playerEquipRepository)
    {
        this.modifiers = modifiers;
        this.playerEquipRepository = playerEquipRepository;
    }

    public void Equip(string id)
    {
        var oldEquip = string.Empty;
        var newEquip = playerEquipRepository.FindEquipId(id);
        if (newEquip.Id == id)
        {
            oldEquip = newEquip.Id;
        }

        if (oldEquip != string.Empty)
        {
            UnEquip(oldEquip);
        }
        playerEquipRepository.AddEquip(id);
        modifiers.AddModifiers(id);

    }



    public void UnEquip(string id)
    {
        playerEquipRepository.RemoveEquip(id);
        modifiers.RemoveModifiers(id);
    }
}
