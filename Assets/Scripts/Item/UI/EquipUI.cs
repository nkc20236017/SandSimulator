using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    private Image Image;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    public void UpdateUI(Sprite sprite)
    {
        Image.color = Color.white;
        Image.sprite = sprite;
    }

    public void CleanUp()
    {
        Image.color = Color.clear;
        Image.sprite = null;
    }
}
