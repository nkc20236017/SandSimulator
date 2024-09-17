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
    [SerializeField]
    private Image backPanelImage;

    private void Start()
    {
        CleanUp();
    }

    public void UpdateUI(InventoryItem itemInventory)
    {
        itemIamge.color = Color.white;
        itemIamge.sprite = itemInventory.ItemData.itemIcom;
        this.itemAmount.text = itemInventory.StackSize.ToString();
        backPanelImage.color = Color.black;
    }
    public void CleanUp()
    {
        itemIamge.sprite = null;
        itemIamge.color = Color.clear;
        itemAmount.text = string.Empty;
        backPanelImage.color = Color.clear;
    }

}
