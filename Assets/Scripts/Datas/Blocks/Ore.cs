using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Ore", menuName = "ScriptableObjects/Datas/New Ore")]
public class Ore : Block
{
	public OreType oreType;
	public int attackPower;
	public int[] weightPerSize;
	public int[] endurancePerSize;
	[ShowAssetPreview] public Sprite[] oreSprites;
}
