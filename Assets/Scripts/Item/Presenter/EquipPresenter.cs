using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipPresenter : MonoBehaviour, IOutPutEquip
{
    [SerializeField]
    private EquipUI itemUI;
    private UnEquipContller unEquipContller;

    public void OutPutUI(EquipData data)
    {
        unEquipContller = itemUI.gameObject.GetComponent<UnEquipContller>();
        itemUI.UpdateUI(data.EquipIcom);
        unEquipContller.InjectEquip(data);
    }

    public void OutPutUnEquipUI()
    {
        itemUI.CleanUp();
    }
}
