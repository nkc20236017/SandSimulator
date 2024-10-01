using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

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
        private WorldMap backgroundWorldMap;
        [SerializeField]
        private LayerMask touchLayer;
        [SerializeField]
        private Transform tilemapParent;
        [SerializeField]
        private GameObject tilemapPrefab;
        [SerializeField]
        private GameObject worldMapManagerPrefab;
        [Space]
        [SerializeField]
        private MainGameEntoryPoint entoryPoint;
        [SerializeField]
        private GameObject orePrefab;
        [SerializeField]
        private CameraSystem mainCameraSystem;
        [SerializeField]
        private CameraSystem mapCameraSystem;
        [SerializeField]
        private Minimap minimap;
        [Space]
        [SerializeField]
        private GameObject startObject;
        [SerializeField]
        private UpdateTile updateTilemapPrefab;
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private Vector2 startPosition;
        [SerializeField]
        private ExitGateObject goalObject;
        [SerializeField]
        private Vector2 goalPosition;
        [SerializeField]
        private int structureRadius;

        (Ore ore, int size, float angle) setOreData;

        private bool _isQuitting;
        private LayerGenerator _layer;
        private CancellationTokenSource _cancelTokenSource;
        private List<GameObject> activeStanbyObject = new();
        private Vector2 _tilemapOrigin;
        private ManagedRandom _randomization;
        private Chunk[,] _chunks;
        private Chunk[,] _backgroundChunks;
        private IInputTank inputTank;
        private IGameLoad gameLoad;
        private IWorldGeneratable[] _worldGenerators =
        {
            new LayerGenerator(),
            new CaveGenerator(),
            // new OreGenerator(),
            // new TempWorldMapCreator(),
            new ChunkLoader()
        };
        private IWorldGeneratable[] _backgroundGenerators =
        {
            new LayerGenerator(),
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
            _backgroundChunks = new Chunk[split.x, split.y];

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
                        _randomization,
                        new Vector2Int(x, y),
                        tilemap.GetComponent<Tilemap>(),
                        new int[worldMap.OneChunkSize.x, worldMap.OneChunkSize.y]
                    );

                    _backgroundChunks[x, y] = new Chunk
                    (
                        _randomization,
                        new Vector2Int(x, y),
                        tilemap.transform.Find("Background").gameObject.GetComponent<Tilemap>(),
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
            entoryPoint.SetProgress(new(0, "0%", "世界を生成します..."));
            int initalUsageCount = _randomization.UsageCount;
            // 全てのチャンクを読み込む
            for (int y = 0; y < _chunks.GetLength(1); y++)
            {
                for (int x = 0; x < _chunks.GetLength(0); x++)
                {
                    foreach (IWorldGeneratable worldGenerator in _worldGenerators)
                    {
                        worldGenerator.Initalize(_chunks[x, y], worldMap, _randomization.UsageCount);
                        _chunks[x, y] = await worldGenerator.Execute(_chunks[x, y], worldMap, token);

                    }

                    foreach (IWorldGeneratable backgourndGenerator in _backgroundGenerators)
                    {
                        backgourndGenerator.Initalize(_chunks[x, y], backgroundWorldMap, initalUsageCount);
                        _backgroundChunks[x, y] = await backgourndGenerator.Execute(_backgroundChunks[x, y], backgroundWorldMap, token);
                    }
                }
            }

            entoryPoint.SetProgress(new(0.7f, "70%", "鉱石を配置中..."));

            // 鉱石生成
            OreGenerate();

            entoryPoint.SetProgress(new(0.8f, "80%", "敵を配置中..."));

            // 敵召喚
            EnemyGenerate();

            entoryPoint.SetProgress(new(0.9f, "90%", "スポーン地点を整理中..."));

            // スタートとゴール召喚
            StructureGenerate();

            GameObject worldMapManager = Instantiate(worldMapManagerPrefab);
            worldMapManager.GetComponent<IWorldMapManager>()
                .Initialize(_chunks, worldMap.OneChunkSize, _tilemapOrigin);
            Debug.Log($"<color=#ffff00ff>WorldMapManagerの生成完了</color>");

            foreach (GameObject activateObject in activeStanbyObject)
            {
                activateObject.SetActive(true);

                // 鉱石の場合は初期設定をする
                OreObject oreObject;
                if (activateObject.TryGetComponent(out oreObject))
                {
                    // 初期データをセット
                    oreObject.SetOre(setOreData.ore, setOreData.size, setOreData.angle);
                }
            }

            entoryPoint.SetProgress(new(1f, "100%", "世界の生成が完了しました"));

        }

        private void EnemyGenerate()
        {
            Vector2Int[] noisePoints = BlueNoise(worldMap.WorldSize.x, worldMap.WorldSize.y, worldMap.EnemySpase, seed);

            foreach (Vector2Int noisePoint in noisePoints)
            {
                int chunkX = noisePoint.x / worldMap.OneChunkSize.x;
                int chunkY = noisePoint.y / worldMap.OneChunkSize.y;
                Vector2Int withinChunkPosition
                    = _chunks[chunkX, chunkY].GetWithinChunkPosition(noisePoint);
                int id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);
                bool hit = Physics2D.Raycast(noisePoint, Vector2.down, 10, touchLayer);
                if (id == 0 && hit)
                {
                    activeStanbyObject.Add(Instantiate(worldMap.EnemyPrefab, (Vector2)noisePoint, Quaternion.identity));
                }
            }
        }

        private void OreGenerate()
        {
            Vector2Int[] noisePoints = BlueNoise(worldMap.WorldSize.x, worldMap.WorldSize.y, worldMap.WorldLayers[0].PrimevalOres[0].Space, seed);

            foreach (Vector2Int noisePoint in noisePoints)
            {
                int oreIndex = Random.Range(0, worldMap.WorldLayers[0].PrimevalOres.Length);

                if (Random.Range(0, 100) > worldMap.WorldLayers[0].PrimevalOres[oreIndex].Probability)
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
                    ExposedOreProcess(noisePoint, oreIndex);
                }
                else
                {
                    BuriedOreProcess(noisePoint, oreIndex);
                }
            }
        }

        /// <summary>
        /// 地上に生成される鉱石の処理
        /// </summary>
        private void ExposedOreProcess(Vector2 spownPoint, int oreIndex)
        {
            // 生成位置から8方位へrayを飛ばし、一番近い点を求める
            RaycastHit2D nearest = default;
            for (int i = 0; i < directions.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(spownPoint, directions[i], Mathf.Infinity, touchLayer);
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
            Ore ore = worldMap.WorldLayers[0].PrimevalOres[oreIndex].ExposedOreData;
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
                orePrefab,
                _chunks[0, 0].TileMap.WorldToCell(nearest.point),
                Quaternion.identity
            );

            activeStanbyObject.Add(substanceOre);

            setOreData.ore = worldMap.WorldLayers[0].PrimevalOres[oreIndex].ExposedOreData;
            setOreData.size = Random.Range(1, 4);
            setOreData.angle = angle;
        }

        private void BuriedOreProcess(Vector2Int spownPoint, int oreIndex)
        {
            PrimevalOre ore = worldMap.WorldLayers[0].PrimevalOres[oreIndex];
            int lumpRadius = ore.BlockAmount + Random.Range(0, ore.BlockAmount);
            Vector2Int[] circulePoints = GenerateCircularGrid(lumpRadius, spownPoint);

            // 中心からの距離で昇順に並び替え
            circulePoints = circulePoints
                .OrderBy(point => (point - spownPoint).magnitude)
                .ToArray();

            float chance = 100;
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

                if (Random.Range(0f, 100f) < chance)
                {
                    // 地中なら鉱石のタイルマップを配置
                    _chunks[chunkX, chunkY].TileMap.SetTile
                    (
                        (Vector3Int)withinChunkPosition,
                        worldMap.Blocks.GetBlock(ore.BuriedOreID)
                    );

                    _chunks[chunkX, chunkY].TileMap.SetColor
                    (
                        (Vector3Int)withinChunkPosition,
                        UnityEngine.Color.white
                    );
                }

                chance -= circulePoints.Length / 100;
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

        private void StructureGenerate()
        {
            // スタート
            int chunkX = (int)startPosition.x / worldMap.OneChunkSize.x;
            int chunkY = (int)startPosition.y / worldMap.OneChunkSize.y;
            chunkX = Mathf.Clamp(chunkX, 0, _chunks.GetLength(0) - 1);
            chunkY = Mathf.Clamp(chunkY, 0, _chunks.GetLength(1) - 1);
            Vector2Int withinChunkPosition
                = _chunks[chunkX, chunkY].GetWithinChunkPosition(startPosition);
            int id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);

            Vector2Int center;
            Vector2 start;

            if (id == 0)
            {
                // 空中なら地面を探して設置
                RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, Mathf.Infinity, touchLayer);
                center = new((int)hit.point.x, (int)hit.point.y + structureRadius);
                start = hit.point;
            }
            else
            {
                center = new((int)startPosition.x, (int)startPosition.y + structureRadius);
                start = startPosition;
            }
            Vector2Int[] circulePoints = GenerateCircularGrid(structureRadius, center);

            foreach (Vector2Int circulePoint in circulePoints)
            {
                int circleX = circulePoint.x / worldMap.OneChunkSize.x;
                int circleY = circulePoint.y / worldMap.OneChunkSize.y;
                circleX = Mathf.Clamp(circleX, 0, _chunks.GetLength(0) - 1);
                circleY = Mathf.Clamp(circleY, 0, _chunks.GetLength(1) - 1);
                Vector2Int withinCirclePosition
                    = _chunks[chunkX, chunkY].GetWithinChunkPosition(circulePoint);

                _chunks[circleX, circleY].TileMap.SetTile
                (
                    (Vector3Int)withinCirclePosition,
                    null
                );
            }

            Instantiate(startObject, start, Quaternion.identity);
            // プレイヤーの設定
            GameObject player = Instantiate(playerPrefab, (Vector2)center, Quaternion.identity);
            UpdateTile updateTilemap = Instantiate(updateTilemapPrefab);
            updateTilemap.SetPlayer(player.transform);
            SuckUp suckUp = player.GetComponentInChildren<SuckUp>();
            BlowOut blowOut = player.GetComponentInChildren<BlowOut>();
            SelectTank select = player.GetComponent<SelectTank>();

            //blowOut.SetTilemap(updateTilemap.gameObject.GetComponent<Tilemap>());

            suckUp.Inject(inputTank);
            blowOut.Inject(inputTank);
            select.Inject(inputTank);

            activeStanbyObject.Add(player);
            activeStanbyObject.Add(updateTilemap.gameObject);

            mainCameraSystem.CameraConfig(player.transform, worldMap.WorldSize);

            mapCameraSystem.CameraConfig(player.transform, worldMap.WorldSize);
            minimap.AddTargetIcons(MinimapIconType.Player, player);

            // ゴール
            chunkX = (int)goalPosition.x / worldMap.OneChunkSize.x;
            chunkY = (int)goalPosition.y / worldMap.OneChunkSize.y;
            chunkX = Mathf.Clamp(chunkX, 0, _chunks.GetLength(0) - 1);
            chunkY = Mathf.Clamp(chunkY, 0, _chunks.GetLength(1) - 1);
            withinChunkPosition
                = _chunks[chunkX, chunkY].GetWithinChunkPosition(goalPosition);
            id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);

            if (id == 0)
            {
                // 空中なら地面を探して設置
                RaycastHit2D hit = Physics2D.Raycast(goalPosition, Vector2.down, Mathf.Infinity, touchLayer);
                if (!hit)
                {
                    hit = Physics2D.Raycast(goalPosition, Vector2.right, Mathf.Infinity, touchLayer);
                }
                if (!hit)
                {
                    hit = Physics2D.Raycast(goalPosition, Vector2.left, Mathf.Infinity, touchLayer);
                }
                start = hit.point;
            }
            else
            {
                start = goalPosition;
            }
            circulePoints = GenerateCircularGrid(structureRadius, center);

            foreach (Vector2Int circulePoint in circulePoints)
            {
                int circleX = circulePoint.x / worldMap.OneChunkSize.x;
                int circleY = circulePoint.y / worldMap.OneChunkSize.y;
                circleX = Mathf.Clamp(circleX, 0, _chunks.GetLength(0) - 1);
                circleY = Mathf.Clamp(circleY, 0, _chunks.GetLength(1) - 1);
                Vector2Int withinCirclePosition
                    = _chunks[chunkX, chunkY].GetWithinChunkPosition(circulePoint);

                _chunks[circleX, circleY].TileMap.SetTile
                (
                    (Vector3Int)withinCirclePosition,
                    null
                );
            }

            ExitGateObject exitGateObject = Instantiate(goalObject, start, Quaternion.identity);
            exitGateObject.Inject(gameLoad);

            minimap.AddTargetIcons(MinimapIconType.Goal, exitGateObject.gameObject);
        }

        private Vector2Int[] BlueNoise(int width, int height, int space, int seed)
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

                for (int i = 0; i < 30; i++) // 30回試行
                {
                    float angle = (float)random.NextDouble() * Mathf.PI * 2;
                    float distance = space + (float)random.NextDouble() * space;
                    Vector2Int newPoint = new Vector2Int(
                        Mathf.RoundToInt(point.x + Mathf.Cos(angle) * distance),
                        Mathf.RoundToInt(point.y + Mathf.Sin(angle) * distance)
                    );

                    if (IsValidPoint(newPoint, width, height, grid, space))
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

        [Inject]
        public void Inject(IInputTank inputTank, IGameLoad gameLoad)
        {
            this.inputTank = inputTank;
            this.gameLoad = gameLoad;
        }
    }
}