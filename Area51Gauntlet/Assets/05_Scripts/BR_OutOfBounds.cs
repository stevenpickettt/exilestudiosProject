using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_OutOfBounds : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider _target)
    {
        if (_target.tag == "Player")
        {
            _target.GetComponent<PlayerController>().TeleportPlayer(_target.GetComponent<PlayerController>().lastLocation);
            Debug.Log("Player was out of bounds, teleported to last known location");
        }
    }
}
