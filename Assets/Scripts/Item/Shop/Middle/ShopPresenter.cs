using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ShopPresenter : MonoBehaviour , IOutPutShop
{
    private IEquipRepository equipRepository;
    private IShopRepository shopRepository;

    [SerializeField]
    private ShopUI ShopUIObject;
    [SerializeField]
    private ShopUI EquipUIObject;

    [SerializeField]
    private Text text;

    [Inject]
    public void Inject(IEquipRepository equipRepository,IShopRepository shopRepository)
    {
        this.equipRepository = equipRepository;
        this.shopRepository = shopRepository;
    }

    public void EquipUI(OutPutData outPutData)
    {
        CleanUp();
        Debug.Log("ëïîı");
        text.text = outPutData.Money.ToString();
        var equipShop = equipRepository.FindData(outPutData.EquipId);
        EquipUIObject.UpdateUI(equipShop.EquipIcom);
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
        var shopData = shopRepository.FindShopData(outPutData.EquipId);
        ShopUIObject.UpdateUI(shopData.Equipment.EquipIcom);

    }

    private void CleanUp()
    {
        ShopUIObject.GetComponent<Image>().sprite = null;
        EquipUIObject.GetComponent<Image>().sprite = null;
    }

}

