using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_Portals : MonoBehaviour
{

    public GameObject player;
    public Transform playerTransform;
    public GameObject secretRoomSpawnPoint;
    public GameObject repairRoomSpawnPoint;

    [Header("ParticleEffects")]
    public GameObject SecretRoomDust;
    public GameObject RepairPortalPS;
    public GameObject SecretPortalPS;



    public bool isSecretPortal;
    public bool arePortalsActive;

	private bool isPlayerTeleported; 
	private float playerTimer; 

	private SP_HUD HUD; 

    DJ_SecretSwitch[] SS;

	private AudioController audioController; 

    // Use this for initialization
    void Start()
    {
		HUD = FindObjectOfType<SP_HUD> (); 

        SS = FindObjectsOfType<DJ_SecretSwitch>();

		audioController = FindObjectOfType<AudioController> (); 

        //turns off collider 
        gameObject.GetComponent<BoxCollider>().enabled = false;

	
		player = GameObject.FindGameObjectWithTag("Player");
		

    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            playerTransform = player.GetComponent<Transform>();
        }

        

        int count = 0;
        for (int x = 0; x < SS.Length; x++)
        {
            if (SS[x].isON)
            {
                count++;
          
            }
        }

        if (count == 4)
        {
            arePortalsActive = true;
            //turns on the collider so they can teleport
            gameObject.GetComponent<BoxCollider>().enabled = true;
            RepairPortalPS.SetActive(true);
        }

        if (isPlayerTeleported == true) {
			playerTimer += Time.deltaTime; 
		} else {
			playerTimer = 0f; 
		}

		if (playerTimer >= 1.5f) {
			isPlayerTeleported = false; 
			playerTimer = 0; 
		}
    }


	void OnTriggerEnter(Collider other){

		if (other.tag == "Player") {
			HUD.secretRoomPanel.SetActive (true); 
		}
	}

    void OnTriggerStay(Collider other)
    {
		
		if(other.tag == "Player" && !isSecretPortal)
        {
			if (playerTimer == 0)
            {
					audioController.PlayPortalSFX (); 
					isPlayerTeleported = true; 
					SecretRoomDust.SetActive (true);
                    SecretPortalPS.SetActive(true);
					playerTransform.transform.position = secretRoomSpawnPoint.transform.position;
			}
        }

        if(other.tag == "Player" && isSecretPortal)
        {
			if (playerTimer == 0)
            {
					audioController.PlayPortalSFX (); 
					isPlayerTeleported = true; 
					SecretRoomDust.SetActive (false);
                    SecretPortalPS.SetActive(false);
					playerTransform.transform.position = repairRoomSpawnPoint.transform.position;
			}
        }


    }
	void OnTriggerExit(Collider other){

		if (other.tag == "Player") {
			HUD.secretRoomPanel.SetActive (false); 

		}
	}
}
