using UnityEngine.Tilemaps;

public interface ITankRepository
{
    Block Find(BlockType type);
    Block Find(TileBase tile);
}
