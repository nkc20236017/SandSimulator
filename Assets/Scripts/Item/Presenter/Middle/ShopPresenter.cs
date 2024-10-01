using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ShopPresenter : MonoBehaviour , IOutPutShop
{
    private IShopRepository shopRepository;

    [SerializeField]
    private GameObject shopUIObject;

    [SerializeField]
    private Text text;

    [Inject]
    public void Inject(IShopRepository shopRepository)
    {
        this.shopRepository = shopRepository;
    }

    private void Start()
    {
        ShopUISetUp();
    }

    private void ShopUISetUp()
    {
        for (int i = 0; i < shopRepository.FindAll().Count; i++)
        {
            var shopObject = Instantiate(this.shopUIObject, this.transform);
            var shopUI = shopObject.GetComponent<ShopUI>();
            shopUI.UpdateUI(shopRepository.FindAll()[i].Equipment.EquipIcom);
            shopUI.SoldOut(shopRepository.FindAll()[i].SoldOut);
            var shopContller = shopObject.GetComponent<ShopContller>();
            shopContller.InjectEquip(shopRepository.FindAll()[i].Equipment);
        }
    }

    public void NotBuyUI(OutPutData outPutData)
    {
        CleanUp();
        Debug.Log("çwì¸Ç≈Ç´Ç‹ÇπÇÒÇ≈ÇµÇΩ");
        
    }

    public void ShopUI(OutPutData outPutData)
    {
        CleanUp();
        Debug.Log("çwì¸ÇµÇ‹ÇµÇΩÅB");
        text.text = outPutData.Money.ToString();
        ShopUISetUp();
    }

    private void CleanUp()
    {
        var objectChilled = GetComponentsInChildren<ShopUI>();
        for (int i = 0; i < objectChilled.Length; i++)
        {
            objectChilled[i].CleanUp();
        }
    }

}

