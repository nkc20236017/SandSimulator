using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInput : MonoBehaviour
{

    private PlayerTank itemTank;

    private ITankRepository tankRepository;

    [SerializeField]
    private MineralDataBase MineralDataBase;

    [SerializeField]
    private TankUI tankUI;

    private void Start()
    {
        tankRepository = new MineralDataAccess(MineralDataBase);
        itemTank = new(tankUI, 100, tankRepository);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            itemTank.InputAddTank(MineralType.Sand);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            itemTank.InputAddTank(MineralType.Mad);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            itemTank.InputRemoveTank(MineralType.Sand);
        }

    }




}
