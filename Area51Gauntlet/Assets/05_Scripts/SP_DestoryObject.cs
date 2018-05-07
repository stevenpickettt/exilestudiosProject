using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SP_DestoryObject : MonoBehaviour {

	private PlayerController playerController;
	private BR_EnemyHealth enemyHealth; 
	private GameObject singleFireSP; 
	private GameObject autoFireSP; 
	private GameObject scatterFireSP; 
	private GameObject explosiveDroneSP; 



	public enum MELEETYPE {DEFAULT = 0, MELEE = 1, EMPEXPLOSION = 2, LASER = 3, BOSSEXPLOSION =4, HEALINGTURRET =5, SingleFirePS =6, AutoFirePS =7, ScatterFirePS =8, ExplosiveDrone =9, StunPS =10, Guns=11};
	public MELEETYPE MeleeType = MELEETYPE.DEFAULT;

	public float speed; 
	private GameObject meleeSP;

	public GameObject myGunHolder; 

    public float bossExplosionDuration = 2f; 
	public float healingTurretDuration = 5f; 
	[Space]
    [SerializeField]
	private Vector3 maxScale = new Vector3(1.05f, 1.05f, 1.05f); 
	// Use this for initialization
	void Awake () {
		if (MeleeType == MELEETYPE.StunPS) {
			enemyHealth = FindObjectOfType<BR_EnemyHealth> (); 
		}

		playerController = FindObjectOfType<PlayerController>();
		if (MeleeType == MELEETYPE.SingleFirePS) {
			singleFireSP = GameObject.Find ("SingleFireSpawnPoint"); 
		}
		if (MeleeType == MELEETYPE.AutoFirePS) {
			autoFireSP = GameObject.Find ("AutoFireSpawnPoint"); 
		}
		if (MeleeType == MELEETYPE.ScatterFirePS) {
			scatterFireSP = GameObject.Find ("ScatterFireSpawnPoint"); 
		}


		meleeSP = GameObject.FindGameObjectWithTag("MeleeVolumeSP"); 

	}
	
	// Update is called once per frame
	void Update () {

		if (MeleeType == MELEETYPE.Guns) {

			transform.position = myGunHolder.transform.position; 
			
		}
		
		if (MeleeType == MELEETYPE.MELEE)
        {
			MoveTowardsPlayerMelee (); 
			Destroy (gameObject, playerController.curMeleeDuration);
		
		}

        if (MeleeType == MELEETYPE.EMPEXPLOSION)
        {
			MoveTowardsPlayerEMP (); 
			if (transform.localScale != maxScale)
            {
				
				transform.localScale = Vector3.Scale (transform.localScale, maxScale); 


			}


			Destroy (gameObject, playerController.curEmpExplosionDur);
		}

        if (MeleeType == MELEETYPE.LASER)
        {
			LookAtPlayerLaser (); 
		}

        if(MeleeType == MELEETYPE.BOSSEXPLOSION)
        {
            Destroy(gameObject, bossExplosionDuration);
        }
		if (MeleeType == MELEETYPE.HEALINGTURRET) {
			Destroy (gameObject, healingTurretDuration); 
		}

		if (MeleeType == MELEETYPE.SingleFirePS) {
			MoveTowardsSingleFireSP (); 
		}
		if (MeleeType == MELEETYPE.AutoFirePS) {
			MoveTowardsAutoFireSP (); 
		}
		if (MeleeType == MELEETYPE.ScatterFirePS) {
			MoveTowardsScatterFireSP (); 
		}
		if (MeleeType == MELEETYPE.ExplosiveDrone) {
			Destroy (gameObject, 0.5f); 
		}

			if (MeleeType == MELEETYPE.StunPS) {
				Destroy (gameObject, .25f); 
			}
		

	}

	void MoveTowardsPlayerEMP(){
		float step = speed * Time.deltaTime; 
		transform.position = Vector3.MoveTowards (transform.position, playerController.transform.position, step); 

	}
	void MoveTowardsPlayerMelee(){
		float step = speed * Time.deltaTime; 
		transform.position = Vector3.MoveTowards (transform.position, meleeSP.transform.position, step);
        transform.rotation = meleeSP.transform.rotation;

	}

	void MoveTowardsSingleFireSP(){
		float step = speed * Time.deltaTime; 
		transform.position = Vector3.MoveTowards (transform.position, singleFireSP.transform.position, step); 

	}
	void MoveTowardsAutoFireSP(){
		float step = speed * Time.deltaTime; 
		transform.position = Vector3.MoveTowards (transform.position, autoFireSP.transform.position, step); 

	}
	void MoveTowardsScatterFireSP(){
		float step = speed * Time.deltaTime; 
		transform.position = Vector3.MoveTowards (transform.position, scatterFireSP.transform.position, step); 

	}
	void LookAtPlayerLaser(){
		/*
		float step = speed * Time.deltaTime; 
		transform.LookAt (playerController.gameObject.transform.position); 
		*/ 
		Transform target = playerController.GetComponent<Transform> (); 

		Vector3 relativePos = target.position - transform.position; 
		Quaternion rotation = Quaternion.LookRotation (relativePos);
		transform.rotation = rotation; 

	}


}

