using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Ore", menuName = "ScriptableObjects/Data/Blocks/New Ore")]
public class Ore : Block
{
	public int attackPower;
	public int[] weightPerSize;
	public int[] endurancePerSize;
	[ShowAssetPreview] public Sprite[] oreSprites;
}
