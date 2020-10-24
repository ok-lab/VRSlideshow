using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControlForSAFBIK : MonoBehaviour
{
    SA.FullBodyIKBehaviour fullBodyIKBehaviour;
    MyVRPlayerController controller;

    private void Awake()
    {
        fullBodyIKBehaviour = GetComponent<SA.FullBodyIKBehaviour>();
        controller = GetComponent<MyVRPlayerController>();
    }

    private void LateUpdate()
    {
        if (controller.v == 0 && controller.h == 0)
        {
            fullBodyIKBehaviour.enabled = true;
        }
        else
        {
            fullBodyIKBehaviour.enabled = false;
        }
    }
}
