using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    private Text resultText;

    [SerializeField]
    private Text moneyText;

    [SerializeField]
    private Image Image;

    public void SetUpUI(Sprite sprite, string resultText,string moneyText)
    {
        Image.sprite = sprite;
        this.resultText.text = "Å~"+resultText ;
        this.moneyText.text = moneyText;
    }

}
