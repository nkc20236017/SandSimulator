using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ShopInput : MonoBehaviour
{
    private IInputShop inputShop;

    [SerializeField]
    private EquipmentData equipmentData;

    [Inject]
    public void Inject(IInputShop inputShop)
    {
        this.inputShop = inputShop;
    }

    public void InputShop()
    {
        inputShop.ShopBuy(equipmentData.EquipmentId);
    }

}
