using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatformController : MonoBehaviour
{
    public Vector3 startTarget, endTarget, currentTarget;
    public Transform platform;
    public bool loopMovement;
    public List<buttonController> assignedButtons;

    public void Awake()
    {
        if (loopMovement)
            return;

        foreach(buttonController element in assignedButtons) 
        {
            element.buttonAssignedAction = movePlatform;
        }
    }

    public void movePlatform()
    {
        if (currentTarget == startTarget)
            currentTarget = endTarget;
        else
            currentTarget = startTarget;
    }

    public void LateUpdate()
    {
        if (platform.transform.localPosition != currentTarget)
        {
            platform.localPosition = Vector3.MoveTowards(platform.localPosition, currentTarget, Time.deltaTime * 5f);
        }
    }
}
