using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New ExpCrystal", menuName = "ScriptableObjects/Data/Blocks/New ExpCrystal")]
public class ExpCrystal : Block
{
	public int expAmount;
	public int[] weightPerSize;
	public int[] endurancePerSize;
	[ShowAssetPreview] public Sprite[] oreSprites;
}
