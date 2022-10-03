using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
    protected Animator animator;

    private bool ikActive = false;

    private Transform rightHandPick = null;
    private Transform leftHandPick = null;
    private Camera mainCamera;
    void Start ()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    } 
    

    //a callback for calculating IK
    private void OnAnimatorIK()
    {
        //if the IK is active, set the position and rotation directly to the goal.
        animator.SetLookAtWeight(1f, 0f, 1f);
        Ray lookAtRay = new Ray(transform.position, mainCamera.transform.forward);
        animator.SetLookAtPosition(lookAtRay.GetPoint(25));
        if(ikActive) {
        // Set the look target position, if one has been assigned
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPick.position);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPick.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPick.rotation);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPick.rotation);

        }

        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            //animator.SetLookAtWeight(0);
        }
    }

    public void PickUp(Transform rightPos, Transform leftPos)
    {
        leftHandPick = leftPos;
        rightHandPick = rightPos;
        ikActive = true;
    }
    public void Drop()
    {
        leftHandPick = null;
        rightHandPick = null;
        ikActive = false;
    }

}