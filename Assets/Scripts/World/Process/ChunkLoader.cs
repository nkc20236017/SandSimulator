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
            for (int y = 0; y < worldMap.OneChunkSize.y; y++)
            {
                for (int x = 0; x < worldMap.OneChunkSize.x; x++)
                {
                    chunk.TileMap.SetTile
                    (
                        new Vector3Int(x, y),
                        worldMap.WorldLayers[0].MaterialTile
                    );
                }
            }

            return await UniTask.RunOnThreadPool(() => chunk);
        }
    }
}