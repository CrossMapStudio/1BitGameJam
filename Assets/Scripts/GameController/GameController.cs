using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool LightsOn = false;
    public static List<toggleObject> darkPlatforms;
    public static List<toggleObject> lightPlatforms;

    public void Awake()
    {
        darkPlatforms = new List<toggleObject>();
        lightPlatforms = new List<toggleObject>();
    }

    public void Start()
    {
        toggleObjects();
    }

    public static void addToggleObjects(bool darkPlatform, toggleObject obj)
    {
        if (darkPlatform)
        {
            darkPlatforms.Add(obj);
        }
        else
        {
            lightPlatforms.Add(obj);
        }
    }

    public static void toggleLights()
    {
        LightsOn = !LightsOn;

        if (LightsOn)
        {
            Camera.main.backgroundColor = Color.white;
            playerColorController.renderer.color = Color.black;
        }
        else
        {
            Camera.main.backgroundColor = Color.black;
            playerColorController.renderer.color = Color.white;
        }

        toggleObjects();
    }

    public static void toggleObjects()
    {
        if (LightsOn)
        {
            foreach(toggleObject element in darkPlatforms)
            {
                element.toggle(true);
            }

            foreach(toggleObject element in lightPlatforms)
            {
                element.toggle(false);
            }
        }
        else
        {
            foreach (toggleObject element in darkPlatforms)
            {
                element.toggle(false);
            }

            foreach (toggleObject element in lightPlatforms)
            {
                element.toggle(true);
            }
        }
    }
}
