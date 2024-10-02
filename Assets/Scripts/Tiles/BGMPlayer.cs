using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField]
    [Header("—¬‚µ‚½‚¢BGM")]
    private string bgm;

    void Start()
    {
        AudioManager.Instance.PlayBGM(bgm);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopBGM(bgm);
    }
}
