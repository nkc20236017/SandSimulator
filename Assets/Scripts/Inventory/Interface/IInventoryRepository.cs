using System.Collections.Generic;

public interface IInventoryRepository
{
    void AddToInventory(ItemData item);
    void RemoveFromInventory(ItemData item);
    List<ItemInventory> FindAll();
}
