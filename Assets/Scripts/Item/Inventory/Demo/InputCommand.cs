using UnityEngine;
using VContainer;

public class InputCommand : MonoBehaviour
{
    private IInventoryCommand inventoryCommand;

    [SerializeField]
    private EquipData equipData;

    [Inject]
    public void Inject(IInventoryCommand inventoryCommand)
    {
        this.inventoryCommand = inventoryCommand;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryCommand.AddCommand(equipData.EquipId);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            inventoryCommand.RemoveCommand(equipData.EquipId);
        }
    }

}
