using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace WorldCreation.Temp
{
    public class TempWorldMapCreator : IWorldGeneratable
    {
        private int _executionOrder;

        public int ExecutionOrder => _executionOrder;

        public void Initalize(Chunk chunk, WorldMap worldMap, int executionOrder)
        {

        }

        public async UniTask<Chunk> Execute(Chunk chunk, WorldMap worldMap, CancellationToken token)
        {
            int[,] map = split(worldMap.GetMap(Random.Range(0, worldMap.GetMapLength())));

            // チャンクの存在する場所の番号を取得する
            for (int y = 0; y < chunk.GetChunkLength(1); y++)
            {
                for (int x = 0; x < chunk.GetChunkLength(0); x++)
                {
                    Vector2Int worldPosition = chunk.GetWorldPosition(x, y, worldMap.OneChunkSize);

                    // 範囲外であれば次のループへ移動する
                    if (map.GetLength(0) < worldPosition.x && map.GetLength(1) < worldPosition.y) { break; }

                    // TODO: 元のレイヤーの番号から参照するブロックを変更する
                    int blockID = map[worldPosition.x, worldPosition.y];

                    // 地層の影響を受けるIDを指定する新しい変数を追加して元の地層IDがそのIDに一致したら変更する
                    if (worldMap.LayerTileID.Contains(blockID) == true)
                    {
                        blockID += chunk.GetLayerIndex(x, y);
                    }

                    chunk.SetBlock(x, y, blockID);
                }
            }

            return await UniTask.RunOnThreadPool(() => chunk);
        }

        private int[,] split(string map)
        {
            string[] line = map.Split("\n");
            string[][] subdivision = new string[line.Length][];
            for (int i = 0; i < line.Length; i++)
            {
                subdivision[i] = line[i].Split(",");
            }

            int[,] tileIDs = new int[subdivision[0].Length, subdivision.Length];
            for (int y = 0; y < subdivision.Length; y++)
            {
                for (int x = 0; x < subdivision[0].Length; x++)
                {
                    if (tileIDs.GetLength(1) <= x - 1)
                    {
                        break;
                    }
                    Debug.Log($"{tileIDs[x, y]}");
                    tileIDs[x, y] = int.Parse(subdivision[y][x]);
                }
            }

            return tileIDs;
        }

        private string Generate()
        {
            string result = "";
            for (int y = 0; y < 720; y++)
            {
                for (int x = 0; x < 1280; x++)
                {
                    result += Random.Range(0, 3) + ",";
                }
                result += "\n";
            }
            return result;
        }
    }
}