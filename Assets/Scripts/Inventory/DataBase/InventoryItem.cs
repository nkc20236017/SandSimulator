public class InventoryItem
{
    public int StackSize { get; private set; }
    public readonly ItemData ItemData;

    public InventoryItem(ItemData itemData)
    {
        ItemData = itemData;
        StackSize = 1;
    }

    public void AddStack()
    {
        StackSize += 1;
    }
    public void RemoveStack()
    {
        StackSize -= 1;
    }
}
