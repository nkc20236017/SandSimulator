using System;
using NaughtyAttributes;
using UnityEngine;

public enum CursorType
{
	None,
	Game,
	UI,
	OnButton,
}

[CreateAssetMenu(fileName = "Cursor", menuName = "ScriptableObjects/Cursor")]
public class Cursor : ScriptableObject
{
	public CursorData[] cursorData;
}

[Serializable]
public class CursorData
{
	public CursorType cursorType;
	[ShowAssetPreview] public Texture2D cursorTexture;
	public Vector2 hotSpot;
}