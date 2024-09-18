using UnityEngine;
using UnityEngine.Tilemaps;

public interface IChunkInformation
{
    /// <summary>
    /// 指定の座標が位置している座標を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>チャンクのタイルマップオブジェクト</returns>
    public Tilemap GetChunk(Vector2 position);

    /// <summary>
    /// 指定の座標が位置している座標を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>チャンクのタイルマップオブジェクト</returns>
    public Tilemap GetChunk(Vector2 position, Vector2Int chunkVector);

    /// <summary>
    /// 指定の座標の層番号を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>層の番号(上が0)</returns>
    public int GetLayer(Vector2 position);
}