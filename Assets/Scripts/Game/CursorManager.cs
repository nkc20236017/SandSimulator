﻿using UnityEngine;

public class CursorManager : MonoBehaviour
{
	[Header("Cursor Config")]
	[SerializeField] private Cursor cursor;

	/// <summary>
	/// カーソルを設定
	/// </summary>
	/// <param name="cursorType">カーソルの種類</param>
	/// <param name="size">カーソルの大きさ</param>
	public void SetCursor(CursorType cursorType, Vector2Int size = default)
	{
		if (cursorType == CursorType.None)
		{
			UnityEngine.Cursor.visible = false;
			return;
		}
		
		foreach (var cursorData in cursor.cursorData)
		{
			if (cursorData.cursorType != cursorType) { continue; }

			UnityEngine.Cursor.visible = true;
			Texture2D cursorTexture = SetCursorSize(cursorData.cursorTexture, size);
			UnityEngine.Cursor.SetCursor(cursorTexture, cursorData.hotSpot, CursorMode.Auto);
			return;
		}
	}

	private static Texture2D SetCursorSize(Texture texture, Vector2Int size)
	{
		var resizedTexture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
		Graphics.ConvertTexture(texture, resizedTexture);
		return resizedTexture;
	}
}

