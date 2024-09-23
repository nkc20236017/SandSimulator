using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Image Image;
    [SerializeField]
    private GameObject SoldOutObject;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    public void UpdateUI(Sprite sprite)
    {
        Image.sprite = sprite;
    }

    public void SoldOut(bool soldout)
    {
        SoldOutObject.SetActive(soldout);
    }

    public void CleanUp()
    {
        Destroy(this.gameObject);
    }
}
