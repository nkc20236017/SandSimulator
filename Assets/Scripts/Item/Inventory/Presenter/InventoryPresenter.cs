using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour , IOutPutInventory
{
    [SerializeField]
    private GameObject PrefabUI;


    public void OutPutUI(InventoryOutputData OutputData)
    {
        var InventoryUI = GetComponentsInChildren<InventoryUI>();
        for(int i = 0; i < InventoryUI.Length; i++)
        {
            InventoryUI[i].CleanUp();
        }

            Debug.Log("¶¬");

        for(int i = 0;i < OutputData.equipData.Count; i++)
        {
            Debug.Log(OutputData.equipData[i].EquipName);
            var createObject = Instantiate(PrefabUI,this.transform);
            createObject.GetComponent<InventoryUI>().UpdateUI(OutputData.equipData[i].EquipIcom);
        }
    }

}
