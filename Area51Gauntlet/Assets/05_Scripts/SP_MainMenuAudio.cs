using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 


public class SP_MainMenuAudio : MonoBehaviour {

	public AudioClip pressedAudio; 
	public AudioClip highlightAudio; 
	private AudioSource myAudio; 



	// Use this for initialization
	void Start () {
		
		myAudio = GetComponent<AudioSource> (); 

	}

	void Update (){
		
	}


	public void TaskOnClick(){


		myAudio.clip = pressedAudio;
		myAudio.Play (); 
	}

	public void TaskOnHighlight(){

		myAudio.clip = highlightAudio;
		myAudio.Play (); 
	}
}
