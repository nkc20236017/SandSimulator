using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New EquipmentData",menuName = "Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    public string EquipmentId;
    public string EquipmentName;
    public Sprite EquipmentSprite;
    public int EquipmentPrice;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        EquipmentId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
