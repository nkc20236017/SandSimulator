using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
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
        private LayerMask oreTouchLayer;
        [SerializeField]
        private Ore[] oreData;
        [SerializeField]
        private Transform tilemapParent;
        [SerializeField]
        private GameObject tilemapPrefab;
        [SerializeField]
        private GameObject worldMapManagerPrefab;
        [SerializeField]
        private UnityEvent onWorldReady;
        public UnityEvent OnWorldReady => onWorldReady;


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
            // new OreGenerator(),
            // new TempWorldMapCreator(),
            new ChunkLoader()
        };

        Vector2[] directions =
            {
                new(-1, 1),
                new(0, 1),
                new(1, 1),
                new(1, 0),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 0),
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
            TilemapRenderer renderer = tilemapPrefab.GetComponent<TilemapRenderer>();
            worldMap.OneChunkSize = new((int)renderer.chunkCullingBounds.x, (int)renderer.chunkCullingBounds.y);

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
                    // 名前をチャンクの番号にする
                    tilemap.name = $"{tilemap.name} ({x}, {y})";

                    _chunks[x, y] = new Chunk
                    (
                        // TODO: タイルマップの左下の座標を求める計算をする
                        _randomization,
                        new Vector2Int(x, y),
                        tilemap.GetComponent<Tilemap>(),
                        new int[worldMap.OneChunkSize.x, worldMap.OneChunkSize.y]
                    );

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
                        worldGenerator.Initalize(_chunks[x, y], worldMap, _randomization.UsageCount);
                        _chunks[x, y] = await worldGenerator.Execute(_chunks[x, y], worldMap, token);

                    }
                }
            }

            // 鉱石生成
            Vector2Int[] noisePoints = BlueNoise(worldMap.WorldSize.x, worldMap.WorldSize.y, seed, 0);

            foreach (Vector2Int noisePoint in noisePoints)
            {
                if (Random.Range(0, 100) > worldMap.WorldLayers[0].PrimevalOres[0].Probability)
                {
                    continue;
                }
                int chunkX = noisePoint.x / worldMap.OneChunkSize.x;
                int chunkY = noisePoint.y / worldMap.OneChunkSize.y;
                Vector2Int withinChunkPosition
                    = _chunks[chunkX, chunkY].GetWithinChunkPosition(noisePoint);
                int id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);
                if (id == 0)
                {
                    // 空気中なら鉱石のオブジェクトを設置
                    ExposedOreProcess(noisePoint);
                }
                else
                {
                    BuriedOreProcess(noisePoint);
                }
            }

            GameObject worldMapManager = Instantiate(worldMapManagerPrefab);
            worldMapManager.GetComponent<IWorldMapManager>()
                .Initialize(_chunks, worldMap.OneChunkSize, _tilemapOrigin);
            onWorldReady?.Invoke();

            Debug.Log($"<color=#ffff00ff>WorldMapManagerの生成完了</color>");
        }

        /// <summary>
        /// 地上に生成される鉱石の処理
        /// </summary>
        private void ExposedOreProcess(Vector2 spownPoint)
        {
            // 生成位置から8方位へrayを飛ばし、一番近い点を求める
            RaycastHit2D nearest = default;
            for (int i = 0; i < directions.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(spownPoint, directions[i], Mathf.Infinity, oreTouchLayer);
                float currentDistance = (hit.point - spownPoint).magnitude;
                float nearestDistance = (nearest.point - spownPoint).magnitude;
                if (nearest == default || currentDistance < nearestDistance)
                {
                    // 今回のほうが近いところに当たれば最寄りを今回のものにする
                    nearest = hit;
                }
            }

            // 最寄りが存在しなければ終了
            if (nearest == default) { return; }

            // 初期値
            Ore ore = oreData[Random.Range(0, oreData.Length)];
            int size = Random.Range(1, 4);
            float angle = 0;

            // TODO: ここのリファクタリング
            // 0, 1
            if ((-0.5f < nearest.normal.x && nearest.normal.x < 0.5f) && 0.5f < nearest.normal.y)
            {
                angle = 0;
            }
            // 1, 0
            else if (0.5f < nearest.normal.x && (-0.5f < nearest.normal.y && nearest.normal.y < 0.5f))
            {
                angle = 270;
            }
            // 0, -1
            else if ((-0.5f < nearest.normal.x && nearest.normal.x < 0.5f) && nearest.normal.y < -0.5f)
            {
                angle = 180;
            }
            // -1, 0
            else if (nearest.normal.x < -0.5f && (-0.5f < nearest.normal.y && nearest.normal.y < 0.5f))
            {
                angle = 90;
            }
            else
            {
                // 鉱石を設置しない
                return;
            }

            // 鉱石を設置する
            GameObject substanceOre = Instantiate
            (
                worldMap.WorldLayers[0].PrimevalOres[0].ExposedOrePrefab,
                nearest.point,
                Quaternion.identity
            );

            OreObject oreObject;
            if (substanceOre.TryGetComponent(out oreObject))
            {
                // 初期データをセット
                oreObject.SetOre(ore, size, angle);
                onWorldReady.AddListener(oreObject.OnEnable);
            }
        }

        private void BuriedOreProcess(Vector2Int spownPoint)
        {
            PrimevalOre ore = worldMap.WorldLayers[0].PrimevalOres[0];
            int lumpRadius = ore.BlockAmount + Random.Range(0, ore.BlockAmount);
            Vector2Int[] circulePoints = GenerateCircularGrid(lumpRadius, spownPoint);

            int chance = 80;
            for (int i = 0; i < circulePoints.Length; i++)
            {
                Vector2Int point = circulePoints[i];
                // マップの外なら次の場所へ
                bool isOrverX = point.x < 0 || worldMap.WorldSize.x < point.x;
                bool isOrverY = point.y < 0 || worldMap.WorldSize.y < point.y;
                if (isOrverX || isOrverY) { continue; }

                int chunkX = point.x / worldMap.OneChunkSize.x;
                int chunkY = point.y / worldMap.OneChunkSize.y;
                chunkX = Mathf.Clamp(chunkX, 0, _chunks.GetLength(0) - 1);
                chunkY = Mathf.Clamp(chunkY, 0, _chunks.GetLength(1) - 1);
                Vector2Int withinChunkPosition
                    = _chunks[chunkX, chunkY].GetWithinChunkPosition(point);
                int id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);

                // 空間であれば設置しない
                if (id == 0) { continue; }

                if (Random.Range(0, 100) < chance)
                {
                    // 地中なら鉱石のタイルマップを配置
                    _chunks[chunkX, chunkY].TileMap.SetTile
                    (
                        (Vector3Int)withinChunkPosition,
                        worldMap.Blocks.GetBlock(worldMap.WorldLayers[0].PrimevalOres[0].BuriedOreID)
                    );
                }
            }
        }

        public Vector2Int[] GenerateCircularGrid(int radius, Vector2Int center)
        {
            List<Vector2Int> circularGrid = new List<Vector2Int>();

            for (int x = center.x - radius; x <= center.x + radius; x++)
            {
                for (int y = center.y - radius; y <= center.y + radius; y++)
                {
                    Vector2Int point = new Vector2Int(x, y);
                    Vector2Int relativePoint = point - center;
                    if (relativePoint.sqrMagnitude <= radius * radius)
                    {
                        circularGrid.Add(point);
                    }
                }
            }

            return circularGrid.ToArray();
        }

        public Vector2Int[] BlueNoise(int width, int height, int seed, int oreNum)
        {
            System.Random random = new System.Random(seed);
            bool[,] grid = new bool[width, height];
            List<Vector2Int> points = new List<Vector2Int>();
            List<Vector2Int> activeList = new List<Vector2Int>();

            // 最初のポイントをランダムに配置
            Vector2Int firstPoint = new Vector2Int(random.Next(width), random.Next(height));
            activeList.Add(firstPoint);
            points.Add(firstPoint);
            grid[firstPoint.x, firstPoint.y] = true;

            while (activeList.Count > 0)
            {
                int index = random.Next(activeList.Count);
                Vector2Int point = activeList[index];

                bool foundValidPoint = false;
                PrimevalOre ore = worldMap.WorldLayers[0].PrimevalOres[oreNum]; ;

                for (int i = 0; i < 30; i++) // 30回試行
                {
                    float angle = (float)random.NextDouble() * Mathf.PI * 2;
                    float distance = ore.Space + (float)random.NextDouble() * ore.Space;
                    Vector2Int newPoint = new Vector2Int(
                        Mathf.RoundToInt(point.x + Mathf.Cos(angle) * distance),
                        Mathf.RoundToInt(point.y + Mathf.Sin(angle) * distance)
                    );

                    if (IsValidPoint(newPoint, width, height, grid, ore.Space))
                    {
                        grid[newPoint.x, newPoint.y] = true;
                        activeList.Add(newPoint);
                        points.Add(newPoint);
                        foundValidPoint = true;
                        break;
                    }
                }

                if (!foundValidPoint)
                {
                    activeList.RemoveAt(index);
                }
            }

            return points.ToArray();
        }

        private bool IsValidPoint(Vector2Int point, int width, int height, bool[,] grid, int space)
        {
            if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                return false;

            if (grid[point.x, point.y])
                return false;

            for (int x = Mathf.Max(0, point.x - space); x < Mathf.Min(width, point.x + space + 1); x++)
            {
                for (int y = Mathf.Max(0, point.y - space); y < Mathf.Min(height, point.y + space + 1); y++)
                {
                    if (grid[x, y])
                    {
                        float distance = Vector2Int.Distance(new Vector2Int(x, y), point);
                        if (distance < space)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}