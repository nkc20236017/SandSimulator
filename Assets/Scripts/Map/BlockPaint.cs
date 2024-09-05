using System.Linq;
using UnityEngine;

public class BlockPaint : MonoBehaviour
{
    [Header("Brush Config")]
    [SerializeField] private BlockType blockType;
    [SerializeField] private int radius;
    
    [Header("Grid Config")]
    [SerializeField] private SpriteRenderer demoGrid;
    [SerializeField] private GameObject blockGrid;
    
    [Header("Block Config")]
    [SerializeField] private Block[] blocks;
    
    private Sprite demoSprite;
    private Camera _camera;
    private MapGenerator _mapGenerator;

    private BlockType BlockType
    {
        get => blockType;
        set
        {
            blockType = value;
            demoSprite = SpriteCreate();
        }
    }

    private void Start()
    {
        _camera = Camera.main;
        _mapGenerator = MapGenerator.Instance;
        
        demoSprite = SpriteCreate();
    }

    private void Update()
    {
        DemoBlockView();
        
        if (Input.GetMouseButton(1))
        {
            demoSprite = SpriteCreate();
            
            var mousePosition = Input.mousePosition;
            var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
            PlaceBlock(GetBlock(blockType), worldPosition, radius);
        }
    }
    
    private void DemoBlockView()
    {
        var mousePosition = Input.mousePosition;
        var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        demoGrid.transform.position = new Vector3(Mathf.Floor(worldPosition.x) + 0.01f, Mathf.Floor(worldPosition.y) + 0.01f, 0);
        
        demoGrid.sprite = demoSprite;
    }

    private void PlaceBlock(Block block, Vector3 center, int radius)
    {
        
    }
    
    private Sprite SpriteCreate()
    {
        return Sprite.Create(GetBlock(blockType).GetRandomTexture(), new Rect(0, 0, radius, radius), Vector2.zero);
    }
    
    private Block GetBlock(BlockType blockType)
    {
        return blocks.FirstOrDefault(block => block.BlockType == blockType);
    }
}
