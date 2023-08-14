using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class buttonController : MonoBehaviour
{
    private Animator anim;
    public Action buttonAssignedAction;


    public void Awake()
    {
        anim = transform.parent.GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetBool("playerOn", true);
        if (buttonAssignedAction != null)
            buttonAssignedAction();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("playerOn", false);
    }
}
