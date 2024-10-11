using UnityEngine;

public interface ICursorable
{
	/// <summary>
	/// カーソルを設定
	/// </summary>
	/// <param name="cursorType">カーソルの種類</param>
	/// <param name="size">カーソルの大きさ</param>
	public void SetCursor(CursorType cursorType, Vector2Int size = default);
}
