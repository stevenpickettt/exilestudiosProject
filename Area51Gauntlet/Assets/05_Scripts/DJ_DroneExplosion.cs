using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_DroneExplosion : MonoBehaviour {

	private BR_EnemyHealth EnemyHealth; 

    public int explosionDamage = 25;


	// Use this for initialization
	void Start () {
		EnemyHealth = FindObjectOfType<BR_EnemyHealth> (); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider target)
    {
		/*
		if (target.tag == "Enemy")
        {
				EnemyHealth.TakeDamage (explosionDamage); 
		}
		*/ 

	}

  


}
