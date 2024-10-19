using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContllerText : MonoBehaviour
{
    [SerializeField]
    private Text Text;

    [SerializeField]
    private SelectAction InputDeviceManager;



    private void Update()
    {

        if(InputDeviceManager.CurrentDeviceType == InputDeviceType.Keyboard)
        {
            Text.text = "Press A To Start";
        }
        if(InputDeviceManager.CurrentDeviceType == InputDeviceType.Xbox)
        {
            Text.text = "Press A To Start";
        }
    }


}
