using UnityEngine;
using UnityEngine.InputSystem;

public class SelectTank : MonoBehaviour
{
    private PlayerActions playerInput;
    private IInputTank inputTank;

    private void OnEnable()
    {
        playerInput = new PlayerActions();

        playerInput.Vacuum.TankSelect.performed += OnWheel;
        playerInput.Vacuum.RightSelect.performed += OnRightButton;
        playerInput.Vacuum.LeftSelect.performed += OnLeftButton;

        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Vacuum.TankSelect.performed -= OnWheel;
        playerInput.Vacuum.RightSelect.performed -= OnRightButton;
        playerInput.Vacuum.LeftSelect.performed -= OnLeftButton;
        playerInput.Disable();
    }

    public void Inject(IInputTank inputTank)
    {
        this.inputTank = inputTank;
    }

    public void OnWheel(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        switch (value.y)
        {
            case < 0:
                inputTank.LeftSelectTank();
                break;
            case > 0:
                inputTank.RightSelectTank();
                break;
        }
    }

    private void OnLeftButton(InputAction.CallbackContext context)
    {
        inputTank.LeftSelectTank();
    }

    private void OnRightButton(InputAction.CallbackContext context)
    {
        inputTank.RightSelectTank();
    }


}
