using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIKControl : MonoBehaviourPun
{
    protected Animator animator;

    public bool ikActive = true;

    // IK Target
    [SerializeField] public Transform targetLookAt = null;
    [SerializeField] public Transform targetHandLeft = null;
    [SerializeField] public Transform targetHandRight = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnAnimatorIK()
    {
        if (photonView.IsMine)
        {
            //Debug.Log("is my IK");
        }
        else
        {
            //Debug.Log("is Not my IK");
        }

        if (animator)
        {
            // IK が有効ならば、位置と回転を直接設定します
            if (ikActive)
            {

                // すでに指定されている場合は、視線のターゲット位置を設定します
                if (targetLookAt != null)
                {
                    animator.SetLookAtWeight(1, 0, 1, 1, 1);
                    animator.SetLookAtPosition(targetLookAt.position);
                }
                // 指定されている場合は、右手のターゲット位置と回転を設定します
                if (targetHandRight != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, targetHandRight.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, targetHandRight.rotation);
                }

                // 指定されている場合は、右手のターゲット位置と回転を設定します
                if (targetHandLeft != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, targetHandLeft.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, targetHandLeft.rotation);
                }
            }
        }
    }
}
