using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class DJ_PowerPanel : MonoBehaviour {


    public int maxHealth = 50;
    public int curHealth;

	public GameObject destoryedPS;
    public GameObject powerPanelLight;

    public bool isAlive;
    public bool isDamagable;
    public bool isPulsing = false;

	[Space]
	[Header("Power Panel Healthbar UI")]

	public GameObject HealthBar; 
	public RectTransform HealthBarORANGE; 
	public RectTransform HealthBarRED; 
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f); 
	[Space]
	private float orangeSpeed = 0.01f;
	private bool orangeLerp = false; 
	private float fromSize; 
	private float newSize;

	private AudioController audioController;

    private GameObject Boss2;

    private Color curColor;

	// Use this for initialization
	void Start ()
    {
        curColor = powerPanelLight.GetComponent<Light>().color;
		audioController = FindObjectOfType<AudioController> (); 
		HealthBar.SetActive (false);
        isAlive = false;
        powerPanelLight.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		HUD_Health (); 
        if (isPulsing)
        {
            powerPanelLight.GetComponent<Light>().color = Color.Lerp(curColor, Color.white, Mathf.PingPong(Time.time, .5f));
        }
	}

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "PlayerProjectile" && isAlive && isDamagable)
        {
            //take damage function
            TakeDamage(other.GetComponent<KW_PlayerProjectile>().damageDealt);
            Destroy(other);

        }
    }

	public float GetHealthPct(){
		return Mathf.Clamp((float)curHealth / maxHealth,0,1); 
	}

	void HUD_Health(){

		if (curHealth < maxHealth && curHealth > 0f) {
			HealthBar.SetActive (true); 
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

	public void DoFromSize(){
		fromSize = GetHealthPct();
	}
	public void DoOrangeLerp(){
		orangeLerp = true;
	}
	public void DoNewSize(){
		newSize = GetHealthPct ();
	}
	public void DoOrangeColor(){
		HealthBarORANGE.GetComponent<Image>().color = orangeColor; // Change Orange Health Bar to Orange Color

	}


    void TakeDamage(int DMGamount)
    {
		DoFromSize (); 
	
        curHealth -= DMGamount;

		DoOrangeLerp (); 
		DoNewSize (); 
		DoOrangeColor (); 

        if(curHealth <= 0)
        {
            Boss2.GetComponent<BR_EnemyHealth>().enemyShield.hasShownTutorial = true;
			destoryedPS.SetActive (true); 
			audioController.Boss2_PowerPanelExplosionSFX (); 
            isAlive = false;
            isPulsing = false;
            powerPanelLight.SetActive(false);
			HealthBar.SetActive (false);
            Boss2.GetComponent<BR_EnemyHealth>().MakeBossVulnerable();
        }
    }

   public void SetActive (GameObject _boss)
    {
        Boss2 = _boss;
        curHealth = maxHealth;
        isAlive = true;
        isDamagable = true;
        powerPanelLight.SetActive(true);
        destoryedPS.SetActive(false);
    }
}
