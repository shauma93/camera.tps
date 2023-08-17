using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;

    [HideInInspector]public Rigidbody playerRigidbody;

    [Header("Camera Transform")][Space(5)]
    public Transform playerCamera;
    
    [Header("Custom Gravity")][Space(5)]
    public float gravity = -9.81f; // Adjust this value to control the strength of gravity

    [Header("Movement Speed")][Space(5)]
    public float rotationSpeed = 3.5f;
    public float quickTurnSpeed = 8;

    
    // Rotation Variables
    Quaternion targetRotation; 
    Quaternion playerRotation; 
    
    
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void HandleAllLocomotion()
    {
        HandleRotation(); 
        ApplyGravity();
        
    }
    
    private void HandleRotation()
    {
        targetRotation = Quaternion.Euler(0, playerCamera.eulerAngles.y, 0);
        playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (inputManager.verticalMovementInput != 0 || inputManager.horizontalMovementInput != 0)
        {
            transform.rotation = playerRotation;
        }

        if (playerManager.isPreformingQuickTurn)
        {
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, quickTurnSpeed * Time.deltaTime);
            transform.rotation = playerRotation;
        }
    }
    
    private void ApplyGravity()
    {
        Vector3 gravityVector = new Vector3(0, gravity, 0);
        playerRigidbody.AddForce(gravityVector, ForceMode.Acceleration);
    }

}


    
