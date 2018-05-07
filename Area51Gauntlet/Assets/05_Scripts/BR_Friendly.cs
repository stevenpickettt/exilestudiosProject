using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public class BR_Friendly : MonoBehaviour {

	private BR_GameController gameController;
	private SP_HUD HUD; 
	private AIAgent aiAgent; 
	private BR_EnemyHealth enemyHealth; 
	private GameObject playerController;

    [SerializeField]
    private float repairRange =5f;
    

	[Header("Turrets's Health")]
	public int maxHealth = 100;
	public int curHealth;
    [SerializeField]
    GameObject EnemyDeathPS;
    [Space]
	[Header("TURRET HEALTH BAR UI")]

	public GameObject HealthBar; 
	public RectTransform HealthBarORANGE; 
	public RectTransform HealthBarRED; 
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f); 

	private float orangeSpeed = 0.01f;
	private bool orangeLerp = false; 
	private float fromSize; 
	private float newSize;

    public bool done = false;
    public bool isActive = false;
    float curCount = 0f;
    Transform curPosition;
	// Use this for initialization
	void Start ()
	{
		HUD = FindObjectOfType<SP_HUD> (); // Reference for HUD PREFAB
		gameController = FindObjectOfType<BR_GameController> ();
        HUD.turretScript = this;
        HUD.SetTurretHUD(true);
        curPosition = gameObject.transform;
        HealthBar.SetActive (false);
        if (gameController.showTutorial)
        {
            HUD.DoTutorialScreen("Hold {0}{1}</color> near the Friendly Turret to Activate", "Interact", "X Button", "F Key");
            HUD.TutorialTurretScreen();
        }
        HUD.AddObjective(BR_Objectives.ObjectiveType.ACTIVATE, "The Turret");
    }

	// Update is called once per frame
	void Update ()
	{
        gameObject.transform.position = new Vector3 (curPosition.transform.position.x, 0,curPosition.transform.position.z);
        gameObject.transform.rotation = curPosition.transform.rotation;
        HUD_Health();
        if (playerController != null)
        {
            float dist = Vector3.Distance(playerController.transform.position, transform.position);
            if (dist < repairRange && curHealth < maxHealth && !done)
            {
                HUD.turretPanel.SetActive(true);
                HUD.turretFillBar.fillAmount = GetHealthPct();
                if (Input.GetButton("Interact"))
                {
                    curCount += Time.deltaTime /2;
                    curHealth += Mathf.RoundToInt(curCount);
                }
            }
            else
                HUD.turretPanel.SetActive(false);
        }
        else
            playerController = GameObject.FindGameObjectWithTag("Player");

   }

	void OnTriggerEnter(Collider target)
	{
        if (isActive)
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
                TakeDamage(target.GetComponent<DJ_DroneExplosion>().explosionDamage);
            }
        }
	}
		
	public void TakeDamage(int DMGamount)
	{
        HUD.TurretTakingDamage();
		fromSize = GetHealthPct(); // Grab the Starting Point
		HUD.fromSizeTurret = HUD.GetTurretHealthPct();
		fromSize = Mathf.Clamp(fromSize, 0, 1);
		curHealth -= DMGamount; // Take Damage 
		orangeLerp = true; // Start your Lerping Process
		HUD.orangeLerpTurret = true;  
		newSize = GetHealthPct (); // Grab your End Point
		HUD.newSizeTurret =  HUD.GetTurretHealthPct ();  
		HealthBarORANGE.GetComponent<Image> ().color = orangeColor; // Change Orange Health Bar to Orange Color
		HUD.HealthBarORANGETurret.GetComponent<Image> ().color = HUD.orangeColor;
		curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        if (curHealth <= 0)
        {
            StartDestroy();
        }
    }

	

	public float GetHealthPct(){
		return Mathf.Clamp((float)curHealth / maxHealth,0,1); 
	}

	void HUD_Health(){
        if (!isActive)
        {
            HealthBarRED.GetComponent<Image>().color = Color.gray;
            if (curHealth >= maxHealth)
            {
                HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.ACTIVATE);
                HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "Friendly Turret");
                HealthBarRED.GetComponent<Image>().color = Color.cyan;
                isActive = true;
            }
        }
		if (curHealth <= maxHealth && curHealth !=0)
        {
			HealthBar.SetActive (true);
        }
        else
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
    void StartDestroy()
    {
        if (HealthBar.activeInHierarchy)
        {
            done = true;
            HUD.SetTurretHUD(false);
            HUD.objectivePanel.FailObjective(BR_Objectives.ObjectiveType.PROTECT);
            HUD.turretPanel.SetActive(false);
            HealthBar.SetActive(false);
            GetComponent<Renderer>().enabled = false;
            GetComponent<TurretRotation>().rotationSpeed = 0;
            GetComponent<Collider>().enabled = false;
            Instantiate(EnemyDeathPS, transform.position, transform.rotation);
            Destroy(gameObject, 1f);
        }
    }
}
