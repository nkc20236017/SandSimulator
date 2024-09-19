using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using UnityEngine;

// TODO: 座標からチャンク取得
// TODO: Vector2Intで渡された場所のチャンク取得
// TODO: 層の取得


namespace WorldCreation
{
    public class ChunkLoader : IWorldGeneratable
    {
        private int _executionOrder;
        public int ExecutionOrder
        {
            get => _executionOrder;
            set => _executionOrder = value;
        }

        async UniTask<Chunk> IWorldGeneratable.Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            int limitter = 0;
            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (chunk.GetBlockID(position) == -1)
                    {
                        continue;
                    }
                    chunk.TileMap.SetTile
                    (
                        (Vector3Int)position,
                        worldMap.BlockList[chunk.GetBlockID(position)]
                    );

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