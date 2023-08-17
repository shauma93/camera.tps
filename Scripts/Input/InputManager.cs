using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    Animator animator;
    PlayerManager playerManager;

    [Header("Player Movement Input")]
    public float verticalMovementInput;
    public float horizontalMovementInput;
    private Vector2 movementInput;

    [Header("Camera Rotation Input")]
    public float verticalCameraInput;
    public float horizontalCameraInput;
    private Vector2 cameraInput;

    [Header("Button/Action Input")]
    public bool runInput;
    public bool quickTurnInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Run.performed += i => runInput = true;
            playerControls.PlayerMovement.Run.canceled += i => runInput = false;
            playerControls.PlayerMovement.QuickTurn.performed += i => quickTurnInput = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleCameraInput();
        HandleQuickTurnInput();
    }

    private void HandleMovementInput()
    {
        horizontalMovementInput = movementInput.x;
        verticalMovementInput = movementInput.y;

        animatorManager.HandleAnimatorValues(horizontalMovementInput, verticalMovementInput, runInput);
    }

    private void HandleCameraInput()
    {
        horizontalCameraInput = cameraInput.x;
        verticalCameraInput = cameraInput.y;
    }

    private void HandleQuickTurnInput()
    {
        if (playerManager.isPreformingAction)
            return;

        if (quickTurnInput)
        {
            animator.SetBool("isPreformingQuickTurn", true);
            animatorManager.PlayAnimationWithoutRootMotion("QuickTurn", true);
        }
    }
}
