using UnityEngine;
using UnityEngine.Tilemaps;
using WorldCreation;

public class WorldMapManager : MonoBehaviour, IChunkInformation, IWorldMapManager
{
    private GameChunk[,] _chunks;
    private Vector2Int _oneChunkSize;
    private Vector2 _tilemapOrigin;

    void IWorldMapManager.Initialize(GameChunk[,] chunks, Vector2Int oneChunkSize, Vector2 tilemapOrigin)
    {
        _chunks = chunks;
        _oneChunkSize = oneChunkSize;
        _tilemapOrigin = tilemapOrigin;
    }

    Tilemap IChunkInformation.GetChunkTilemap(Vector2 position)
    {
        return GetChunkTilemap(position, Vector2Int.zero);
    }

    public Tilemap GetChunkTilemap(Vector2 position, Vector2Int chunkVector)
    {
        if (GetChunk(position, Vector2Int.zero) == null) { return null; }

        GameChunk chunk = GetChunk(position, Vector2Int.zero);

        return chunk.GameChunkTilemap;
    }

    public int GetLayer(Vector2 position)
    {
        Vector2Int gridPosition = (Vector2Int)WorldToChunk(position);

        if (GetChunk(position, Vector2Int.zero) == null)
        {
            return 0;
        }

        GameChunk chunk = GetChunk(position, Vector2Int.zero);

        return chunk.GetLayerIndex(gridPosition.x, gridPosition.y) + 1;
    }

    private GameChunk GetChunk(Vector2 position, Vector2Int chunkVector)
    {
        // 原点からの距離を求める
        Vector2 originDistance = position - _tilemapOrigin;

        // 差がマイナスであればエラーとする
        if (originDistance.x < 0 || originDistance.y < 0)
        {
            return null;
        }
        // 1チャンク
        Vector2Int splitedVector = new
        (
            (int)(originDistance.x / _oneChunkSize.x),
            (int)(originDistance.y / _oneChunkSize.y)
        );

        // チャンクが存在すればそれを返し、なければNull
        GameChunk result;
        try
        {
            result
               = _chunks[splitedVector.x + chunkVector.x, splitedVector.y + chunkVector.y];
        }
        catch
        {
            result = null;
        }

        return result;
    }

    public Vector3Int WorldToChunk(Vector2 world)
    {
        Vector2Int originWorldInt = new Vector2Int((int)world.x, (int)world.y) - new Vector2Int((int)_tilemapOrigin.x, (int)_tilemapOrigin.y);

        Vector2Int result = new(originWorldInt.x % _oneChunkSize.x, originWorldInt.y % _oneChunkSize.y);

        return (Vector3Int)result;
    }

    public Vector3 ChunkToWorld(Vector2Int chunkIndex, Vector3Int tilePosition)
    {
        Vector2Int worldPosition = _oneChunkSize * chunkIndex;

        Vector3Int result = (Vector3Int)worldPosition + tilePosition;

        return result + new Vector3Int((int)_tilemapOrigin.x, (int)_tilemapOrigin.y, 0);
    }
}