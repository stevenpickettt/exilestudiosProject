using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoAnimation : MonoBehaviour {


    public GameObject container;
    public Animator exoGuard;

    private bool startDestroy = false;
	private bool spawnGrenadeOnce = false; 
    public float destroyTimer;

    public GameObject grenadeLaucherSpawnPoint;
    public GameObject grenadeLaucherPickup;

    public float maxDestroyTimer = 10f;

    private MusicController myMusic;
    

	// Use this for initialization
	void Start ()
    {
        destroyTimer = maxDestroyTimer;
        myMusic = FindObjectOfType<MusicController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (startDestroy)
        {
            destroyTimer -= Time.deltaTime;
			if (spawnGrenadeOnce == true && destroyTimer < 2f) {
				spawnGrenadeOnce = false; 
				Instantiate (grenadeLaucherPickup, grenadeLaucherSpawnPoint.transform.position, grenadeLaucherSpawnPoint.transform.rotation);

			}
            myMusic.audiosource.volume = 0.1f;
			if (destroyTimer <= 0f) {
				startDestroy = false; 
				myMusic.audiosource.volume = 0.4f;
			}
        }



	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (container.activeInHierarchy)
            {
                exoGuard.SetBool("isPlayerHere", true);
                myMusic.audiosource.volume = 0.1f;
                startDestroy = true;
				spawnGrenadeOnce = true; 
				Invoke ("TurnOffContainer", maxDestroyTimer); 
            }
        }
    }

	void TurnOffContainer(){
		container.SetActive(false);
	}


    public void ResetFriendlyExoGuard()
    {
        container.SetActive(true);
        exoGuard.SetBool("isPlayerHere", false);
        startDestroy = false;
		spawnGrenadeOnce = false; 
        destroyTimer = maxDestroyTimer;

        
    }
}
