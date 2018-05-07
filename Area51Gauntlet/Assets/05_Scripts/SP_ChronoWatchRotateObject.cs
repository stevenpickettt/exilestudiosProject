using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SP_ChronoWatchRotateObject : MonoBehaviour {
	[Range(-1,1)]
	public float rotSpeed = 0.25f; 
	// Use this for initialization
	void Start () {
		rotSpeed = rotSpeed += Time.deltaTime; 
	}
	
	// Update is called once per frame
	void Update () {
		
		gameObject.transform.Rotate (0, 0, rotSpeed);  
		
	}
}
