using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Eject : Vacuum
{
    [Header("Eject Config")]
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject blowEffect;
    
    [Header("Eject Settings")]
    [Tooltip("吐き出し範囲")] [Min(0f)]
    [SerializeField] private float radius;
    [Tooltip("吐き出し距離")] [Min(0f)]
    [SerializeField] private float distance;
    [Tooltip("吐き出し範囲の幅")] [Min(0f)]
    [SerializeField] private float range;
    [Tooltip("吐き出し速度")] [Min(1f)]
    [SerializeField] private float blowOutSpeed = 1;
    [Tooltip("吐き出し高さ")] [Min(0)]
    [SerializeField] private int blowOutUp = 3;
    
    [Header("Instantiation Settings")]
    [SerializeField] private float interval;
    [MinMaxSlider(0, 10)]
    [SerializeField] private Vector2Int generateTileCount;
    [FormerlySerializedAs("blowOutOrePrefab")] [SerializeField] private EjectOre _ejectOrePrefab;

    [Header("Debug Settings")]
    [SerializeField] private bool debug;
    
    private float _weight;
    private float _lastUpdateTime;
    private List<Vector3Int> _blowOutTilesList = new();
    private BlockType _blockType;
    private PlayerMovement _playerMovement;
    private Absorption _absorption;
    private Parabola _parabola;
    private Tilemap _updateTilemap;
    private Vacuum _vacuum;
    private IInputTank _inputTank;
    private ISoundSourceable _soundSource;

    public bool IsBlowOut { get; private set; }

    public void Inject(IInputTank inputTank)
    {
        _inputTank = inputTank;
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
        _absorption = GetComponent<Absorption>();
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
        _blockType = _inputTank.GetSelectType();
        
        if (_blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
        {
            _parabola.GenerateParabola();
        }
        else
        {
            _parabola.DestroyParabola();
        }
        
        _lastUpdateTime -= Time.deltaTime;
        if (VacuumActions.SpittingOut.IsPressed() && !_absorption.IsSuckUp)
        {
            blowEffect.SetActive(true);
            IsBlowOut = true;
            BlowOutTiles();
        }
    }

    private void BlowOutTiles()
    {
        if (_blockType != BlockType.None)
        {
            if (_blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
            {
                _weight = _vacuum.BlockData.GetOre(_blockType).weightPerSize[0] * 10;
            }
            else
            {
                _weight = _vacuum.BlockData.GetBlock(_blockType).weight;
            }
        
            if (_lastUpdateTime <= 0f)
            {
                if (_blockType == BlockType.Liquid) { return; }

                _lastUpdateTime = interval * _weight;
                if (_inputTank.FiringTank())
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
        if (_blockType is BlockType.Ruby or BlockType.Crystal or BlockType.Emerald)
        {
            AudioManager.Instance.PlaySFX("SpitoutSE");
            var position = distance * Direction.normalized + pivot.position;
            var blowOutOre = Instantiate(_ejectOrePrefab, position, Quaternion.identity);
            blowOutOre.gameObject.SetActive(true);
            var ore = _vacuum.BlockData.GetOre(_blockType);
            blowOutOre.SetOre(ore, Direction.normalized);
            _inputTank.RemoveTank();
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
                
                Block block = _vacuum.BlockData.GetBlock(_blockType);
                _updateTilemap.SetTile(randomCell, block.tile);

                var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(randomCell.x, randomCell.y));
                if (block.GetStratumGeologyData(tileLayer) != null)
                {
                    _updateTilemap.SetColor(randomCell, block.GetStratumGeologyData(tileLayer).color);
                }
                
                _inputTank.RemoveTank();
                _soundSource.InstantiateSound("Eject", randomPosition);
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
            var blockStratumGeologyData = _vacuum.BlockData.GetBlock(tile).GetStratumGeologyData(tileLayer);
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
                    
            if (_vacuum.BlockData.GetBlock(tile).type == BlockType.Sand)
            {
                _updateTilemap.SetTile(newTilePosition, tile);
                        
                var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
                var blockStratumGeologyData = _vacuum.BlockData.GetBlock(tile).GetStratumGeologyData(tileLayer);
                if (blockStratumGeologyData != null)
                {
                    _updateTilemap.SetColor(newTilePosition, blockStratumGeologyData.color);
                }
            }
            else
            {
                newMapTilemap.SetTile(localNewTilePosition, tile);
                        
                if (_vacuum.BlockData.GetBlock(tile).type is not (BlockType.Ruby or BlockType.Crystal or BlockType.Emerald))
                {
                    var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(localNewTilePosition.x, localNewTilePosition.y));
                    var blockStratumGeologyData = _vacuum.BlockData.GetBlock(_vacuum.BlockData.GetBlock(_blockType).tile).GetStratumGeologyData(tileLayer);
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
