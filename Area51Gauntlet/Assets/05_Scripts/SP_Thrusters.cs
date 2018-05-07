using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public class SP_Thrusters : MonoBehaviour {

	private BR_GameController gameController;
	private SP_HUD HUD; 
	private AIAgent aiAgent; 
	private BR_EnemyHealth enemyHealth; 
	private GameObject playerController;
    [SerializeField]
    GameObject blocker;
    [SerializeField]
    GameObject ThrusterModel;

	public enum THRUSTERTYPE { THRUSTER_A = 0, THRUSTER_B = 1 };
	public THRUSTERTYPE CurrentThruster = THRUSTERTYPE.THRUSTER_A;

	[Header("Thruster's Health")]
	public int maxHealth = 100;
	public int curHealth;

	[Space]
	[Header("THRUSTER HEALTH BAR UI")]

	public GameObject HealthBar; 
	public RectTransform HealthBarORANGE; 
	public RectTransform HealthBarRED; 
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f); 

	private float orangeSpeed = 0.01f;
	private bool orangeLerp = false; 
	private float fromSize; 
	private float newSize;

    public bool done = false;


	[Space]
	[Header("Engine Room Pick-ups")]
	//pickup
	//lists pickups to spawn
	public GameObject thrusterDrop;


	// Use this for initialization
	void Start ()
	{
		HUD = FindObjectOfType<SP_HUD> (); // Reference for HUD PREFAB
		gameController = FindObjectOfType<BR_GameController> ();

        SetHealth(); 

		HealthBar.SetActive (false);


  

    }

	// Update is called once per frame
	void Update ()
	{
        if (!done)
		HUD_Health ();

	}

	void OnTriggerEnter(Collider target)
	{
		
		if (target.tag == "Melee")
		{
			TakeDamage(target.GetComponent<DJ_MeleeAttack>().meleeDamage);
		}

		if (target.tag == "EnemyProjectile")
		{
			TakeDamage(target.GetComponent<KW_Enemy_Projectile>().damageDealt);
		}

		if (target.tag == "DroneExplosion")
		{
			TakeDamage (target.GetComponent<DJ_DroneExplosion>().explosionDamage); 
		}

	}
		
	public void TakeDamage(int DMGamount)
	{
        HUD.ThrusterTakingDamage(CurrentThruster);
		fromSize = GetHealthPct(); // Grab the Starting Point

		if (CurrentThruster == THRUSTERTYPE.THRUSTER_A) {
			HUD.fromSizeThrusterA = HUD.GetThrusterAHealthPct (); 
		} else {
			HUD.fromSizeThrusterB = HUD.GetThrusterBHealthPct (); 
		}

		fromSize = Mathf.Clamp(fromSize, 0, 1);

		curHealth -= DMGamount; // Take Damage 

		orangeLerp = true; // Start your Lerping Process

		if (CurrentThruster == THRUSTERTYPE.THRUSTER_A) {
			HUD.orangeLerpThrusterA = true;  
		} else {
			HUD.orangeLerpThrusterB = true; 
		}

		newSize = GetHealthPct (); // Grab your End Point

		if (CurrentThruster == THRUSTERTYPE.THRUSTER_A) {
			HUD.newSizeThrusterA =  HUD.GetThrusterAHealthPct ();   
		} else {
			HUD.newSizeThrusterB =  HUD.GetThrusterBHealthPct ();  
		}


		HealthBarORANGE.GetComponent<Image> ().color = orangeColor; // Change Orange Health Bar to Orange Color

		if (CurrentThruster == THRUSTERTYPE.THRUSTER_A) {
			HUD.HealthBarORANGEThrusterA.GetComponent<Image> ().color = HUD.orangeColor;
		} else {
			HUD.HealthBarORANGEThrusterB.GetComponent<Image> ().color = HUD.orangeColor;
		}

		curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

        if (curHealth <= 0)
        {
            playerController = GameObject.FindGameObjectWithTag("Player");
            Destroy(playerController);
            gameController.PlayerDeath();
        }
    }

	 public void SpawnPickup()
	{
        done = true;
        GameObject dropItem = Instantiate(thrusterDrop, transform.position, Quaternion.Euler(0, 180, 0));
        switch (CurrentThruster)
        {
            case THRUSTERTYPE.THRUSTER_A:
                dropItem.GetComponent<DJ_PickUp>().amount = 0;
                break;

            case THRUSTERTYPE.THRUSTER_B:
                dropItem.GetComponent<DJ_PickUp>().amount = 1;
                break;
        }

        blocker.GetComponent<Collider>().enabled = false;
        ThrusterModel.SetActive(false);
        HealthBar.SetActive(false);
    }

	public float GetHealthPct(){
		return Mathf.Clamp((float)curHealth / maxHealth,0,1); 
	}

	void HUD_Health(){

		if (curHealth < maxHealth && curHealth !=0)
        {
			HealthBar.SetActive (true);
        }
        else
        {
            HealthBar.SetActive(false);
        }
        blocker.GetComponent<Collider>().enabled = true;
        ThrusterModel.SetActive(true);

        HealthBarRED.localScale = new Vector3 (GetHealthPct(), 1f, 1f); 

		if (orangeLerp) {
			HealthBarORANGE.localScale = new Vector3 (fromSize -= orangeSpeed, 1f, 1f);

		}
		if (fromSize <= newSize) {
			orangeLerp = false;
			HealthBarORANGE.localScale = HealthBarRED.localScale;
		}

	}

	public void ResetThruster()
    {
        done = false;
        //gameObject.GetComponent<MeshRenderer>().enabled = true;
        blocker.GetComponent<Collider>().enabled = true;
        ThrusterModel.SetActive(true);
        SetHealth();
    }

    public void SetHealth()
    {

        curHealth = maxHealth;
        /*
        switch (gameController.spawner.waveDifficulty)
        {
            case BR_GameController.GameDifficulty.EASY:
                {
                    curHealth = maxHealth + 250;

                }
                break;
            case BR_GameController.GameDifficulty.NORMAL:
                {
                    curHealth = maxHealth;
                }
                break;
            case BR_GameController.GameDifficulty.HARD:
                {
                    curHealth = maxHealth - 250;
                }
                break;

        }
        */ 
    }


}
