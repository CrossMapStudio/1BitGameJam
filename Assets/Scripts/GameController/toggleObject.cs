using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleObject : MonoBehaviour
{
    public bool darkPlatform;
    internal Collider2D coll;


    public void Start()
    {
        GameController.addToggleObjects(darkPlatform, this);
        coll = GetComponent<Collider2D>();
    }

    public void toggle(bool value)
    {
        coll.enabled = value;
    }
}
