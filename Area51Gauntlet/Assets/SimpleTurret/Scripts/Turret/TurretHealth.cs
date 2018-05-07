using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHealth : MonoBehaviour {

	private BR_GameController gameController;
	private SP_HUD HUD;

	TurretController controller;
	private GameObject player; 
	private GameObject rayCast; 

	[Tooltip("Weather the turret is Destroyed or not")]
	public bool isDestroyed = false;
    [SerializeField]
    private GameObject enemyContainer;

	[Tooltip("Turret Current Health")]
	private int curHealth; 
	public int maxHealth = 100; 
	[Tooltip("Total Health of the Turret in Start")]
	public float totalHealth = 100; //Should be used when repairing is done

	[Space]

	//HUD
	public GameObject HealthBar; 
	public RectTransform HealthBarORANGE; 
	public RectTransform HealthBarRED; 

	private float orangeSpeed = 0.01f;
	private bool orangeLerp = false; 
	private float fromSize; 
	private float newSize; 

	[Space]
	//pickup
	//lists pickups to spawn
	public GameObject[] PickupTypes;


	void Start(){
		gameController = FindObjectOfType<BR_GameController> (); 
		HUD = FindObjectOfType<SP_HUD> (); // Reference for HUD PREFAB
		controller = this.GetComponent<TurretController> ();
		curHealth = maxHealth; 

		//rayCast = GameObject.Fin
	}

	void Update (){
		HUD_Health ();
	}

	void OnTriggerEnter(Collider target)
	{
		if (target.tag == "PlayerMelee")
		{
			player = GameObject.FindGameObjectWithTag("Player");
			TakeDamage(BR_EnemyHealth.DMGTYPE.MELEE,player.GetComponent<PlayerController>().meleeDamageAmount);
			//gameObject.GetComponent<AIAgent>().ChangeState(AIAgent.AISTATE.CHASE);
			Debug.Log("AI's CurHealth " + curHealth);
		}

		if (target.tag == "PlayerProjectile")
		{
			//player = GameObject.FindGameObjectWithTag("Player");
			//Change melee damage amount to projectile damage amount when it is made
			TakeDamage(target.GetComponent<KW_PlayerProjectile>().bulletType,target.GetComponent<KW_PlayerProjectile>().damageDealt);
			//gameObject.GetComponent<AIAgent>().ChangeState(AIAgent.AISTATE.CHASE);
			Debug.Log("AI's CurHealth " + curHealth);
			Destroy(target);
		}


	}
	void TakeDamage(BR_EnemyHealth.DMGTYPE _dmgType,int DMGamount)
	{
		gameController.ModHighScore (curHealth); // Grant High Score per Damage Taken

		fromSize = GetHealthPct(); 
		curHealth -= DMGamount;

		orangeLerp = true; 
		newSize = GetHealthPct (); 

		curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

		if (curHealth <= 0)
		{
			HUD.ComboScoreMultiplerADDTIME (_dmgType); // Reset Timer for Combo Meter 
			gameController.ModHighScore (maxHealth); // Grant High Score base off overall enemy health	

			SpawnPickup(); 
			isDestroyed = true;
			controller._Audio.Play_GetHit ();
			Destroy(enemyContainer);

		}
	}

	void SpawnPickup()
	{
		if (PickupTypes.Length != 0)
		{
			int randIndex = Random.Range(0, PickupTypes.Length);
			//Selects one pick up to spawn.
			Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 0, 0));
		}
	}


	/*
	//Apply damage to Turret
	public void ApplyDamage(float damage){
		fromSize = GetHealthPct(); 

		if (curHealth - damage > maxHealth) {
		
			curHealth -= damage;
			orangeLerp = true; 
			newSize = GetHealthPct (); 
		
		} else {
		
			isDestroyed = true;
			curHealth = 0;

			Destroy ();
		}

		controller._Audio.Play_GetHit ();
	}
	*/

	void Destroy(){
	
		controller.MeshCollider_Status (true);
		controller.isKinematicRigidbodies (false);
		controller._Raycast.TurretLaser_Status (false);

		gameObject.SetActive(false);
	}

	public float GetHealthPct(){
		return (float)curHealth / maxHealth; 
	}

	void HUD_Health(){
		if (curHealth >= maxHealth) {
			HealthBar.SetActive (false);
		}
		if (curHealth < maxHealth) {
			HealthBar.SetActive (true); 
		}

		HealthBarRED.localScale = new Vector3 (GetHealthPct (), 1f, 1f); 

		if (orangeLerp) {
			HealthBarORANGE.localScale = new Vector3 (fromSize -= orangeSpeed, 1f, 1f);
		}
		if (fromSize <= newSize) {
			orangeLerp = false;
			HealthBarORANGE.localScale = HealthBarRED.localScale;
		}
	}

}
