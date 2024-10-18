using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;

public enum BlockType
{
    None,
    Sand,
    Mud,
    Stone,
    Liquid,
    ExpCrystal,
    Ruby,
    Emerald,
    Crystal,
    HealOre,
    SandBack
}

public enum LiquidType
{
    Water,
    Lava
}

[CreateAssetMenu(fileName = "New Block", menuName = "ScriptableObjects/Datas/New Block")]
public class Block : ScriptableObject
{
    public BlockType type;
    public string id;
    public string name;
    public TileBase tile;
    public StratumGeologyData[] stratumGeologyDatas;
    public int weight;
    [ShowAssetPreview] public Sprite sprite;
    [ShowAssetPreview]
    public Sprite resultSprite;
    public int endurance;
    public int price;
    public Material material;
    public int AddAmount;
    public int RemoveAmount;

    [HideInInspector] public List<Vector3Int> tilePositions = new();
    
    public StratumGeologyData GetStratumGeologyData(int depth)
    {
        return stratumGeologyDatas.FirstOrDefault(stratumGeologyData => stratumGeologyData.depth == depth);
    }
}

[Serializable]
public class StratumGeologyData
{
    [SerializeField] private string stratumName;
    public int depth;
    public Color color;
}