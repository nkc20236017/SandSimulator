using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankImage : MonoBehaviour
{
    private RectTransform tankImage;
    public ItemType itemType {  get; private set; }

    private void Awake()
    {
        tankImage = GetComponent<RectTransform>();
    }

    public void Setup(ItemType itemType,float imageSize)
    {
        this.itemType = itemType;
        Debug.Log(imageSize+itemType.ToString() +"‚Ì’l");
        tankImage.sizeDelta = new Vector2(tankImage.sizeDelta.x, imageSize);
    }

}
