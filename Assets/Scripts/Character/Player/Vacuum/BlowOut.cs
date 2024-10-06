using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class BlowOut : MonoBehaviour
{
    [Header("Tile Config")]
    [SerializeField] private BlockDatas blockDatas;

    [Header("BlowOut Config")]
    [SerializeField] private BlockType blockType;
    [SerializeField] private Transform pivot;
    [SerializeField, Min(0f)] private float radius; // 吐き出し範囲
    [SerializeField, Min(0f)] private float distance; // 吐き出し距離（現状意味ない）
    [SerializeField, Min(0f)] private float range; // 吐き出し範囲の幅（現状意味ない）
    [SerializeField, Min(1f)] private float blowOutSpeed = 1;
    
    [Header("Instantiation Config")]
    [SerializeField] private float interval;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2Int generateTileCount;
    [SerializeField] private BlowOutOre blowOutOrePrefab;

    [Header("Debug Config")]
    [SerializeField] private bool debug;

    private float _weight;
    private float _lastUpdateTime;
    private PlayerMovement _playerMovement;
    private Camera _camera;
    private SuckUp _suckUp;
    private Parabola _parabola;
    private PlayerActions _playerActions;
    private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;
    private Tilemap _updateTilemap;
    private IInputTank inputTank;
    private IChunkInformation _chunkInformation;
    private ISoundSourceable _soundSource;

    public bool IsBlowOut { get; private set; }

    public void Inject(IInputTank inputTank)
    {
        this.inputTank = inputTank;
    }
    
    /// <summary>
    /// 吐き出し対象のTilemapを設定する
    /// </summary>
    /// <param name="tilemap">吐き出し対象のTilemap</param>
    public void SetTilemap(Tilemap tilemap)
    {
        _updateTilemap = tilemap;
    }

    private void Awake()
    {
        _playerActions = new PlayerActions();

        _suckUp = GetComponent<SuckUp>();
        _parabola = GetComponentInParent<Parabola>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Start()
    {
        _lastUpdateTime = 0f;

        VacuumActions.SpittingOut.started += _ => _playerMovement.IsMoveFlip = false;
        VacuumActions.SpittingOut.canceled += _ => CancelBlowOut();
    }

    private void Update()
    {
        blockType = inputTank.GetSelectType();
        
        if (blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
        {
            _parabola.GenerateParabola();
        }
        else
        {
            _parabola.DestroyParabola();
        }
        
        _lastUpdateTime -= Time.deltaTime;
        if (VacuumActions.SpittingOut.IsPressed() && !_suckUp.IsSuckUp)
        {
            IsBlowOut = true;
            BlowOutTiles();
        }
    }

    private void BlowOutTiles()
    {
        if (blockType != BlockType.None)
        {
            if (blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
            {
                _weight = blockDatas.GetOre(blockType).weightPerSize[0] * 10;
            }
            else
            {
                _weight = blockDatas.GetBlock(blockType).weight;
            }
        
            if (_lastUpdateTime <= 0f)
            {
                if (blockType == BlockType.Liquid) { return; }

                _lastUpdateTime = interval * _weight;
                if (inputTank.FiringTank())
                {
                    // TODO: ［効果音］吐き出し
                    GenerateTile();
                }
            }
        }

        UpdateTiles();
    }

    private void GenerateTile()
    {
        if (_camera == null)
        {
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        
        var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        var centerCell = (Vector3)_updateTilemap.WorldToCell(mouseWorldPosition);

        var direction = centerCell - pivot.position;
        var angle = Mathf.Atan2(direction.y, direction.x);

        var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
        var angle2 = Mathf.Acos(distance / chordLength);
        var newCell1 = GetNewCell(angle - angle2, chordLength);
        var newCell2 = GetNewCell(angle + angle2, chordLength);
        
        // TODO: blockTypeが鉱石かどうかの判定
        if (blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
        {
            var position = distance * direction.normalized + pivot.position;
            var blowOutOre = Instantiate(blowOutOrePrefab, position, Quaternion.identity);
            blowOutOre.gameObject.SetActive(true);
            var ore = blockDatas.GetOre(blockType);
            blowOutOre.SetOre(ore.attackPower, ore.weightPerSize[0], direction.normalized, ore.oreSprites[0]);
            inputTank.RemoveTank();
        }
        else
        {
            var randomGenerateTileCount = Random.Range(generateTileCount.x, generateTileCount.y);
            for (var i = 0; i < randomGenerateTileCount; i++)
            {
                var randomX = Random.Range(newCell1.x, newCell2.x);
                var randomY = Random.Range(newCell1.y, newCell2.y);
                var randomPosition = new Vector3(randomX, randomY, 0);
                var randomCell = _updateTilemap.WorldToCell(randomPosition);
                var mapTilemap = _chunkInformation.GetChunkTilemap(new Vector2(randomCell.x, randomCell.y));
                if (mapTilemap == null) { continue; }
                
                var localPosition = _chunkInformation.WorldToChunk(new Vector2(randomCell.x, randomCell.y));
                if (_updateTilemap.HasTile(randomCell) || mapTilemap.HasTile(localPosition)) { continue; }
                
                _updateTilemap.SetTile(randomCell, blockDatas.GetBlock(blockType).tile);

                var tileLayer = _chunkInformation.GetLayer(new Vector2(randomCell.x, randomCell.y));
                var block = blockDatas.GetBlock(blockDatas.GetBlock(blockType).tile);
                if (block.GetStratumGeologyData(tileLayer) != null)
                {
                    _updateTilemap.SetColor(randomCell, block.GetStratumGeologyData(tileLayer).color);
                }
                
                inputTank.RemoveTank();
                _soundSource.InstantiateSound("BlowOut", randomPosition);
            }
        }
    }

    // 疑似吐き出し範囲
    private void UpdateTiles()
    {
        var bounds = new BoundsInt(_updateTilemap.WorldToCell(pivot.position) - new Vector3Int((int)radius, (int)radius, 0), new Vector3Int((int)radius * 2, (int)radius * 2, 1));
        
        var hasTile = false;
        Tilemap mapTilemap;
        var positions = new List<Vector3Int>();
        foreach (var position in bounds.allPositionsWithin)
        {
            var pos = new Vector2(position.x, position.y);
            mapTilemap = _chunkInformation.GetChunkTilemap(pos);
            if (mapTilemap == null) { continue; }
            
            var localPosition = _chunkInformation.WorldToChunk(pos);
            if (!mapTilemap.HasTile(localPosition) && !_updateTilemap.HasTile(position)) { continue; }

            hasTile = true;
            positions.Add(position);
        }
        if (!hasTile) { return; }
        
        positions = positions.OrderBy(_ => Random.value).ToList();
        foreach (var position in positions)
        {
            mapTilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (mapTilemap == null) { continue; }
            
            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;

            var direction1 = position - pivot.position;
            var direction2 = mouseWorldPosition - pivot.position;
            var angle = Vector3.Angle(direction1, direction2);

            var dis = Vector3.Distance(pivot.position, position);

            if (angle <= 30 && dis <= radius)
            {
                var newTilePosition = Vector3Int.RoundToInt(position + direction1.normalized * blowOutSpeed);
                var localNewTilePosition = _chunkInformation.WorldToChunk(new Vector2(newTilePosition.x, newTilePosition.y));
                var newMapTilemap = _chunkInformation.GetChunkTilemap(new Vector2(newTilePosition.x, newTilePosition.y));
                if (newMapTilemap == null) { continue; }
                if (_updateTilemap.HasTile(newTilePosition) || newMapTilemap.HasTile(localNewTilePosition)) { continue; }
                
                // 更新用タイルの移動
                if (_updateTilemap.HasTile(position))
                {
                    var tile = _updateTilemap.GetTile(position);
                    
                    _updateTilemap.SetTile(position, null);
                    _updateTilemap.SetTile(newTilePosition, tile);
                    
                    var tileLayer = _chunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
                    var blockStratumGeologyData = blockDatas.GetBlock(tile).GetStratumGeologyData(tileLayer);
                    if (blockStratumGeologyData != null)
                    {
                        _updateTilemap.SetColor(newTilePosition, blockStratumGeologyData.color);
                    }
                    
                    continue;
                }

                // マップタイルの移動（砂の場合更新用タイルに移行）
                var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
                if (mapTilemap.HasTile(localPosition))
                {
                    var tile = mapTilemap.GetTile(localPosition);
                    mapTilemap.SetTile(localPosition, null);
                    
                    if (blockDatas.GetBlock(tile).type == BlockType.Sand)
                    {
                        _updateTilemap.SetTile(newTilePosition, tile);
                        
                        var tileLayer = _chunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
                        var blockStratumGeologyData = blockDatas.GetBlock(tile).GetStratumGeologyData(tileLayer);
                        if (blockStratumGeologyData != null)
                        {
                            _updateTilemap.SetColor(newTilePosition, blockStratumGeologyData.color);
                        }
                    }
                    else
                    {
                        newMapTilemap.SetTile(localNewTilePosition, tile);
                        
                        if (blockDatas.GetBlock(tile).type is not (BlockType.Ruby or BlockType.Crystal or BlockType.Emerald))
                        {
                            var tileLayer = _chunkInformation.GetLayer(new Vector2(localNewTilePosition.x, localNewTilePosition.y));
                            var blockStratumGeologyData = blockDatas.GetBlock(blockDatas.GetBlock(blockType).tile).GetStratumGeologyData(tileLayer);
                            if (blockStratumGeologyData != null)
                            {
                                newMapTilemap.SetColor(localNewTilePosition, blockStratumGeologyData.color);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CancelBlowOut()
    {
        IsBlowOut = false;
        _playerMovement.IsMoveFlip = true;
    }

    private void OnDrawGizmos()
    {
        if (!debug) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivot.position, distance);

        var mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (mainCamera == null) { return; }

        var mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (_updateTilemap == null) { return; }

        var centerCell = (Vector3)_updateTilemap.WorldToCell(mouseWorldPosition);

        var direction = centerCell - pivot.position;

        var targetPosition = pivot.position + direction.normalized * distance;
        Gizmos.DrawLine(pivot.position, targetPosition);

        var angle = Mathf.Atan2(direction.y, direction.x);

        var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
        var angle2 = Mathf.Acos(distance / chordLength);
        var newCell1 = GetNewCell(angle - angle2, chordLength);
        var newCell2 = GetNewCell(angle + angle2, chordLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(newCell1, newCell2);
        Gizmos.DrawWireSphere(pivot.position, chordLength);

        var chordLength2 = Mathf.Sqrt(Mathf.Pow(0, 2) + Mathf.Pow(range, 2));
        var angle3 = Mathf.Acos(0 / chordLength2);
        var h = radius * (1 - Mathf.Cos(angle3));
        var y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(range, 2)) - (radius - h) - distance;

        var newX3 = y * Mathf.Cos(angle);
        var newY3 = y * Mathf.Sin(angle);
        var newDirection3 = new Vector3(newX3, newY3, 0);

        Gizmos.color = Color.yellow;
        var newCell3 = newCell1 + newDirection3;
        Gizmos.DrawLine(newCell1, newCell3);

        var newCell4 = newCell2 + newDirection3;
        Gizmos.DrawLine(newCell2, newCell4);
    }

    private Vector3 GetNewCell(float angle, float chordLength)
    {
        var newX = chordLength * Mathf.Cos(angle);
        var newY = chordLength * Mathf.Sin(angle);
        var newDirection = new Vector3(newX, newY, 0);
        var newCell = pivot.position + newDirection;
        return newCell;
    }

    private void OnEnable()
    {
        _playerActions.Enable();
        
        var worldMapManager = FindObjectOfType<WorldMapManager>();
        _chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
        
        var soundSource = FindObjectOfType<SoundSource>();
        _soundSource = soundSource.GetComponent<ISoundSourceable>();
        _soundSource.SetInstantiation("BlowOut");
        
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void OnDisable()
    {
        _playerActions.Disable();
    }
}
