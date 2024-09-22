using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }


    public void UpdateUI(Sprite sprite)
    {
            image.sprite = sprite;
    }

    public void CleanUp()
    {
        Destroy(this.gameObject);
    }
}
