using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class LevelPresenter : MonoBehaviour, IExpOutPut
{
    [SerializeField]
    private Slider expSlider;

    public void OutPutExp(ExpOutPutData data)
    {
        var expPercent = (float)data.Exp / (float)data.NextLevelHurdle;

        expSlider.DOValue(expPercent, 0.1f).SetEase(Ease.InFlash);

    }
}
