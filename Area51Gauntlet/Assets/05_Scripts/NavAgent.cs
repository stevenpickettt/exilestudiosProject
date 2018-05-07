using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class NavAgent : MonoBehaviour {


    //accesses the navmesh agent 
    [SerializeField]
    NavMeshAgent myNavAgent;

    // the time the guard must wait at each navpoint
    [SerializeField]
    float idleTimer = 5.0f;

    //the timer that starts when the guard hits the nav point
    [SerializeField]
    float timePassed;

    //checks to see if the nav point was hit to set off timer
    [SerializeField]
    private bool hitNavPoint = false;

    // the array of nav points in the scene, allows us to drop them via inspector 
    [SerializeField]
    private NavPoint[] myNavPoints;

    // int that allows us to determine which nav point we are at in the arrray 
    [SerializeField]
    int navIndex = 0;

    //give access to the statemachine
    //DJ_NormalRobot_SM target;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
