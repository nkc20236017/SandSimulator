using UnityEngine;
using VContainer;

public class DemoInput : MonoBehaviour
{
    [SerializeField]
    private int expAmount;

    private IExp exp;

    [Inject]
    public void Inject(IExp exp)
    {
        this.exp = exp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            exp.AddExp(expAmount);
        }
    }
}
