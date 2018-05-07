using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class EnemyShield : MonoBehaviour {



    [SerializeField]
    public int maxShield = 100;
    [SerializeField]
    public int curShield;

    private SP_HUD HUD;
    private BR_GameController gameController;
    public BR_EnemyHealth enemyHealth; 

	public RectTransform healthBarBLUE; 
	public GameObject ShieldBar;
	public GameObject HealthBar; 

	public GameObject playerBlockage;

    public bool isShieldActive = true;
    public bool isOvershield = false;

    int count = 0;
    public bool hasShownTutorial = false;

	// Use this for initialization
	void Start ()
    {
		
        HUD = FindObjectOfType < SP_HUD > ();
        gameController = FindObjectOfType<BR_GameController>();

        isShieldActive = true; 
        curShield = maxShield; 
	}
	
	// Update is called once per frame
	void Update () {
		
        if (!isOvershield)
            HUD_Shield();
    }

    void OnTriggerEnter(Collider target)
    {
        if (!isOvershield)
        {
            if (target.tag == "PlayerProjectile")
            {
                TakeShieldDamage(target.GetComponent<KW_PlayerProjectile>().damageDealt);

                Destroy(target);

            }

            if (target.tag == "DroneExplosion")
            {
                TakeShieldDamage(enemyHealth.explosiveDamage);

            }
        }
        else if (target.tag == "PlayerProjectile")
        {
            count++;
            if (count >= 5 && !hasShownTutorial && gameController.showTutorial)
            {
                HUD.DoTutorialScreen("Bullets will not penitrate this shield. Find a way to bring it down\n \nPress {0}{1}</color>", "Interact", "X Button", "F Key");
                HUD.TutorialPowerPanelScreen();
                hasShownTutorial = true;
                enemyHealth.toolTipActive = true;
                HUD.AddObjective(BR_Objectives.ObjectiveType.FIND, "shield");
            }
        }
    }



	public float GetShieldPct(){
		return Mathf.Clamp((float)curShield / maxShield,0,1); 
	}

	void HUD_Shield(){
		if (curShield < maxShield) {
			
			ShieldBar.SetActive (true); 
		}

		healthBarBLUE.localScale = new Vector3 (GetShieldPct(), 1f, 1f); 
	}


    void TakeShieldDamage(int ShieldDMG)
    {
        curShield -= ShieldDMG;

        curShield = Mathf.Clamp(curShield, 0, maxShield);



        if(curShield <= 0)
        {
			HealthBar.SetActive (true);
            isShieldActive = false; 
			 
			/*
			gameObject.GetComponent<MeshRenderer>().enabled = false; 
			gameObject.GetComponent<SphereCollider>().enabled = false;
			playerBlockage.GetComponent<SphereCollider>().enabled = false;
			*/ 

			Destroy (ShieldBar);
			Destroy (gameObject); 

			 

        }
    }
}
