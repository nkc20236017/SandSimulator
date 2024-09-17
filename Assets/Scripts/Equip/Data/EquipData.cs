using UnityEditor;
using UnityEngine;

public class EquipData : ScriptableObject
{
    public string EquipName;
    public string EquipId;
    public Sprite EquipIcom;
    public int EquipPrice;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        EquipId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
