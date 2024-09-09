using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Ore")]
public class Ore : ScriptableObject
{
	[Header("Item Config")]
	public string ID;
	public string Name;
	[ShowAssetPreview] public Sprite Icon;
	[TextArea] public string Description;
	
	[Header("Ore Config")]
	public int AttackPower;
	public int Weight;
}
