using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankImage : MonoBehaviour
{
    private RectTransform tankImage;
    private Image mineralImage;
    public ItemType itemType {  get; private set; }

    private void Awake()
    {
        tankImage = GetComponent<RectTransform>();
        mineralImage = GetComponent<Image>();
    }

    public void Setup(ItemType itemType,float imageSize,Sprite sprite)
    {
        this.itemType = itemType;
        tankImage.sizeDelta = new Vector2(tankImage.sizeDelta.x, imageSize);
        mineralImage.sprite = sprite;
    }

    public void TankUpdate(ItemType itemType, float imageSize)
    {
        this.itemType = itemType;
        tankImage.sizeDelta = new Vector2(tankImage.sizeDelta.x, imageSize);
    }

}
