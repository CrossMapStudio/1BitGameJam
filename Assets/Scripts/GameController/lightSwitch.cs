using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSupportLibrary;

public class lightSwitch : MonoBehaviour
{
    public void Update()
    {
        if (Vector3.Distance(TaroController.controller.transform.position, transform.position) < 1f && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("LIGHTS");
            GameController.toggleLights();
        }
    }
}
