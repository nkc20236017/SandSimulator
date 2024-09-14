using UnityEngine;
using NaughtyAttributes;

public class OreObject : MonoBehaviour, IDamagable
{
    [Header("Ore Object Config")]
    [SerializeField] private Ore ore;
    [SerializeField, MinValue(1), MaxValue(3)] private int setSize;

    private int _currentEndurance;
    private SpriteRenderer _spriteRenderer;

    public bool CanSuckUp { get; private set; }
    public Ore Ore => ore;
    public int Size { get; private set; }

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
        Size = size;
        _currentEndurance = ore.endurancePerSize[Size - 1];
        if (_spriteRenderer == null) { _spriteRenderer = GetComponent<SpriteRenderer>(); }
        _spriteRenderer.sprite = ore.oreSprites[Size - 1];
        transform.localScale = Vector3.one * Size;
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
            }
        }
        if (_currentEndurance > 0) { return; }

        CanSuckUp = true;
    }
}