using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public class BR_EnemyHealth : MonoBehaviour {

    public bool canDropLoot = true;
	private BR_GameController gameController;
	private SP_HUD HUD;
	public EnemyShield enemyShield; 
    private NavMeshAgent MyNavMesh;
    private AIAgent aiAgent;
	private ShakeOnKeyPress cameraShake;

    public enum DMGTYPE {DRONEEXPLD, MELEE, SINGLE, AUTO, SCATTER, BLEED, TURRET, ROCKET, GRENADEXPLOSION };

	AudioSource audiosource;
	AudioController audioController;
	MusicController musicController; 

	public int bossSlot; 

    public int maxHealth = 100;

    public int curHealth;
    private GameObject player;

    public bool isInvunerable = true;
    public float timer = .51f;
    public bool startedExplosion; 
	public bool startColorLoop = false; 
	public float colorLoopTimer; 
	 
 

	[Space]
	[Header("ENEMY HEALTH BAR UI")]

	public GameObject HealthBar; 
	public RectTransform HealthBarORANGE; 
	public RectTransform HealthBarRED; 
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f); 
	public Color purpleColor = new Color(1f, 0f, 1f, 1f);
	[Space]
	private float orangeSpeed = 0.01f;
	private bool orangeLerp = false; 
	private float fromSize; 
	private float newSize;

    [Space]
    //for exo guard move speed
    private bool isHalfSpeed = false;
	private float navAngularSpeedExo; 

    //for explosive drone 
    [Space]
	public GameObject droneExplosionVolume;
	[Tooltip("The amount of damage taken from explosive drone")]
	public int explosiveDamage = 12;
    public GameObject droneExplosionPS;


    [Space]

    //pickup
    //lists pickups to spawn
    public GameObject[] PickupTypes;
    public GameObject enemyToSpawn;

	[Header("FOR ALL ENEMY MODELS")]

	public GameObject enemyModel; 


    [Header("For Boss1")]
    [SerializeField]
    private bool isHalfHealth;
    [SerializeField]
    public GameObject EnemyContainer;

    [Space]
    [Header("TrilogyGuards")]
    [SerializeField]
    BR_isTrilogyGuard[] TrilogyGuards;
    public Color isAngry;
    private Renderer rend;
    public bool isTrilogyGuard;

	[Space]
	[Header("Emp Explosion Ability")]
	public float empStunTimer = 5f;
    private int curStunTime;
    private int lastStunTime =0;
	public GameObject stunParticles;
	[HideInInspector] public float stopwatch; 
	private float enemySpeed; 
	private bool isEmpExplosionON = false;
	private bool SpawnParticles = true; 
	[Tooltip("How long Emp Explosion effect last")]


    [Space]
    [Header("ParticleEffects")]
    public GameObject SpawnPS;
    public GameObject EnemyDeathPS;
    [Tooltip("put length of death particle duration here")]
    public float startDestoryTime;

    [Space]
    [Header("ForPickUpSpawning")]
    [Tooltip("50% = .5")]
    public float spawnPercentage;


    [Space]
    [Header("For Difficulty")]
    [SerializeField]
    [Tooltip("modifies the spawn percentage, .25 = +25%")]
    public float easySpawnPercentageModifer = .25f;
    [SerializeField]
    [Tooltip("modifies the spawn percentage, .15 = -15%")]
    public float hardSpawnPercentageModifer = .15f;

    public bool isBossVulnerable = false;
    float healthCheck;
    float meleeTimer = 0;
    public float toolTipTimer = 15;
    public bool toolTipActive = false;

    public bool isOnScreen;


    // Use this for initialization
    void Start ()
    {
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
		HUD = FindObjectOfType<SP_HUD> (); // Reference for HUD PREFAB
		gameController = FindObjectOfType<BR_GameController> (); 
        aiAgent = this.GetComponent<AIAgent>();
		musicController = FindObjectOfType<MusicController> (); 
        curHealth = maxHealth;
        if(aiAgent.EnemyType != AIAgent.ENEMYTYPE.REGTURRET)
        {
            MyNavMesh = this.GetComponent<NavMeshAgent>();
            enemySpeed = MyNavMesh.speed;
            navAngularSpeedExo = MyNavMesh.angularSpeed;
        }
		cameraShake = FindObjectOfType<ShakeOnKeyPress> ();

		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO) {
			if (cameraShake.isCameraShakeOn == false && cameraShake.curTimeBetweenShakes == 0) {
				cameraShake.isCameraShakeOn = true; 
				cameraShake.ShakeTheCamera ();
			}
		}

		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1) {
			HUD.BossShieldColorChangeBlue (); 
			musicController.Misc_Boss01SFX (); 
		}
		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2) {
			HUD.BossShieldColorChangeYellow (); 
			musicController.Misc_Boss02SFX (); 
		}
		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO) {
			HUD.BossShieldColorChangeBlue (); 
			musicController.Misc_TriGuardsSFX (); 

		}
        if (enemyShield != null)
            enemyShield.enemyHealth = this;

        player = GameObject.FindGameObjectWithTag("Player");
        HealthBar.SetActive (false);
	
        rend = GetComponent<Renderer>();


		stopwatch = empStunTimer; 
		
        SpawnInPS();

        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                {
                    spawnPercentage = spawnPercentage + easySpawnPercentageModifer;
                }
                break;
            case BR_GameController.GameDifficulty.NORMAL:
                {
                    spawnPercentage = spawnPercentage; 
                }
                break;
            case BR_GameController.GameDifficulty.HARD:
                {
                    spawnPercentage = spawnPercentage - hardSpawnPercentageModifer; 
                }
                break; 
        }
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (meleeTimer > 0)
            meleeTimer -= Time.deltaTime;
        if (toolTipActive)
        {
            toolTipTimer -= Time.deltaTime;
            if (toolTipTimer <= 0)
            {
                DJ_PowerPanel[] powerPanels = FindObjectsOfType<DJ_PowerPanel>();
                int alivePanels = 0;
                for (int x = 0; x < powerPanels.Length; x++)
                {
                    if (powerPanels[x].isAlive)
                        alivePanels++;
                }
                if (alivePanels == 4)
                {
                    HUD.objectivePanel.FailObjective(BR_Objectives.ObjectiveType.FIND);
                    HUD.AddObjective(BR_Objectives.ObjectiveType.POWERPANEL, "power panel to bring down the shield");
                    for (int x = 0; x < powerPanels.Length; x++)
                    {
                        powerPanels[x].isPulsing = true;
                    }
                }
                toolTipActive = false;
            }
        }
		HUD_Health ();

        TrilogyGuards = FindObjectsOfType<BR_isTrilogyGuard>();
        if (isTrilogyGuard)
        {
            ChangeTrilogyColor();
        }

        if(isInvunerable)
        {
            timer -= Time.deltaTime; 
            if (timer <= 0)
                isInvunerable = false;
        }
		if (aiAgent.EnemyType != AIAgent.ENEMYTYPE.REGTURRET && curHealth >0) {
			EmpExplosionSpeedStopper (); 
		}

		ExplosiveDroneChargingState ();

        if (isBossVulnerable)
        {
            BossVulnerableCheck();
        }
    }

    void OnTriggerEnter(Collider target)
    {
     //   Debug.Log("AI Entered:" + target.tag);

       	if (target.tag == "PlayerMelee" && meleeTimer <= 0)
        {
            switch (aiAgent.EnemyType)
            {
                 default:
                    TakeDamage(DMGTYPE.MELEE,player.GetComponent<PlayerController>().meleeDamageAmount);
                    break;

                case AIAgent.ENEMYTYPE.BOSS1:
                    if (enemyShield.curShield <= 0)
                    {
                        TakeDamage(DMGTYPE.MELEE, player.GetComponent<PlayerController>().meleeDamageAmount);
                    }
                    break;

                case AIAgent.ENEMYTYPE.BOSS2:
                    if (isBossVulnerable)
                    {
                        TakeDamage(DMGTYPE.MELEE, player.GetComponent<PlayerController>().meleeDamageAmount);
                    }
                    break;

                case AIAgent.ENEMYTYPE.REGULAREXO:
                    if(enemyShield.curShield <= 0)
                    {
                        TakeDamage(DMGTYPE.MELEE, player.GetComponent<PlayerController>().meleeDamageAmount);
                    }
                    break;

                case AIAgent.ENEMYTYPE.HEALINGTURRET:
                    TakeDamage(DMGTYPE.MELEE, player.GetComponent<PlayerController>().meleeDamageAmount);
                    break; 

				case AIAgent.ENEMYTYPE.EXPLOSIVEDRONE:
				TakeDamage(DMGTYPE.MELEE, player.GetComponent<PlayerController>().meleeDamageAmount);
				break; 
            }
            meleeTimer = .5f;
        }

		if (target.tag == "EmpExplosionRange" && !isInvunerable) {
			switch (aiAgent.EnemyType) 
			{
			default:
				isEmpExplosionON = true;
                
				break;

			case AIAgent.ENEMYTYPE.REGULAREXO:
				if (enemyShield == null) {
					isEmpExplosionON = true; 
				}
				break;

			case AIAgent.ENEMYTYPE.TRILOGYEXO:
				if (enemyShield  == null) {
					isEmpExplosionON = true; 
				}
				break;
			case AIAgent.ENEMYTYPE.BOSS1:
				if (enemyShield == null) {
					isEmpExplosionON = true; 
				}
				break;
			case AIAgent.ENEMYTYPE.BOSS2:
				if (isBossVulnerable) {
					isEmpExplosionON = true; 
				}
				break;
			
			case AIAgent.ENEMYTYPE.EXPLOSIVEDRONE:
				isEmpExplosionON = true; 
				break; 
		}

		} 

        if (target.tag == "PlayerProjectile" && !isInvunerable)
        {
            TakeDamage(target.GetComponent<KW_PlayerProjectile>().bulletType, target.GetComponent<KW_PlayerProjectile>().damageDealt);
            //if (target.GetComponent<KW_PlayerProjectile>().bulletType != DMGTYPE.TURRET)
            //    aiAgent.TargetPlayer();
            Destroy(target);
        }


		if (target.tag == "DroneExplosion" && !isInvunerable) {
			if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGULAREXO && enemyShield.isShieldActive == false )
            {
                Debug.Log("This is an exo guard and it has no shield so we will damage the health of this object");
				TakeDamage (DMGTYPE.DRONEEXPLD,explosiveDamage); 
			}
			else 
            {
				TakeDamage (DMGTYPE.DRONEEXPLD,explosiveDamage); 
			}
		}
    }

   void OnTriggerStay(Collider target)
	    {
      //  Debug.Log("AI staying in:" + target.tag);

		if (target.tag == "EnemyHealing" && aiAgent.EnemyType != AIAgent.ENEMYTYPE.HEALINGTURRET)
        {
            HealDamage(target.GetComponent<KW_EnemyHealing>().healAmount);
        }



        
    }

	// HUD / World Space Healthbars Functions
	//-----------------------------------------------------------------------------------------------------------------

	public void DoFromSize(){
		
		fromSize = GetHealthPct();
        // TRILOGYEXO_01 Health Bar UI
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO)
        {
            switch (bossSlot)
            {
                case 0:
                    HUD.fromSizeBoss01 = GetHealthPct();
                    break;

                case 1:
                    HUD.fromSizeBoss02 = GetHealthPct();
                    break;

                case 2:
                    HUD.fromSizeBoss03 = GetHealthPct();
                    break;
            }
        }
	}
	public void DoOrangeLerp(){
		orangeLerp = true;
        // TRILOGYEXO_01 Health Bar UI
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO)
        {
            switch (bossSlot)
            {
                case 0:
                    HUD.orangeLerpBoss01 = true;
                    break;

                case 1:
                    HUD.orangeLerpBoss02 = true;
                    break;

                case 2:
                    HUD.orangeLerpBoss03 = true;
                    break;
            }
        }
	}
	public void DoNewSize(){
		newSize = GetHealthPct ();
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO)
        {
            switch (bossSlot)
            {
                case 0:
                    HUD.newSizeBoss01 = GetHealthPct();
                    break;

                case 1:
                    HUD.newSizeBoss02 = GetHealthPct();
                    break;

                case 2:
                    HUD.newSizeBoss03 = GetHealthPct();
                    break;
            }
        }
	}
	public void DoOrangeColor(){
		HealthBarORANGE.GetComponent<Image> ().color = orangeColor; // Change Orange Health Bar to Orange Color
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO)
        {
            switch (bossSlot)
            {
                case 0:
                    HUD.HealthBarORANGEBoss01.GetComponent<Image>().color = orangeColor;
                    break;

                case 1:
                    HUD.HealthBarORANGEBoss02.GetComponent<Image>().color = orangeColor;
                    break;

                case 2:
                    HUD.HealthBarORANGEBoss03.GetComponent<Image>().color = orangeColor;
                    break;
            }
        }
	}

	//-----------------------------------------------------------------------------------------------------------------
    
    public void HealDamage(int Healamount)
    {

        curHealth += Healamount;
        fromSize = GetHealthPct();
        fromSize = Mathf.Clamp(fromSize, 0, 1);
        orangeLerp = true;
        newSize = GetHealthPct();
        HealthBarORANGE.GetComponent<Image>().color = orangeColor; // Change Orange Health Bar to Orange Color

        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        if (aiAgent.EnemyType != AIAgent.ENEMYTYPE.REGTURRET)
            ModSpeed();


    }
		
    public void TakeDamage(DMGTYPE _dmgType,int DMGamount)
    {
		
		gameController.ModHighScore (curHealth); // Grant High Score per Damage Taken

		DoFromSize (); 

        fromSize = Mathf.Clamp(fromSize, 0, 1);
        curHealth -= DMGamount;

		DoOrangeLerp (); 

		DoNewSize (); 

		DoOrangeColor (); 

        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        if (aiAgent.EnemyType != AIAgent.ENEMYTYPE.REGTURRET)
            ModSpeed(); 

        if (curHealth <= 0 && !startedExplosion)
        {
            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO)
            {
                HUD.BossSlot[bossSlot] = null;
            }
            HUD.ComboScoreMultiplerADDTIME (_dmgType); // Reset Timer for Combo Meter 
			gameController.ModHighScore (maxHealth); // Grant High Score base off overall enemy health	
            SpawnPickup();
            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2)
            {
                HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.ATTACK);
            }
            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.EXPLOSIVEDRONE)
            {
				ExplosiveDroneDeath ();
			}
            else
            {
				if (aiAgent.healingHitBox != null)
                {
					Destroy (GameObject.Find("HealingHitbox(Clone)")); 
				}
                StartDestroy(startDestoryTime);
			}
        }
    }

   void SpawnPickup()
    {
        float percentage = Random.value;
        if (percentage < spawnPercentage)
        {
            if (PickupTypes.Length != 0 && canDropLoot)
            {
                int randIndex = Random.Range(0, PickupTypes.Length);
                DJ_PickUp potentialDrop = PickupTypes[randIndex].GetComponent<DJ_PickUp>();
                //Selects one pick up to spawn.
                int curCount = 0;
                DJ_PickUp[] pickups = FindObjectsOfType<DJ_PickUp>();
                for (int x = 0; x < pickups.Length; x++)
                {
                    if (pickups[x].subTypeName == potentialDrop.subTypeName)
                        curCount += pickups[x].amount;
                }
                switch (potentialDrop.subTypeName)
                {
                    case DJ_PickUp.SUBTYPE.Auto:
                        if (gameController.isAutoUnlocked && gameController.curAuto_Ammo + curCount < 420)
                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    case DJ_PickUp.SUBTYPE.Scatter:
                        if (gameController.isScatterUnlocked && gameController.curScatter_Ammo + curCount < 250)
                                Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    case DJ_PickUp.SUBTYPE.Shield:
                            
                        if (gameController.isShieldUnlocked && player.GetComponent<PlayerController>().curShield + curCount < 100)
                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    case DJ_PickUp.SUBTYPE.Health:

                        if (gameController.isShieldUnlocked && player.GetComponent<PlayerController>().curHealth + curCount  < 100)
                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    default:
                        Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;
                }
            }
        }
    }

	public float GetHealthPct(){
		return Mathf.Clamp((float)curHealth / maxHealth,0,1); 
	}

	void HUD_Health(){

		if (curHealth < maxHealth) {
			HealthBar.SetActive (true); 
		}
        if(curHealth <= 0)
        {
            HealthBar.SetActive(false);
        }

		HealthBarRED.localScale = new Vector3 (GetHealthPct(), 1f, 1f); 

		if (orangeLerp) {
			HealthBarORANGE.localScale = new Vector3 (fromSize -= orangeSpeed, 1f, 1f);
		}
		if (fromSize <= newSize) {
			orangeLerp = false;
			HealthBarORANGE.localScale = HealthBarRED.localScale;
		}

	}

    public void TakeCritcalDamage(DMGTYPE _dmgType,int CRTamount)
    {
        gameController.ModHighScore(curHealth);

        fromSize = GetHealthPct();
        fromSize = Mathf.Clamp(fromSize, 0, 1);
        curHealth -= CRTamount;
		HealthBarORANGE.GetComponent<Image>().color = purpleColor; // Change Orange Health Bar to Purple Color
		orangeLerp = true;
		newSize = GetHealthPct ();
        
        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

        ModSpeed();

        if (curHealth <= 0)
        {
            HUD.ComboScoreMultiplerADDTIME(_dmgType); // Reset Timer for Combo Meter 
            gameController.ModHighScore(maxHealth); // Grant High Score base off overall enemy health	

            SpawnPickup();
			audioController.Enemy_EnemyExplosionSFX();
            StartDestroy(startDestoryTime);

        }
    }

    void ModSpeed()
    {
        //checks to see if health is at half max and that the bool is false and that the enemy type is only an exogaurd
        if(curHealth <= maxHealth/2 && isHalfSpeed == false && aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGULAREXO)
        {
            MyNavMesh.speed = MyNavMesh.speed / 2;
            MyNavMesh.angularSpeed = MyNavMesh.angularSpeed / 2;
            Debug.Log("Exo guard is half speed " + MyNavMesh.speed);
            isHalfSpeed = true; 
        }

		if (curHealth >= maxHealth / 2 && isHalfSpeed == true) {
			isHalfSpeed = false; 
			MyNavMesh.speed = enemySpeed; 
			MyNavMesh.angularSpeed = navAngularSpeedExo; 
			Debug.Log ("Speed is normal");
		}
    }

	public void ExplosiveDroneChargingState()
    {
		if (startColorLoop == true)
        {
			colorLoopTimer += Time.deltaTime;
			if (colorLoopTimer <= 0.3f)
            {
				rend.material.color = Color.black; 
			}
            else if(colorLoopTimer >= 0.3f)
            {
				rend.material.color = Color.red; 
				if (colorLoopTimer >= 0.6f)
                {
					colorLoopTimer = 0; 
				}
			}
		}
	}

   public void ExplosiveDroneDeath()
    {
        if (!startedExplosion)
        {
			if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.EXPLOSIVEDRONE) {
				if(cameraShake.isCameraShakeOn == false && cameraShake.curTimeBetweenShakes == 0){
					cameraShake.isCameraShakeOn = true; 
					cameraShake.ShakeForDroneExplosion (); 
				}
			}
            // set the damage volume to active
            MyNavMesh.speed = 0;
			Instantiate (droneExplosionVolume, transform.position, transform.rotation); 
			Destroy(EnemyContainer);
            //Instantiate(enemyToSpawn, transform.position, Quaternion.Euler(0, 180, 0));
            Instantiate(enemyToSpawn, transform.position, Quaternion.Euler(0, 180, 0));
            Instantiate(enemyToSpawn, transform.position, Quaternion.Euler(0, 180, 0));
            SpanwDroneExplosionPS();
            
            startedExplosion = true;
			audioController.Enemy_EnemyExplosionSFX();
        }
    }

    void ChangeTrilogyColor()
    {
        if (TrilogyGuards.Length <= 3)
        {
          
            Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
            switch (TrilogyGuards.Length)
            {
                case 1:
                    rend.material.color = isAngry;
                    halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
			
                    break;
                case 2:
                    rend.material.color = isAngry;
                    break;
            }
        }

    }

	void EmpExplosionSpeedStopper()
    {
		
		if (isEmpExplosionON ) {
			empStunTimer -= Time.deltaTime;
            curStunTime = Mathf.RoundToInt(empStunTimer);
            if (MyNavMesh.isActiveAndEnabled)
            {
                MyNavMesh.speed = 0;
                if (lastStunTime != curStunTime)
                {
                    float dmgAmount = 0;
                    Instantiate(stunParticles, transform.position, transform.rotation);
                    lastStunTime = curStunTime;
                    switch (aiAgent.EnemyType)
                    {
                        case AIAgent.ENEMYTYPE.DEFAULT:
                            dmgAmount = maxHealth / 10f;
                            break;

                        case AIAgent.ENEMYTYPE.REGTURRET:
                            dmgAmount = maxHealth / 10f;
                            break;

                        case AIAgent.ENEMYTYPE.HEALINGTURRET:
                            dmgAmount = maxHealth / 10f;
                            break;

                        case AIAgent.ENEMYTYPE.EXPLOSIVEDRONE:
                            dmgAmount = maxHealth / 10f;
                            break;

                        case AIAgent.ENEMYTYPE.BOSS2:
                            if(isBossVulnerable)
                            dmgAmount = 10f;
                            break;

                        default:
                            if (enemyShield.curShield >= 0)
                            dmgAmount = 10f;
                            break;
                    }
                    TakeDamage(DMGTYPE.BLEED, (Mathf.RoundToInt(dmgAmount)));
                }
			}
            if(aiAgent.isActiveAndEnabled)
            {
			    aiAgent.ChangeState (AIAgent.AISTATE.STUN);
                if (empStunTimer <= 0)
                {
                    empStunTimer = stopwatch;
                    MyNavMesh.enabled = true;
                    MyNavMesh.speed = enemySpeed;

                    aiAgent.ChangeState(AIAgent.AISTATE.CHASE);
                    isEmpExplosionON = false;
                }
			}
		}

	}

    void SpanwDroneExplosionPS()
    {
        Instantiate(droneExplosionPS, transform.position, transform.rotation);
    }

    void SpawnInPS()
    {
        Instantiate(SpawnPS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z), transform.rotation); 
		audioController.Enemy_EnemySpawnSFX ();
    }

    void SpawnEnemyDeathPS()
    {
        Instantiate(EnemyDeathPS, transform.position, transform.rotation);
    }

    void StartDestroy(float timeDelay)
    {
		
		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO ) {
			if (cameraShake.isCameraShakeOn == false && cameraShake.curTimeBetweenShakes == 0) {
				cameraShake.isCameraShakeOn = true; 
				cameraShake.ShakeTheCamera ();
			}

			if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1 || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2) {
				audioController.Boss_DeathSFX ();
			}

			if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO) {
				if (gameController.HUD.BossSlot [0] == null && gameController.HUD.BossSlot [1] == null && gameController.HUD.BossSlot [2] == null) {
					audioController.Boss_DeathSFX ();

				}
			}
		}




        //turn off drawing and colliding 
        GetComponent<Renderer>().enabled = false;
		if(enemyModel != null){
		enemyModel.SetActive (false); // turn off enemy model
		}
        GetComponent<Collider>().enabled = false;
        SpawnEnemyDeathPS();
        HealthBar.SetActive(false);
       // aiAgent.enabled = false;
        Destroy(aiAgent.meleeVolume);

        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.HEALINGTURRET)
            Destroy(aiAgent.healingVolume);

        if (aiAgent.EnemyType != AIAgent.ENEMYTYPE.REGTURRET)
        MyNavMesh.speed = 0;

        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGULAREXO)
        {
            aiAgent.fireTimer = 10f;
        }

		if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.TRILOGYEXO || aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS2)
        {
            aiAgent.fireTimer = 10f;
        }



        Destroy(EnemyContainer, timeDelay);
		audioController.Enemy_EnemyDeathSFX ();
    }

   public void MakeBossVulnerable()
    {
        HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.FIND);
        HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.POWERPANEL);
        enemyShield.gameObject.SetActive(false);
        DJ_PowerPanel[] powerPanels = FindObjectsOfType<DJ_PowerPanel>();
        int alivePanels = 0;
        for (int x = 0; x < powerPanels.Length; x++)
        {
            if (powerPanels[x].isAlive)
            {
                powerPanels[x].isPulsing = false;
                powerPanels[x].powerPanelLight.SetActive(false);
                powerPanels[x].isDamagable = false;
                alivePanels++;
            }
                
        }
        switch(alivePanels)
        {
            case 3:
                healthCheck = maxHealth * .75f;
                break;

            case 2:
                healthCheck = maxHealth * .5f;
                break;

            case 1: 
                healthCheck = maxHealth *.25f;
                break;

            default:
                healthCheck = 0f;
                break;
        }
        
            HUD.AddObjective(BR_Objectives.ObjectiveType.ATTACK, "Boss");
        isBossVulnerable = true;
    }

    void BossVulnerableCheck()
    {
        DJ_PowerPanel[] powerPanels = FindObjectsOfType<DJ_PowerPanel>();
        int alivePanels = 0;
        for (int x = 0; x < powerPanels.Length; x++)
        {
            if (powerPanels[x].isAlive)
            {
                alivePanels++;
            }
        }
        curHealth = Mathf.Clamp(curHealth, Mathf.RoundToInt(healthCheck)-1, maxHealth);
        if ( curHealth < healthCheck)
        {
            for (int x = 0; x < powerPanels.Length; x++)
            {
                if (powerPanels[x].isAlive)
                {
                    powerPanels[x].powerPanelLight.SetActive(true);
                    powerPanels[x].isDamagable = true;
                    powerPanels[x].isPulsing = true;
                }
            }
            HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.ATTACK);
            enemyShield.gameObject.SetActive(true);
			audioController.Boss02_RegainShieldSFX (); 
            if (curHealth > 0)
                HUD.AddObjective(BR_Objectives.ObjectiveType.POWERPANEL, "power panel to bring down the shield");
            isBossVulnerable = false;
        }
    }

    void OnBecameVisible()
    {
        isOnScreen = true;
    }

    void OnBecameInvisible()
    {
        if (curHealth >0)
        isOnScreen = false;
    }
}
