using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animator;
    PlayerManager playerManager;
    PlayerLocomotionManager playerLocomotionManager;

    float snappedHorizontal;
    float snappedVertical;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }
    public void PlayAnimationWithRootMotion(string targetAnimation, bool isPreformingAction, bool disableRootMotion = false)
    {
        animator.SetBool("isPreformingAction", isPreformingAction); // Set the "isInteracting" parameter
        animator.SetBool("disableRootMotion", disableRootMotion);
        animator.applyRootMotion = true;
        animator.CrossFade(targetAnimation, 0.2f); // Crossfade to the target animation with a duration of 0.2 seconds
    }
    
    public void PlayAnimationWithoutRootMotion(string targetAnimation, bool isPreformingAction)
    {
        animator.SetBool("isPreformingAction", isPreformingAction);
        animator.SetBool("disableRootMotion", true);
        animator.applyRootMotion = false;
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void HandleAnimatorValues (float horizontalMovement, float verticalMovement, bool isRunning)
    {
        #region SnappedValues
        if (horizontalMovement > 0)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalMovement > 0)
        {
            snappedVertical = 1;
        }
        else if(verticalMovement < 0)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion
        
        if (isRunning && snappedVertical > 0) // we dont want to be able to run backwards
        {
            snappedVertical = 2;
        }

       
        animator.SetFloat("Horizontal", snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", snappedVertical, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.disableRootMotion)
            return;

        playerLocomotionManager.playerRigidbody.drag = 0;
        
        Vector3 deltaPosition = Vector3.Lerp(Vector3.zero, animator.deltaPosition, 0.9f);
        transform.position += deltaPosition;

        Quaternion deltaRotation = animator.deltaRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * deltaRotation, 0.9f);

        Vector3 smoothDeltaPosition = deltaPosition;
        smoothDeltaPosition.y = 0;

        Vector3 velocity = smoothDeltaPosition / Time.deltaTime;

        playerLocomotionManager.playerRigidbody.velocity = velocity;
    }
}
