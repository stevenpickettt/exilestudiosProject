using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_Animation : MonoBehaviour {

	public enum ANIMATION { NONE = 0, SPIDER = 1, EXPLOSIVEDRONE = 2 };
	public ANIMATION CurrentState = ANIMATION.NONE;

    private AIAgent aiAgent;
    private Animator myAnimator; 
    


	// Use this for initialization
	void Start ()
    {
		myAnimator = GetComponent<Animator>();
        aiAgent = FindObjectOfType<AIAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (CurrentState == ANIMATION.SPIDER) {
			if (aiAgent.CurrentState == AIAgent.AISTATE.CHASE) {
				myAnimator.SetBool ("IsWalking", true);

			} else {
				myAnimator.SetBool ("IsWalking", false);

			}
		}


	}

	public void StartExplosiveDroneAnimation(){
		myAnimator.SetBool("DoExplosion", true);
	}
	public void StopExplosiveDroneAnimation(){
		myAnimator.SetBool("DoExplosion", false);
	}
}
