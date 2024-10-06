using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DemoInputAction : MonoBehaviour
{
    private PlayerActions playerActions;

    private void Start()
    {
        playerActions = new();
        playerActions.Vacuum.VacuumPos.performed += OnContller;
        playerActions.Vacuum.Absorption.started += OnAbsorptionSound;
        playerActions.Vacuum.Absorption.canceled += OnAbsorptionSound;
        playerActions.Enable();
    }

    private void OnContller(InputAction.CallbackContext context)
    {
        Vector2 stickValue = context.ReadValue<Vector2>();
        float angle = Mathf.Atan2(stickValue.y, stickValue.x) * Mathf.Rad2Deg;
        if (angle < 0)
            angle += 360;
        Debug.Log("スティックの角度: " + angle);
    }

    private void OnAbsorptionSound(InputAction.CallbackContext context)
    {
        Debug.Log("qqq");
        if (context.started)
        {
            AudioManager.Instance.PlaySFX("VacuumSE");
            return;
        }
        AudioManager.Instance.StopSFX("VacuumSE");
    }
}
