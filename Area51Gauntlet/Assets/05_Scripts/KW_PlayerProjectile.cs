using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_PlayerProjectile : MonoBehaviour
{

    [Header("Changeable values:")]
    public float destroyTime;
    public float destroyTimer;
    public float MoveSpeed;
    public BR_EnemyHealth.DMGTYPE bulletType;

    private BR_GameController gameController;
    private PlayerController playerController;

    [SerializeField]
    float sloMoX;

    [Header("Damage of bullet:")]
    [Tooltip("How much damage the enemy will take.")]
    public int damageDealt;

    [Header("ParticleEffects")]
    public GameObject EnemyHitPS;
    public GameObject ShieldHitPS;
    public GameObject CPHitPS;
    public GameObject BulletTrail;

    [Space]
    [Header("For Difficulty")]
    [SerializeField]
    [Tooltip("adds damage to weapon")]
    public int easyDamageModifier = 2;
    [SerializeField]
    [Tooltip("subrtacts damage from weapon/ insert postive #")]
    public int hardDamageModifier = 1;

    [Space]
    [Header("For Grenade Launcher")]
    public bool isStuck;
    [SerializeField]
    GameObject StickyLocation;
    GameObject collisionLoc;
    [SerializeField]
    GameObject GrenadeExplosion;
    public float curGrenadeTimer;
    public float maxGrenadeTimer = 1f;


    // Use this for initialization
    void Start ()
    {
        gameController = FindObjectOfType<BR_GameController>();

        playerController = FindObjectOfType<PlayerController>();

        switch (gameController.spawner.waveDifficulty)
        {
            
            case BR_GameController.GameDifficulty.EASY:
                {
                    damageDealt = damageDealt + easyDamageModifier; 
                }
                break;

            case BR_GameController.GameDifficulty.NORMAL:
                {
                    damageDealt = damageDealt; 
                }
                break;

            case BR_GameController.GameDifficulty.HARD:
                {
                    damageDealt = damageDealt - hardDamageModifier;
                }
                break;
        }


    }
	
	// Update is called once per frame
	void Update ()
    {
        SloMoMultipler(); 
     
            destroyTime += Time.deltaTime;

        if (!isStuck)
            transform.position += transform.forward * Time.deltaTime * MoveSpeed * sloMoX;

        else if(collisionLoc != null)
            gameObject.transform.position = collisionLoc.transform.position;
        

        if (destroyTime >= destroyTimer)
        {
            if (bulletType != BR_EnemyHealth.DMGTYPE.ROCKET)
                Destroy(gameObject);
            else
                isStuck = true;
        }
            
  

            
        if(curGrenadeTimer > -1 && bulletType == BR_EnemyHealth.DMGTYPE.ROCKET)
        {
            curGrenadeTimer -= Time.deltaTime;
            if(curGrenadeTimer <= 0)
            {
                Instantiate(GrenadeExplosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (bulletType != BR_EnemyHealth.DMGTYPE.ROCKET)
        {
            if (other.tag == "Enemy")
            {
                SpawnEnemyHitPS();
                StartDestroy(1f);
            }

            if (other.tag == "Environment")
            {
                Destroy(gameObject);
            }

            if (other.tag == "Shield")
            {
                SpawnShieldHitPS();
                StartDestroy(1f);
            }

            if (other.tag == "CriticalPoint")
            {
                SpawnCPHitPS();
                StartDestroy(1f);
            }

            if (other.tag == "ThrusterA")
            {

                Destroy(gameObject);
            }

            if (other.tag == "TrilogyGuard")
            {
                SpawnEnemyHitPS();
                StartDestroy(1f);
            }
            if (other.tag == "Boss1")
            {
                SpawnEnemyHitPS();
                StartDestroy(1f);
            }
            if (other.tag == "Boss2")
            {
                SpawnEnemyHitPS();
                StartDestroy(1f);
            }

            if (other.tag == "PowerPanel")
            {
                SpawnCPHitPS();
                StartDestroy(1f);

            }
        }
        else if (other.tag != "PlayerProjectile" && other.tag != "EnemyProjectile" && other.tag != "PickUp" && other.tag != "Untagged")
        {
            collisionLoc = Instantiate(StickyLocation, gameObject.transform.position, gameObject.transform.rotation);
            //gameObject.transform.parent = other.transform;
            //gameObject.transform.position = collisionLoc.transform.position;
            BulletTrail.SetActive(false);
            isStuck = true;
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

     void StartDestroy(float timeDelay)
    {
        //turn off drawing and colliding 
        if(bulletType != BR_EnemyHealth.DMGTYPE.GRENADEXPLOSION)
        {
            Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<ParticleSystem>().enableEmission = false;
            MoveSpeed = 0f;
     
        }
       
       // SpawnEnemyHitPS();
      //  GetComponent<AudioSource>().PlayOneShot(shotSfxClip);
        Destroy(gameObject, timeDelay);
    }


    void SpawnEnemyHitPS()
    {
        Instantiate(EnemyHitPS, transform.position, transform.rotation);
    }


    void SpawnShieldHitPS()
    {
        Instantiate(ShieldHitPS, transform.position, transform.rotation);
    }

    void SpawnCPHitPS()
    {
        Instantiate(CPHitPS, transform.position, transform.rotation);
    }

    public void StartStickyExplosion()
    {
           curGrenadeTimer = maxGrenadeTimer; 
    }

}
