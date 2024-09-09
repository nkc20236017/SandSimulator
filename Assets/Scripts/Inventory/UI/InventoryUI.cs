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

    public void UpdateUI(Sprite spriteRenderer ,string itemAmount)
    {
            itemIamge.sprite = spriteRenderer;
            this.itemAmount.text = itemAmount;
    }

}
