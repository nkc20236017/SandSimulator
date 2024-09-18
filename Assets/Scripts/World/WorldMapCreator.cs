using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: 層の取得

namespace WorldCreation
{
    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private int seed;   // シード値
        public int Seed => seed;
        [SerializeField]    // 
        private WorldMap worldMap;
        [SerializeField]
        private Transform tilemapParent;
        [SerializeField]
        private GameObject tilemapPrefab;
        [SerializeField]
        private GameObject worldMapManagerPrefab;


        private bool _isQuitting;
        private LayerGenerate _layer;
        private CancellationTokenSource _cancelTokenSource;
        private Vector2 _tilemapOrigin;
        private ManagedRandom _randomization;
        private Chunk[,] _chunks;
        private IWorldGeneratable[] _worldGenerators =
        {
            new ChunkLoader()
        };

        private void Start()
        {
            if (seed == 0)
            {
                // seedが設定されていなければ新規で生成する
                seed = Random.Range(0, int.MaxValue);
            }

            _randomization = new(seed);

            // 初期化
            _layer = new(_randomization.Range(0, 0));
            _cancelTokenSource = new();

            // 生成処理
            Initalize();
            GenerateAll(_cancelTokenSource.Token);
        }

        private void Initalize()
        {
            // チャンクアレイの範囲を決める
            Vector2Int split = new Vector2Int
            (
                worldMap.WorldSize.x / worldMap.OneChunkSize.x,
                worldMap.WorldSize.y / worldMap.OneChunkSize.y
            );

            _chunks = new Chunk[split.x, split.y];

            // チャンクの生成原点を取得
            Vector2 origin = new
            (
                _randomization.Range(worldMap.MinOriginGapRange.x, worldMap.MaxOriginGapRange.x),
                _randomization.Range(worldMap.MinOriginGapRange.y, worldMap.MaxOriginGapRange.y)
            );
            _tilemapOrigin = origin;

            // チャンク単位の座標を計算し、生成
            for (int y = 0; y < split.y; y++)
            {
                for (int x = 0; x < split.x; x++)
                {
                    // チャンクを生成
                    GameObject tilemap = Instantiate(tilemapPrefab, origin, Quaternion.identity, tilemapParent);

                    _chunks[x, y] = new Chunk(_randomization, tilemap.GetComponent<Tilemap>());

                    // 次のX座標を設定
                    origin.x += worldMap.OneChunkSize.x;
                }

                // 次の座標を設定
                origin.x = _tilemapOrigin.x;
                origin.y += worldMap.OneChunkSize.y;
            }
        }

        private void OnApplicationQuit()
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _isQuitting = true;
        }


        private async void GenerateAll(CancellationToken token)
        {
            // 全てのチャンクを読み込む
            for (int x = 0; x < _chunks.GetLength(0); x++)
            {
                for (int y = 0; y < _chunks.GetLength(1); y++)
                {
                    foreach (IWorldGeneratable worldGenerator in _worldGenerators)
                    {
                        _chunks[x, y] = await worldGenerator.Execute(_chunks[x, y], worldMap, token);
                        worldGenerator.ExecutionOrder = _randomization.UsageCount;
                        Debug.Log($"<color=#00ff00ff>{worldGenerator}の処理終了</color>");
                    }
                }
            }

            GameObject worldMapManager = Instantiate(worldMapManagerPrefab);
            worldMapManager.GetComponent<IWorldMapManager>()
                .Initialize(_chunks, worldMap.OneChunkSize, _tilemapOrigin);
            Debug.Log($"<color=#ffff00ff>WorldMapManagerの生成完了</color>");
        }
    }
}