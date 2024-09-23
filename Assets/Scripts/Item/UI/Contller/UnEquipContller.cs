using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnEquipContller : MonoBehaviour , IPointerDownHandler
{
    [SerializeField]
    private EquipData EquipData;
    private InputCommand inputCommand;

    private void Start()
    {
        inputCommand = GetComponentInParent<InputCommand>();
    }

    public void InjectEquip(EquipData equipData)
    {
        EquipData = equipData;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (EquipData == null) { return; }

        inputCommand.UnEquipCommand(EquipData);
        EquipData = null;
    }
}
