using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SR_DoorKey : MonoBehaviour {

	public bool GotKey = false;
	public GameObject Key ;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
			
		{
			GotKey = true;
			Destroy (Key.gameObject);
		}
	}


}