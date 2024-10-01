using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace WorldCreation
{
    public class ChunkLoader : IWorldGeneratable
    {
        private int _executionOrder;
        public int ExecutionOrder => _executionOrder;

        public void Initalize(Chunk chunk, WorldMap worldMap, int executionOrder)
        {
            _executionOrder = executionOrder;
        }

        async UniTask<Chunk> IWorldGeneratable.Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            int limitter = 0;
            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (chunk.GetBlockID(position) == 0)
                    {
                        continue;
                    }
                    chunk.TileMap.SetTile
                    (
                        (Vector3Int)position,
                        worldMap.Blocks.GetBlock(chunk.GetBlockID(position))
                    );

                    if (chunk.TileMap.GetTile((Vector3Int)position) != null)
                    {
                        chunk.TileMap.SetColor
                        (
                            (Vector3Int)position,
                            worldMap.WorldLayers[chunk.GetLayerIndex(x, y)].LayerColor
                        );
                    }

                    limitter++;

                    if (worldMap.FillLimit < limitter)
                    {
                        await UniTask.NextFrame(token).SuppressCancellationThrow();
                        limitter = 0;
                    }
                }
            }

            return await UniTask.RunOnThreadPool(() => chunk);
        }
    }
}