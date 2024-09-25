using UnityEngine;
using UnityEngine.UI;

public class CircleImage : MonoBehaviour
{
    private Image image;
    public BlockType blockType;
    public float fillAmount;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void FillInject(float fillAmount)
    {
        this.fillAmount = fillAmount;
    }

    public void SetUp(float fillAmount, BlockType blockType, Sprite sprite)
    {
        image.fillAmount = fillAmount;
        this.blockType = blockType;
        image.sprite = sprite;
    }

    public void UpdateUI(float fillAmount)
    {
        image.fillAmount = fillAmount;
    }

    public void CircleRotate(float itemRate)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, itemRate);
    }

    public void CleanUp()
    {
        image.fillAmount = 0;
        fillAmount = 0;
    }

}
