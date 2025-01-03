using UnityEngine;

public class TankUI : MonoBehaviour
{

    [SerializeField]
    private GameObject tankImagePrefab;

    [SerializeField]
    private TankImage[] tankImages;

    [SerializeField]
    private RectTransform totaleTransform;
    [SerializeField]
    private RectTransform maxTransfom;

    public void OutputTank(OutPutTankData outPutData)
    {
        Debug.Log("�o��");
        var totaleSize = outPutData.totalRatio * maxTransfom.sizeDelta.y;
        totaleTransform.sizeDelta = new Vector2(totaleTransform.sizeDelta.x, totaleSize);

        tankImages = GetComponentsInChildren<TankImage>();

        for (int i = 0; i < tankImages.Length; i++)
        {
            if (tankImages[i].itemType == outPutData.itemType)
            {
                tankImages[i].TankUpdate(outPutData.itemType, outPutData.itemRatio * totaleSize);
                return;
            }
        }

        var stankObject = Instantiate(tankImagePrefab, totaleTransform);
        stankObject.GetComponent<TankImage>().Setup(outPutData.itemType,
            outPutData.itemRatio * totaleSize, outPutData.Sprite);
    }
}
