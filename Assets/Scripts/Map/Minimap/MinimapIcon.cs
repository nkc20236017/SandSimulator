using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public enum MinimapIconType
{
	Player,
	Goal
}

[CreateAssetMenu(fileName = "MinimapIcon", menuName = "ScriptableObjects/MinimapIcon")]
public class MinimapIcon : ScriptableObject
{
	public MinimapIconData[] MinimapIconDatas;
	
	public Sprite GetIcon(MinimapIconType type)
	{
		return (from data in MinimapIconDatas where data.Type == type select data.Icon).FirstOrDefault();
	}
}

[Serializable]
public class MinimapIconData
{
	public MinimapIconType Type;
	[ShowAssetPreview] public Sprite Icon;
}
