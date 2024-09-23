using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour , IOutPutInventory
{
    [SerializeField]
    private GameObject PrefabUI;


    public void OutPutUI(InventoryOutputData OutputData)
    {
        var InventoryUI = GetComponentsInChildren<ItemUI>();
        for(int i = 0; i < InventoryUI.Length; i++)
        {
            InventoryUI[i].CleanUp();
        }


        for(int i = 0;i < OutputData.equipData.Count; i++)
        {
            var createObject = Instantiate(PrefabUI,this.transform);
            createObject.GetComponent<ItemUI>().UpdateUI(OutputData.equipData[i].EquipIcom);
            createObject.GetComponent<EquipContller>().InjectEquip(OutputData.equipData[i]);
        }
    }

}
