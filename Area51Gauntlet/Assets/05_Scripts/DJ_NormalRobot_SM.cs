using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class DJ_NormalRobot_SM : ByTheTale.StateMachine.MachineBehaviour {

    
    //bools to toggle states
    bool paused = false;

    bool isBecomingEnraged = false;

    // holds the last color
    private Color lastColor;

    //accesses navagent script
    NavAgent myNavAgent;

    ByTheTale.StateMachine.State lastState = null;

    //add new states
    public override void AddStates()
    {
        throw new NotImplementedException();
        AddState<Chase>();
        AddState<Idle>();
        AddState<BecomeAngry>();
        AddState<Pause>();

        SetInitialState<Chase>();
    }

  // the pause function
  public void Pause()
    {
        //toggle paused value
        paused = !paused;

        if (paused)
        {
            //store current state for use when unpausing 
            lastState = currentState;

            //change state to pause
            ChangeState<Pause>();
        }

        else
        {
            //restore state when pausing earlier
            ChangeState(lastState.GetType());
        }
    }

    public void BecomeAngry()
    {
        //toggle is the bool 
        isBecomingAngry = !isBecomingAngry;

        //if the bool is true 
        if (isBecomingAngry)
        {
            ChangeState<BecomeAngry>();
        }
        // if it is false 
        else
        {
            ChangeState<CalmingState>();
        }

    }
}
public class GuardState : ByTheTale.StateMachine.State
{
    protected NavAgent Guard;

    NavPoint[] ArrayPoints;

    // sets up our base enter function 
    public override void Enter()
    {
        // finds the script
        Guard = GetMachine<DJ_NormalRobot_SM>().GetComponent<NavAgent>();

        //accesses array of nav points 
        ArrayPoints = new NavPoint[Guard.GetArrayLength()];
        for (int index = 0; index < Guard.GetMyNavPoints(); index++)
        {
            ArrayPoints[index] = Guard.GetNavPoints(index);
        }
        //resets the transition timer
        Guard.ResetTimeSinceLastTransition();

    }

}
/*
public class Pause : GuardState
{
    public override void Enter()
    {
        base.Enter();
        // change guard to gray when paused 
        Guard.GetComponent<Renderer>().material.color = Color.gray;
    }
    public override void Execute()
    {
        Guard.CurDestination();
    }
}*/
