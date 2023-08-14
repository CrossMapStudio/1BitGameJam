using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorChanger : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private SpriteRenderer renderer;
    private colorSwapElement colorSwapper;

    bool currentLightValue;

    public void Awake()
    {
        currentLightValue = GameController.LightsOn;


        TryGetComponent<ParticleSystem>(out particleSystem);
        TryGetComponent<SpriteRenderer>(out renderer);
        if (particleSystem != null)
        {
            colorSwapper = new particleSwap(particleSystem);
        }
        else
        {
            colorSwapper = new rendererSwap(renderer);
        }
    }

    public void Update()
    {
        if (GameController.LightsOn != currentLightValue)
        {
            colorSwapper.changeColor();
            currentLightValue = GameController.LightsOn;
        }
    }
}

public interface colorSwapElement
{
    public void changeColor();
}


public class particleSwap : colorSwapElement
{
    ParticleSystem system;
    public particleSwap(ParticleSystem _system)
    {
        system = _system;
    }

    public void changeColor()
    {

    }
}

public class rendererSwap : colorSwapElement
{
    SpriteRenderer rend;
    public rendererSwap(SpriteRenderer _rend)
    {
        rend = _rend;
    }

    public void changeColor()
    {
        if (GameController.LightsOn)
        {
            rend.color = Color.black;
        }
        else
        {
            rend.color = Color.white;
        }
    }
}