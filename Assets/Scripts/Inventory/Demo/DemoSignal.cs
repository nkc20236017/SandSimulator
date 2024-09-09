using UnityEngine;

public class DemoSignal : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;
    [SerializeField]
    private ItemData itemData2;

    public void InputSignal()
    {
        ItemPickupSignal.Instance.PickupSignal(itemData.ItemId);
        ItemPickupSignal.Instance.PickupSignal(itemData2.ItemId);
    }
}
