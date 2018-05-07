using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {



	[HideInInspector] public AudioSource audiosource;
	[Header ("Music tracks")]
	[Tooltip("Background music for the main menu")]
	public AudioClip MainMenuSFX;
	[Tooltip("Background music for in game")]
	public AudioClip InGameSFX;
	[Tooltip("Background Boss 01 music for in game")]
	public AudioClip Boss01Music;
	[Tooltip("Background TriGuards music for in game")]
	public AudioClip TriGuardsMusic;
	[Tooltip("Background Boss 02 music for in game")]
	public AudioClip Boss02Music;







	private ShowPanels showPanels; 




	// Use this for initialization
	void Start () 
	{
		
		showPanels = FindObjectOfType<ShowPanels> (); 

		//everytime audiosource is used, it gets checks and uses the AudioSource Componant
		audiosource = GetComponent<AudioSource>();

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        string sceneName = currentScene.name;

        if (sceneName == "00_MainMenu")
        {
            Misc_MainMenuSFX();
        }
		else if (sceneName == "Game")
        {
            Misc_InGameSFX();
        }
    }
	public void Misc_MainMenuSFX()
	{
		audiosource.PlayOneShot (MainMenuSFX, 1f);
	}	
	public void Misc_InGameSFX()
	{
		//audiosource.PlayOneShot (InGameSFX, 1f);
		audiosource.clip = InGameSFX; 
		audiosource.Play ();  

	}

	public void Misc_Boss01SFX()
	{
		//audiosource.PlayOneShot (InGameSFX, 1f);
		audiosource.clip = Boss01Music; 
		audiosource.Play (); 

	}
	public void Misc_TriGuardsSFX()
	{
		//audiosource.PlayOneShot (InGameSFX, 1f);
		audiosource.clip = TriGuardsMusic; 
		audiosource.Play (); 

	}
	public void Misc_Boss02SFX()
	{
		//audiosource.PlayOneShot (InGameSFX, 1f);
		audiosource.clip = Boss02Music; 
		audiosource.Play (); 

	}



	void Update(){

		CheckForPausePanel (); 

	}

	public void CheckForPausePanel(){
		if (showPanels != null) {
			if (showPanels.pausePanel.activeInHierarchy == true) {
				audiosource.Pause (); 
			} else {
				audiosource.UnPause ();
			}

			if (showPanels.gameOverPanel.activeInHierarchy == true || showPanels.winScreenPanel.activeInHierarchy == true) {
				audiosource.Pause (); 
			}
		}


	}




}
