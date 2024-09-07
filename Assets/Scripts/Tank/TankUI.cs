using UnityEngine;

public class TankUI : MonoBehaviour, IOutPutTank
{
    [SerializeField]
    private RectTransform sandTransform;
    [SerializeField]
    private RectTransform madeTransform;
    [SerializeField]
    private RectTransform totaleTransform;
    [SerializeField]
    private RectTransform maxTransfom;

    private ItemTank itemTank;

    public void OutputTank(OutPutData outPutData)
    {
        var totaleSize = outPutData.totalVaule * maxTransfom.sizeDelta.y;
        totaleTransform.sizeDelta = new Vector2(totaleTransform.sizeDelta.x, totaleSize);
        sandTransform.sizeDelta = new Vector2(sandTransform.sizeDelta.x, outPutData.sandVaule * totaleSize);
        madeTransform.sizeDelta = new Vector2(madeTransform.sizeDelta.x, outPutData.madVaule * totaleSize);

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
