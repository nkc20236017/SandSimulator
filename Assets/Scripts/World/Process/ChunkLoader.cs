using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using UnityEngine;

// TODO: ���W����`�����N�擾
// TODO: Vector2Int�œn���ꂽ�ꏊ�̃`�����N�擾
// TODO: �w�̎擾


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