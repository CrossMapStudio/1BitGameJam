using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerColorController : MonoBehaviour
{
    public static SpriteRenderer renderer;

    public void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
}
