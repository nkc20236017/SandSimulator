using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Image itemIamge;
    [SerializeField]
    private Text itemAmount;


    public void UpdateUI(ItemInventory itemInventory)
    {
        itemIamge.color = Color.white;
        itemIamge.sprite = itemInventory.ItemData.itemIcom;
            this.itemAmount.text = itemInventory.StackSize.ToString();
    }
    public void CleanUp()
    {
        itemIamge.sprite = null;
        itemIamge.color = Color.clear;
        itemAmount.text = string.Empty;
    }

}
