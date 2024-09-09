using UnityEngine;
using VContainer;

public class InventoryPresenter : MonoBehaviour, IOutPutInventoryUI
{

    private IInventoryRepository inventoryRepository;
    [SerializeField]
    private InventoryUI[] inventoryUI;

    [Inject]
    public void Inject(IInventoryRepository inventoryRepository)
    {
        this.inventoryRepository = inventoryRepository;
    }

    public void OutPut(string itemId)
    {
        var inventoryList = inventoryRepository.FindAll();
        for (int i = 0; i < inventoryList.Count; i++)
        {
            inventoryUI[i].UpdateUI(inventoryList[i].ItemData.itemIcom, inventoryList[i].StackSize.ToString());
        }
    }
}

