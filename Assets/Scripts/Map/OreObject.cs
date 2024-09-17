using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;

public class OreObject : MonoBehaviour, IDamagable
{
    [Header("Ore Object Config")]
    [SerializeField] private Ore ore;
    [SerializeField, MinValue(1), MaxValue(3)] private int setSize;
    [SerializeField] private Tilemap mapTilemap;
    
    [Header("Fall Ore Config")]
    [SerializeField] private float fallDamageInterval;
    [SerializeField] private float fundamentalDistance;
    
    private int _currentEndurance;
    private float _fallDamageTimer;
    private bool _canFall;
    private bool _canDestroy;
    private CapsuleCollider2D _capsuleCollider2D;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    public bool CanSuckUp { get; private set; }
    public Ore Ore => ore;
    public int Size { get; private set; }

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (setSize == 1)
        {
            SetOreConfig(setSize);
        }
    }

    // Debug: プレイ中にsetSizeを変更してSetupボタンを押すことで、OreObjectのサイズを変更できる
    [Button]
    public void Setup()
    {
        SetOreConfig(setSize);
    }

    private void SetOreConfig(int size)
    {
        Size = size;
        _currentEndurance = ore.endurancePerSize[Size - 1];
        if (_spriteRenderer == null) { _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); }
        _spriteRenderer.sprite = ore.oreSprites[Size - 1];
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
        
        if (_canFall) { return; }
        
        var size = Mathf.Max(_capsuleCollider2D.size.x, _capsuleCollider2D.size.y) / 2 + 0.9f;
        var angle = (transform.eulerAngles.z + 270) * Mathf.Deg2Rad;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        
        var position = _capsuleCollider2D.bounds.center + (Vector3)direction.normalized * size;
        var cellPosition = mapTilemap.WorldToCell(position);
        if (mapTilemap.HasTile(cellPosition)) { return; }
        
        _canFall = true;
        _rigidbody2D.isKinematic = false;
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
                destroyedOre.SetOreConfig(1);
                destroyedOre.CanSuckUp = true;
                destroyedOre._rigidbody2D.isKinematic = false;
            }
        }
        if (_currentEndurance > 0) { return; }

        CanSuckUp = true;
        _rigidbody2D.isKinematic = false; 
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_canDestroy) { return; }
        if (other.collider.CompareTag("Player")) { return; }
		
        if (other.collider.TryGetComponent<IDamagable>(out var target))
        {
            target.TakeDamage(Ore.attackPower);
            Destroy(gameObject);
        }
        
        // TODO: ［正規実装］魔鉱石が壊れると能力が発動する
		
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (mapTilemap == null) { return; }
        
        var capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider2D == null) { return; }
        
        var size = Mathf.Max(capsuleCollider2D.size.x, capsuleCollider2D.size.y) / 2 + 0.9f;
        var angle = (transform.eulerAngles.z + 270) * Mathf.Deg2Rad;
        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        
        var startPosition = capsuleCollider2D.bounds.center + (Vector3)direction.normalized * size;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mapTilemap.GetCellCenterWorld(mapTilemap.WorldToCell(startPosition)), new Vector3(0.9f, 0.9f));
    }
}