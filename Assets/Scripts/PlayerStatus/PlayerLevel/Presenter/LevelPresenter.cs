using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class LevelPresenter : MonoBehaviour, IExpOutPut
{
    [SerializeField]
    private Slider expSlider;

    public void OutPutExp(ExpOutPutData data)
    {
        var expPercent = (float)data.Exp / (float)data.NextLevelHurdle;
        expSlider.value = expPercent;
    }
}
