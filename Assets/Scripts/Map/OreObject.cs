using UnityEngine;
using NaughtyAttributes;

public class OreObject : MonoBehaviour, IDamagable
{
    [Header("Ore Object Config")]
    [SerializeField] private Ore ore;
    [SerializeField, MinValue(1), MaxValue(3)] private int setSize;

    private int _currentEndurance;
    private int _currentSize;
    private SpriteRenderer _spriteRenderer;

    public bool CanSuckUp { get; private set; }
    public Ore Ore => ore;
    public int Size => _currentSize;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Debug: プレイ中にsetSizeを変更してSetupボタンを押すことで、OreObjectのサイズを変更できる
    [Button]
    public void Setup()
    {
        SetOreConfig(setSize);
    }

    private void SetOreConfig(int size)
    {
        _currentSize = size;
        _currentEndurance = ore.endurancePerSize[_currentSize - 1];
        if (_spriteRenderer == null) { _spriteRenderer = GetComponent<SpriteRenderer>(); }
        _spriteRenderer.sprite = ore.oreSprites[_currentSize - 1];
        transform.localScale = Vector3.one * _currentSize;
    }

    public void TakeDamage(int damage)
    {
        if (CanSuckUp) { return; }

        _currentEndurance -= damage;
        if (_currentSize > 1)
        {
            if (_currentEndurance <= ore.endurancePerSize[_currentSize - 2])
            {
                _currentSize--;
                _spriteRenderer.sprite = ore.oreSprites[_currentSize - 1];
                transform.localScale = Vector3.one * _currentSize;
                var destroyedOre = Instantiate(this, transform.position, Quaternion.identity);
                destroyedOre.SetOreConfig(1);
                destroyedOre.CanSuckUp = true;
            }
        }
        if (_currentEndurance > 0) { return; }

        CanSuckUp = true;
    }
    
    public void SetOre(Ore ore) // TODO: 消す可能性（吐き出した鉱石の吸い込み）
    {
        this.ore = ore;
        SetOreConfig(setSize);
        CanSuckUp = true;
    }
}