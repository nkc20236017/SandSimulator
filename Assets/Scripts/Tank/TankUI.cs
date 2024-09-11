using UnityEngine;

public class TankUI : MonoBehaviour, IOutPutTank
{

    [SerializeField]
    private GameObject tankImagePrefab;

    [SerializeField]
    private TankImage[] tankImages;

    [SerializeField]
    private RectTransform totaleTransform;
    [SerializeField]
    private RectTransform maxTransfom;

    private ItemTank itemTank;

    public void OutputTank(OutPutData outPutData)
    {
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
        stankObject.GetComponent<TankImage>().TankUpdate(outPutData.itemType,
            outPutData.itemRatio * totaleSize);

    }

    private void Start()
    {
        itemTank = new(this, 100);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            itemTank.InputAddTank(ItemType.Sand);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            itemTank.InputAddTank(ItemType.Mad);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            itemTank.InputRemoveTank(ItemType.Sand);
        }

    }

}
