using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTest : MonoBehaviour 
{
	public float MoveSpeed = 5f;
	private Rigidbody RigBod;





	AudioSource audiosource;
	[SerializeField]
	private bool GODMODE = false;
	//HUD
	private SP_HUD HUD; 
	AudioController audioController;
	WaveSpawner spawner;

	//health varaibles 
	public int maxHealth = 100;
	public int curHealth;
	public int maxShield = 100;
	public int curShield;

	//moving variables 
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
	private bool isMeleeActive = false;

	private BR_GameController gameController;

	[Space]
	[Header("FireMode")]

	public GameObject singleFireModel; 
	public GameObject autoFireModel; 
	public GameObject scatterFireModel; 
	[Space]
	public GameObject singleFirePoint;
	public GameObject autoFirePoint;
	public GameObject scatterFirePoint;
	[Space]
	public GameObject singleFirePS; 
	public GameObject autoFirePS; 
	public GameObject scatterFirePS; 

	[SerializeField]
	private float singleShot_RateOfFire = 1f;
	[SerializeField]
	private float autoShot_RateOfFire = .5f;
	[SerializeField]
	private float scatterShot_RateOfFire = 1.5f;
	public float weaponTimer;
	public float FiringTriggerState;
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
	public GameObject shieldVisual;

	void Start()
	{
		RigBod = GetComponent<Rigidbody> ();



		cameraPrefab = FindObjectOfType<CameraController>();
		cameraPrefab.FindPlayer ();
		HUD = FindObjectOfType<SP_HUD>(); // Reference for HUD PREFAB
		//reference to the audiosource object
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
		spawner = FindObjectOfType<WaveSpawner>();
		//meleeVolume = GameObject.Find("MeleeVolume");
		meleeVolumeSpawnPoint.transform.Translate(new Vector3(0, 0, meleeDistance));
		// meleeVolume.SetActive(false);
		gameController = FindObjectOfType<BR_GameController>();
		//CurEmpDuration = EmpBoostDur;
		curHealth = maxHealth;
		if (gameController.isShieldUnlocked)
			curShield = maxShield;
		else
			curShield = 0;
		curEmpExplosionDur = maxEmpExplosionCoolDownDuration;
		//emp explosion---------------------------------------------
		//EmpExplosionVol = GameObject.Find ("EmpExplosion");

		//EmpExplosionVol.SetActive (false);

		//shieldVisual.SetActive(false);
	}


	void FixedUpdate()
	{
		float MoveHorizontal = Input.GetAxis ("Horizontal");
		transform.Translate(movement, Space.World);
		float MoveVertical = Input.GetAxis ("Vertical");
		transform.Translate(movement, Space.World);
		Vector3 Movement = new Vector3 (MoveHorizontal, 0.0f, MoveVertical);
		RigBod.velocity = Movement * MoveSpeed;

		if (gameController.isMouseActive)
		{
			Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
			Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

			angle = Mathf.Atan2 (mouseOnScreen.y - positionOnScreen.y, mouseOnScreen.x - positionOnScreen.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (new Vector3 (0f, 90 - angle, 0));
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

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.G))
			GODMODE = !GODMODE;
		if (isEmpDashActive)
		{
			EmpDash ();
			EmpDashPS.SetActive(true);
		}
		else
		{
			//UpdatePosition ();
			EmpDashPS.SetActive(false);
		}

		WeaponFunction();
		SloMoMultipler();

		if (Input.GetButtonDown("Melee")&& curMeleeDuration <= 0 && Time.timeScale != 0)
		{
			TurnOnMelee();
		}
		if (isMeleeActive)
		{
			DoMelee();
		}
		FiringTriggerState = Input.GetAxis("Fire");

		//emp explosion-------------------------------------
		if(isEmpExplosionActive == true)
		{
			EmpExplosion();
		}
		EmpExplosionActive ();
		EmpExplosionCoolDown ();
		EmpDashCoolDown (); 
		TurnOnEmpDash ();
		SheildActivate();
		RayCastDetection ();
		HealthCheck();
		ShowSingleFireModel (); 
		ShowAutoFireModel (); 
		ShowScatterFireModel (); 
	}
	void DoMelee()
	{
		curMeleeDuration -= Time.deltaTime * sloMoX;
		if (curMeleeDuration <= 0)
		{
			//meleeVolume.SetActive(false); 
			isMeleeActive = false;
		}
	}
	void TurnOnMelee()
	{
		curMeleeDuration = meleeDuration;
		Instantiate(meleeVolume, meleeVolumeSpawnPoint.transform.position, meleeVolumeSpawnPoint.transform.rotation); 
		isMeleeActive = true;
		audioController.Player_MeleeSFX();
	}

	void WeaponFunction()
	{
		if (weaponTimer > 0)
			weaponTimer -= Time.deltaTime * sloMoX;
		if (Input.GetAxis("Fire") != 0 && Time.timeScale != 0)
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
		HUD.isDamageTaken (); 
		HUD.ShieldFillAmount (); 

		HUD.DoFromSizeShield (); 
		curShield -= value;

		HUD.DoOrangeLerpShield (); 
		HUD.DoNewSizeShield ();
		curShield = Mathf.Clamp(curShield, 0, maxShield);
		Debug.Log("TAKING DAMAGE" + curShield);
	}

	public void ModHealth(int value)
	{

		HUD.isDamageTaken (); 
		HUD.HealthFillAmount (); 

		HUD.DoFromSizeHealth (); 

		curHealth -= value;
		curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

		HUD.DoOrangeLerpHealth (); 
		HUD.DoNewSizeHealth (); 
		Debug.Log("TAKING DAMAGE" + curHealth);
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

	}

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
				}

				// if the player doesnt have a shield take away health 
				if (curShield <= 0)
				{
					ModHealth(other.GetComponent<KW_Enemy_Projectile>().damageDealt);
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
			print("the player has hit a pickup");

			DJ_PickUp pickUP = other.GetComponent<DJ_PickUp>();
			pickUP.hitPlayer();
			audioController.PickUos_PickUpSFX();

			switch (pickUP.subTypeName)
			{
			case DJ_PickUp.SUBTYPE.Health:
				print("the player has hit a health pickup");
				AddHealth(pickUP.amount);
				break;

			case DJ_PickUp.SUBTYPE.Shield:
				print("the player has hit a shield pickup");
				if (!gameController.isShieldUnlocked)
				{
					gameController.TurnOnShild();
					AddShield(maxShield);
				}
				else
					AddShield(pickUP.amount);
				break;

			case DJ_PickUp.SUBTYPE.Auto:
				print("the player has hit a automatic weapon pickup");
				gameController.UnlockAuto();
				gameController.curAuto_Ammo += pickUP.amount;
				HUD.UpdateAutoAmmoAmount();
				break;

			case DJ_PickUp.SUBTYPE.Scatter:
				print("the player has hit a scatter ammo pickup");
				gameController.UnlockScatter();
				gameController.curScatter_Ammo += pickUP.amount;
				HUD.UpdateScatterAmmoAmount();
				break;

			case DJ_PickUp.SUBTYPE.Blaster:
				print("the player has unlocked the blaster");
				gameController.UnlockBlaster();
				break;

			case DJ_PickUp.SUBTYPE.Key:
				print("the player has picked up a key");
				//code here to open the door associated with that key 
				gameController.OpenDoor(pickUP.amount);
				break;

			case DJ_PickUp.SUBTYPE.RegenVol:
				AddHealth(1000000);
				AddShield(1000000);
				gameController.curAuto_Ammo += 300;
				gameController.curScatter_Ammo += 120;
				break;

			case DJ_PickUp.SUBTYPE.Thruster:
				print("the player has picked up the thruster");
				break;

			case DJ_PickUp.SUBTYPE.Engine:
				spawner.ResetSpawner();
				print("the player has picked up the engine");
				break;
			case DJ_PickUp.SUBTYPE.EmpBlast:
				gameController.UnlockEmpExplosion();
				break;
			case DJ_PickUp.SUBTYPE.ChronoWatch:
				gameController.UnlockChronoWatch();
				break;
			}
		}
	}

	public void ShowSingleFireModel(){

		if (gameController.CurrentWeapon == BR_GameController.WEAPON.SINGLE) {
			singleFireModel.SetActive (true);

			autoFireModel.SetActive (false);
			scatterFireModel.SetActive (false);
		}



	}

	public void ShowAutoFireModel(){


		if (gameController.CurrentWeapon == BR_GameController.WEAPON.AUTO) {
			autoFireModel.SetActive (true);
			scatterFireModel.SetActive (false);
			singleFireModel.SetActive (false);
		}


	}

	public void ShowScatterFireModel(){

		if (gameController.CurrentWeapon == BR_GameController.WEAPON.SCATTER) {
			scatterFireModel.SetActive (true);
			singleFireModel.SetActive (false);
			autoFireModel.SetActive (false);
		}
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

			if (!GODMODE) {
				gameController.curAuto_Ammo -= 1;
				HUD.UpdateAutoAmmoAmount (); // Update the amount of ammo is shown for the Auto Ammo Amount. 
				weaponTimer = autoShot_RateOfFire;
				audioController.Player_SingleShotProjectile ();

			}
			//audioController.Player_AutoShotProjectile (); 
		}


	}



	void DoScatterShot()
	{
		if (isTriggerReleased && weaponTimer <= 0 && gameController.curScatter_Ammo >0)
		{
			Instantiate (scatterFirePS, scatterFirePoint.transform.position, scatterFirePoint.transform.rotation); 
			KW_PlayerProjectile newBullet1 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation) as KW_PlayerProjectile;
			KW_PlayerProjectile newBullet2 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, 6f, 0)) as KW_PlayerProjectile;
			KW_PlayerProjectile newBullet3 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, 12f, 0)) as KW_PlayerProjectile;
			KW_PlayerProjectile newBullet4 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, -6f, 0)) as KW_PlayerProjectile;
			KW_PlayerProjectile newBullet5 = Instantiate(projectile[2], scatterFirePoint.transform.position, scatterFirePoint.transform.rotation * Quaternion.Euler(0, -12f, 0)) as KW_PlayerProjectile;


			if (!GODMODE)
			{
				gameController.curScatter_Ammo -= 3;
				HUD.UpdateScatterAmmoAmount(); // Update the amount of ammo is shown for the Scatter Ammo Amount. 
				weaponTimer = scatterShot_RateOfFire;
				isTriggerReleased = false;
				audioController.Player_MultiShotProjectile();
			}
		}

	}

	void SloMoMultipler()
	{
		if (gameController.isChronoWatchActive)
		{
			sloMoX = 4f; 
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

		if (Input.GetAxis("Blast") != 0 && isEmpExplosionActive == false && isEmpExplosionCoolDownActive == false && gameController.empBlastOn == true && Time.timeScale !=0) 
		{
			print("Player has exploded.");

			Instantiate(empExplosionVolume, empExplosionVolumeSpawnPoint.transform.position, empExplosionVolumeSpawnPoint.transform.rotation);
			curEmpExplosionDur = maxEmpExplosionCoolDownDuration; 
			isEmpExplosionActive = true;
			audioController.Player_EmpExplosionSFX();

		}

	}

	void EmpExplosionCoolDown()
	{
		if(isEmpExplosionCoolDownActive == true)
		{
			curEmpExplosionCoolDownDuration -= Time.deltaTime * sloMoX;
			HUD.empExplosionFillBar.color = HUD.regenColor; 
			curEmpExplosionDur += 0.0035f; 

		}

		if(curEmpExplosionCoolDownDuration <= 0.0f)
		{
			curEmpExplosionCoolDownDuration = maxEmpExplosionCoolDownDuration;
			isEmpExplosionCoolDownActive = false;
			HUD.empExplosionFillBar.color = HUD.empExplosionColor; 
			curEmpExplosionDur = maxEmpExplosionDuration; 
		}


	}

	void EmpDash()
	{

		curEmpDashDuration -= Time.deltaTime * sloMoX; 
		curEmpDashDuration = Mathf.Clamp (curEmpDashDuration, 0.0f, maxEmpDashActiveTimer);

		movement.z = Time.deltaTime * empDashSpeed * sloMoX;
		transform.Translate (movement); 

		if (curEmpDashDuration <= 0)
		{
			isEmpDashActive = false;
			isEmpDashCoolDown = true; 
		}

	}

	void EmpDashCoolDown(){
		// if the cool down is greater than or equal to zero start timer
		if (curEmpDashCoolDownTimer >= 0.0f && isEmpDashCoolDown == true)
		{
			curEmpDashCoolDownTimer -= Time.deltaTime * sloMoX;
			isEmpDashActive = false; 
			HUD.empDashFillBar.color = HUD.regenColor; 
			curEmpDashDuration += 0.0009f; 

		}

		if(curEmpDashCoolDownTimer <= 0)
		{
			curEmpDashCoolDownTimer = maxEmpDashCoolDownDuration;
			isEmpDashCoolDown = false;
			HUD.empDashFillBar.color = HUD.empDashColor; 
			curEmpDashDuration = maxEmpDashActiveTimer; 

		}
	}

	void TurnOnEmpDash(){
		if (Input.GetButtonDown ("Dash") && !isEmpDashActive && !isEmpDashCoolDown && Time.timeScale != 0) {

			isEmpDashActive = true;

			print("Player has Dahsed");
			audioController.Player_EmpDashSFX();
		}

	}

	void RayCastDetection(){

		RaycastHit hit; 
		if(Physics.Raycast(transform.position, transform.forward, out hit, 0.5f)){
			if (hit.collider.gameObject.tag == "Environment") {

				if (isEmpDashActive == true) {
					isEmpDashActive = false; 
				}
			}

		}
	}


	void SheildActivate ()
	{
		if (curShield > 0)
		{
			gameObject.GetComponent<Collider>().enabled =false;
			shieldVisual.SetActive(true);
		}
		else
		{
			gameObject.GetComponent<Collider>().enabled = true;
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


}





