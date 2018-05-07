using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//-----------------------------------
public class AIAgent : MonoBehaviour
{
    //-----------------------------------
    public enum AISTATE { IDLE = 0, CHASE = 1, ATTACK = 2, HEALING = 3, ATTACKTHRUSTERA = 4, ATTACKTHRUSTERB = 5, BOSSATTACK =6, STUN=7, ATTACKTURRET=8 };
	public AISTATE CurrentState = AISTATE.IDLE;
	private NavMeshAgent ThisAgent = null;
	private Transform ThisTransform = null;
    GameObject player;
    public Transform playerTransform = null;
    private Transform ThrusterObjectA = null;
	private Transform ThrusterObjectB = null;
    private GameObject TurretObject = null;

	public enum ENEMYTYPE {DEFAULT = 0,REGULAREXO =1,EXPLOSIVEDRONE =2, HEALINGTURRET =3, TRILOGYEXO =4, BOSS1=5, BOSS2=6, REGTURRET=7, FRIENDLYTURRET =8};
    public ENEMYTYPE EnemyType = ENEMYTYPE.DEFAULT;

    //reference Scripts
    private BR_EnemyHealth enemyHealth;

    [Space]
    [Header("Visablity")]
    //AI Visiility Settings
	public bool CanSeePlayer = false;
	public float ViewAngle = 90f;
	public float AttackDistance = 1f;
    public float boss1AttackDistance = 3f; 


    //Thruster info 
    [Space]
    [Header("Thrusters")]
    public bool CanSeeThrusterA = false;
	public bool CanSeeThrusterB = false; 
    public float AttackThrusterDistance = 4f;
    public bool isAtStateActive;
    public float atTimer = 2f;

    public bool CanSeeTurret = false;

    [Space]
    [Header("AttackVariables")]
    //AI Attack Variables
    public GameObject meleeVolume;
    [SerializeField]
    private float meleeDuration;
    [SerializeField]
    private float curMeleeDuration;
    private float curMeleeCooldown =0;
    [SerializeField]
    private bool isMeleeActive = false;

    [Space]
    [Header("FireRates")]
    //Shooting Variables 
    public KW_Enemy_Projectile projectile;
    [SerializeField]
    private float fireRate = 1.5f;
    [SerializeField]
    public float fireTimer;
    [SerializeField]
    private bool isFiringActive = false;

    
    [Space]
    [Header("HealingTurret")]
    //healing turret 
    private bool isHealingCoolDown = false;
    [SerializeField]
    public float maxHealCoolDown = 5f;
    [SerializeField]
    public float curHealCoolDown;
    [SerializeField]
    private bool isHealingActive = false;
    public float maxHealingDuration = 5f;
    [SerializeField]
    private float curHealingDuration;
    public GameObject healingVolume;
    [SerializeField]
    private float spawnTime =.5f;
    public int healingAmount;

    //for boss1
    [Header("Boss 1")]
    [SerializeField]
    private int delayBetweenSpawns;
    public GameObject NormalDrones;
    public GameObject[] SpawnPoints;
    public int MaxAliveEnemies = 9;

	[Header("Boss 2")]
	public GameObject laserSpawnPoint; 
	public GameObject laser; 

	[Header("HealingTurret")]
	public GameObject healingHitBoxSpawnPoint; 
	public GameObject healingHitBox;

    [Space]
    [Header("References")]
	AudioSource audiosource;
	AudioController audioController;
	DJ_Animation djAnimation;
    float chance;
    private BR_GameController gameController; 

    //-----------------------------------
    // Use this for initialization

    void Awake () 
	{

        //ExoGuard = GameObject.Find("ExoGuard");
        

    }
	//-----------------------------------
	void Start()
	{
        chance = Random.value;
        
            //gameObject.GetComponent<NavMeshAgent>().enabled = true;
           if (EnemyType != ENEMYTYPE.REGTURRET && EnemyType != ENEMYTYPE.FRIENDLYTURRET)
        ThisAgent = gameObject.GetComponent<NavMeshAgent>();
        ThisTransform = gameObject.GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) 
            playerTransform = player.GetComponent<Transform>();

        //Set Starting State
        ChangeState (CurrentState);
        //this way is the right way to refence a script
        if (EnemyType != ENEMYTYPE.FRIENDLYTURRET)
        {
            enemyHealth = gameObject.GetComponent<BR_EnemyHealth>();
            TurretObject = GameObject.FindGameObjectWithTag("FriendlyTurret");
            ThrusterObjectA = GameObject.FindGameObjectWithTag("ThrusterA").GetComponent<Transform>();
            ThrusterObjectB = GameObject.FindGameObjectWithTag("ThrusterB").GetComponent<Transform>();
        }


            
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<BR_GameController>();

        //for boss 1 difficulty
        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                {
                    delayBetweenSpawns = delayBetweenSpawns + 2; 
                }
                break;
            case BR_GameController.GameDifficulty.NORMAL:
                {
                    delayBetweenSpawns = delayBetweenSpawns;
                }
                break;
            case BR_GameController.GameDifficulty.HARD:
                {
                    delayBetweenSpawns = delayBetweenSpawns - 1; 
                }
                break;
        }

    }

    void Update()
    {
        if (enemyHealth != null)
        {

            if (enemyHealth.isActiveAndEnabled)
            {

                if (EnemyType == ENEMYTYPE.HEALINGTURRET)
                    FireHealingPulse();

                if (isAtStateActive)
                    atTimer -= Time.deltaTime;

                if (atTimer <= 0)
                {
                    if (meleeVolume != null)
                    {
                        meleeVolume.SetActive(false);
                        if (EnemyType != ENEMYTYPE.REGTURRET)
                            ChangeState(AISTATE.CHASE);
                    }

                    else
                        gameObject.transform.rotation = new Quaternion(-90, 165, 0, 0);
                }

                if (EnemyType == ENEMYTYPE.BOSS1)
                {
                    if (Vector3.Distance(gameObject.transform.position, playerTransform.position) <= AttackDistance)
                    {

                        ChangeState(AISTATE.ATTACK);
                        //insert audio for boss 1
                    }
                }
            }
        }
        if (spawnTime > 0)
            spawnTime -= Time.deltaTime;
        else if (spawnTime <= 0 && player != null && EnemyType != ENEMYTYPE.REGTURRET && EnemyType != ENEMYTYPE.FRIENDLYTURRET && !ThisAgent.isActiveAndEnabled)
            ThisAgent.enabled = true;
    }
	//-----------------------------------
	public IEnumerator Idle()
	{
		//Get Random Point
		Vector3 Point = RandomPointOnNavMesh();

		//Loop while idling
		while(CurrentState == AISTATE.IDLE)
		{
            
            if (spawnTime <= 0 && player != null && EnemyType != ENEMYTYPE.REGTURRET && EnemyType != ENEMYTYPE.FRIENDLYTURRET)
            {
                ThisAgent.enabled = true;
                if (CanSeePlayer )
                {
                    ChangeState(AISTATE.CHASE);
                    yield break;
                }
                if (EnemyType == ENEMYTYPE.HEALINGTURRET)
                {
                    ChangeState(AISTATE.HEALING);
                    yield break;
                }
            }
			yield return null;
		}
	}
	//-----------------------------------
	public IEnumerator Chase()
	{
        if (gameObject.GetComponent<BR_EnemyHealth>().isActiveAndEnabled)
        {
            while (CurrentState == AISTATE.CHASE)
            {
                isAtStateActive = false;

                if (ThisAgent != null && player != null)
                {
                    ThisAgent.SetDestination(playerTransform.position);
                    switch (EnemyType)
                    {
                        case ENEMYTYPE.REGULAREXO:
                            FireWeapon();
                            break;

                        case ENEMYTYPE.TRILOGYEXO:
                            FireWeapon();
                            break;

                        case ENEMYTYPE.BOSS1:
                            GameObject[] aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                            if (aliveEnemies.Length < MaxAliveEnemies)
                                SpawnEnemies(NormalDrones, SpawnPoints[0]);
                            yield return new WaitForSeconds(delayBetweenSpawns);
                            break;
                            //case ENEMYTYPE.EXPLOSIVEDRONE:



                    }
                    if (!CanSeePlayer)
                    {
                        yield return new WaitForSeconds(2f);

                        if (!CanSeePlayer)
                        {
                            ChangeState(AISTATE.IDLE);
                            //ChangeState(AISTATE.CHASE);
                            yield break;
                        }
                    }

                    if (Vector3.Distance(gameObject.transform.position, playerTransform.position) <= AttackDistance)
                    {
                        ChangeState(AISTATE.ATTACK);
                        yield break;
                    }

                    if (ThrusterObjectA != null )
                    {

                        if (Vector3.Distance(gameObject.transform.position, ThrusterObjectA.position) <= AttackThrusterDistance && !ThrusterObjectA.GetComponent<SP_Thrusters>().done)
                        {
                            CanSeeThrusterA = true;
                            ChangeState(AISTATE.ATTACKTHRUSTERA);
                            yield break;
                        }
                    }

                    if (ThrusterObjectB != null)
                    {

                        if (Vector3.Distance(gameObject.transform.position, ThrusterObjectB.position) <= AttackThrusterDistance && !ThrusterObjectB.GetComponent<SP_Thrusters>().done)
                        {
                            CanSeeThrusterB = true;
                            ChangeState(AISTATE.ATTACKTHRUSTERB);
                            yield break;
                        }

                    }
                    if (TurretObject != null)
                    {
                        if (Vector3.Distance(gameObject.transform.position, TurretObject.transform.position) <= AttackThrusterDistance && TurretObject.GetComponent<BR_Friendly>().isActive && chance >.5f)
                        {
                            CanSeeTurret = true;
                            ChangeState(AISTATE.ATTACKTURRET);
                            yield break;
                        }

                    }
                    yield return null;
                }
            }
        }
	}
	//-----------------------------------
	public IEnumerator Attack()
	{
		while(CurrentState == AISTATE.ATTACK)
		{

            //Deal damage here
            //   ThisAgent.SetDestination(PlayerObject.position);
            switch (EnemyType)
            {
                case ENEMYTYPE.DEFAULT:
                    MeleeFunction();
                    break;

			case ENEMYTYPE.EXPLOSIVEDRONE:
				enemyHealth.startColorLoop = true;
				gameObject.GetComponent<DJ_Animation> ().StartExplosiveDroneAnimation (); 
					yield return new WaitForSeconds(1f);
					enemyHealth.startColorLoop = false;
				gameObject.GetComponent<DJ_Animation> ().StopExplosiveDroneAnimation (); 
				yield return new WaitForSeconds(0.5f);
                	gameObject.GetComponent<BR_EnemyHealth>().ExplosiveDroneDeath();
                    break;

                case ENEMYTYPE.REGULAREXO:
                    MeleeFunction();
                    break;

				case ENEMYTYPE.TRILOGYEXO:
                    MeleeFunction();
                    break;

                case ENEMYTYPE.BOSS1:
                  
                    MeleeFunction();
                    yield return new WaitForSeconds(1f);
                    Debug.Log("PLAY BOSS1 VO");
                    audioController.Boss1_VOSFX();

                    break;
				case ENEMYTYPE.BOSS2:
				//enemyHealth.EnemyContainer.transform.LookAt (playerTransform);
				Boss2LookAtPlayer (); 
				Boss2LaserArea (); 
				//FireLaserWeapon (); 
                    break;
            } 

            if (!CanSeePlayer || Vector3.Distance (ThisTransform.position, playerTransform.position) > AttackDistance)
			{
                TurnOffMelee();
				ChangeState (AISTATE.CHASE);
			}
			yield return null;
		}
	}
    //-----------------------------------
    public IEnumerator Heal()

    {
		/*
            HealingPulse();
            HealingCoolDown();
            */ 
		FireHealingPulse (); 

        yield return null;

    }
    //-----------------------------------
    public IEnumerator AttackThrusterA()
    {

        //set the ai agent destination to the thruster 
		if (CanSeeThrusterA) 
		{
			ThisAgent.SetDestination (ThrusterObjectA.position);
			ThisTransform.transform.LookAt (ThrusterObjectA);

            if (Vector3.Distance(gameObject.transform.position, ThrusterObjectA.position) <= AttackThrusterDistance)
            {
                ThisAgent.SetDestination(ThrusterObjectA.position);
                ThisTransform.transform.LookAt(ThrusterObjectA);
            }

        }

		if (ThrusterObjectA == null) {
			CanSeeThrusterA = false; 
			ChangeState (AISTATE.CHASE);
			yield break;
		}
			
            isAtStateActive = true;

            switch (EnemyType)
            {
                case ENEMYTYPE.DEFAULT:
                MeleeFunction();
                break;

                case ENEMYTYPE.EXPLOSIVEDRONE:
                    gameObject.GetComponent<BR_EnemyHealth>().ExplosiveDroneDeath();
                    break;

                case ENEMYTYPE.REGULAREXO:
                MeleeFunction();
                break;

			case ENEMYTYPE.TRILOGYEXO:
                MeleeFunction();
                break;
            }



        yield return null;
    }
	//-----------------------------------
	public IEnumerator AttackThrusterB()
	{

		//set the ai agent destination to the thruster 
		if (CanSeeThrusterB) 
		{
			ThisAgent.SetDestination (ThrusterObjectB.position);
			ThisTransform.transform.LookAt (ThrusterObjectB); 
			
			if (Vector3.Distance (gameObject.transform.position, ThrusterObjectB.position) <= AttackThrusterDistance) 
			{
				ThisAgent.SetDestination (ThrusterObjectB.position);
				ThisTransform.transform.LookAt (ThrusterObjectB); 
			}
			
		}
		if (ThrusterObjectB == null) {
			CanSeeThrusterB = false; 
			ChangeState (AISTATE.CHASE);
			yield break;
		}

		isAtStateActive = true;

		switch (EnemyType)
		{
		case ENEMYTYPE.DEFAULT:
                MeleeFunction();
                break;

		case ENEMYTYPE.EXPLOSIVEDRONE:
			gameObject.GetComponent<BR_EnemyHealth>().ExplosiveDroneDeath();
			break;

		case ENEMYTYPE.REGULAREXO:
                MeleeFunction();
                break;

		case ENEMYTYPE.TRILOGYEXO:
                MeleeFunction();
            break;


		}

		yield return null;
	}
    public IEnumerator AttackTurret()
    {

        //set the ai agent destination to the thruster 
        if (CanSeeTurret)
        {
            ThisAgent.SetDestination(TurretObject.transform.position);
            ThisTransform.transform.LookAt(TurretObject.transform);

            if (Vector3.Distance(gameObject.transform.position, TurretObject.transform.position) <= AttackThrusterDistance)
            {
                ThisAgent.SetDestination(TurretObject.transform.position);
                ThisTransform.transform.LookAt(TurretObject.transform);
            }

        }
        if (TurretObject == null)
        {
            CanSeeTurret = false;
            ChangeState(AISTATE.CHASE);
            yield break;
        }

        isAtStateActive = true;

        switch (EnemyType)
        {
            case ENEMYTYPE.DEFAULT:
                MeleeFunction();
                break;

            case ENEMYTYPE.EXPLOSIVEDRONE:
                gameObject.GetComponent<BR_EnemyHealth>().ExplosiveDroneDeath();
                break;

            case ENEMYTYPE.REGULAREXO:
                MeleeFunction();
                break;

            case ENEMYTYPE.TRILOGYEXO:
                MeleeFunction();
                break;


        }

        yield return null;
    }
    //-----------------------------------

    public IEnumerator Stun(){

		while (CurrentState == AISTATE.STUN) {
			
			switch (EnemyType) {

			case ENEMYTYPE.DEFAULT:
				TurnOffMelee (); 
				break;

			case ENEMYTYPE.REGULAREXO:
				TurnOffMelee ();
				break;

			case ENEMYTYPE.TRILOGYEXO:
				TurnOffMelee ();
				break;

			case ENEMYTYPE.BOSS1:
				TurnOffMelee ();
				break;
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

            case AISTATE.HEALING:
                StartCoroutine(Heal());
            break;

            case AISTATE.ATTACKTHRUSTERA:
                StartCoroutine(AttackThrusterA());
            break; 

			case AISTATE.ATTACKTHRUSTERB:
			StartCoroutine(AttackThrusterB());
            break;

            case AISTATE.ATTACKTURRET:
                StartCoroutine(AttackTurret());
                break;

            case AISTATE.STUN:
				StartCoroutine (Stun()); 
			break; 


		}
	}
	//-----------------------------------
	void OnTriggerStay(Collider Col)
	{
		if(!Col.CompareTag ("Player"))
			return;

		CanSeePlayer = true;

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

        CanSeePlayer = true;
        
	}
    //-----------------------------------
    void MeleeFunction()
    {
        switch (isMeleeActive)
        {
            case true:
                curMeleeDuration -= Time.deltaTime;
                if (curMeleeDuration <= 0)
                {
                    if(meleeVolume != null)
                    {
                        meleeVolume.SetActive(false);
                        curMeleeCooldown = meleeDuration;
                        isMeleeActive = false;
                    }
                   
                }
                return;

            case false:
                curMeleeCooldown -= Time.deltaTime;
                if (curMeleeCooldown <= 0)
                {
                    if(meleeVolume != null)
                    {
                        meleeVolume.SetActive(true);
                        curMeleeDuration = meleeDuration;
                        isMeleeActive = true;
					audioController.Enemy_EnemyMeleeSFX ();
                    }
                   
                }
                return;
        }
    }

    void TurnOffMelee()
    {
        if(meleeVolume != null)
        {
            meleeVolume.SetActive(false);
            curMeleeCooldown = 0;
            isMeleeActive = false;
        }


    }
    
    //shooting functions
    void DoScatterShot()
    {
        if (fireTimer <= 0)
        {
           

            KW_Enemy_Projectile newBullet1 = Instantiate(projectile, transform.position, transform.rotation) as KW_Enemy_Projectile;
            KW_Enemy_Projectile newBullet2 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, 7.5f, 0)) as KW_Enemy_Projectile;
            KW_Enemy_Projectile newBullet3 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, -7.5f, 0)) as KW_Enemy_Projectile;

            fireTimer = fireRate;
			audioController.Enemy_EnemyExoGuardAttackSFX ();
            
        }
    }
    //fires the weapon
    void FireWeapon()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;

        {
            DoScatterShot();
        }
        

    }

	//shooting functions
	void DoLaserShot()
	{
        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                fireRate = 2.5f;
                break;
            case BR_GameController.GameDifficulty.NORMAL:
                fireRate = 2.0f;
                break;
            case BR_GameController.GameDifficulty.HARD:
                fireRate = 1.5f;
                break;
        }
        if (fireTimer <= 0)
		{
			Instantiate (laser, laserSpawnPoint.transform.position, laserSpawnPoint.transform.rotation); 

			fireTimer = fireRate;
			audioController.Boss2_Boss2LaserAttackSFX ();
		}
	}
	//fires the weapon
	void FireLaserWeapon()
	{
		if (fireTimer > 0)
			fireTimer -= Time.deltaTime;
		{
            
			DoLaserShot();
		}


	}

	void DoHealingPulse()
	{

		if (fireTimer <= 0) 
		{
			Instantiate (healingHitBox, healingHitBoxSpawnPoint.transform.position, healingHitBoxSpawnPoint.transform.rotation);  

			fireTimer = fireRate; 
			audioController.HealingTurret_HealingTurretActiveSFX ();

		}
	}

	void FireHealingPulse(){
		if (fireTimer > 0)
			fireTimer -= Time.deltaTime;

		{
			DoHealingPulse();
		}

	}
		

    void UpdateMeleeTimer()
    {
        curMeleeDuration -= Time.deltaTime;
        if (curMeleeDuration <= 0)
        {
            curMeleeDuration = meleeDuration;
        }
    }

    void SpawnEnemies(GameObject NormalDrones, GameObject SpawnPoints)
    {
        GameObject enemy = Instantiate(NormalDrones, SpawnPoints.transform.position, new Quaternion(0, Random.Range(0, 359), 0, 0));
        enemy.GetComponent<BR_EnemyCost>().enemyCost = 1;
	
    }

	void Boss2LookAtPlayer(){
		float speed = 3f; 
		float step = speed * Time.deltaTime; 

        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                speed = 3f;
                break;

            case BR_GameController.GameDifficulty.NORMAL:
                speed = 4f;
                break;

            case BR_GameController.GameDifficulty.HARD:
                speed = 5f;
                break; 
                
        }

		enemyHealth.EnemyContainer.transform.rotation = Quaternion.RotateTowards(transform.rotation, playerTransform.rotation, step); 
	}

	// Can only shoot laser when facing the player. 
	void Boss2LaserArea(){

		Vector3 directionToTarget = transform.position - playerTransform.position; 
		float angel = Vector3.Angle (transform.forward, directionToTarget); 
		if (Mathf.Abs (angel) > 90) {
			FireLaserWeapon (); 
			Debug.Log ("Target is behind me");
		}
	}

   
}
