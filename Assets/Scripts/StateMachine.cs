using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    #region StateMachine/State
    private State currentState, previousState;

    public void changeState(State newState)
    {
        if (currentState != null)
        {
            this.currentState.onExit();
        }
        this.previousState = this.currentState;
        this.currentState = newState;
        this.currentState.onEnter();
    }

    public void executeStateUpdate()
    {
        var runningState = this.currentState;
        if (runningState != null)
        {
            this.currentState.onUpdate();
        }
    }

    public void executeStateFixedUpdate()
    {
        var runningState = this.currentState;
        if (runningState != null)
        {
            this.currentState.onFixedUpdate();
        }
    }

    /*
    public void executeStateLateUpdate()
    {
        var runningState = this.currentState;
        if (runningState != null)
        {
            this.currentState.onLateUpdate();
        }
    }
    */

    public void previousStateSwitch()
    {
        if (this.previousState != null)
        {
            this.currentState.onExit();
            this.currentState = this.previousState;
            this.currentState.onEnter();
        }
        else
        {
            return;
        }
    }

    //To Allow Us to Check for the State
    public bool checkCurrentStateComp(string comp)
    {
        if (comp == currentState.ToString())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}

public interface State
{
    public string stateName { get; }
    public void onEnter();
    public void onUpdate();
    public void onFixedUpdate();
    public void onExit();
}
