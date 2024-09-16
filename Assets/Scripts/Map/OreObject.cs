using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;

public class OreObject : MonoBehaviour, IDamagable
{
    [Header("Ore Object Config")]
    [SerializeField] private Ore ore;
    [SerializeField, MinValue(1), MaxValue(3)] private int setSize;
    [SerializeField] private Tilemap mapTilemap;
    [SerializeField] private float fallDamageSpeed;

    private int _currentEndurance;
    private bool _canFall;
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
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (_spriteRenderer == null) { _spriteRenderer = GetComponent<SpriteRenderer>(); }
        _spriteRenderer.sprite = ore.oreSprites[Size - 1];
    }

    private void Update()
    {
        // if (_canFall) { return; }
        
        var position = new Vector3(_capsuleCollider2D.bounds.center.x, _capsuleCollider2D.bounds.min.y - 0.1f, 0);
        var cellPosition = mapTilemap.WorldToCell(position);
        if (mapTilemap.HasTile(cellPosition)) { return; }
        
        _canFall = true;
        // _rigidbody2D.isKinematic = false;
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
        if (_rigidbody2D.velocity.y > -fallDamageSpeed) { return; }
        if (other.collider.CompareTag("Player")) { return; }
		
        if (other.collider.TryGetComponent<IDamagable>(out var target))
        {
            target.TakeDamage(Ore.attackPower);
            Destroy(gameObject);
        }
		
        Destroy(gameObject);
    }
}