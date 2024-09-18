using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{

    private Image Image;

    private void Start()
    {
        Image = GetComponent<Image>();
    }

    public void UpdateUI(Sprite sprite)
    {
        Image.sprite = sprite;
    }

}
