using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HANDLES PLAYER CAMERA
/*
Camera hierarchy - Camera Manager(empty) -> Camera Pivot(empty) -> Main Camera.
Attach CameraManager.cs to Camera Manager Object.

Camera Manager - reset to zero, doesn't matter for transform values as it will always find player.
Camera Pivot - modify Y transform position value for camera height. (Position - Y = 1.8)
Main Camera - modify Z transform position for distance, X for left/right position. (Position - Z = -1.75)
 */
public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;

    [Header("Camera Module")] [Space(5)]
    [Tooltip("The transform of the target object (player character).")]
    public Transform targetTransform; // The transform of the target object (player character)
    
    [Tooltip("The pivot for the camera's rotation.")]
    public Transform cameraPivot; // The pivot for the camera's rotation
    
    [Tooltip("The transform of the main camera.")]
    public Transform cameraTransform; // The transform of the main camera
    
    [Tooltip("Layers to consider for camera collision.")]
    public LayerMask collisionLayers; // Layers to consider for camera collision
    private float defaultPosition; // The default local Z position of the camera
    private Vector3 cameraFollowVelocity = Vector3.zero; // Velocity for camera follow smoothing
    private Vector3 cameraVectorPosition; // Vector to store camera position
    
    [Header("Camera Collisions")] [Space(5)]
    [Tooltip("Offset to prevent camera clipping through objects.")]
    public float cameraCollisionOffset = 0.2f;  // Offset to prevent camera clipping through objects
    
    [Tooltip("Minimum offset to avoid camera getting too close.")]
    public float minimumCollisionOffset = 0.2f; // Minimum offset to avoid camera getting too close
    
    [Tooltip("Radius for sphere cast to check for collisions.")]
    public float cameraCollisionRadius = 0.2f;  // Radius for sphere cast to check for collisions

    [Header("Camera Speed")] [Space(5)]
    [Tooltip("Speed of camera following the target.")]
    public float cameraFollowSpeed = 0.04f;     // Speed of camera following the target
    
    [Tooltip("Speed of camera rotation left/right.")]
    public float cameraLookSpeed = 35;         // Speed of camera rotation left/right
    
    [Tooltip("Speed of camera pivot rotation.")]
    public float cameraPivotSpeed = 35;        // Speed of camera pivot rotation
    
    [Tooltip("Smoothing time for camera rotation.")]
    public float cameraLookSmoothTime = 1;     // Smoothing time for camera rotation

    [Header("Camera Pivot Angles")] [Space(5)]
    [Tooltip("Vertical angle of camera (looking up and down).")]
    public float lookAngle;       // Vertical angle of camera (looking up and down)
    
    [Tooltip("Horizontal angle of camera (looking left and right).")]
    public float pivotAngle;      // Horizontal angle of camera (looking left and right)
    
    [Tooltip("Minimum vertical angle for camera pivot.")]
    public float minimumPivotAngle = -35; // Minimum vertical angle for camera pivot
    
    [Tooltip("Maximum vertical angle for camera pivot.")]
    public float maximumPivotAngle = 35;  // Maximum vertical angle for camera pivot

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();  // Find and store InputManager component
        targetTransform = FindObjectOfType<PlayerManager>().transform; // Find and store player's transform
        cameraTransform = Camera.main.transform; // Find and store main camera's transform
        defaultPosition = cameraTransform.localPosition.z; // Store the default local Z position of the camera
    }

    public void HandleAllCameramovement()
    {
        FollowTarget();    // Update camera position to follow the target
        RotateCamera();    // Rotate the camera based on input
        HandleCameraCollisions(); // Handle camera collisions with objects in the scene
    }

    private void FollowTarget()
    {
        // Calculate a smooth follow position for the camera
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position,
                                                    ref cameraFollowVelocity, cameraFollowSpeed * Time.deltaTime);

        transform.position = targetPosition; // Update the camera's position to the calculated target
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        // Update the vertical angle of camera rotation based on input
        lookAngle = Mathf.Lerp(lookAngle, lookAngle + (inputManager.horizontalCameraInput * cameraLookSpeed),
                               cameraLookSmoothTime * Time.deltaTime);
        
        // Update the horizontal angle of camera pivot rotation based on input
        pivotAngle = Mathf.Lerp(pivotAngle, pivotAngle - (inputManager.verticalCameraInput * cameraPivotSpeed),
                                cameraLookSmoothTime * Time.deltaTime);
        
        // Clamp the vertical pivot angle within the specified range
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);
        
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        //targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraLookSmoothTime);
        transform.rotation = targetRotation; // Apply the rotation to the camera

        // if we are performing a quick turn, we need to swap our camera 180
        if (inputManager.quickTurnInput)
        {
            inputManager.quickTurnInput = false;
            lookAngle = lookAngle + 180;
            rotation.y = rotation.y + 180;
            transform.rotation = targetRotation;
        }
        
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        //targetRotation = Quaternion.Slerp(cameraPivot.localRotation, targetRotation, cameraLookSmoothTime);
        cameraPivot.localRotation = targetRotation; // Apply the pivot rotation to the camera pivot
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition; // Initialize the target position with the default
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        // Check for collisions using a SphereCast to prevent camera clipping
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction,
                               out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = - (distance - cameraCollisionOffset); // Adjust target position based on collision
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset; // Prevent camera from getting too close
        }

        // Smoothly adjust the camera's local Z position to the calculated target
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition; // Apply the updated position to the camera
    }
}
