using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; 
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class ShowPanels : MonoBehaviour {
	public BR_GameController.GameDifficulty difficultySettings;
	public BR_GameController.DebugRoom DebugRoomSet;
	private SP_HUDScaler HUDScaler;
	private SetAudioLevels audioLevels;




    [Header("Main Menu Panels")]
	public GameObject optionsPanel;   						//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject mainMenuButtons; 						//Store a reference to the Game Object MainMenu Buttons
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
	public GameObject pausePanelButtons; 					//Store a reference to the Game Object Pause Panel Buttons
	public GameObject creditPanel;							//Store a reference to the Game Object creditPanel
	public GameObject controlsPanel;						//Store a reference to the Game Object controlsPanel

	public GameObject debugStartPanel;
	public GameObject debugStartButtons; 
	public GameObject difficultyPanel; 

	public GameObject controlsKeyboard; 
	public GameObject controlsController; 

	public GameObject gameOverPanel; 
	public GameObject winScreenPanel; 
	public GameObject gameOverButtons;
	public GameObject winScreenButtons;
	public GameObject mainMenuQuitPanel; 
	public GameObject quitPanel;
	public GameObject tutorialPanel; 
	public GameObject tutorialToggleGameObject; 
	public Toggle showTutorialsToggle; 

	[Space]
	[Header("Buttons")]

	public GameObject startingButton; 
	public GameObject startingPauseButton;

	public GameObject startingDebugStartingButton; 
	public GameObject startingDifficultyButton;

	public GameObject pausePanelTint; 
	public GameObject optionsResumeBar; 


	public GameObject controlsPauseBar; 
	public GameObject controlResumeBackBar; 
	public GameObject creditsResumeBar; 
 
	public GameObject checkpointButton; 
	public GameObject startAgainButton; 
	public GameObject startingMainMenuQuitButton;
	public GameObject startingQuitButton; 

	public GameObject startingTutorialButton; 

	private SP_HUDContainer HUDContainer; 
	private MusicController musicController;
	private AudioController audioController; 
	private SP_HUD HUD; 
	private BR_GameController gameController; 

	[Space]
	[Header("Game Over Buttons")]
	public Button gameOverCheckpoint; 
	public Button gameOverMainMenu;
	public Button gameOverQuit;
 
	[Space]
	[Header("Option's Sliders")]
	public Slider sfxSlider; 
	public Slider mainMusicSlider; 


	void Start(){
		difficultySettings = BR_GameController.GameDifficulty.NORMAL;
	
		DebugRoomSet = BR_GameController.DebugRoom.NONE;

		tutorialToggleGameObject.SetActive (false); // Turning off Tutorial Button in the main menu. 

		gameOverCheckpoint = gameOverCheckpoint.GetComponent<Button> (); 
		gameOverMainMenu = gameOverMainMenu.GetComponent<Button> (); 
		gameOverQuit = gameOverQuit.GetComponent<Button> (); 
		GameOverButtonsFalse (); 
		HUDScaler = GetComponent<SP_HUDScaler> (); 

		
	}

	void Update() {
		 
		if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Game")) {


			
			HUDContainer = GameObject.FindObjectOfType<SP_HUDContainer> ();
			HUD = GameObject.FindObjectOfType<SP_HUD> ();
			musicController = GameObject.FindObjectOfType<MusicController> ();
			gameController = GameObject.FindObjectOfType<BR_GameController> (); 
			audioController = GameObject.FindObjectOfType<AudioController> (); 

		

		}



	}

	public void SetDifficultyToEASY(){
		difficultySettings = BR_GameController.GameDifficulty.EASY;
		if (debugStartPanel.activeInHierarchy == true) {
			debugStartPanel.SetActive (false);
		}
	}
	public void SetDifficultyToNORMAL(){
		difficultySettings = BR_GameController.GameDifficulty.NORMAL;
		if (debugStartPanel.activeInHierarchy == true) {
			debugStartPanel.SetActive (false);
		}
	}
	public void SetDifficultyToHARD(){
		difficultySettings = BR_GameController.GameDifficulty.HARD;
		if (debugStartPanel.activeInHierarchy == true) {
			debugStartPanel.SetActive (false);
		}
	}


	//Call this function to activate and display the Options panel during the main menu
	public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(optionsResumeBar);

	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);

		optionsTint.SetActive(false);

		GameObject myEventSystem = GameObject.Find("EventSystem");
		if (menuPanel.activeInHierarchy) {
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingButton);
		} else {
			ShowPausePanel (); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingPauseButton);
		}
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(startingTutorialButton);
	}

	//Call this function to activate and hide the main menu panel during the main menu
	public void ShowTutorialPanel(){
		Time.timeScale = 0; 
		tutorialPanel.SetActive (true); 
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(startingTutorialButton);
		gameController.doDash = false; 
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideTutorialPanel(){
		
		tutorialPanel.SetActive (false); 
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(startingPauseButton);
		Time.timeScale = 1;

		gameController.canDashSwitch = true;  
	}
	public void Toggle_Changed(bool newValue){
		gameController.showTutorial = newValue;

	}

	//Call this function to show tutorials during game
	public void TutorialYES(){
		Toggle_Changed (true);  
		showTutorialsToggle.isOn = true; 
	}

	//Call this function to NOT show tutorials during game
	public void TutorialNO(){
		Toggle_Changed (false); 
		showTutorialsToggle.isOn = false; 
	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
		if (pausePanelButtons.activeInHierarchy == false) {
			pausePanelButtons.SetActive (true); 
		}
		HUDContainer.isPausePanel = true;
		pausePanelTint.SetActive (true); 
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
		gameController.doDash = false; 
		 
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		
		HUDContainer.isPausePanel = false;
		pausePanelTint.SetActive (false);
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);

		if (controlsPanel.activeInHierarchy == true) {
			controlsPanel.SetActive (false); 
		}
		if (optionsPanel.activeInHierarchy == true) {
			optionsPanel.SetActive (false); 
		}
		if (mainMenuQuitPanel.activeInHierarchy == true) {
			mainMenuQuitPanel.SetActive (false); 
		}
		if (quitPanel.activeInHierarchy == true) {
			quitPanel.SetActive (false); 
		}

		gameController.canDashSwitch = true; 

	}

	//Call this function to activate and display the Options panel during the main menu
	public void ShowCreditPanel()
	{
		creditPanel.SetActive(true);
		optionsTint.SetActive(true);
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(creditsResumeBar);
	}

	public void HideCreditPanel()
	{
		creditPanel.SetActive(false);
		optionsTint.SetActive(false);
		GameObject myEventSystem = GameObject.Find("EventSystem");
		myEventSystem .GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(startingButton);

	}

	//Call this function to activate and display the Options panel during the main menu
	public void ShowControlsPanel()
	{
		controlsPanel.SetActive(true);
		optionsTint.SetActive(true);
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (controlResumeBackBar);
	}

	public void HideControlsPanel()
	{
		controlsPanel.SetActive(false);
		optionsTint.SetActive(false);
		if (menuPanel.activeInHierarchy == true) {
			GameObject myEventSystem = GameObject.Find ("EventSystem");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingButton);
		} 
		if (menuPanel.activeInHierarchy == false) {
			ShowPausePanel (); 
			GameObject myEventSystem = GameObject.Find ("EventSystem");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingPauseButton);
		}




	}

	//Call this function to activate and display the Pause panel during game play
	public void ShowGameOverPanel()
	{
		HUDContainer.isPausePanel = true;
		gameOverPanel.SetActive (true);
		Invoke("GameOverButtonsTrue", 2); 
	}
	public void ShowGameOverPanelDELAY(){
		gameOverPanel.SetActive (true);
		Invoke("GameOverButtonsTrue", 2); 
	}

	public void GameOverButtonsTrue(){
		
		if (gameOverQuit.interactable == false) {
			gameOverQuit.interactable = true; 
		}

		if (gameOverMainMenu.interactable == false) { 
			gameOverMainMenu.interactable = true; 
		}

		if (gameOverCheckpoint.interactable == false) {
			gameOverCheckpoint.interactable = true; 
		}
	}
	public void GameOverButtonsFalse(){
		if (gameOverQuit.interactable == true) {
			gameOverQuit.interactable = false; 
		}

		if (gameOverMainMenu.interactable == true) { 
			gameOverMainMenu.interactable = false; 
		}

		if (gameOverCheckpoint.interactable == true) {
			gameOverCheckpoint.interactable = false; 
		}
	}


	//Call this function to deactivate and hide the Pause panel during game play
	public void HideGameOverPanel()
	{
		HUDContainer.isPausePanel = false;
		GameOverButtonsFalse(); 
		gameOverPanel.SetActive (false);
		//optionsTint.SetActive(false);


	}

	public void CheckPointGameOver(){
		BR_GameController gamecontroller = FindObjectOfType<BR_GameController> (); 

		 
		gamecontroller.RespawnPlayer (); 
	


		if (gameOverPanel.activeInHierarchy) {
			GameOverButtonsFalse(); 
			gameOverPanel.SetActive (false);
		}
		if (winScreenPanel.activeInHierarchy) {
			audioController.PauseAudioController (); 
			winScreenPanel.SetActive (false);

		}
		
	}

	public void ShowQuitPanel(){
		quitPanel.SetActive (true); 

		if (pausePanel.activeInHierarchy == true) {
			pausePanelButtons.SetActive (false); 
		}

		if (mainMenuButtons.activeInHierarchy == true) {
			mainMenuButtons.SetActive (false); 
		}

		if (gameOverPanel.activeInHierarchy == true) {
			GameOverButtonsFalse(); 
			gameOverButtons.SetActive (false); 
		}

		if (winScreenPanel.activeInHierarchy == true) {
			winScreenButtons.SetActive (false); 
		}
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingQuitButton);



	}

	public void HideQuitPanel(){
		

		GameObject myEventSystem = GameObject.Find("EventSystem");
		if (menuPanel.activeInHierarchy) {
			mainMenuButtons.SetActive (true); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingButton);
		} 
		if (pausePanel.activeInHierarchy) {
			ShowPausePanel (); 
			pausePanelButtons.SetActive (true); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingPauseButton);
		}
		if (gameOverPanel.activeInHierarchy) {
			gameOverButtons.SetActive (true); 
			GameOverButtonsTrue (); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (checkpointButton);
		}
		if (winScreenPanel.activeInHierarchy) {
			winScreenButtons.SetActive (true); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startAgainButton);
		}

		quitPanel.SetActive (false);
	}

	public void ShowKeyboardText(){
		
			controlsKeyboard.SetActive (true); 
			controlsController.SetActive (false); 
			
		
	}
	public void ShowControllerText(){
		
			controlsController.SetActive (true); 
			controlsKeyboard.SetActive (false); 
			
		
	}

	public void ShowMainMenuQuitPanel(){
		mainMenuQuitPanel.SetActive (true);

		if (pausePanel.activeInHierarchy == true) {
			pausePanelButtons.SetActive (false); 
		}
		if (gameOverPanel.activeInHierarchy == true) {
			GameOverButtonsFalse(); 
			gameOverButtons.SetActive (false); 
		}

		if (winScreenPanel.activeInHierarchy == true) {
			winScreenButtons.SetActive (false); 
		}
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingMainMenuQuitButton);

	}

	public void HideMainMenuQuitPanel(){
		
		mainMenuQuitPanel.SetActive (false);
		GameObject myEventSystem = GameObject.Find ("EventSystem");

		if (pausePanel.activeInHierarchy == true) {
			pausePanelButtons.SetActive (true); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingPauseButton);
		}

		if (winScreenPanel.activeInHierarchy) {
			winScreenButtons.SetActive (true); 
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startAgainButton);
		}

		if (gameOverPanel.activeInHierarchy) { 
			gameOverButtons.SetActive (true); 
			GameOverButtonsTrue(); 

			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (checkpointButton);
		}
	}



	public void LoadMainMenu(){
		SceneManager.LoadScene ("00_MainMenu");
		Time.timeScale = 1; // Resetting Time Scale
		Destroy (gameObject);  
	}

	public void ShowDebugStartPanel(){
		debugStartPanel.SetActive (true); 
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingDebugStartingButton);
	}

	public void HideDebugStartPanel(){
		debugStartPanel.SetActive (false); 
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingButton);
	}

	public void ShowDifficultyPanel(){
		difficultyPanel.SetActive (true); 
		if (debugStartPanel.activeInHierarchy == true) {
			debugStartButtons.SetActive (false); 
		}
		GameObject myEventSystem = GameObject.Find ("EventSystem");
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingDifficultyButton);
	}

	public void HideDifficultyPanel(){
		difficultyPanel.SetActive (false); 
		if (debugStartPanel.activeInHierarchy == true) {
			debugStartButtons.SetActive (true);
			GameObject myEventSystem = GameObject.Find ("EventSystem");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingDebugStartingButton);
		} else {
			GameObject myEventSystem = GameObject.Find ("EventSystem");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (startingButton);
		}
	}



	public void GoToBoss01Room(){
		DebugRoomSet = BR_GameController.DebugRoom.BOSS1;
	

	}
	public void GoToThruster01Room(){
		DebugRoomSet = BR_GameController.DebugRoom.THRUSTER1;



	}
	public void GoToThruster02Room(){
		DebugRoomSet = BR_GameController.DebugRoom.THRUSTER2;

	
	}
	public void GoToTriGuardsRoom(){
		DebugRoomSet = BR_GameController.DebugRoom.TRIGUARDS;
	
	
	}
	public void GoToBoss02Room(){
		DebugRoomSet = BR_GameController.DebugRoom.BOSS2;
	
	}


	public void PlayAudioGameOverScreen(){
		musicController.Misc_InGameSFX (); 
	}

	public void ResetOptionsSettings(){
		HUDScaler.HUDScalerSlider.value = 0.5f; 
		sfxSlider.value = 0f;
		mainMusicSlider.value = 0f; 
	}
}
