using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class BlowOut : Vacuum
{
    [Header("BlowOut Config")]
    [SerializeField] private BlockType blockType;
    [SerializeField] private Transform pivot;
    [SerializeField, Min(0f)] private float radius; // 吐き出し範囲
    [SerializeField, Min(0f)] private float distance; // 吐き出し距離（現状意味ない）
    [SerializeField, Min(0f)] private float range; // 吐き出し範囲の幅（現状意味ない）
    [SerializeField, Min(1f)] private float blowOutSpeed = 1;
    [SerializeField, Min(0)] private int blowOutUp = 3;
    
    [Header("Instantiation Config")]
    [SerializeField] private float interval;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2Int generateTileCount;
    [SerializeField] private BlowOutOre blowOutOrePrefab;

    [Header("Debug Config")]
    [SerializeField] private bool debug;

    [SerializeField] private GameObject blowEffect;

    private float _weight;
    private float _lastUpdateTime;
    private List<Vector3Int> _blowOutTilesList = new();
    private PlayerMovement _playerMovement;
    private SuckUp _suckUp;
    private Parabola _parabola;
    private Tilemap _updateTilemap;
    private IInputTank inputTank;
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
            blowEffect.SetActive(true);
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
                _weight = _blockDatas.GetOre(blockType).weightPerSize[0] * 10;
            }
            else
            {
                _weight = _blockDatas.GetBlock(blockType).weight;
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
        var angle = Mathf.Atan2(Direction.y, Direction.x);

        var chordLength = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(range, 2));
        var angle2 = Mathf.Acos(distance / chordLength);
        var newCell1 = GetNewCell(angle - angle2, chordLength);
        var newCell2 = GetNewCell(angle + angle2, chordLength);
        
        // TODO: blockTypeが鉱石かどうかの判定
        if (blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
        {
            AudioManager.Instance.PlaySFX("SpitoutSE");
            var position = distance * Direction.normalized + pivot.position;
            var blowOutOre = Instantiate(blowOutOrePrefab, position, Quaternion.identity);
            blowOutOre.gameObject.SetActive(true);
            var ore = _blockDatas.GetOre(blockType);
            blowOutOre.SetOre(ore, Direction.normalized);
            inputTank.RemoveTank();
        }
        else
        {
            var randomGenerateTileCount = Random.Range(generateTileCount.x, generateTileCount.y);
            for (var i = 0; i < randomGenerateTileCount; i++)
            {
                var randomPosition = RandomPosition(newCell1, newCell2);
                var randomCell = _updateTilemap.WorldToCell(randomPosition);
                var mapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(randomCell.x, randomCell.y));
                if (mapTilemap == null) { continue; }
                
                var localPosition = _playerMovement.ChunkInformation.WorldToChunk(new Vector2(randomCell.x, randomCell.y));
                if (_updateTilemap.HasTile(randomCell) || mapTilemap.HasTile(localPosition)) { continue; }
                
                _updateTilemap.SetTile(randomCell, _blockDatas.GetBlock(blockType).tile);

                var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(randomCell.x, randomCell.y));
                var block = _blockDatas.GetBlock(_blockDatas.GetBlock(blockType).tile);
                if (block.GetStratumGeologyData(tileLayer) != null)
                {
                    _updateTilemap.SetColor(randomCell, block.GetStratumGeologyData(tileLayer).color);
                }
                
                inputTank.RemoveTank();
                _soundSource.InstantiateSound("BlowOut", randomPosition);
            }
        }
    }
    
    private Vector3 RandomPosition(Vector3 cellA, Vector3 cellB)
    {
        var index = Random.Range(0f, 1f);
        var randomLerpPosition = Vector3.Lerp(cellA, cellB, index);
        return randomLerpPosition;
    }

    // 疑似吐き出し範囲
    private void UpdateTiles()
    {
        _blowOutTilesList.Clear();
        var bounds = new BoundsInt(_updateTilemap.WorldToCell(pivot.position) - new Vector3Int((int)radius, (int)radius, 0), new Vector3Int((int)radius * 2, (int)radius * 2, 1));
        
        var hasTile = false;
        Tilemap mapTilemap;
        var positions = new List<Vector3Int>();
        foreach (var position in bounds.allPositionsWithin)
        {
            var pos = new Vector2(position.x, position.y);
            mapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(pos);
            if (mapTilemap == null) { continue; }
            
            var localPosition = _playerMovement.ChunkInformation.WorldToChunk(pos);
            if (!mapTilemap.HasTile(localPosition) && !_updateTilemap.HasTile(position)) { continue; }

            hasTile = true;
            positions.Add(position);
        }
        if (!hasTile) { return; }
        
        positions = positions.OrderBy(_ => Random.value).ToList();
        foreach (var position in positions)
        {
            mapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (mapTilemap == null) { continue; }
            
            var direction1 = position - pivot.position;
            var angle = Vector3.Angle(direction1, Direction);

            var dis = Vector3.Distance(pivot.position, position);
            if (angle <= 30 && dis <= radius)
            {
                _blowOutTilesList.Add(position);
            }
        }

        foreach (var position in _blowOutTilesList)
        {
            SetUpdateTile(position);
        }
    }

    private void SetUpdateTile(Vector3Int position)
    {
        var mapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
        if (mapTilemap == null) { return; }
        
        var direction = position - pivot.position;
        var newTilePosition = Vector3Int.RoundToInt(position + direction.normalized * blowOutSpeed) + Vector3Int.up * blowOutUp;
        var localNewTilePosition = _playerMovement.ChunkInformation.WorldToChunk(new Vector2(newTilePosition.x, newTilePosition.y));
        var newMapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(newTilePosition.x, newTilePosition.y));
        if (newMapTilemap == null) { return; }
        if (_updateTilemap.HasTile(newTilePosition) || newMapTilemap.HasTile(localNewTilePosition)) { return; }
                
        // 更新用タイルの移動
        if (_updateTilemap.HasTile(position))
        {
            var tile = _updateTilemap.GetTile(position);
                    
            _updateTilemap.SetTile(position, null);
            _updateTilemap.SetTile(newTilePosition, tile);
                    
            var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
            var blockStratumGeologyData = _blockDatas.GetBlock(tile).GetStratumGeologyData(tileLayer);
            if (blockStratumGeologyData != null)
            {
                _updateTilemap.SetColor(newTilePosition, blockStratumGeologyData.color);
            }
                    
            return;
        }

        // マップタイルの移動（砂の場合更新用タイルに移行）
        var localPosition = _playerMovement.ChunkInformation.WorldToChunk(new Vector2(position.x, position.y));
        if (mapTilemap.HasTile(localPosition))
        {
            var tile = mapTilemap.GetTile(localPosition);
            mapTilemap.SetTile(localPosition, null);
                    
            if (_blockDatas.GetBlock(tile).type == BlockType.Sand)
            {
                _updateTilemap.SetTile(newTilePosition, tile);
                        
                var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
                var blockStratumGeologyData = _blockDatas.GetBlock(tile).GetStratumGeologyData(tileLayer);
                if (blockStratumGeologyData != null)
                {
                    _updateTilemap.SetColor(newTilePosition, blockStratumGeologyData.color);
                }
            }
            else
            {
                newMapTilemap.SetTile(localNewTilePosition, tile);
                        
                if (_blockDatas.GetBlock(tile).type is not (BlockType.Ruby or BlockType.Crystal or BlockType.Emerald))
                {
                    var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(localNewTilePosition.x, localNewTilePosition.y));
                    var blockStratumGeologyData = _blockDatas.GetBlock(_blockDatas.GetBlock(blockType).tile).GetStratumGeologyData(tileLayer);
                    if (blockStratumGeologyData != null)
                    {
                        newMapTilemap.SetColor(localNewTilePosition, blockStratumGeologyData.color);
                    }
                }
            }
        }
    }

    private void CancelBlowOut()
    {
        blowEffect.SetActive(false);
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

        var targetPosition = pivot.position + Direction.normalized * distance;
        Gizmos.DrawLine(pivot.position, targetPosition);

        var angle = Mathf.Atan2(Direction.y, Direction.x);

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
}
