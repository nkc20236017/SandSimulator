using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Absorption : MonoBehaviour
{
    [Header("Suction Config")]
    [SerializeField] private GameObject _absorptionEffect;
    [SerializeField] private LayerMask _absorptionTileLayerMask;
    [SerializeField] private LayerMask _absorptionObjectLayerMask;

    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode;
    
    private int _numberExecutions;
    private List<Vector3Int> _suckUpTilePositions = new();
    private List<OreObject> _suckUpOreObject = new();
    private Tilemap _updateTilemap;
    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;
    private Eject _eject;
    private Vacuum _vacuum;
    private VacuumData _vacuumData;
    private IInputTank _inputTank;
    private IChunkInformation _chunkInformation;

    private string[] _seName =
    {
        "VacuumSE",
        "VacuumSE-2",
        "VacuumSE-4",
        "VacuumSE-6",
        "VacuumSE-8"
    };

    public bool IsSuckUp { get; private set; }

    public void Inject(IInputTank inputTank)
    {
        _inputTank = inputTank;
    }

    /// <summary>
    /// タイルマップの注入
    /// </summary>
    /// <param name="tilemap">タイルマップ</param>
    public void Inject(Tilemap tilemap)
    {
        _updateTilemap = tilemap;
    }
    
    private void OnEnable()
    {
        _eject = GetComponent<Eject>();
        _vacuum = GetComponent<Vacuum>();
        _playerHealth = GetComponentInParent<PlayerHealth>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
        
        _vacuumData = _vacuum.VacuumData;
        _chunkInformation = _playerMovement.ChunkInformation;
    }

    private void Start()
    {
        _vacuum.VacuumActions.Absorption.canceled += _ => CancelSuckUp();
    }

    private void Update()
    {
        if (!_vacuum.VacuumActions.Absorption.IsPressed() || _eject.IsBlowOut) { return; }
        if (_inputTank.TamkMaxSignal()) { return; }
        
        _absorptionEffect.SetActive(true);
        IsSuckUp = true;
        Performed();
        _numberExecutions++;
    }

    private void Performed()
    {
        if (_inputTank.TamkMaxSignal()) { return; }

        // 吸い込み対象の座標取得
        GetSuckUpTilePositions();

        // 鉱石の吸い込み
        SuckUpOres();
        if (_suckUpOreObject.Count > 0) { return; }

        // タイルの吸い込み
        SuckUpTiles();
    }

    private void CancelSuckUp()
    {
        _absorptionEffect.SetActive(false);
        IsSuckUp = false;
        _suckUpTilePositions.Clear();
        _numberExecutions = 0;
    }

    private void GetSuckUpTilePositions()
    {
        _suckUpTilePositions.Clear();
        _suckUpOreObject.Clear();
        
        Tilemap mapTilemap = _chunkInformation.GetChunkTilemap(_vacuum.Pivot.position);
        if (mapTilemap == null) { return; }

        Vector3Int boundsPosition = Vector3Int.RoundToInt(_vacuum.Pivot.position) - new Vector3Int((int)_vacuumData.AbsorptionDistance, (int)_vacuumData.AbsorptionDistance);
        var boundsSize = new Vector3Int((int)_vacuumData.AbsorptionDistance * 2, (int)_vacuumData.AbsorptionDistance * 2);
        var bounds = new BoundsInt(boundsPosition, boundsSize);
        
        var hasTile = false;
        foreach (var position in bounds.allPositionsWithin)
        {
            var pos = new Vector2(position.x, position.y);
            mapTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(pos);
            if (mapTilemap == null) { continue; }

            var localPosition = _playerMovement.ChunkInformation.WorldToChunk(pos);
            if (mapTilemap.HasTile(localPosition) || _updateTilemap.HasTile(position))
            {
                hasTile = true;
            }
        }
        if (!hasTile) { return; }

        foreach (var position in bounds.allPositionsWithin)
        {
            var tilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (tilemap == null) { continue; }

            Vector2 direction1 = position - _vacuum.Pivot.position;
            var angle = Vector3.Angle(direction1, _vacuum.Direction);

            var distance = Vector3.Distance(_vacuum.Pivot.position, position);
            if (angle <= _vacuumData.AbsorptionAngle && distance <= _vacuumData.AbsorptionDistance)
            {
                DetectOre(new Vector2(position.x, position.y));
                if (_suckUpOreObject.Count > 0) { continue; }

                var localPosition = _playerMovement.ChunkInformation.WorldToChunk(new Vector2(position.x, position.y));
                if (tilemap.HasTile(localPosition) || _updateTilemap.HasTile(position))
                {
                    _suckUpTilePositions.Add(position);
                }
            }
        }
    }

    private void DetectOre(Vector2 position)
    {
        var hitAll = Physics2D.OverlapBoxAll(position, Vector2.one, 0, _absorptionObjectLayerMask);
        if (hitAll.Length == 0) { return; }

        foreach (var hit in hitAll)
        {
            if (hit == null) { continue; }
            if (IsBlock(hit.transform.position)) { continue; }
            if (!hit.TryGetComponent<OreObject>(out var oreObject)) { continue; }
            if (oreObject == null) { continue; }
            if (!hit.TryGetComponent<IDamageable>(out var target)) { continue; }
            if (_suckUpOreObject.Contains(oreObject)) { continue; }
            _suckUpOreObject.Add(oreObject);

            if (_numberExecutions % oreObject.Ore.weightPerSize[oreObject.Size - 1] == 0)
            {
                target.TakeDamage(3);
                _vacuum.SoundSource?.InstantiateSound("Absorption", transform.position);
            }
        }
    }

    private bool IsBlock(Vector3 position)
    {
        var hit = Physics2D.Linecast(_vacuum.Pivot.position, position, _absorptionTileLayerMask);
        return hit.collider != null;
    }

    private void SuckUpTiles()
    {
        if (_suckUpTilePositions.Count == 0) { return; }

        _suckUpTilePositions = _suckUpTilePositions.OrderBy(_ => Random.value).ToList();

        foreach (var position in _suckUpTilePositions)
        {
            var tilemap = _playerMovement.ChunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (tilemap == null) { continue; }

            var direction = _vacuum.Pivot.position - position;
            var newTilePosition = position + direction.normalized * _vacuumData.AbsorptionSpeed;

            var newTilemap = _playerMovement.ChunkInformation.GetChunkTilemap(newTilePosition);
            var localNewTilePosition = _playerMovement.ChunkInformation.WorldToChunk(newTilePosition);

            TileBase tile = null;
            var localTilePosition = _playerMovement.ChunkInformation.WorldToChunk(new Vector2(position.x, position.y));
            if (tilemap.HasTile(localTilePosition))
            {
                tile = tilemap.GetTile(localTilePosition);
                var isContinue = _vacuum.BlockData.Block.Where(tileData => tileData.tile == tile).Any(tileData => _numberExecutions % tileData.weight != 0);
                if (isContinue) { continue; }
            }
            else if (_updateTilemap.HasTile(position))
            {
                tile = _updateTilemap.GetTile(position);
                var isContinue = _vacuum.BlockData.Block.Where(tileData => tileData.tile == tile).Any(tileData => _numberExecutions % tileData.weight != 0);
                if (isContinue) { continue; }
            }
            if (tile == null) { continue; }

            _vacuum.SoundSource.InstantiateSound("Absorption", transform.position);
            if ((position - _vacuum.Pivot.position).sqrMagnitude <= _vacuumData.AbsorptionDeleteDistance * _vacuumData.AbsorptionDeleteDistance)
            {
                _inputTank.InputAddTank(tile);//タンクに追加
                tilemap.SetTile(localTilePosition, null);
                _updateTilemap.SetTile(position, null);
            }

            if (newTilemap.HasTile(localNewTilePosition) || _updateTilemap.HasTile(_updateTilemap.WorldToCell(newTilePosition))) { continue; }

            if (_vacuum.BlockData.GetBlock(tile).type == BlockType.Sand)
            {
                _updateTilemap.SetTile(_updateTilemap.WorldToCell(newTilePosition), tile);
            }
            else
            {
                newTilemap.SetTile(localNewTilePosition, tile);
            }
            // TODO: 層ごとに色を変える
            var tileLayer = _playerMovement.ChunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
            var block = _vacuum.BlockData.GetBlock(tile);
            if (block.GetStratumGeologyData(tileLayer) != null)
            {
                newTilemap.SetColor(localNewTilePosition, block.GetStratumGeologyData(tileLayer).color);
                _updateTilemap.SetColor(_updateTilemap.WorldToCell(newTilePosition), block.GetStratumGeologyData(tileLayer).color);
            }

            _updateTilemap.SetTile(_updateTilemap.WorldToCell(position), null);
            tilemap.SetTile(localTilePosition, null);

            if ((newTilePosition - _vacuum.Pivot.position).sqrMagnitude <= _vacuumData.AbsorptionDeleteDistance * _vacuumData.AbsorptionDeleteDistance)
            {
                _inputTank.InputAddTank(tile);//タンクに追加
                newTilemap.SetTile(localNewTilePosition, null);
                _updateTilemap.SetTile(_updateTilemap.WorldToCell(newTilePosition), null);
            }
        }
    }

    private void SuckUpOres()
    {
        if (_suckUpOreObject.Count == 0) { return; }

        _suckUpOreObject = _suckUpOreObject.OrderBy(_ => Random.value).Where(x => x != null).ToList();

        foreach (var oreObject in _suckUpOreObject.ToList())
        {
            if (!oreObject.CanSuckUp) { continue; }

            var position = oreObject.transform.position;
            var direction = _vacuum.Pivot.position - position;
            oreObject.transform.position = position + direction.normalized;

            if (Vector3.Distance(oreObject.transform.position, _vacuum.Pivot.position) <= _vacuumData.AbsorptionDeleteDistance)
            {
                if (oreObject.TryGetComponent<HealOre>(out var ore))
                {
                    _playerHealth.TakeHeal(ore.healPoint);
                }
                else
                {
                    _inputTank.InputAddTank(oreObject.Ore.type);//タンクに追加
                }
                int x = Random.Range(0, _seName.Length);
                AudioManager.Instance.PlaySFX(_seName[x]);
                _suckUpOreObject.Remove(oreObject);
                Destroy(oreObject.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_debugMode) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_vacuum.Pivot.position, _vacuumData.AbsorptionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_vacuum.Pivot.position, _vacuumData.AbsorptionDeleteDistance);

        Gizmos.color = Color.green;
        var angleInRadians = _vacuumData.AbsorptionAngle * Mathf.Deg2Rad;
        var angle = Mathf.Atan2(_vacuum.Direction.y, _vacuum.Direction.x);

        var newCell1 = GetNewCell(angle - angleInRadians, _vacuumData.AbsorptionDistance);
        Gizmos.DrawLine(_vacuum.Pivot.position, newCell1);

        var newCell2 = GetNewCell(angle + angleInRadians, _vacuumData.AbsorptionDistance);
        Gizmos.DrawLine(_vacuum.Pivot.position, newCell2);
    }

    private Vector3 GetNewCell(float angle, float chordLength)
    {
        var newX = chordLength * Mathf.Cos(angle);
        var newY = chordLength * Mathf.Sin(angle);
        var newDirection = new Vector3(newX, newY, 0);
        var newCell = _vacuum.Pivot.position + newDirection;
        return newCell;
    }
}