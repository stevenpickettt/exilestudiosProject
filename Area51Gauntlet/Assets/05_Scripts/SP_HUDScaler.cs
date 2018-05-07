using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class SP_HUDScaler : MonoBehaviour {

	private CanvasScaler mainMenucanvasScaler;
	private CanvasScaler gameCanvasScaler; 
	public Slider HUDScalerSlider; 
	private SP_HUD gameHUD; 

	void Start () {
		mainMenucanvasScaler = GetComponent<CanvasScaler> (); 
		HUDScalerSlider.value = mainMenucanvasScaler.matchWidthOrHeight;
	}

	void Update(){
		if (gameCanvasScaler == null) {
			if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Game")) {

				gameHUD = GameObject.FindObjectOfType<SP_HUD> ();
				gameCanvasScaler = gameHUD.GetComponent<CanvasScaler> (); 


			}
		}
		
		if (gameCanvasScaler != null) {
			gameCanvasScaler.matchWidthOrHeight = mainMenucanvasScaler.matchWidthOrHeight; 
		}

		mainMenucanvasScaler.matchWidthOrHeight = HUDScalerSlider.value;
	
	}

}
