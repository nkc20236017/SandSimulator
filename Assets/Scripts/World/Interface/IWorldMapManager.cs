using UnityEngine;

public interface IWorldMapManager
{
    public void Initialize(Chunk[,] chunks, Vector2Int oneChunkSize, Vector2 tilemapOrigin);
}