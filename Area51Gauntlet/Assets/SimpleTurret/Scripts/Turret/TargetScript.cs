using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour {

    [SerializeField]
    private ShootingSystem turretShooting;

    // Use this for initialization
    void Start () {
		 
        gameObject.transform.localScale = new Vector3(turretShooting.range*2, turretShooting.range*2, turretShooting.range*2);
    }
	
	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            turretShooting.target = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            turretShooting.target = null;
        }
    }
}
