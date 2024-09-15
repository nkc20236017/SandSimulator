using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void MoneyUI(int amount,Sprite sprite)
    {
        text.text = amount.ToString();
        this.gameObject.GetComponent<Image>().sprite = sprite;
    }
}
