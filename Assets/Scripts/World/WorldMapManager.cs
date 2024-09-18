using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class WorldMapManager : MonoBehaviour, IChunkInformation, IWorldMapManager
{
    private Chunk[,] _chunks;
    private Vector2Int _oneChunkSize;
    private Vector2 _tilemapOrigin;

    void IWorldMapManager.Initialize(Chunk[,] chunks, Vector2Int oneChunkSize, Vector2 tilemapOrigin)
    {
        _chunks = chunks;
        _oneChunkSize = oneChunkSize;
        _tilemapOrigin = tilemapOrigin;
    }

    public Tilemap GetChunk(Vector2 position)
    {
        return GetChunk(position, Vector2Int.zero);
    }

    public Tilemap GetChunk(Vector2 position, Vector2Int chunkVector)
    {
        // 原点からの距離を求める
        Vector2 originDistance = position - _tilemapOrigin;

        // 差がマイナスであればエラーとする
        if (originDistance.x < 0 || originDistance.y < 0)
        {
            return null;
        }
        Vector2Int splitedVector = new
        (
            (int)(originDistance.x / _oneChunkSize.x),
            (int)(originDistance.y / _oneChunkSize.y)
        );

        // チャンクが存在すれば
        Tilemap result;
        try
        {
            result
               = _chunks[splitedVector.x + chunkVector.x, splitedVector.y + chunkVector.y].TileMap;
        }
        catch
        {
            result = null;
        }

        return result;
    }

    public int GetLayer(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}