using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPresenter : MonoBehaviour , IOutPutShop
{
    private IEquipRepository equipRepository;
    private IShopRepository shopRepository;

    [SerializeField]
    private ShopUI ShopUIObject;

    [SerializeField]
    private Text text;

    public void Inject(IEquipRepository equipRepository,IShopRepository shopRepository)
    {
        this.equipRepository = equipRepository;
        this.shopRepository = shopRepository;
    }

    public void EquipUI(OutPutData outPutData)
    {
        Debug.Log("����");
        text.text = outPutData.Money.ToString();
        var equipShop = equipRepository.FindData(outPutData.EquipId);
        ShopUIObject.UpdateUI(equipShop.EquipIcom);
    }

    public void NotBuyUI(OutPutData outPutData)
    {
        Debug.Log("�w���ł��܂���ł���");
        
    }

    public void ShopUI(OutPutData outPutData)
    {
        Debug.Log("�w�����܂����B");
        text.text = outPutData.Money.ToString();
        var shopData = shopRepository.FindShopData(outPutData.EquipId);
        ShopUIObject.UpdateUI(shopData.Equipment.EquipIcom);

    }


}

