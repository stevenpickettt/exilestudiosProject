using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryTurret : MonoBehaviour {

	public GameObject myTurret; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (myTurret.activeInHierarchy == false) {
			Destroy (gameObject); 
		}
	}
}
