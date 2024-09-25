using UnityEngine;
using UnityEngine.Tilemaps;

public interface IChunkInformation
{
    /// <summary>
    /// ワールド座標からチャンクの座標に変換します
    /// </summary>
    /// <param name="world"></param>
    /// <returns></returns>
    public Vector3Int WorldToChunk(Vector2 world);

    /// <summary>
    /// チャンクの座標からワールド座標に変換します
    /// </summary>
    /// <param name="chunkIndex">チャンクの座標</param>
    /// <param name="tilePosition">チャンク内のタイルの座標</param>
    /// <returns></returns>
    public Vector3 ChunkToWorld(Vector2Int chunkIndex, Vector3Int tilePosition);

    /// <summary>
    /// 指定の座標が位置している座標を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>チャンクのタイルマップオブジェクト</returns>
    public Tilemap GetChunkTilemap(Vector2 position);

    /// <summary>
    /// 指定の座標が位置している座標を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>チャンクのタイルマップオブジェクト</returns>
    public Tilemap GetChunkTilemap(Vector2 position, Vector2Int chunkVector);

    /// <summary>
    /// 指定の座標の層番号を取得する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <returns>層の番号(上が1)</returns>
    public int GetLayer(Vector2 position);
}