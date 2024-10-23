using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSG : MonoBehaviour
{
    public string text = "彼は役人に囲まれて長崎の牢屋に送られる時も、少しも悪びれる様子を見せませんでした。むしろ伝説によると、愚か者の吉助の顔が、その時まるで天上の光に照らされたかと思うほど、不思議な威厳に満ちていたということです。\n役人の前に連れてこられた吉助は、素直にキリスト教を信仰していることを認めました。それから彼と役人の間で、以下のようなやり取りがありました。\n役人「お前たちの宗教の神様は誰と言うのだ。」\n吉助「ポルトガル国の王子様、イエス・キリスト様、そしてお隣の国の王女様、サンタ・マリア様です。」\n役人「その方たちはどんな姿をしているのだ。」\n吉助「私たちが夢に拝見するイエス・キリスト様は、紫の大きな袖の着物を着た、美しい若者の姿です。またサンタ・マリア様は、金糸銀糸の刺繍がされた、襠（かいどり：袴のようなもの）の姿と拝んでおります。」\n役人「その方たちが宗教の神様になったのは、どういう理由があるのだ。」";
    public Text textUI;
    private int _index;
    private bool _isTextEnd;
    private List<string> _textList = new();

    private void Start()
    {
        DelimitText();
        SetText();
    }

    private void DelimitText()
    {
        var lines = text.Split('\n');
        foreach (var line in lines)
        {
            for (var i = 0; i < line.Length; i += 20)
            {
                _textList.Add(line.Substring(i, Mathf.Min(20, line.Length - i)));
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isTextEnd)
        {
            SetText();
        }
    }

    private void SetText()
    {
        if (_index >= _textList.Count)
        {
            _isTextEnd = true;
            return;
        }

        textUI.text = _textList[_index];
        _index++;
    }
}