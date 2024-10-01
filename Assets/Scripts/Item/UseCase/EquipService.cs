using UnityEngine;
using VContainer;

public class EquipService : IEquip
{
    private IModifiers modifiers;
    private IPlayerEquipRepository playerEquipRepository;
    private IInputInventory inventoryCommand;
    private IOutPutEquip outPutEquip;

    [Inject]
    public EquipService(IModifiers modifiers,
        IPlayerEquipRepository playerEquipRepository,
        IInputInventory inventoryCommand,IOutPutEquip outPutEquip)
    {
        this.modifiers = modifiers;
        this.playerEquipRepository = playerEquipRepository;
        this.inventoryCommand = inventoryCommand;
        this.outPutEquip = outPutEquip;
    }

    public void Equip(EquipData equipData)
    {

        if (playerEquipRepository.PlayerEquipCheck())
        {
            var oldEquip = playerEquipRepository.FistData();
            UnEquip(oldEquip);
        }
        playerEquipRepository.AddEquip(equipData.EquipId);
        modifiers.AddModifiers(equipData.EquipId);
        inventoryCommand.RemoveFromInventory(equipData);
        outPutEquip.OutPutUI(equipData);
    }



    public void UnEquip(EquipData equipData)
    {
        inventoryCommand.AddToInventory(equipData);
        playerEquipRepository.RemoveEquip(equipData.EquipId);
        modifiers.RemoveModifiers(equipData.EquipId);
        outPutEquip.OutPutUnEquipUI();
    }
}
