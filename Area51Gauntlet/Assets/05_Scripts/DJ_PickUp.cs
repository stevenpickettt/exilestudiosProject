using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_PickUp : MonoBehaviour {
    public enum SUBTYPE { NONE, Health, Shield, Auto, Scatter, Blaster, Key, RegenVol, Thruster, Engine, BodyofShip, AIController, EmpBlast, ChronoWatch, hackKey, GrenadeLauncher};

    public SUBTYPE subTypeName = SUBTYPE.NONE;
    private Light light;

    public int amount;
    [SerializeField]
    private int destroyTime = 180;
    BR_GameController gameController;
    //    public GameObject HitParticles;

    //  [SerializeField]
    //AudioClip shotSfxClip; 

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<BR_GameController>();
        light = gameObject.GetComponent<Light>();
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z);
        GetComponent<Collider>().name = subTypeName.ToString();
        switch (subTypeName)
        {
            case SUBTYPE.AIController:
                light.enabled = true;
                break;

            case SUBTYPE.Auto:
                if (!gameController.isAutoUnlocked)
                    light.enabled = true;
                else
                    RegPickupSetup();
                break;

            case SUBTYPE.Blaster:
                light.enabled = true;
                break;

            case SUBTYPE.ChronoWatch:
                light.enabled = true;
                break;

            case SUBTYPE.EmpBlast:
                light.enabled = true;
                break;

            case SUBTYPE.Engine:
                light.enabled = true;
                break;

            case SUBTYPE.hackKey:
                light.enabled = true;
                break;

            case SUBTYPE.Key:
                light.enabled = true;
                break;

            case SUBTYPE.Scatter:
                if (!gameController.isScatterUnlocked)
                    light.enabled = true;
                else
                    RegPickupSetup();
                break;

            case SUBTYPE.Shield:
                if (!gameController.isShieldUnlocked)
                    light.enabled = true;
                else
                    RegPickupSetup();
                break;

            case SUBTYPE.Thruster:
                light.enabled = true;
                break;

			case SUBTYPE.GrenadeLauncher:
			light.enabled = true;
			break; 

            default:
                RegPickupSetup();
                break;
        }
    }
    void RegPickupSetup()
    {
        light.enabled = false;
        Destroy(gameObject, destroyTime);
    }
    
    // Update is called once per frame
    void Update ()
    {
		
	}

    public void hitPlayer()
    {
        if (subTypeName != SUBTYPE.RegenVol && subTypeName != SUBTYPE.Engine && subTypeName != SUBTYPE.Thruster && subTypeName != SUBTYPE.AIController)
            Destroy(gameObject);
        else
            light.enabled = false;
    } 

  /*  void OnTiggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(gameObject);

            //SpawnHitParticles();
           // GetComponent<AudioSource>().PlayOneShot(CoinSFX);
            //StartDestroy(CoinSFX.length);
        }
    }

    //Spawns the particle system.
   /* void SpawnHitParticles()
    {
        Instantiate(HitParticles, transform.position, transform.rotation);
    }

    void StartDestroy(float timeDelay)
    {
        GetComponent<AudioSource>().PlayOneShot(shotSfxClip);

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, timeDelay);
    }*/
}
