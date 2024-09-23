using UnityEngine;
using VContainer;

public class InputCommand : MonoBehaviour
{
    private IInventoryCommand inventoryCommand;
    private IEquipCommand equipCommand;
    private IShopCommand shopCommand;


    [Inject]
    public void Inject(IInventoryCommand inventoryCommand,IEquipCommand equipCommand,IShopCommand shopCommand)
    {
        this.shopCommand = shopCommand;
        this.inventoryCommand=inventoryCommand;
        this.equipCommand = equipCommand;
    }

    public void EquipCommand(EquipData equipData)
    {
        equipCommand.Equip(equipData.EquipId);
    }

    public void UnEquipCommand(EquipData equipData)
    {
        equipCommand.UnEquip(equipData.EquipId);
    }

    public void ShopCommand(EquipData equipData)
    {
        shopCommand.ShopBuy(equipData.EquipId);
    }

}
