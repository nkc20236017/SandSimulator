using Codice.Client.GameUI.Update;
using UnityEngine;

public class TankUI : MonoBehaviour, IOutPutTank
{
    [SerializeField]
    private TankImage sandTransform;
    [SerializeField]
    private TankImage madeTransform;

    [SerializeField]
    private RectTransform totaleTransform;
    [SerializeField]
    private RectTransform maxTransfom;

    private ItemTank itemTank;

    public void OutputTank(OutPutData outPutData)
    {
        var totaleSize = outPutData.totalRatio * maxTransfom.sizeDelta.y;
        totaleTransform.sizeDelta = new Vector2(totaleTransform.sizeDelta.x, totaleSize);

        if (outPutData.itemType == ItemType.Sand)
        {
            sandTransform.Setup(ItemType.Sand,outPutData.itemRatio * totaleSize);
        }
        else if(outPutData.itemType == ItemType.Mad)
        {
            madeTransform.Setup(ItemType.Mad, outPutData.itemRatio * totaleSize);
        }

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

    }

}
