public class ItemInventory
{
    public int StackSize { get; private set; }
    public readonly ItemData ItemData;

    public ItemInventory(ItemData itemData)
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
