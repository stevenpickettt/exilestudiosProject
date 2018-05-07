﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DJ_ExoGuard : MonoBehaviour {

		//-----------------------------------
	public enum AISTATE {IDLE=0,CHASE=1,ATTACK=2};
	public AISTATE CurrentState = AISTATE.IDLE;
	private NavMeshAgent ThisAgent = null;
	private Transform ThisTransform = null;
	private Transform PlayerObject = null;

	//AI Visiility Settings
	public bool CanSeePlayer = false;
	public float ViewAngle = 90f;
	public float AttackDistance = 1f;

    //AI Attack Variables
    public GameObject meleeVolume;
    [SerializeField]
    private float meleeDuration;
    [SerializeField]
    private float curMeleeDuration;
    [SerializeField]
    private bool isMeleeActive = false;

    
    public KW_Enemy_Projectile projectile;
    [SerializeField]
    private float fireRate = 1.5f;
    [SerializeField]
    private float fireTimer; 
    [SerializeField]
    private bool isFiringActive = false;
    

    //-----------------------------------
    // Use this for initialization
    void Awake () 
	{
		ThisAgent = GetComponent<NavMeshAgent> ();
		ThisTransform = GetComponent<Transform>();
		PlayerObject = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
	}
	//-----------------------------------
	void Start()
	{
		//Set Starting State
		ChangeState (CurrentState);
	}
	//-----------------------------------
	public IEnumerator Idle()
	{
		//Get Random Point
		Vector3 Point = RandomPointOnNavMesh();
		float WaitTime = 2f;
		float ElapsedTime = 0f;

		//Loop while idling
		while(CurrentState == AISTATE.IDLE)
		{
			ThisAgent.SetDestination (Point);

			ElapsedTime += Time.deltaTime;

			if(ElapsedTime >= WaitTime)
			{
				ElapsedTime = 0f;
				Point = RandomPointOnNavMesh();
			}

			if(CanSeePlayer)
			{
				ChangeState (AISTATE.CHASE);
				yield break;
			}

			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Chase()
	{
		while(CurrentState == AISTATE.CHASE)
		{
			ThisAgent.SetDestination (PlayerObject.position);

			if(!CanSeePlayer)
			{
				yield return new WaitForSeconds (2f);

				if(!CanSeePlayer)
				{
                    ChangeState (AISTATE.IDLE);
                    //ChangeState(AISTATE.CHASE);
					yield break;
				}
			}

			if(Vector3.Distance (ThisTransform.position, PlayerObject.position) <= AttackDistance)
			{
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Attack()
	{
		while(CurrentState == AISTATE.ATTACK)
		{
            //Deal damage here
            /*     if(curMeleeDuration <= 0)
                 {
                     TurnOnMelee();
                 }
                 if (isMeleeActive)
                 {
                     DoMelee();
                 }
              */
            FireWeapon();
           
               
			if(!CanSeePlayer || Vector3.Distance (ThisTransform.position, PlayerObject.position) > AttackDistance)
			{
               // TurnOffMelee();
				ChangeState (AISTATE.CHASE);
			}

			yield return null;
		}
	}
	//-----------------------------------
	public void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch(NewState)
		{
			case AISTATE.IDLE:
				StartCoroutine (Idle());
			break;

			case AISTATE.CHASE:
				StartCoroutine (Chase());
			break;

			case AISTATE.ATTACK:
				StartCoroutine (Attack());
			break;
		}
	}
	//-----------------------------------
	void OnTriggerStay(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

		CanSeePlayer = false;

		//Player transform
		Transform PlayerTransform = Col.GetComponent<Transform>();

		//Is player in sight
		Vector3 DirToPlayer = PlayerTransform.position - ThisTransform.position;

		//Get viewing angle
		float ViewingAngle = Mathf.Abs(Vector3.Angle(ThisTransform.forward, DirToPlayer));

		if(ViewingAngle > ViewAngle)
			return;

		//Is there a direct line of sight?
		if(!Physics.Linecast(ThisTransform.position, PlayerTransform.position))
			CanSeePlayer = true;
	}
	//-----------------------------------
	public Vector3 RandomPointOnNavMesh()
	{
		float Radius = 5f;
		Vector3 Point = ThisTransform.position + Random.insideUnitSphere * Radius;
		NavMeshHit NH;
		NavMesh.SamplePosition (Point, out NH, Radius, NavMesh.AllAreas);
		return NH.position;
	}
	//-----------------------------------
	void OnTriggerExit(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

        CanSeePlayer = false;
        
	}
	//-----------------------------------

    void DoMelee()
    {
        curMeleeDuration -= Time.deltaTime;
        if(curMeleeDuration <= 0)
        {
            meleeVolume.SetActive(false);
            isMeleeActive = false;

        }
    }

    void TurnOnMelee()
    {
        curMeleeDuration = meleeDuration;
        meleeVolume.SetActive(true);
        isMeleeActive = true;
     
    }

    void TurnOffMelee()
    {
        meleeVolume.SetActive(false);
        isMeleeActive = true;
        curMeleeDuration = 0;
       
    }
    //-----------------------------------

    void DoScatterShot()
    {
         if (fireTimer <= 0)
        {
            KW_Enemy_Projectile newBullet1 = Instantiate(projectile, transform.position, transform.rotation) as KW_Enemy_Projectile;
            KW_Enemy_Projectile newBullet2 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, 7.5f, 0)) as KW_Enemy_Projectile;
            KW_Enemy_Projectile newBullet3 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, -7.5f, 0)) as KW_Enemy_Projectile;
          
            fireTimer = fireRate;
        }
    }

    void FireWeapon()
    {
        if (fireTimer > 0)
        fireTimer -= Time.deltaTime;
       
        {   
            DoScatterShot(); 
        }
          
    }
}
