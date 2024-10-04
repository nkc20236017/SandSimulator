using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutLineController : MonoBehaviour
{  
    public void OnOutLine()
    {
        Outline outline = GetComponent<Outline>();
        outline.enabled = true;
    }

    public void OffOutLine()
    {
        Outline outline = GetComponent<Outline>();
        outline.enabled = false;
    }
}
