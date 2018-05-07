using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour {

	//Main turret controller Component
	TurretController controller;
    GameObject PlayerPrefab;
	AudioController audioController;
    AIAgent aiAgent;
    BR_Friendly friendlyHealth;
    WaveSpawner spawner;

	//Random Variables
	bool reloading = false;
	float time = 0;

	[Tooltip("Target to be attacked by turret")]
	public Transform target; 

	[Header("Effects")]
	[Tooltip("Bullet Fire effect")]
	public ParticleSystem fireMuzzle;
	[Tooltip("Effect initialized at point where bullet hits")]
	public GameObject bulletHitEffect;

	[Header("Attack")]
	[Tooltip("Time after which next shot will be fired")]
	public float fireDelay = 0.1f;
	[Tooltip("Range of the Turret")]
	public float range = 20;


	[Header("Ammo")]
	[Tooltip("Current available ammo of the gun")]
	public int ammo;
	[Tooltip("Magzine size of the Gun")]
	public int magzineSize = 50;
	[Tooltip("Totale Ammo available for the GUN")]
	public int totalAmmo = 500;
	[Tooltip("Time taken to reload the Gun")]
	public float reloadTime = 2f;
	[Tooltip("Damage done by the bullet")]
	public float ammoDamage = 1;

    [SerializeField]
    private Transform muzzleLocation;

    [SerializeField]
    private KW_Enemy_Projectile projectile;
    [SerializeField]
    private KW_PlayerProjectile playerProjectile;

    //Get the component
    void Start(){
        aiAgent = this.GetComponent<AIAgent>();
        PlayerPrefab = GameObject.FindGameObjectWithTag("Player");
		controller = this.GetComponent<TurretController> ();
		audioController = FindObjectOfType<AudioController> ();
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.FRIENDLYTURRET)
            friendlyHealth = this.GetComponent<BR_Friendly>();
        spawner = FindObjectOfType<WaveSpawner>();
	}
		
	void Update(){
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGTURRET)
        {
            float dist = Vector3.Distance(PlayerPrefab.transform.position, transform.position);
            if (dist < range)
                target = PlayerPrefab.transform;
            else
                target = null;
            //check FireDelay after fire
           
        }
        else
        {
            if (friendlyHealth.isActive)
            {
                if (target == null)
                {
                    GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                    if (allEnemies.Length != 0)
                    {
                        int curSelection = 0;
                        for (int x = 0; x < allEnemies.Length; x++)
                        {
                            float curDist = Vector3.Distance(allEnemies[curSelection].transform.position, transform.position);
                            float dist = Vector3.Distance(allEnemies[x].transform.position, transform.position);
                            if (dist < range && dist <= curDist)
                                curSelection = x;
                        }
                        target = allEnemies[curSelection].transform;
                    }
                    else
                        target = null;
                }
            }
        }
        if (time <= fireDelay)
            time += Time.deltaTime;
    }

	public void Fire(){ //without Hit Effect

		if (ammo > 0 && time > fireDelay) {
		
			ammo--;

			fireMuzzle.Stop ();		
			fireMuzzle.Play ();
			time = 0;

		} 
	}

	public void Fire(Vector3 hitPoint, GameObject hitObject){ //with hit effect
        switch (aiAgent.EnemyType)
        {
            case AIAgent.ENEMYTYPE.REGTURRET:
                if (reloading || hitObject.tag != "Player")
                    return;
                else
                    break;

            case AIAgent.ENEMYTYPE.FRIENDLYTURRET:
                if (reloading || hitObject.tag == "Player" || hitObject.tag == "Environment" || spawner.state == WaveSpawner.SpawnState.WAITING)
                    return;
                else
                    break;
        }
		if (ammo > 0) {

			if (time > fireDelay) {

				ammo--;

				fireMuzzle.Stop ();		
				fireMuzzle.Play ();

				//hitObject.SendMessage ("ApplyDamage", ammoDamage, SendMessageOptions.DontRequireReceiver);
                if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGTURRET)
				    Instantiate (projectile, muzzleLocation.position, muzzleLocation.rotation);
                else
                    Instantiate(playerProjectile, muzzleLocation.position, muzzleLocation.rotation);
                audioController.ShootingTurret_ShootingTurretActiveSFX (); 
				time = 0;

			}

		} else {
			

			Reload ();
		}
	}

	//Reload Turret 
	public void Reload(){

		reloading = true;

		StartCoroutine (ReloadAfterDelay ());

	}

	//Reload Turret after the delay
	IEnumerator ReloadAfterDelay (){

		yield return new WaitForSeconds (reloadTime);

		if (totalAmmo - magzineSize > 0) {
		
			totalAmmo -= magzineSize;
			ammo = magzineSize;

		} else {		
			ammo = totalAmmo;
			totalAmmo = 0;
		}

		reloading = false;
	}

}
