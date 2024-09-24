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
        private LayerGenerator _layer;
        private CancellationTokenSource _cancelTokenSource;
        private Vector2 _tilemapOrigin;
        private ManagedRandom _randomization;
        private Chunk[,] _chunks;
        private IWorldGeneratable[] _worldGenerators =
        {
            new LayerGenerator(),
            new CaveGenerator(),
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
            _cancelTokenSource = new();

            // 生成処理
            Initalize();
            GenerateAll(_cancelTokenSource.Token);
        }

        [ContextMenu("Refresh")]
        private void Regenerate()
        {
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

                    _chunks[x, y] = new Chunk
                    (
                        // TODO: タイルマップの左下の座標を求める計算をする
                        _randomization,
                        new Vector2Int(x, y),
                        tilemap.GetComponent<Tilemap>(),
                        new int[worldMap.OneChunkSize.x, worldMap.OneChunkSize.y]
                    );

                    TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
                    worldMap.OneChunkSize = new((int)renderer.chunkCullingBounds.x, (int)renderer.chunkCullingBounds.y);

                    // 次のX座標を設定
                    origin.x += worldMap.OneChunkSize.x;
                }

                // 次の座標を設定
                origin.x = _tilemapOrigin.x;
                origin.y += worldMap.OneChunkSize.y;
            }
            // 次の乱数に合わせるために空の乱数を生成しておく
            _randomization.Range(0, 0);

            Debug.Log($"<color=#00ff00ff>初期化処理完了</color>");
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
            for (int y = 0; y < _chunks.GetLength(0); y++)
            {
                for (int x = 0; x < _chunks.GetLength(1); x++)
                {
                    foreach (IWorldGeneratable worldGenerator in _worldGenerators)
                    {
                        worldGenerator.ExecutionOrder = _randomization.UsageCount;
                        _chunks[x, y] = await worldGenerator.Execute(_chunks[x, y], worldMap, token);

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