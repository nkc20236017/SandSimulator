using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRemoveSignal : MonoBehaviour
{
    [SerializeField]
    ItemData itemData;

    public void Remove()
    {
        ItemRemoveSignal.Instance.Remove(itemData.ItemId);
    }
}
