using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//Ç±ÇÍÇÕÉfÉÇÇ≈çÏÇ¡ÇΩÇ‡ÇÃÇ≈Ç∑ê≥éÆÇ…ÇÕçÃópÇµÇ»Ç¢Ç≈

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private bool moveTutorial;
    [SerializeField]
    private bool jumpTutorial;
    [SerializeField]
    private bool SlicUpTutorial;
    [SerializeField]
    private bool BlowOutTutorial;
    [SerializeField]
    private bool SelectTutorial;

    [SerializeField]
    private Animator ImageAnimator;

    [TextArea]
    [SerializeField]
    private string moveText;
    [TextArea]
    [SerializeField]
    private string jumpText;
    [TextArea]
    [SerializeField]
    private string slicUpText;
    [TextArea]
    [SerializeField]
    private string blowDownText;
    [TextArea]
    [SerializeField]
    private string selectText;

    [SerializeField]
    private Text text;

    [SerializeField]
    private GameObject tutorialObject;

    private PlayerActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerActions();
    }

    private void Start()
    {
        inputActions.Movement.Move.performed += OnMove;
        inputActions.Movement.Jump.performed += OnJump;
        inputActions.Vacuum.Absorption.performed += OnSlicUp;
        inputActions.Vacuum.SpittingOut.performed += OnBlowOut;
        inputActions.Vacuum.RightSelect.performed += OnSelect;
        inputActions.Vacuum.LeftSelect.performed += OnSelect;
        inputActions.Vacuum.TankSelect.performed += OnSelect;

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (!moveTutorial)
        {
            text.text = moveText;
            ImageAnimator.SetFloat("Blend", 1);
            return;
        }

        if (!jumpTutorial)
        {
            text.text = jumpText;
            ImageAnimator.SetFloat("Blend", 2);
            return;
        }

        if (!SlicUpTutorial)
        {
            text.text = slicUpText;
            ImageAnimator.SetFloat("Blend", 3);
            return;
        }

        if(!BlowOutTutorial)
        {
            text.text = blowDownText;
            ImageAnimator.SetFloat("Blend", 4);
            return;
        }

        if(!SelectTutorial)
        {
            text.text = selectText;
            ImageAnimator.SetFloat("Blend", 5);
            return;
        }

        tutorialObject.SetActive(false);

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveTutorial = true;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (moveTutorial)
        {
            jumpTutorial = true;
        }
    }
    private void OnSlicUp(InputAction.CallbackContext context)
    {
        if (jumpTutorial)
        {
            SlicUpTutorial = true;
        }
    }
    private void OnBlowOut(InputAction.CallbackContext context)
    {
        if (SlicUpTutorial)
        {
            BlowOutTutorial = true;
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (BlowOutTutorial)
        {
            SelectTutorial = true;
        }
    }



}
