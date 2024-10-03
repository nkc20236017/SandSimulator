using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class DemoInputItemTank : MonoBehaviour
{

    private IInputTank itemTank;
    private IGameLoad gameLoad;
    private PlayerActions playerActions;

    [Inject]
    public void Inject(IInputTank itemTank, IGameLoad gameLoad)
    {
        this.itemTank = itemTank;
        this.gameLoad = gameLoad;
    }

    private void Awake()
    {
        playerActions = new PlayerActions();
        playerActions.Vacuum.TankSelect.performed += OnWheel;
        playerActions.Enable();
    }

    public void OnWheel(InputAction.CallbackContext context)
    {
        var vaule = context.ReadValue<Vector2>();
        if(vaule.y < 0)
        {
            itemTank.RightSelectTank();
        }
        else if(vaule.y > 0)
        {
            itemTank.LeftSelectTank();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemTank.LeftSelectTank();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemTank.RightSelectTank();
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (itemTank.FiringTank())
            {
                Debug.Log("èoÇµÇ‹Ç∑");
            }
            else
            {
                Debug.Log("èoÇ‹ÇπÇÒ");
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameLoad.GameLoad();
        }

        if(Input.GetKey(KeyCode.A))
        {
            itemTank.InputAddTank(BlockType.Sand);
        }
        if(Input.GetKey(KeyCode.Q))
        {
            itemTank.InputAddTank(BlockType.Mud);
        }

    }


}
