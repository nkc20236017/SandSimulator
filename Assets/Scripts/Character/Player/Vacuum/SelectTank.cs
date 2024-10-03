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

        playerInput.Vacuum.TankSelect.performed += OnWheel;
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void Inject(IInputTank inputTank)
    {
        this.inputTank = inputTank;
    }

    public void OnWheel(InputAction.CallbackContext context)
    {
        var vaule = context.ReadValue<Vector2>();
        if (vaule.y < 0)
        {
            inputTank.LeftSelectTank();
        }
        else if (vaule.y > 0)
        {
            inputTank.RightSelectTank();
        }
    }


}
