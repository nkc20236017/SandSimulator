using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockType
{
    Sand,
    Mud,
    Stone,
    Ore,
    Liquid,
    ExpCrystal
}

public enum OreType
{
    Gurus,
    Crystal,
    Dynamism,
    Malakud,
    Prodect,
    Emerald,
    Natrum,
    Kdot,
    Diamond,
    Bismuth,
    MagicOre
}

public enum MagicOreType
{
    FlameStone,
    WaterStone,
    Thunderstone,
    IceStone
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
    public int endurance;
    public int price;

    [HideInInspector] public List<Vector3Int> tilePositions = new();
}

[Serializable]
public class StratumGeologyData
{
    [SerializeField] private string stratumName;
    public int depth;
    public Color colors;
}