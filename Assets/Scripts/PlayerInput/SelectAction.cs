using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SelectAction : MonoBehaviour
{
    [SerializeField]
    private GameObject virtualMouse;

    /// 入力デバイスの種別
    /// </summary>
    public enum InputDeviceType
    {
        Keyboard,   // キーボード・マウス
        Xbox,       // Xboxコントローラー
    }

    // 直近に操作された入力デバイスタイプ
    public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.Keyboard;

    // 各デバイスのすべてのキーを１つにバインドしたInputAction（キー種別検知用）
    private InputAction keyboardAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/AnyKey", interactions: "Press");
    private InputAction mouseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/*", interactions: "Press");
    private InputAction xInputAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<XInputController>/*", interactions: "Press");
    // 入力デバイスタイプ変更イベント
    public UnityEvent OnChangeDeviceType { get; private set; } = new();

    private void Awake()
    {

        // キー検知用アクションの有効化
        keyboardAnyKey.Enable();
        mouseAnyKey.Enable();
        xInputAnyKey.Enable();
    }


    private void Start()
    {
        // 初回のみ、必ず入力デバイスの種別検知を行ってコールバック発火
        StartCoroutine(InitializeDetection());
    }

    private void Update()
    {
        // 検知の更新処理
        UpdateDeviceTypesDetection();
    }

    /// <summary>
    /// 入力デバイスの種別検知を初期化する
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeDetection()
    {
        // 入力デバイスの種別検知を更新
        UpdateDeviceTypesDetection();
        // １フレーム待機
        yield return null;
        // イベント強制発火
        OnChangeDeviceType.Invoke();
    }


    /// <summary>
    /// 入力デバイスの種別検知を更新する
    /// </summary>
    public void UpdateDeviceTypesDetection()
    {
        var beforeDeviceType = CurrentDeviceType;

        if (xInputAnyKey.triggered)
        {
            CurrentDeviceType = InputDeviceType.Xbox;
            virtualMouse.SetActive(true);
        }

        if (keyboardAnyKey.triggered || mouseAnyKey.triggered)
        {
            CurrentDeviceType = InputDeviceType.Keyboard;
            virtualMouse.SetActive(false);
        }

        // 操作デバイスが切り替わったとき、イベント発火
        if (beforeDeviceType != CurrentDeviceType)
        {
            OnChangeDeviceType.Invoke();
        }
    }
}
