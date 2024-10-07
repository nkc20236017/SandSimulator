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

    /// ���̓f�o�C�X�̎��
    /// </summary>
    public enum InputDeviceType
    {
        Keyboard,   // �L�[�{�[�h�E�}�E�X
        Xbox,       // Xbox�R���g���[���[
    }

    // ���߂ɑ��삳�ꂽ���̓f�o�C�X�^�C�v
    public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.Keyboard;

    // �e�f�o�C�X�̂��ׂẴL�[���P�Ƀo�C���h����InputAction�i�L�[��ʌ��m�p�j
    private InputAction keyboardAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/AnyKey", interactions: "Press");
    private InputAction mouseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/*", interactions: "Press");
    private InputAction xInputAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<XInputController>/*", interactions: "Press");
    // ���̓f�o�C�X�^�C�v�ύX�C�x���g
    public UnityEvent OnChangeDeviceType { get; private set; } = new();

    private void Awake()
    {

        // �L�[���m�p�A�N�V�����̗L����
        keyboardAnyKey.Enable();
        mouseAnyKey.Enable();
        xInputAnyKey.Enable();
    }


    private void Start()
    {
        // ����̂݁A�K�����̓f�o�C�X�̎�ʌ��m���s���ăR�[���o�b�N����
        StartCoroutine(InitializeDetection());
    }

    private void Update()
    {
        // ���m�̍X�V����
        UpdateDeviceTypesDetection();
    }

    /// <summary>
    /// ���̓f�o�C�X�̎�ʌ��m������������
    /// </summary>
    /// <returns></returns>
    IEnumerator InitializeDetection()
    {
        // ���̓f�o�C�X�̎�ʌ��m���X�V
        UpdateDeviceTypesDetection();
        // �P�t���[���ҋ@
        yield return null;
        // �C�x���g��������
        OnChangeDeviceType.Invoke();
    }


    /// <summary>
    /// ���̓f�o�C�X�̎�ʌ��m���X�V����
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

        // ����f�o�C�X���؂�ւ�����Ƃ��A�C�x���g����
        if (beforeDeviceType != CurrentDeviceType)
        {
            OnChangeDeviceType.Invoke();
        }
    }
}
