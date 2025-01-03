﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class SuckUp : MonoBehaviour
{
    [Header("Tile Config")]
    [SerializeField] private BlockDatas blockDatas;
    [SerializeField] private LayerMask blockLayerMask;
    
    [Header("Suction Config")]
    [SerializeField] private Transform pivot;
    [SerializeField, Range(0f, 180f)] private float _suctionAngle; // この角度以内のオブジェクトは吸い寄せられる
    [SerializeField, Min(0f)] private float _suctionDistance; // この距離以内のオブジェクトは吸い寄せられる
    [SerializeField, Min(0f)] private float _deleteDistance; // この距離以内のオブジェクトは削除される
    [SerializeField] private LayerMask oreLayerMask;
    [SerializeField, Min(1f)] private float suckUpSpeed;
    
    [Header("Debug Config")]
    [SerializeField] private bool _debugMode;

    [Header("Delete Config")]
    [SerializeField] private bool matchTheSizeOfTheCollider;

    [SerializeField] private GameObject suckEffect;

    private int _numberExecutions;
    private List<Vector3Int> _suckUpTilePositions = new();
    private List<OreObject> _suckUpOreObject = new();
    private Tilemap _updateTilemap;
    private PlayerMovement _playerMovement;
    private Camera _camera;
    private BlowOut _blowOut;
    private PlayerActions _playerActions;
    private PlayerActions.VacuumActions VacuumActions => _playerActions.Vacuum;
    private IInputTank inputTank;
    private IChunkInformation _chunkInformation;
    private ISoundSourceable _soundSource;

    private string[] seName =
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
        this.inputTank = inputTank;
    }
    
    public void SetTilemap(Tilemap tilemap)
    {
        _updateTilemap = tilemap;
    }

    private void Awake()
    {
        _playerActions = new PlayerActions();
        
        _blowOut = GetComponent<BlowOut>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Start()
    {
        _numberExecutions = 0;
        
        VacuumActions.Absorption.started += _ => PlaySuckUp();
        VacuumActions.Absorption.canceled += _ => CancelSuckUp();
    }

    private void Update()
    {
        RotateToCursorDirection();
        
        if (VacuumActions.Absorption.IsPressed() && !_blowOut.IsBlowOut)
        {
            if (inputTank.TamkMaxSignal())
            {
                //AudioManager.Instance.PlaySFX("MaxtankSE");
                return;
            }
            suckEffect.SetActive(true);
            // TODO: ［効果音］吸い込み
            IsSuckUp = true;
            Performed();
            _numberExecutions++;
        }
    }

    private void Performed()
    {
        if (inputTank.TamkMaxSignal()) { return; }
        
        // 吸い込み対象の座標取得
        GetSuckUpTilePositions();
        
        // 鉱石の吸い込み
        SuckUpOres();
        if (_suckUpOreObject.Count > 0) { return; }
        
        // タイルの吸い込み
        SuckUpTiles();
    }

    private void PlaySuckUp()
    {
        _playerMovement.IsMoveFlip = false;
        if (inputTank.TamkMaxSignal())
        {
            AudioManager.Instance.PlaySFX("MaxtankSE");
        }
    }

    private void CancelSuckUp()
    {
        suckEffect.SetActive(false);
        IsSuckUp = false;
        _suckUpTilePositions.Clear();
        _playerMovement.IsMoveFlip = true;
        _numberExecutions = 0;
    }

    private void RotateToCursorDirection()
    {
        if (_camera == null)
        {
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        
        var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        var direction = mouseWorldPosition - pivot.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (pivot.parent.localScale.x < 0)
        {
            angle += 180;
        }

        pivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void GetSuckUpTilePositions()
    {
        _suckUpTilePositions.Clear();
        _suckUpOreObject.Clear();
        
        var mapTilemap = _chunkInformation.GetChunkTilemap(pivot.position);
        if (mapTilemap == null) { return; }
        
        var bounds = new BoundsInt(Vector3Int.RoundToInt(pivot.position) - new Vector3Int((int)_suctionDistance, (int)_suctionDistance, 0), new Vector3Int((int)_suctionDistance * 2, (int)_suctionDistance * 2, 1));
        var hasTile = false;
        foreach (var position in bounds.allPositionsWithin)
        {
            var pos = new Vector2(position.x, position.y);
            mapTilemap = _chunkInformation.GetChunkTilemap(pos);
            if (mapTilemap == null) { continue; }
            
            var localPosition = _chunkInformation.WorldToChunk(pos);
            if (mapTilemap.HasTile(localPosition) || _updateTilemap.HasTile(position))
            {
                hasTile = true;
            }
        }
        if (!hasTile) { return; }
        
        foreach (var position in bounds.allPositionsWithin)
        {
            var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (tilemap == null) { continue; }

            var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            
            Vector2 direction1 = position - pivot.position;
            Vector2 direction2 = mouseWorldPosition - pivot.position;
            var angle = Vector3.Angle(direction1, direction2);

            var distance = Vector3.Distance(pivot.position, position);
            if (angle <= _suctionAngle && distance <= _suctionDistance)
            {
                DetectOre(new Vector2(position.x, position.y));   
                if (_suckUpOreObject.Count > 0) { continue; }
                
                var localPosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
                if (tilemap.HasTile(localPosition) || _updateTilemap.HasTile(position))
                {
                    // if (distance <= _deleteDistance)
                    // {
                    //     if (tilemap.HasTile(localPosition))
                    //     {
                    //         tilemap.SetTile(localPosition, null);
                    //     }
                    //     else if (_updateTilemap.HasTile(position))
                    //     {
                    //         _updateTilemap.SetTile(position, null);
                    //     }
                    //     return;
                    // }

                    _suckUpTilePositions.Add(position);
                }
            }
        }
    }
    
    private void DetectOre(Vector2 position)
    {
        var hitAll = Physics2D.OverlapBoxAll(position, Vector2.one, 0, oreLayerMask);
        if (hitAll.Length == 0) { return; }
        
        foreach (var hit in hitAll)
        {
            // if (hit == null) { continue; }
            if (IsBlock(hit.transform.position)) { continue; }
            if (!hit.TryGetComponent<OreObject>(out var oreObject)) { continue; }
            // if (oreObject == null) { continue; }
            if (!hit.TryGetComponent<IDamageable>(out var target)) { continue; }
            if (_suckUpOreObject.Contains(oreObject)) { continue; }
            _suckUpOreObject.Add(oreObject);
            
            if (_numberExecutions % oreObject.Ore.weightPerSize[oreObject.Size - 1] == 0)
            {
                target.TakeDamage(3);
                _soundSource.InstantiateSound("SuckUp", transform.position);
            }
        }
    }
    
    private bool IsBlock(Vector3 position)
    {
        var hit = Physics2D.Linecast(pivot.position, position, blockLayerMask);
        return hit.collider != null;
    }
    
    private void SuckUpTiles()
    {
        if (_suckUpTilePositions.Count == 0) { return; }
        
        _suckUpTilePositions = _suckUpTilePositions.OrderBy(_ => Random.value).ToList();

        foreach (var position in _suckUpTilePositions)
        {
            var tilemap = _chunkInformation.GetChunkTilemap(new Vector2(position.x, position.y));
            if (tilemap == null) { continue; }
            
            var direction = pivot.position - position;
            var newTilePosition = position + direction.normalized * suckUpSpeed;

            var newTilemap = _chunkInformation.GetChunkTilemap(newTilePosition);
            var localNewTilePosition = _chunkInformation.WorldToChunk(newTilePosition);
            if (newTilemap.HasTile(localNewTilePosition) || _updateTilemap.HasTile(_updateTilemap.WorldToCell(newTilePosition))) { continue; }

            TileBase tile = null;
            var localTilePosition = _chunkInformation.WorldToChunk(new Vector2(position.x, position.y));
            if (tilemap.HasTile(localTilePosition))
            {
                tile = tilemap.GetTile(localTilePosition);
                var isContinue = blockDatas.Block.Where(tileData => tileData.tile == tile).Any(tileData => _numberExecutions % tileData.weight != 0);
                if (isContinue) { continue; }
            }
            else if (_updateTilemap.HasTile(position))
            {
                tile = _updateTilemap.GetTile(position);
                var isContinue = blockDatas.Block.Where(tileData => tileData.tile == tile).Any(tileData => _numberExecutions % tileData.weight != 0);
                if (isContinue) { continue; }
            }
            if (tile == null) { continue; }

            _soundSource.InstantiateSound("SuckUp", transform.position);
            if (blockDatas.GetBlock(tile).type == BlockType.Sand)
            {
                _updateTilemap.SetTile(_updateTilemap.WorldToCell(newTilePosition), tile);
            }
            else
            {
                newTilemap.SetTile(localNewTilePosition, tile);
            }
            // TODO: 層ごとに色を変える
            var tileLayer = _chunkInformation.GetLayer(new Vector2(newTilePosition.x, newTilePosition.y));
            var block = blockDatas.GetBlock(tile);
            if (block.GetStratumGeologyData(tileLayer) != null)
            {
                newTilemap.SetColor(localNewTilePosition, block.GetStratumGeologyData(tileLayer).color);
                _updateTilemap.SetColor(_updateTilemap.WorldToCell(newTilePosition), block.GetStratumGeologyData(tileLayer).color);
            }
            
            _updateTilemap.SetTile(_updateTilemap.WorldToCell(position), null);
            tilemap.SetTile(localTilePosition, null);

            if ((newTilePosition - pivot.position).sqrMagnitude <= _deleteDistance * _deleteDistance)
            {
                inputTank.InputAddTank(tile);//タンクに追加
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
            var direction = pivot.position - position;
            oreObject.transform.position = position + direction.normalized;
            
            if (Vector3.Distance(oreObject.transform.position, pivot.position) <= _deleteDistance)
            {
                int x = Random.Range(0, seName.Length);
                AudioManager.Instance.PlaySFX(seName[x]);
                inputTank.InputAddTank(oreObject.Ore.type);//タンクに追加
                _suckUpOreObject.Remove(oreObject);
                Destroy(oreObject.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        _deleteDistance = Mathf.Min(_suctionDistance, _deleteDistance);
        _suctionDistance = Mathf.Max(_deleteDistance, _suctionDistance);
        
        if (!_debugMode) { return; }

        if (matchTheSizeOfTheCollider)
        {
            var boxCollider2D = GetComponentInParent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                _suctionDistance = pivot.transform.position.y - boxCollider2D.bounds.min.y;
                var left = pivot.transform.position.x - boxCollider2D.bounds.min.x;
                var right = boxCollider2D.bounds.max.x - pivot.transform.position.x;
                _deleteDistance = Mathf.Max(left, right);
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot.position, _suctionDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivot.position, _deleteDistance);

        Gizmos.color = Color.green;
        var camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (camera == null) { return; }

        var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

        var angleInRadians = _suctionAngle * Mathf.Deg2Rad;
        var direction2 = mouseWorldPosition - pivot.position;
        var angle = Mathf.Atan2(direction2.y, direction2.x);

        var newCell1 = GetNewCell(angle - angleInRadians, _suctionDistance);
        Gizmos.DrawLine(pivot.position, newCell1);

        var newCell2 = GetNewCell(angle + angleInRadians, _suctionDistance);
        Gizmos.DrawLine(pivot.position, newCell2);
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
        _soundSource.SetInstantiation("SuckUp");
        
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    
    private void OnDisable()
    {
        _playerActions.Disable();
    }
}
