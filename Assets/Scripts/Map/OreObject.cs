using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class OreObject : MonoBehaviour, IDamagable
{
    [Header("Datas Config")]
    [SerializeField] private BlockDatas blockDatas;
    
    [Header("Ore Object Config")]
    [SerializeField] private Ore ore;
    [SerializeField, MinValue(1), MaxValue(3)] private int size;
    [SerializeField, MinValue(0), MaxValue(360)] private float angle;

    [Header("Random Config")]
    [SerializeField] private bool isRandomOre;
    [SerializeField] private bool isRandomSize;
    [SerializeField] private bool isRandomAngle;
    [SerializeField] private bool canObliqueAngle;
    
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
    public Ore Ore => ore;

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (_isChild) { return; }
        
        if (isRandomOre)
        {
            ore = blockDatas.GetRandomOre();
        }
        if (isRandomSize)
        {
            size = Random.Range(1, 4);
        }
        if (isRandomAngle)
        {
            angle = Random.Range(0, 361);
            var minAngle = canObliqueAngle ? 45 : 90;
            angle = Mathf.Round(angle / minAngle) * minAngle;
        }
        SetOre(ore, size, angle);
    }

    private void Update()
    {
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
        
        if (!CanFall) { return; }
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
        ore = setOre;
        setSize = Mathf.Clamp(setSize, 1, 3);
        setAngle = Mathf.Clamp(setAngle, 0, 360);
        transform.eulerAngles = new Vector3(0, 0, setAngle);
        SetOreConfig(setSize);
    }

    public void TakeDamage(int damage)
    {
        if (CanSuckUp) { return; }

        _currentEndurance -= damage;
        if (Size > 1)
        {
            if (_currentEndurance <= ore.endurancePerSize[Size - 2])
            {
                Size--;
                SetOreConfig(Size);
                var destroyedOre = Instantiate(this, transform.position, Quaternion.identity);
                destroyedOre._isChild = true;
                destroyedOre.SetOreConfig(1);
                destroyedOre.CanSuckUp = true;
                destroyedOre._rigidbody2D.isKinematic = false;
            }
        }
        if (_currentEndurance > 0) { return; }

        CanSuckUp = true;
        _rigidbody2D.isKinematic = false; 
    }
    
    private void SetOreConfig(int size)
    {
        Size = size;
        _currentEndurance = ore.endurancePerSize[Size - 1];
        if (_spriteRenderer == null) { _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); }
        _spriteRenderer.sprite = ore.oreSprites[Size - 1];
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_canDestroy) { return; }
		
        if (other.collider.TryGetComponent<IDamagable>(out var target))
        {
            target.TakeDamage(Ore.attackPower);
            Destroy(gameObject);
        }
        
        // TODO: ［正規実装］魔鉱石が壊れると能力が発動する
		
        Destroy(gameObject);
    }
    
    private void OnEnable()
    {
        var worldMapManager = FindObjectOfType<WorldMapManager>();
        _chunkInformation = worldMapManager.GetComponent<IChunkInformation>();
    }
}