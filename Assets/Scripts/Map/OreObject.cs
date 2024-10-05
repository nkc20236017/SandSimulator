using UnityEngine;

public class OreObject : MonoBehaviour, IDamageable, IDetectSoundable
{
    [Header("Datas Config")]
    [SerializeField] private BlockDatas blockDatas;
    
    [Header("Fall Ore Config")]
    [SerializeField] private float fallDamageInterval;
    [SerializeField] private float fundamentalDistance;
    
    private int _currentEndurance;
    private float _fallDamageTimer;
    private bool _isChild;
    private bool _isFall;
    private bool _canDestroy;
    private CapsuleCollider2D _capsuleCollider2D;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private IChunkInformation _chunkInformation;

    public int Size { get; private set; }
    public bool CanFall { get; set; } = true;
    public bool CanSuckUp { get; private set; }
    public bool IsDetectSound { get; set; }
    public Ore Ore { get; private set; }

    private void Update()
    {
        if (!CanFall) { return; }
        if (_isChild) { return; }

        if (_rigidbody2D.velocity.y < 0)
        {
            _fallDamageTimer += Time.deltaTime;
            if (_fallDamageTimer >= fallDamageInterval)
            {
                _canDestroy = true;
            }
        }
        else
        {
            _fallDamageTimer = 0;
        }
        
        if (_isFall) { return; }
        
        var size = Mathf.Max(_capsuleCollider2D.size.x, _capsuleCollider2D.size.y) / 2 + 0.9f;
        var angle = (transform.eulerAngles.z + 270) * Mathf.Deg2Rad;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        
        var position = _capsuleCollider2D.bounds.center + (Vector3)direction.normalized * size;
        var mapTilemap = _chunkInformation.GetChunkTilemap(position);
        if (mapTilemap == null) { return; }
        
        var localPosition = _chunkInformation.WorldToChunk(position);
        if (mapTilemap.HasTile(localPosition)) { return; }
        
        _isFall = true;
        _rigidbody2D.isKinematic = false;
    }

    public void SetOre(Ore setOre, int setSize, float setAngle)
    {
        Ore = setOre;
        setSize = Mathf.Clamp(setSize, 1, 3);
        setAngle = Mathf.Clamp(setAngle, 0, 360);
        transform.eulerAngles = new Vector3(0, 0, setAngle);
        SetOreConfig(setSize);
    }

    private void SetChildOre(Ore setOre)
    {
        SetOre(setOre, 1, transform.eulerAngles.z);
        _isChild = true;
        CanSuckUp = true;
        _currentEndurance = 0;
        _rigidbody2D.isKinematic = false;
    }

    public void TakeDamage(int damage)
    {
        if (CanSuckUp) { return; }

        _currentEndurance -= damage;
        if (Size > 1)
        {
            if (_currentEndurance <= Ore.endurancePerSize[Size - 2])
            {
                Size--;
                SetOreConfig(Size);
                var destroyedOre = Instantiate(this, transform.position, Quaternion.identity);
                destroyedOre.SetChildOre(Ore);
            }
        }
        if (_currentEndurance > 0) { return; }

        CanSuckUp = true;
        _rigidbody2D.isKinematic = false; 
    }
    
    private void SetOreConfig(int size)
    {
        Size = size;
        _currentEndurance = Ore.endurancePerSize[Size - 1];
        if (_spriteRenderer == null) { _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); }
        _spriteRenderer.sprite = Ore.oreSprites[Size - 1];
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_canDestroy) { return; }
		
        IsDetectSound = true;
        if (other.collider.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(Ore.attackPower);
        }
        
        // TODO: ［正規実装］魔鉱石が壊れると能力が発動する
        // TODO: ［エフェクト］鉱石破壊
        Destroy(gameObject);
    }
    
    private void OnEnable()
    {
        var worldMapManager = FindObjectOfType<WorldMapManager>();
        _chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
        
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}