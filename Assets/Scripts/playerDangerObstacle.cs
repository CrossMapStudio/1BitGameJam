using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSupportLibrary;
public class playerDangerObstacle : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            TaroController.controller.playerDead(); 
        }
    }
}
