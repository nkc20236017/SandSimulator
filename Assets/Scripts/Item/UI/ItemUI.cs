using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{

    private Image Image;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    public void UpdateUI(Sprite sprite)
    {
        Image.sprite = sprite;
    }

    public void CleanUp()
    {
        Destroy(this.gameObject);
    }

}
