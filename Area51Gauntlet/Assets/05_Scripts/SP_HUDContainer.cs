using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_HUDContainer : MonoBehaviour {

	public GameObject HUD; 
	public bool isPausePanel = false; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isPausePanel == true) {
			HUD.SetActive (false); 
		}

		if (isPausePanel == false) {
			HUD.SetActive (true);
		}
	}
}
