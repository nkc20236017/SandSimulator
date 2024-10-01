using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class SelectTank : MonoBehaviour
{
    private IInputTank inputTank;

    [SerializeField]
    private PlayerActions playerInput;

    private void OnEnable()
    {
        playerInput = new();

        playerInput.Vacuum.LeftSelect.started += _ => LeftTank();
        playerInput.Vacuum.RightSelect.started += _ => RightTank();
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    [Inject]
    public void Inject(IInputTank inputTank)
    {
        this.inputTank = inputTank;
    }

    public void LeftTank()
    {
        inputTank.LeftSelectTank();
    }

    public void RightTank()
    {
        inputTank.RightSelectTank();
    }



}
