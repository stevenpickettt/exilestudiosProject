using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	[Header("Animation")]
	public PlayerControllerAnimationMachine playerControllerAnimationMachine;

	AudioSource audiosource;
    [SerializeField]
    public bool GODMODE = false;
    //HUD
	private SP_HUD HUD; 
	private ShakeOnKeyPress cameraShake; 
	AudioController audioController;
    WaveSpawner spawner;

    
    //health varaibles 
    public int maxHealth = 100;
    public int curHealth;
    public int maxShield = 100;
    public int curShield;

    //moving variables 
    
	public float MoveSpeed = 5f;
    public float sloMoX; 

   
	public  float MaxSpeed = 10f;
    public int meleeDamageAmount = 10;
    private Vector3 movement = Vector3.zero;
    public float angle;

    [SerializeField]
    private float meleeDistance = .5f;

    [SerializeField]
    private float meleeDuration = .5f;
    public float curMeleeDuration = 0f;
    public GameObject meleeVolumeSpawnPoint;
	public GameObject meleeVolume;
	[HideInInspector]public bool isMeleeActive = false;

    private BR_GameController gameController;

	[Space]
	[Header("FireMode")]

	public GameObject singleFireModel; 
	public GameObject autoFireModel; 
	public GameObject scatterFireModel;
    public GameObject launcherModel; 
	public GameObject gunModels; 
	[Space]
	public GameObject singleFirePoint;
	public GameObject autoFirePoint;
	public GameObject scatterFirePoint;
    public GameObject launcherFirePoint;
	[Space]
	public GameObject singleFirePS; 
	public GameObject autoFirePS; 
	public GameObject scatterFirePS;
    public GameObject launcherFirePS; 

    [SerializeField]
    private float singleShot_RateOfFire = 1f;
    [SerializeField]
    private float autoShot_RateOfFire = .5f;
    [SerializeField]
    private float scatterShot_RateOfFire = 1.5f;
    public float weaponTimer;
    private bool isTriggerReleased;
    private bool isFireable;
    [SerializeField]
    private KW_PlayerProjectile[] projectile;
    

	[Space]
	[Header("Emp Dash")]
	public float maxEmpDashCoolDownDuration = 10f; // Cool Down Stop Time
	public float curEmpDashCoolDownTimer; 

	public float maxEmpDashActiveTimer = 2f; // Emp Dash Stop Time 
	public float curEmpDashDuration;

	public bool isEmpDashActive = false;
	public bool isEmpDashCoolDown = false; 

	public float empDashSpeed = 2.0f;

    public GameObject EmpDashPS;
    [SerializeField]
    private GameObject raycastLocation;

    public Vector3 lastLocation;



    [Space]
	[Header("Emp Explosion")]
	public float maxEmpExplosionCoolDownDuration = 7.0f;
	public float curEmpExplosionCoolDownDuration;
	public bool isEmpExplosionActive = false;
	public float maxEmpExplosionDuration = 5.0f;
	public float curEmpExplosionDur;
	public bool isEmpExplosionCoolDownActive = false;
	public int EmpExplosionDamage = 5;
	public GameObject empExplosionVolumeSpawnPoint;
	public GameObject empExplosionVolume;
    private CameraController cameraPrefab;

    [Space]
    [Header("Grenade Launcher")]
    public bool isSuperActive = false;
	[HideInInspector] public float curSuperTimer;
    int curSuperCheck;
    int lastSuperCheck;
    int superShotsFired;
    public bool allSSfired = true;



    KW_PlayerProjectile[] superProjectiles;


    [Space]
    public GameObject shieldVisual;


    public GameObject pickUpSlot;
    [SerializeField]
    private GameObject carryLocation;



    [Space]
    [Header("For Difficulty")]
    [SerializeField]
    [Tooltip("modifies the player melee damage amt +")]
    public int easyPlayerMeleeModifier = 10;
    [SerializeField]
    [Tooltip("modifies the player melee damage amt -")]
    public int hardPlayerMeleeModifier = 5;

    // Use this for initialization
    void Start()
    {
		curHealth = maxHealth;
		cameraShake = FindObjectOfType<ShakeOnKeyPress> (); 
        cameraPrefab = FindObjectOfType<CameraController>();
        cameraPrefab.FindPlayer();
        HUD = FindObjectOfType<SP_HUD>(); // Reference for HUD PREFAB
                                          //reference to the audiosource object
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
        gameController = FindObjectOfType<BR_GameController>();
        spawner = gameController.spawner;
        //meleeVolume = GameObject.Find("MeleeVolume");
        meleeVolumeSpawnPoint.transform.Translate(new Vector3(0, 0, meleeDistance));
       // meleeVolume.SetActive(false);
        
		//CurEmpDuration = EmpBoostDur;
        
		if (gameController.isShieldUnlocked) {
			curShield = maxShield;
			ShieldActivate (); 
		} else {
			curShield = 0;
		}

		curEmpExplosionDur = maxEmpExplosionDuration; 
        //curEmpExplosionDur = maxEmpExplosionCoolDownDuration;

        //emp explosion---------------------------------------------
        //EmpExplosionVol = GameObject.Find ("EmpExplosion");

        //EmpExplosionVol.SetActive (false);

        //shieldVisual.SetActive(false);

        //--------For Difficulty-------// 
        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                {
                    meleeDamageAmount = meleeDamageAmount + easyPlayerMeleeModifier;
                }
                break;

            case BR_GameController.GameDifficulty.NORMAL:
                {
                    meleeDamageAmount = meleeDamageAmount;
                }
                break;

            case BR_GameController.GameDifficulty.HARD:
                {
                    meleeDamageAmount = meleeDamageAmount - hardPlayerMeleeModifier;
                }
                break;


        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
            GODMODE = !GODMODE;
        if (isEmpDashActive)
        {
			EmpDash ();
            EmpDashPS.SetActive(true);
        }
        else
        {
            if (Time.timeScale > 0.01f)
            UpdatePosition ();
            EmpDashPS.SetActive(false);
        }
			

        SloMoMultipler();

		//emp explosion-------------------------------------
		if(isEmpExplosionActive == true)
		{
			EmpExplosion();
		}
		EmpExplosionActive ();
		EmpExplosionCoolDown ();
		EmpDashCoolDown (); 
		TurnOnEmpDash ();
		RayCastDetection ();


      
		
		AbleToEmpDash ();
        if (pickUpSlot != null)
        {
            scatterFireModel.SetActive(false);
            singleFireModel.SetActive(false);
            autoFireModel.SetActive(false);
            pickUpSlot.transform.position = carryLocation.transform.position;
            pickUpSlot.transform.rotation = carryLocation.transform.rotation;
        }
        else if (curSuperTimer <= 0)
        {
            WeaponFunction();
            switch (gameController.CurrentWeapon)
            {
                case BR_GameController.WEAPON.SINGLE:
                    ShowSingleFireModel();
                    break;

                case BR_GameController.WEAPON.AUTO:
                    ShowAutoFireModel();
                    break;

                case BR_GameController.WEAPON.SCATTER:
                    ShowScatterFireModel();
                    break;
            }
            if (Input.GetAxis("Melee") != 0 && curMeleeDuration <= 0 && Time.timeScale > 0.01f && !isMeleeActive)
            {
                TurnOnMelee();

            }
            if (isMeleeActive)
            {
                DoMelee();
            }
        }
        if (isSuperActive)
        {
            DoSuper();
        }
    }

    void UpdatePosition()
    {
        if (gameController.isMouseActive)
        {
            movement.z = Input.GetAxis("Vertical") * Time.deltaTime * MoveSpeed * sloMoX;
            movement.x = Input.GetAxis("Horizontal") * Time.deltaTime * MoveSpeed * sloMoX;

        }
        else
        {
            movement.z = Input.GetAxis("VerticalLS") * Time.deltaTime * MoveSpeed * sloMoX;
            movement.x = Input.GetAxis("HorizontalLS") * Time.deltaTime * MoveSpeed * sloMoX;

        }
        transform.Translate(movement, Space.World);
        transform.Translate(movement, Space.World);

        if (!isEmpDashActive && Time.timeScale !=0)
        {
            if (gameController.isMouseActive)
            {
                Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
                Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

                angle = Mathf.Atan2(mouseOnScreen.y - positionOnScreen.y, mouseOnScreen.x - positionOnScreen.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 90 - angle, 0));
            }
            else
            {
                if (Input.GetAxis("VerticalRS") != 0 || (Input.GetAxis("HorizontalRS") != 0))
                {
                    angle = Mathf.Atan2(gameController.RS_X, gameController.RS_y) * Mathf.Rad2Deg;
                    transform.eulerAngles = new Vector3(0, angle, 0);
                }
            }
        }


    }



    void DoMelee()
    {
        curMeleeDuration -= Time.deltaTime * sloMoX;
        if (curMeleeDuration <= 0)
        {
            if(Input.GetAxis("Melee") <= 0.5f)
            //meleeVolume.SetActive(false); 
            isMeleeActive = false;
			playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.IDLE);
            gunModels.SetActive(true);
        }
    }
    void TurnOnMelee()
    {
		playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.MELEE);
        gunModels.SetActive(false);
        curMeleeDuration = meleeDuration;
		Instantiate(meleeVolume, meleeVolumeSpawnPoint.transform.position, meleeVolumeSpawnPoint.transform.rotation); 
        isMeleeActive = true;
		audioController.Player_MeleeSFX();
    }

    void WeaponFunction()
    {
        if (weaponTimer > 0)
            weaponTimer -= Time.deltaTime * sloMoX;
        if (Input.GetAxis("Fire") != 0 && Time.timeScale >= 0.01f && !isMeleeActive)
        {
            switch (gameController.CurrentWeapon)
            {
                default:
                    return;

                case BR_GameController.WEAPON.SINGLE:
                    DoSingleShot();
                    return;

                case BR_GameController.WEAPON.AUTO:
                    DoAutoShot();
                    return;

                case BR_GameController.WEAPON.SCATTER:
                        DoScatterShot();
                    return;
            }
        }
        else
            isTriggerReleased = true;
    }

	// When SUBTRACTING Health to the Player Please use these 
	// functions so the player doesn't spawn damageImage when healing. 
	//------------------------------------------------------------------------------------------------------------------
    public void ModShield(int value)
    {
        if (!HUD.tutorialScreen.activeInHierarchy)
        {
            HUD.isDamageTaken();
            HUD.ShieldFillAmount();

            HUD.DoFromSizeShield();
            curShield -= value;

            HUD.DoOrangeLerpShield();
            HUD.DoNewSizeShield();
            curShield = Mathf.Clamp(curShield, 0, maxShield);

            ShieldActivate();
        }

        
    }

    public void ModHealth(int value)
    {
        if (!HUD.tutorialScreen.activeInHierarchy)
        {
            HUD.isDamageTaken();
            HUD.HealthFillAmount();

            HUD.DoFromSizeHealth();

            curHealth -= value;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

            HUD.DoOrangeLerpHealth();
            HUD.DoNewSizeHealth();
            HealthCheck();
        }
    }
	//------------------------------------------------------------------------------------------------------------------

	// When ADDING Health to the Player Please use these 
	// functions so the player doesn't spawn damageImage when healing. 
	//------------------------------------------------------------------------------------------------------------------
	public void AddHealth(int value){
		HUD.HealthFillAmount (); 
		HUD.DoFromSizeHealth ();
		curHealth += value;
		HUD.DoOrangeLerpHealth (); 
		HUD.DoNewSizeHealth (); 
		curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
	
	}
	public void AddShield(int value){
		HUD.ShieldFillAmount (); 
		HUD.DoFromSizeShield (); 
		curShield += value;
		HUD.DoOrangeLerpShield (); 
		HUD.DoNewSizeShield ();
		curShield = Mathf.Clamp(curShield, 0, maxShield);
		ShieldActivate (); 

	}
	//------------------------------------------------------------------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (!GODMODE)
        {
            if (other.tag == "Melee")
            {
                //if the player has shield take away from shield
                if (curShield > 0)
                {
                    ModShield(other.GetComponent<DJ_MeleeAttack>().meleeDamage);
                }

                // if the player doesnt have a shield take away health 
                if (curShield <= 0)
                {
                    ModHealth(other.GetComponent<DJ_MeleeAttack>().meleeDamage);
                }


            }

            if (other.tag == "EnemyProjectile")
            {
                //if the player has shield take away from shield
                if (curShield > 0)
                {
                    ModShield(other.GetComponent<KW_Enemy_Projectile>().damageDealt);
					cameraShake.ShakeForPlayerHitWithProjectile (); 
                }

                // if the player doesnt have a shield take away health 
                if (curShield <= 0)
                {
                    ModHealth(other.GetComponent<KW_Enemy_Projectile>().damageDealt);
					cameraShake.ShakeForPlayerHitWithProjectile (); 
                }

            }

            if (other.tag == "DroneExplosion")
            {
                //if the player has shield take away from shield
                if (curShield > 0)
                {
                    ModShield(other.GetComponent<DJ_DroneExplosion>().explosionDamage);
                }

                // if the player doesnt have a shield take away health 
                if (curShield <= 0)
                {
                    ModHealth(other.GetComponent<DJ_DroneExplosion>().explosionDamage);
                }

            }
        }

        //for the pick ups
        if (other.tag == "PickUp")
        {
            

            DJ_PickUp pickUP = other.GetComponent<DJ_PickUp>();
			audioController.PickUos_PickUpSFX();

            switch (pickUP.subTypeName)
            {
                case DJ_PickUp.SUBTYPE.Health:
                   
                    AddHealth(pickUP.amount);
                    break;

                case DJ_PickUp.SUBTYPE.Shield:
                 
                    if (!gameController.isShieldUnlocked)
                    {
                        gameController.TurnOnShild();
                        AddShield(maxShield);
						ShieldActivate (); 
                    }
                    else
                    AddShield(pickUP.amount);
                    break;

                case DJ_PickUp.SUBTYPE.Auto:
                       
                        gameController.UnlockAuto();
                        gameController.curAuto_Ammo += pickUP.amount;
                        HUD.UpdateAutoAmmoAmount();
                    break;

                case DJ_PickUp.SUBTYPE.Scatter:
                     
                        gameController.UnlockScatter();
                        gameController.curScatter_Ammo += pickUP.amount;
                        HUD.UpdateScatterAmmoAmount();
                    break;

                case DJ_PickUp.SUBTYPE.Blaster:
                       
                        gameController.UnlockBlaster();
                    break;

                case DJ_PickUp.SUBTYPE.Key:
                       
                    //code here to open the door associated with that key 
                    gameController.OpenDoor(pickUP.amount);
                    break;

                case DJ_PickUp.SUBTYPE.hackKey:
                   
                    //code here to open the door associated with that key 
                    gameController.AllowHacking(pickUP.amount);
                    break;

                case DJ_PickUp.SUBTYPE.RegenVol:
                    AddHealth(1000000);
                    AddShield(1000000);
                    gameController.curAuto_Ammo += 300;
                    gameController.curScatter_Ammo += 120;
                    break;

                case DJ_PickUp.SUBTYPE.Thruster:
                      
                    if (pickUpSlot == null)
                        pickUpSlot = other.gameObject;
                    HUD.RemoveObjective(BR_Objectives.ObjectiveType.THRUSTER);
                        HUD.AddObjective(BR_Objectives.ObjectiveType.CARRY, "thruster");
                    spawner.isRewardAvailable = false;
                    break;

                case DJ_PickUp.SUBTYPE.Engine:
                    if (pickUpSlot == null)
                        pickUpSlot = other.gameObject;
                    spawner.isRewardAvailable = false;
                    HUD.RemoveObjective(BR_Objectives.ObjectiveType.PICKUP);
                    HUD.AddObjective(BR_Objectives.ObjectiveType.CARRY, "engine");
                    //spawner.ResetSpawner();
                   
                    break;
                case DJ_PickUp.SUBTYPE.EmpBlast:
                    gameController.UnlockEmpExplosion();
                    break;
                case DJ_PickUp.SUBTYPE.ChronoWatch:
                    gameController.UnlockChronoWatch();
                    break;
				case DJ_PickUp.SUBTYPE.GrenadeLauncher:
					gameController.UnlockGrenadeLauncher(); 
					break; 
                case DJ_PickUp.SUBTYPE.AIController:
                    if (pickUpSlot == null)
                        pickUpSlot = other.gameObject;
                    HUD.RemoveObjective(BR_Objectives.ObjectiveType.PICKUP);
                    HUD.AddObjective(BR_Objectives.ObjectiveType.CARRY, "motherboard");
                    spawner.isRewardAvailable = false;
                    
                   
                    break;
            }
            pickUP.hitPlayer();
        }
     }

	public void ShowSingleFireModel()
    {
		singleFireModel.SetActive (true);
		autoFireModel.SetActive (false);
		scatterFireModel.SetActive (false);
        launcherModel.SetActive(false);
    }

	public void ShowAutoFireModel()
    {
		autoFireModel.SetActive (true);
		scatterFireModel.SetActive (false);
	    singleFireModel.SetActive (false);
        launcherModel.SetActive(false);
    }

	public void ShowScatterFireModel()
    {
		scatterFireModel.SetActive (true);
		singleFireModel.SetActive (false);
		autoFireModel.SetActive (false);
        launcherModel.SetActive(false);
    }

    void DoSingleShot()
    {

        if (isTriggerReleased && weaponTimer <= 0)
        {
			Instantiate (singleFirePS, singleFirePoint.transform.position, singleFirePoint.transform.rotation); 
			KW_PlayerProjectile newBullet = Instantiate(projectile[0],singleFirePoint.transform.position, singleFirePoint.transform.rotation) as KW_PlayerProjectile;

            weaponTimer = singleShot_RateOfFire;
            isTriggerReleased = false;
            //play audio sfx
			audioController.Player_SingleShotProjectile();

        }
    }

    void DoAutoShot()
    {
		if (weaponTimer <= 0 && gameController.curAuto_Ammo > 0) {
			Instantiate (autoFirePS, autoFirePoint.transform.position, autoFirePoint.transform.rotation); 
			KW_PlayerProjectile newBullet = Instantiate (projectile [1], autoFirePoint.transform.position, autoFirePoint.transform.rotation) as KW_PlayerProjectile;
            weaponTimer = autoShot_RateOfFire;
            audioController.Player_SingleShotProjectile();
            if (!GODMODE) {
				gameController.curAuto_Ammo -= 1;
				HUD.UpdateAutoAmmoAmount (); // Update the amount of ammo is shown for the Auto Ammo Amount. 

			}
			//audioController.Player_AutoShotProjectile (); 
		}


    }

    void DoScatterShot()
    {
        if (isTriggerReleased && weaponTimer <= 0 && gameController.curScatter_Ammo >= 3)
        {
			Instantiate (scatterFirePS, scatterFirePoint.transform.position, scatterFirePoint.transform.rotation); 
			KW_PlayerProjectile newBullet1 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation) as KW_PlayerProjectile;
			KW_PlayerProjectile newBullet2 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, 6f, 0)) as KW_PlayerProjectile;
            KW_PlayerProjectile newBullet3 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, 12f, 0)) as KW_PlayerProjectile;
            KW_PlayerProjectile newBullet4 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, -6f, 0)) as KW_PlayerProjectile;
            KW_PlayerProjectile newBullet5 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, -12f, 0)) as KW_PlayerProjectile;
            weaponTimer = scatterShot_RateOfFire;
            isTriggerReleased = false;
            audioController.Player_MultiShotProjectile();

            if (!GODMODE)
            {
            gameController.curScatter_Ammo -= 3;
            HUD.UpdateScatterAmmoAmount(); // Update the amount of ammo is shown for the Scatter Ammo Amount. 
            
            }
		}

    }

    void DoSuper()
    {
        if (curSuperTimer > 0)
        {
            allSSfired = false;
            curSuperTimer -= gameController.superShots * Time.deltaTime;
            curSuperCheck = Mathf.RoundToInt(curSuperTimer);

            if (lastSuperCheck != curSuperCheck)
            {
                superProjectiles[superShotsFired] = Instantiate(projectile[3], launcherFirePoint.transform.position, launcherFirePoint.transform.rotation) as KW_PlayerProjectile;
                audioController.Player_GrenadeLaucherSFX();
                lastSuperCheck = curSuperCheck;
                superShotsFired++;
            }
        }
        else
        {
            if (!allSSfired)
            {
                gameController.CurrentWeapon = gameController.LastWeapon;
                allSSfired = true;
            }
            
            int shotsStuck = 0;
            for (int x = 0; x < gameController.superShots; x++)
            {
                if (superProjectiles[x].isStuck)
                    shotsStuck++;
            }
            if (shotsStuck == gameController.superShots)
            {
                for (int x = 0; x < gameController.superShots; x++)
                {
                    superProjectiles[x].StartStickyExplosion();
                }
                
                superProjectiles = new KW_PlayerProjectile[gameController.superShots];
                isSuperActive = false;

            }
        }
    }

    public void StartSuper()
    {
        curSuperTimer = gameController.superShots;
        lastSuperCheck = gameController.superShots;
        scatterFireModel.SetActive(false);
        singleFireModel.SetActive(false);
        autoFireModel.SetActive(false);
		launcherModel.SetActive(true);

        superShotsFired = 0;
        superProjectiles = new KW_PlayerProjectile[gameController.superShots];
        isSuperActive = true;
    }


    
    void SloMoMultipler()
    {
        if (gameController.isChronoWatchActive)
        {
            sloMoX = 6f; 
        }
   
        else
        {
            sloMoX = 1f;
        }

    }


	//emp explosion---------------------------------
	void EmpExplosion()
	{
		//explode damage
		//EmpExplosionVol.SetActive (true);


		curEmpExplosionDur -= Time.deltaTime * sloMoX;
		curEmpExplosionDur = Mathf.Clamp (curEmpExplosionDur, 0.0f, maxEmpExplosionDuration);

		if(curEmpExplosionDur <= 0.0f)
		{
			 
			//curEmpExplosionDur = maxEmpExplosionCoolDownDuration; 
			isEmpExplosionActive = false;
			//EmpExplosionVol.SetActive (false);
			isEmpExplosionCoolDownActive = true;

		}
	}



	void EmpExplosionActive()
	{

        if (Input.GetAxis("Blast") != 0 && isEmpExplosionActive == false && isEmpExplosionCoolDownActive == false && gameController.empBlastOn == true && Time.timeScale !=0 && allSSfired && !HUD.tutorialScreen.activeInHierarchy && !isEmpDashActive && !gameController.isChronoWatchActive) 
		{
			playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.POWERUP); 
			gunModels.SetActive (false); 

			Instantiate(empExplosionVolume, empExplosionVolumeSpawnPoint.transform.position, empExplosionVolumeSpawnPoint.transform.rotation);
			//EmpExplosionForce(); 
			curEmpExplosionDur = maxEmpExplosionCoolDownDuration; 
			cameraShake.ShakeForEMPDash (); 
			isEmpExplosionActive = true;
			audioController.Player_EmpExplosionSFX();
            
		}

	}



	void EmpExplosionCoolDown()
	{
		if(isEmpExplosionCoolDownActive == true)
		{
			curEmpExplosionCoolDownDuration += Time.deltaTime * sloMoX;
			HUD.empExplosionFillBar.color = HUD.regenColor; 
			playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.IDLE);
			Invoke ("SetGunActiveAgain", 0.5f); 


		}

		if(curEmpExplosionCoolDownDuration >= maxEmpExplosionCoolDownDuration)
		{
			isEmpExplosionCoolDownActive = false;
			curEmpExplosionCoolDownDuration = 0;
			HUD.empExplosionFillBar.color = HUD.empExplosionColor; 
			curEmpExplosionDur = maxEmpExplosionDuration; 
		}

       
	}

	void EmpDash()
	{
		
		curEmpDashDuration -= Time.deltaTime * sloMoX; 
		curEmpDashDuration = Mathf.Clamp (curEmpDashDuration, 0.0f, maxEmpDashActiveTimer);
        if (Input.GetAxis("HorizontalMenu") != 0 || Input.GetAxis("VerticalMenu") != 0)
        {
            float tempAngle = Mathf.Atan2(Input.GetAxis("HorizontalMenu"), Input.GetAxis("VerticalMenu")) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, tempAngle, 0);
        }
        movement.z = Time.deltaTime * empDashSpeed * sloMoX;

		gameObject.GetComponent<CapsuleCollider> ().enabled = false; 
        transform.Translate (movement); 

		if (curEmpDashDuration <= 0)
        { 
			
			gameObject.GetComponent<CapsuleCollider> ().enabled = true; 
			isEmpDashActive = false;
			isEmpDashCoolDown = true; 
			Invoke ("SetGunActiveAgain", 0.3f); 
		}

	}

	public void SetGunActiveAgain(){
		gunModels.SetActive (true); 
	}

	public void ChronoWatchAnimationON(){
		playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.POWERUP); 
	}
	public void ChronoWatchAnimationOFF(){
		playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.IDLE); 

	}

	public void AbleToEmpDash(){
		if (gameController.canDashSwitch) {
			gameController.canDashTimer += Time.deltaTime; 
			if (gameController.canDashTimer >= 0.1f) {
				gameController.doDash = true; 
				gameController.canDashSwitch = false; 
			}
		} else {
			gameController.canDashTimer = 0; 
		}
	}
	void EmpDashCoolDown(){
		
		// if the cool down is greater than or equal to zero start timer
		if (isEmpDashCoolDown == true)
		{
			
			curEmpDashCoolDownTimer += Time.deltaTime * sloMoX;
			isEmpDashActive = false; 
			HUD.empDashFillBar.color = HUD.regenColor; 
			//curEmpDashDuration += 0.0019f; 

		}

		if(curEmpDashCoolDownTimer >= maxEmpDashCoolDownDuration)
		{
			 
			isEmpDashCoolDown = false;
			curEmpDashCoolDownTimer = 0.0f;
			HUD.empDashFillBar.color = HUD.empDashColor; 
			curEmpDashDuration = maxEmpDashActiveTimer; 
		
		}
	}

	void TurnOFFHUDIcons(){
		// Turn off icons when the player has dash out of the colliders of these panels. 
		if (HUD.secretRoomPanel.activeInHierarchy == true) {
			HUD.secretRoomPanel.SetActive (false); 
		}
		if (HUD.hackingPanel.activeInHierarchy == true) {
			HUD.hackingPanel.SetActive (false); 
		}
		if (HUD.turretPanel.activeInHierarchy == true) {
			HUD.turretPanel.SetActive (false); 
		}
	}

	void TurnOnEmpDash(){
		if (gameController.doDash == true) {
			if (Input.GetButtonDown ("Dash") && !isEmpDashActive && !isEmpDashCoolDown && Time.timeScale != 0 && gameController.doDash == true && allSSfired && !HUD.tutorialScreen.activeInHierarchy && !isEmpExplosionActive && !gameController.isChronoWatchActive)
            {
				playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.DASH); 
				
				Invoke ("TurnOFFHUDIcons", 0.3f);
				isEmpDashActive = true;
                lastLocation = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
				cameraShake.ShakeForEMPDash (); 
				audioController.Player_EmpDashSFX ();

				gunModels.SetActive (false); 


			}
		}

	}

	void RayCastDetection()
    {
        if (Input.GetAxis("VerticalMenu") != 0 || (Input.GetAxis("HorizontalMenu") != 0 && !isEmpDashActive))
        {
            float tempAngle = Mathf.Atan2(Input.GetAxis("HorizontalMenu"), Input.GetAxis("VerticalMenu")) * Mathf.Rad2Deg;
            raycastLocation.transform.rotation = new Quaternion(0, tempAngle, 0,0);
        }
        else
        {
            raycastLocation.transform.rotation = new Quaternion(0, 0, 0,0);
        }
        RaycastHit hit;
        //Debug.DrawRay(raycastLocation.transform.position, raycastLocation.transform.forward, Color.green, 2);
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1))
        {
            if (hit.collider.gameObject.tag == "Environment")
            {
                if (isEmpDashActive == true)
                {
                    gameObject.GetComponent<CapsuleCollider>().enabled = true;
					isEmpDashActive = false;
					isEmpDashCoolDown = true; 
					gunModels.SetActive (true); 
					playerControllerAnimationMachine.ChangeState (PlayerControllerAnimationMachine.PLAYERSTATE.IDLE); 
                }
            }
        }
    }
	


    void ShieldActivate ()
    {
        if (curShield > 0)
        {
			//gameObject.GetComponent<Collider>().enabled =false;
            shieldVisual.SetActive(true);
			 
        }
    else
        {
            //gameObject.GetComponent<Collider>().enabled = true;
            shieldVisual.SetActive(false);
		
        }
			
    }



    public void HealthCheck()
    {
        if (curShield <= 0 && curHealth <= 0)
        {
            gameController.PlayerDeath();
            isEmpDashActive = false;
            Destroy(gameObject);
        }
    }

    public void TeleportPlayer(Vector3 _position)
    {
        gameObject.transform.position = _position;
    }
}
