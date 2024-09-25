using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CircleUI : MonoBehaviour, IOutPutTank
{

    [SerializeField]
    private CircleImage[] circleImages;

    [SerializeField]
    private GameObject CicrleImagePrefab;

    private float Maxfloat = 0.5f;
    [SerializeField]
    private float currentFillAmount;


    public void OutputTank(OutPutData outPutData)
    {
        
    }

    private void Sort()
    {
        var sortObject = circleImages.OrderByDescending(n => n.GetComponent<Image>().fillAmount).ToArray();

        for (int i = 0; i < sortObject.Length; i++)
        {
            sortObject[i].transform.SetSiblingIndex(i);
        }
    }

}
