using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;

// TODO: �w�̎擾

namespace WorldCreation
{
    public class WorldMapCreator : MonoBehaviour
    {
        [SerializeField]
        private int seed;   // �V�[�h�l
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
                // seed���ݒ肳��Ă��Ȃ���ΐV�K�Ő�������
                seed = Random.Range(0, int.MaxValue);
            }

            _randomization = new(seed);

            // ������
            _cancelTokenSource = new();

            // ��������
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

            // �`�����N�A���C�͈̔͂����߂�
            Vector2Int split = new Vector2Int
            (
                worldMap.WorldSize.x / worldMap.OneChunkSize.x,
                worldMap.WorldSize.y / worldMap.OneChunkSize.y
            );

            _chunks = new Chunk[split.x, split.y];
            _backgroundChunks = new Chunk[split.x, split.y];

            // �`�����N�̐������_���擾
            Vector2 origin = new
            (
                _randomization.Range(worldMap.MinOriginGapRange.x, worldMap.MaxOriginGapRange.x),
                _randomization.Range(worldMap.MinOriginGapRange.y, worldMap.MaxOriginGapRange.y)
            );
            _tilemapOrigin = origin;

            // �`�����N�P�ʂ̍��W���v�Z���A����
            for (int y = 0; y < split.y; y++)
            {
                for (int x = 0; x < split.x; x++)
                {
                    // �`�����N�𐶐�
                    GameObject tilemap = Instantiate(tilemapPrefab, origin, Quaternion.identity, tilemapParent);
                    // ���O���`�����N�̔ԍ��ɂ���
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

                    // ����X���W��ݒ�
                    origin.x += worldMap.OneChunkSize.x;
                }

                // ���̍��W��ݒ�
                origin.x = _tilemapOrigin.x;
                origin.y += worldMap.OneChunkSize.y;
            }
            // ���̗����ɍ��킹�邽�߂ɋ�̗����𐶐����Ă���
            _randomization.Range(0, 0);

            Debug.Log($"<color=#00ff00ff>��������������</color>");
        }

        private void OnApplicationQuit()
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _isQuitting = true;
        }

        private async void GenerateAll(CancellationToken token)
        {
            entoryPoint.SetProgress(new(0, "0%", "���E�𐶐����܂�..."));
            int initalUsageCount = _randomization.UsageCount;
            // �S�Ẵ`�����N��ǂݍ���
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

            entoryPoint.SetProgress(new(0.7f, "70%", "�z�΂�z�u��..."));

            // �z�ΐ���
            OreGenerate();

            entoryPoint.SetProgress(new(0.8f, "80%", "�G��z�u��..."));

            // �G����
            EnemyGenerate();

            entoryPoint.SetProgress(new(0.9f, "90%", "�X�|�[���n�_�𐮗���..."));

            // �X�^�[�g�ƃS�[������
            StructureGenerate();

            GameObject worldMapManager = Instantiate(worldMapManagerPrefab);
            worldMapManager.GetComponent<IWorldMapManager>()
                .Initialize(_chunks, worldMap.OneChunkSize, _tilemapOrigin);
            Debug.Log($"<color=#ffff00ff>WorldMapManager�̐�������</color>");

            foreach (GameObject activateObject in activeStanbyObject)
            {
                activateObject.SetActive(true);

                // �z�΂̏ꍇ�͏����ݒ������
                OreObject oreObject;
                if (activateObject.TryGetComponent(out oreObject))
                {
                    // �����f�[�^���Z�b�g
                    oreObject.SetOre(setOreData.ore, setOreData.size, setOreData.angle);
                }
            }

            entoryPoint.SetProgress(new(1f, "100%", "���E�̐������������܂���"));

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
                    // ��C���Ȃ�z�΂̃I�u�W�F�N�g��ݒu
                    ExposedOreProcess(noisePoint, oreIndex);
                }
                else
                {
                    BuriedOreProcess(noisePoint, oreIndex);
                }
            }
        }

        /// <summary>
        /// �n��ɐ��������z�΂̏���
        /// </summary>
        private void ExposedOreProcess(Vector2 spownPoint, int oreIndex)
        {
            // �����ʒu����8���ʂ�ray���΂��A��ԋ߂��_�����߂�
            RaycastHit2D nearest = default;
            for (int i = 0; i < directions.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(spownPoint, directions[i], Mathf.Infinity, touchLayer);
                float currentDistance = (hit.point - spownPoint).magnitude;
                float nearestDistance = (nearest.point - spownPoint).magnitude;
                if (nearest == default || currentDistance < nearestDistance)
                {
                    // ����̂ق����߂��Ƃ���ɓ�����΍Ŋ�������̂��̂ɂ���
                    nearest = hit;
                }
            }

            // �Ŋ�肪���݂��Ȃ���ΏI��
            if (nearest == default) { return; }

            // �����l
            Ore ore = worldMap.WorldLayers[0].PrimevalOres[oreIndex].ExposedOreData;
            int size = Random.Range(1, 4);
            float angle = 0;

            // TODO: �����̃��t�@�N�^�����O
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
                // �z�΂�ݒu���Ȃ�
                return;
            }

            // �z�΂�ݒu����
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

            // ���S����̋����ŏ����ɕ��ёւ�
            circulePoints = circulePoints
                .OrderBy(point => (point - spownPoint).magnitude)
                .ToArray();

            float chance = 100;
            for (int i = 0; i < circulePoints.Length; i++)
            {
                Vector2Int point = circulePoints[i];
                // �}�b�v�̊O�Ȃ玟�̏ꏊ��
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

                // ��Ԃł���ΐݒu���Ȃ�
                if (id == 0) { continue; }

                if (Random.Range(0f, 100f) < chance)
                {
                    // �n���Ȃ�z�΂̃^�C���}�b�v��z�u
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
            // �X�^�[�g
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
                // �󒆂Ȃ�n�ʂ�T���Đݒu
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
            // �v���C���[�̐ݒ�
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

            // �S�[��
            chunkX = (int)goalPosition.x / worldMap.OneChunkSize.x;
            chunkY = (int)goalPosition.y / worldMap.OneChunkSize.y;
            chunkX = Mathf.Clamp(chunkX, 0, _chunks.GetLength(0) - 1);
            chunkY = Mathf.Clamp(chunkY, 0, _chunks.GetLength(1) - 1);
            withinChunkPosition
                = _chunks[chunkX, chunkY].GetWithinChunkPosition(goalPosition);
            id = _chunks[chunkX, chunkY].GetBlockID(withinChunkPosition);

            if (id == 0)
            {
                // �󒆂Ȃ�n�ʂ�T���Đݒu
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

            // �ŏ��̃|�C���g�������_���ɔz�u
            Vector2Int firstPoint = new Vector2Int(random.Next(width), random.Next(height));
            activeList.Add(firstPoint);
            points.Add(firstPoint);
            grid[firstPoint.x, firstPoint.y] = true;

            while (activeList.Count > 0)
            {
                int index = random.Next(activeList.Count);
                Vector2Int point = activeList[index];

                bool foundValidPoint = false;

                for (int i = 0; i < 30; i++) // 30�񎎍s
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