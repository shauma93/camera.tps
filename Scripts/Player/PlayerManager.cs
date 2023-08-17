using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    CameraManager cameraManager;
    InputManager inputManager;
    Animator animator;
    PlayerLocomotionManager playerLocomotionManager;

    [Header("Player Flags")]
    public bool disableRootMotion;
    public bool isPreformingAction;
    public bool isPreformingQuickTurn;

    private void Awake()
    {
        cameraManager = FindAnyObjectByType<CameraManager>();
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();

        disableRootMotion = animator.GetBool("disableRootMotion");
        isPreformingAction = animator.GetBool("isPreformingAction");
        isPreformingQuickTurn = animator.GetBool("isPreformingQuickTurn");
    }

    private void FixedUpdate()
    {
        playerLocomotionManager.HandleAllLocomotion();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameramovement();
    }
}
