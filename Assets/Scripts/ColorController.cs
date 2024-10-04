using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private 

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        DOTween.To(() => 0f , x => _spriteRenderer.color = ColorCycle(_spriteRenderer.color,x),1f,1f).SetLoops(-1,LoopType.Restart).SetEase(Ease.Linear);
    }

    private Color ColorCycle(Color currentColor, float value)
    {
        // RGBÉJÉâÅ[ÇHSVÇ…ïœä∑Ç∑ÇÈ
        (float h, float s, float v) hsvColor;
        Color.RGBToHSV(currentColor, out hsvColor.h, out hsvColor.s, out hsvColor.v);

        // HSVÇÃHÇïœçX
        hsvColor.h = value;

        return Color.HSVToRGB(hsvColor.h, hsvColor.s, hsvColor.v);
    }
}
