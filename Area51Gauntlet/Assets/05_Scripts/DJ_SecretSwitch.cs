using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_SecretSwitch : MonoBehaviour {

	public enum SecretRoom { None = 0, Room01 = 1, Room02 = 2, Room03 = 3, Room04 = 4}
	public SecretRoom currentRoom = SecretRoom.None; 

    public bool isON = false;
    private Animator myAnimator;
    public Color brightGreen;
    public GameObject myLight; 

	private SP_HUD HUD; 

	private AudioController audioController; 



	// Use this for initialization
	void Start ()
    {
		HUD = FindObjectOfType<SP_HUD> (); 
        myAnimator = GetComponent<Animator>();
		audioController = FindObjectOfType<AudioController> (); 
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void SecretRoomHUDIcons(){
		if (isON = true) {
			
			if (currentRoom == SecretRoom.Room01) {
				HUD.secretRoom01.color = Color.green; 
			}
			if (currentRoom == SecretRoom.Room02) {
				HUD.secretRoom02.color = Color.green; 
			}
			if (currentRoom == SecretRoom.Room03) {
				HUD.secretRoom03.color = Color.green; 
			}
	
			if (currentRoom == SecretRoom.Room04) {
				HUD.secretRoom04.color = Color.green; 
			}
		}

	}

	public void TutorialScreenForFirstSwitch(){
		
		if (HUD.isFirstSwitch == true) {
			HUD.ShowSecretSwitchTutorial (); 
			HUD.isFirstSwitch = false; 
		}
	}
	void OnTriggerEnter(Collider other){

		if (other.tag == "Player") {

			TutorialScreenForFirstSwitch (); 
		}
	}
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
			if (HUD.secretRoomPanel.activeInHierarchy == false && HUD.tutorialScreen.activeInHierarchy == false) {
				HUD.secretRoomPanel.SetActive (true);
			}
			
            if (Input.GetButton("Interact"))
            {
				if (isON == false) {
					audioController.PlaySecretSwitchSFX (); 
				}
                myAnimator.SetBool("IsOn", true);
                isON = true;
                myLight.GetComponent<Light>().color = brightGreen;
				SecretRoomHUDIcons (); 


            }


        }
    }
	void OnTriggerExit(Collider other){

		if (other.tag == "Player") {
			HUD.secretRoomPanel.SetActive (false); 
		}
	}
}
