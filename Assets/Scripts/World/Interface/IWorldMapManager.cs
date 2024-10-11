using UnityEngine;

public interface IWorldMapManager
{
    public void Initialize(GameChunk[,] chunks, Vector2Int oneChunkSize, Vector2 tilemapOrigin);
}