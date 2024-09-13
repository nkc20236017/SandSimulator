using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInput : MonoBehaviour
{

    private InputTank itemTank;

    private ITankRepository tankRepository;

    [SerializeField]
    private MineralDataBase MineralDataBase;

    [SerializeField]
    private TankUI tankUI;

    private void Start()
    {
        tankRepository = new MineralDataAccess(MineralDataBase);
        var otemTank = new PlayerTank(tankUI, 100, tankRepository);
        itemTank = new(otemTank);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("Input");
            itemTank.AddTankCommand(MineralType.Sand);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            itemTank.AddTankCommand(MineralType.Mad);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            itemTank.RemoveTankCommand(MineralType.Sand);
        }

    }




}
