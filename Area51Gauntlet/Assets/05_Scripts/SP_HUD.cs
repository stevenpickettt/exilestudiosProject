using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public class SP_HUD : MonoBehaviour {

	private PlayerController playerController;
	private BR_GameController gameController;
	private ShowPanels mainMenuShowPanels;
	private SP_HUDContainer HUDContainer;
	private AudioController audioController; 
	private MusicController musicController; 

	private AIAgent aiAgent;  

    [SerializeField]
	public SP_Thrusters thrustersScriptA; 
    [SerializeField]
	public SP_Thrusters thrustersScriptB;
    [SerializeField]
    private Image thrusterIconA;
    [SerializeField]
    private Image thrusterIconB;
    [SerializeField]
    private Text thrusterDamageTextA;
    [SerializeField]
    private Text thrusterDamageTextB;
    private float thrusterTimerA;
    private float thrusterTimerB;

    public BR_Friendly turretScript;
    [SerializeField]
    private Image turretIcon;
    [SerializeField]
    private Text turretDamageText;
    private float turretTimer;

    public BR_Hacking curHackingVol;

    public GameObject[] BossSlot; 

    bool DpadPressed;

    bool waitingForInput;
    string inputType;


	[Space]
	[Header("High Score")]
	public Text highScoreText; 
	public int comboScore = 1; 
	public float comboMeterTimerMax = 15f;
	private float curComboMeterTimer = 15f;
	private bool comboMeterBool = false;
    public Text challengeScore;
    private float challengeTimer;

	public Text comboText; 
	public Image comboFillBar; 

	[Space]
	[Header("WaveSystem")]
	public Text waveUI;
    public Text RoomNameText;
    public Text RoomNameHeaderText;
	public Text waveSystem; 

    [Space]
    [Header("HUD Toggles")]
	private GameObject uiShield; // Shield UI Icon
	private GameObject uiSingleFire; // Single Fire Mode UI Icon
	private GameObject uiAutoFire; // Automatic Fire Mode UI Icon
	private GameObject uiScatterFire; // Scatter Fire Mode UI Icon


    [SerializeField]
	public BR_ObjectivePanel objectivePanel; // Object Panel 
	private float objectivePanelTimer; // Timer 

	[Space]
	[Header("Fill Bars")]
	public Image healthFillBar; // Health Bar RED
	public Image healthFillBarOrange; // Lerping Health Bar 
	public Image shieldFillBar; // Shield Bar BLUE
	public Image shieldFillBarOrange; // Lerping Health Bar 
	public Image chronoWatchFillBar; // Fill bar for Chrono Watch ability
	public Image empDashFillBar; // Fill bar for emp dash ability. 
	public Image empExplosionFillBar; // Fill bar for emp explosion ability.
	public Image hackingFillBar; // The actual fill bar for how much time remaining to hack.
	public Image grenadeLauncherFillBar; 
	public GameObject hackingPanel; // Turn on/off with this gameobject container. 

	// Health Lerp Effect

	[Space]
	private float OrangeSpeed = 0.01f; 
	private bool healthBarOrangeLerp = false; 
	[HideInInspector]private float healthBarFromSize; 
	[HideInInspector]private float healthBarNewSize; 

	// Shield Lerp Effect

	[Space]
	private float shieldBarOrangeSpeed = 0.01f; 
	private bool shieldBarOrangeLerp = false; 
	[HideInInspector]private float shieldBarFromSize; 
	[HideInInspector]private float shieldBarNewSize; 

	[Space]
	[Header("Ammo Amount")]
	public Text autoAmmoText; 
	public Text scatterAmmoText; 


	[Space]
	[Header("Firing Mode Highlight Colors")]
	public Color disableColor; 
	public Color activeColor; 

	[Space]
	[Header("Firing Mode Icons BackGrounds")]
	public Image SingleFireIconBG; 
	public Image AutoFireIconBG;
	public Image ScatterFireIconBG;


	[Space]
	[Header("Firing Modes Icons")]
	public GameObject differentFireModeBG; 
	public GameObject ammoBG; 
	public GameObject ammoText; 
	public GameObject singleFireIcon; 
	public GameObject autoFireIcon; 
	public GameObject scatterFireIcon; 


	[Space]
	[Header("Player's Damage Image")]
	public float flashSpeed = 0.2f; 
	public Color healthflashColor = new Color (1f, 0f, 0f, 0.1f); 
	public Color shieldflashColor = new Color (0f, 0f, 1, 0.1f); 
	public Color lowHealthColor = new Color (1f, 1f, 1f, 1f); 
	public Color lowHealthColorScreenOverlay = new Color (1f, 1f, 1f, 1f); 
	public Color healthflashColorScreenOverlay = new Color (1f, 0f, 0f, 0.1f);
	public Color shieldflashColorScreenOverlay = new Color (0f, 0f, 1, 0.1f); 
	public Image damageImage;
	public Image damageImageScreenOverlay; 

	public bool isDamaged = false;

    [Space]
    [Header("Tutorial")]
    public GameObject tutorialScreen;
    public Text tutorialText;
   
	public bool canShowTutorialPanel = true; 

	[Space]
	[Header("ChronoWatch - ScreenOverlay")]
	public GameObject chronoWatchScreen; 

	[Space]
	[Header("Ability's Icon")]
	public GameObject empDashIcon; 
	public GameObject empExplosionIcon;
	public GameObject chronoWatchIcon; 
	public GameObject grenadeLauncherIcon; 

	[Space]
	[Header("Ability's Colors")]
	public Color empDashColor = new Color(0f, 1f, 0f, 1f); // Green Color
	public Color empExplosionColor = new Color(0f, 1f, 1f, 1f); // Teal Color
	public Color chronoWatchColor = new Color(1f, 0f, 1f, 1f); // Purple Color
	public Color grenadeLauncherColor = new Color (0f,0f,0f,0f); 
	public Color grenadeLauncherColorLerp; 
	public Color regenColor = new Color(0f,0f,0f,1f); 

	[Space]
	[Header("Combo's Color")]
	public Color comboZeroColor = new Color (0f, 0f, 0f, 0f); 

	[Space]
	[Header("Tutorial Icons")]
	public GameObject meleeMode; 
	public GameObject singleFireMode; 
	public GameObject autoFireMode; 
	public GameObject scatterFireMode; 
	public GameObject chronoWatchMode; 
	public GameObject empDashMode;
	public GameObject empExplosionMode; 
	public GameObject comboMode; 
	public GameObject hackingMode;
	public GameObject turretMode; 
	public GameObject powerPanelMode;
	public GameObject grenadeLauncherMode; 
	public GameObject secretRoomMode; 
	public GameObject thrusterRoomMode;

    public GameObject[] trackingSlot;

	[Space]
	[Header("Engine's Thrusters")]

	// Both Thrusters
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f); 
	private float orangeSpeed = 0.01f;
	[Space]
	// Thruster A
	public GameObject HealthBarThrusterA;
	public RectTransform HealthBarREDThrusterA; 
	public RectTransform HealthBarORANGEThrusterA;
	public Image HealthBarThrusterA_BG;
	public bool orangeLerpThrusterA = false; 
	[HideInInspector]public float fromSizeThrusterA; 
	[HideInInspector]public float newSizeThrusterA;

	//------------------------------------------------------------------------------------------------------------------
	[Space]
	[Space]
	// Thruster B
	public GameObject HealthBarThrusterB;
	public RectTransform HealthBarREDThrusterB; 
	public RectTransform HealthBarORANGEThrusterB; 
	public Image HealthBarThrusterB_BG; 
	public bool orangeLerpThrusterB = false; 
	[HideInInspector]public float fromSizeThrusterB; 
	[HideInInspector]public float newSizeThrusterB;

	//------------------------------------------------------------------------------------------------------------------
	[Space]
	[Header("Trilogy Healthbars")]
	// Exo-Guard 01
	public GameObject HealthBarBoss01;
	public RectTransform HealthBarREDBoss01; 
	public RectTransform HealthBarBlueBoss01; 
	public Image HealthBarBoss01Shield; 
	public RectTransform HealthBarORANGEBoss01;
    public GameObject triExoIcon;
    public GameObject boss1Icon;
    public GameObject boss2Icon;
	public bool orangeLerpBoss01 = false; 
	[HideInInspector]public float fromSizeBoss01; 
	[HideInInspector]public float newSizeBoss01;
    public GameObject Slot1Marker;
    public GameObject Boss1TrackerIcon;
    public GameObject Boss2TrackerIcon;
    public GameObject TriGuard1TrackerIcon;
    public GameObject default1Icon;

	//------------------------------------------------------------------------------------------------------------------
	[Space]
	[Space]
	// Exo-Guard 02
	public GameObject HealthBarBoss02;
	public RectTransform HealthBarREDBoss02; 
	public RectTransform HealthBarBlueBoss02; 
	public RectTransform HealthBarORANGEBoss02;
	public bool orangeLerpBoss02 = false; 
	[HideInInspector]public float fromSizeBoss02; 
	[HideInInspector]public float newSizeBoss02;
    public GameObject Slot2Marker;
    public GameObject TriGuard2TrackerIcon;
    public GameObject default2Icon;

	//------------------------------------------------------------------------------------------------------------------
	[Space]
	[Space]
	// Exo-Guard 03
	public GameObject HealthBarBoss03;
	public RectTransform HealthBarREDBoss03; 
	public RectTransform HealthBarBlueBoss03; 
	public RectTransform HealthBarORANGEBoss03;
	public bool orangeLerpBoss03 = false; 
	[HideInInspector]public float fromSizeBoss03; 
	[HideInInspector]public float newSizeBoss03;
    public GameObject Slot3Marker;
    public GameObject TriGuard3TrackerIcon;
    public GameObject default3Icon;

    public GameObject Slot4Marker;
    public GameObject Slot5Marker;

    //------------------------------------------------------------------------------------------------------------------

    bool canTriggerComboTutorial = true;

	[Space]
	[Header("Turret's Healthbar")]
	public GameObject HealthBarTurret;
	public RectTransform HealthBarREDTurret; 
	public RectTransform HealthBarORANGETurret; 
	public Image HealthBarTurret_BG; 
	public bool orangeLerpTurret = false; 
	[HideInInspector]public float fromSizeTurret; 
	[HideInInspector]public float newSizeTurret;

	[Space]
	[Header("Turret's Fillbar")]
	public Image turretFillBar; // The actual fill amount of the turret panel.
	public GameObject turretPanel; // The container for the turret panel gameobject to turn on/off. 

	//------------------------------------------------------------------------------------------------------------------

	[Space]
	[Header("Secret Room Icons")]
	public GameObject secretRoomPanel; 
	[Space]
	public Image secretRoom01;
	public Image secretRoom02; 
	public Image secretRoom03;  
	public Image secretRoom04; 

	public bool isFirstSwitch = true; 


	// Use this for initialization
	void Start () {
		
		
		HUDContainer = GameObject.FindObjectOfType<SP_HUDContainer> (); 
		gameController = FindObjectOfType<BR_GameController> (); 
		mainMenuShowPanels = FindObjectOfType<ShowPanels> (); 
		audioController = FindObjectOfType<AudioController> (); 

		musicController = FindObjectOfType<MusicController> (); 

	 
		// Finding GameObject in Scene without using the inspector. 
		uiSingleFire = GameObject.FindGameObjectWithTag("UI_SingleFire"); 
		uiAutoFire = GameObject.FindGameObjectWithTag("UI_AutoFire"); 
		uiScatterFire = GameObject.FindGameObjectWithTag("UI_ScatterFire");

        uiShield = GameObject.FindGameObjectWithTag("UI_Shield");



		// After Finding All Firing Modes Icons show only Single Fire at when play. 
		uiAutoFire.SetActive (false); 
		uiScatterFire.SetActive (false);
		uiSingleFire.SetActive (false);
	
        uiShield.SetActive(false);
		 
		differentFireModeBG.SetActive (false); 
		ammoBG.SetActive (false); 

		singleFireIcon.SetActive (false); 

		ammoText.SetActive (false); 
		autoFireIcon.SetActive (false); 
		scatterFireIcon.SetActive (false);
	

		chronoWatchIcon.SetActive (false); 
		grenadeLauncherIcon.SetActive (false); 
		empDashIcon.SetActive (true);
		empExplosionIcon.SetActive (false); 
		hackingPanel.SetActive (false); 
		turretPanel.SetActive (false); 

	}
	
	// Update is called once per frame
	void Update () {
		
		CheckForTutorials (); 
		 
		HUD_HealthLerpEffect (); 
		HUD_ShieldLerpEffect (); 
		
		if (HealthBarThrusterA.activeInHierarchy) {
			ThrusterAHUD ();
		}
		if (HealthBarThrusterB.activeInHierarchy) {
			ThrusterBHUD ();
		}
        if (BossSlot[0] != null)
        {
            HealthBarBoss01.SetActive(true);
            switch (BossSlot[0].GetComponent<AIAgent>().EnemyType)
            {
                case AIAgent.ENEMYTYPE.BOSS1:
                    boss1Icon.SetActive(true);
                    break;
                case AIAgent.ENEMYTYPE.BOSS2:
                    boss2Icon.SetActive(true);
                    break;
                case AIAgent.ENEMYTYPE.TRILOGYEXO:
                    triExoIcon.SetActive(true);
                    break;
            }
			Boss01HUD ();
		} else
        {
            boss1Icon.SetActive(false);
            boss2Icon.SetActive(false);
            triExoIcon.SetActive(false);
            HealthBarBoss01.SetActive (false);
		}

		if (BossSlot[1] != null) {
			HealthBarBoss02.SetActive (true);
			Boss02HUD ();
		} else {
			HealthBarBoss02.SetActive (false);
		}

		if (BossSlot[2] != null) {
			HealthBarBoss03.SetActive (true);
			Boss03HUD ();
		} else {
			HealthBarBoss03.SetActive (false);
		}

		playerController = FindObjectOfType<PlayerController>();
		if (playerController != null)
        {
			GetShieldPct (); 
			GetHealthPct ();
			ShowObjectivePanel (); 
			ShieldFillAmount (); 
			HealthFillAmount ();
			EmpDashFillAmount ();
			EmpExplosionFillAmount (); 
			ChronoWatchFillAmount (); 
			ComboMeterFillAmount (); 
			GrenadeLauncherFillAmount (); 
			//HackingFillAmount (); 
			LeftPanelText ();
			ComboScoreMultipler (); 
			FlashColorDamage ();

            if (curHackingVol != null)
            {
                hackingPanel.SetActive(true);
                HackingFillAmount();
            }
            else
                hackingPanel.SetActive(false);

            if (turretScript != null)
                TurretHUD();
            DoTrackingMarker();
		}
	if (waitingForInput)
        {
            switch (inputType)
            {
			case "melee":
                    if (Input.GetAxisRaw("Melee") !=0)
					ClearTutorialScreen ();
                    return;

                case "dash":
                    if (Input.GetButtonDown("Dash"))
                    {
                        ClearTutorialScreen();
                        TutorialResetStatEmpDash();
                    }
                    return;

                case "blaster":
                    if (Input.GetAxisRaw("Fire") != 0)
                        ClearTutorialScreen();
                    return;

                case "auto":
                    if (Input.GetAxisRaw("DpadH") >= .1 || Input.GetButtonDown("Cycle"))
                        ClearTutorialScreen();
                    return;

                case "scatter":
                    if (Input.GetAxisRaw("DpadH") <= -.1 || Input.GetButtonDown("Cycle"))
                        ClearTutorialScreen();
                    return;

                case "empExp":
                    if (Input.GetAxisRaw("Blast") != 0)
                    {
                        ClearTutorialScreen();
                        TutorialResetStatEmpExplosion();
                    }
                    return;

                case "slomo":
                    if (Input.GetAxisRaw("SloMo") != 0)
                    {
                        ClearTutorialScreen();
                        TutorialResetStatChronoWatch();
                    }
                    return;

                case "Interact":
                    if (Input.GetButtonDown("Interact"))
                        ClearTutorialScreen();
                    return;

			    case "super": 
				    if (Input.GetButtonDown ("Super"))
                    {
					    ClearTutorialScreen (); 
					    TutorialResetStatGrenadeLauncher (); 
				    }
				return; 
            }
        }
    }

	public void CheckForTutorials(){
		if (mainMenuShowPanels != null && gameController.tutorialTimer >= 1f && canShowTutorialPanel ) {
			mainMenuShowPanels.ShowTutorialPanel ();
			canShowTutorialPanel = false; 
		}
	}
	public void BossShieldColorChangeYellow(){
		
			HealthBarBoss01Shield.color = Color.yellow; 
	
	}
	public void BossShieldColorChangeBlue(){

		HealthBarBoss01Shield.color = Color.blue;

	}
	 

	public float GetShieldPct()
    {
			return (float)playerController.curShield / playerController.maxShield;
	}
	public float GetHealthPct()
    {
			return (float)playerController.curHealth / playerController.maxHealth; 
	}
	public float GetChronoWatchPct()
    {
			return (float)gameController.curChronoWatchDuration / gameController.chronoWatchDuration; 
	}
	public float GetChronoWatchCoolDownPct(){
		return (float)gameController.curChronoCoolDur / gameController.chronoWatchCoolDownDuration; 
	}

	public float GetGrenadeLauncherPct(){
		return (float)playerController.curSuperTimer / gameController.superShots;  
	}
	public float GetGrenadeLauncherCoolDownPct(){
		return (float)gameController.cur_SuperCooldown / gameController.superCooldown; 
	}


	public float GetComboMeterPct()
    {
		return (float)curComboMeterTimer / comboMeterTimerMax; 
	}
	public float GetEmpDashPct()
    {
		return (float)playerController.curEmpDashDuration / playerController.maxEmpDashActiveTimer; 
	}
	public float GetEmpDashCoolDownPct(){
		return (float)playerController.curEmpDashCoolDownTimer / playerController.maxEmpDashCoolDownDuration; 
	}
	public float GetEmpExplosionPct()
    {
		return (float)playerController.curEmpExplosionDur / playerController.maxEmpExplosionDuration; 
	}

	public float GetEmpExplosionCoolDownPct(){
		return (float)playerController.curEmpExplosionCoolDownDuration / playerController.maxEmpExplosionCoolDownDuration;
	}
    public void ChronoWatchFillAmount()
    {
		if (gameController.isChronoWatchCoolDown == false) {
			chronoWatchFillBar.fillAmount = GetChronoWatchPct ();
		} else {
			chronoWatchFillBar.fillAmount = GetChronoWatchCoolDownPct (); 
		}
    }
	public void GrenadeLauncherFillAmount(){
		if(playerController.isSuperActive == true){
		grenadeLauncherFillBar.fillAmount = GetGrenadeLauncherPct (); 

		}else {
			grenadeLauncherFillBar.fillAmount = GetGrenadeLauncherCoolDownPct (); 
		}
	}

	public void DoSuperLerp(){
		grenadeLauncherFillBar.color = Color.Lerp (grenadeLauncherColor, grenadeLauncherColorLerp, Mathf.PingPong (Time.time, .5f));
	}


	public void ComboMeterFillAmount()
    {
		comboFillBar.fillAmount = GetComboMeterPct ();
	}
	public void ShieldFillAmount()
    {
        shieldFillBar.fillAmount = GetShieldPct();
        if (playerController.curShield <= 0)
        {
            shieldFillBar.fillAmount = 0;
        }
	}
	public void HealthFillAmount()
    {
		healthFillBar.fillAmount = GetHealthPct(); 
	}
	public void EmpDashFillAmount()
    {
		if (playerController.isEmpDashCoolDown == false) {
			empDashFillBar.fillAmount = GetEmpDashPct (); 

		} else { 
			
			empDashFillBar.fillAmount = GetEmpDashCoolDownPct (); 
		}
	}
	public void EmpExplosionFillAmount()
	{
		if (playerController.isEmpExplosionCoolDownActive == false) {
			empExplosionFillBar.fillAmount = GetEmpExplosionPct (); 
		} else {
			empExplosionFillBar.fillAmount = GetEmpExplosionCoolDownPct (); 
		}
	}

	
	public float GetHackingPct (){
		return (float)curHackingVol.curHacking / curHackingVol.maxHackingDuration; 
	}
	public void HackingFillAmount(){
		hackingFillBar.fillAmount = GetHackingPct (); 
	}
	public void HackingColor(){
		hackingFillBar.color = Color.cyan; 
	}
	public void HackingCompleteColor(){
		hackingFillBar.color = Color.green; 
	}

	public void ShowSecretSwitchTutorial(){
		if (gameController.showTutorial) {
			DoTutorialScreen ("A secret switch to a secret room, \nflip them all to unlock the boom, \nThe entrance lies where the ship goes VROOM!. \nPress {0}{1}</color>", "Interact", "X Button", "F Key");
			TutorialSecretRoomScreen (); 
			secretRoomPanel.SetActive (false); 
		}
	}
    public void UnlockShieldIcon()
    {
        uiShield.SetActive(true);
    }
	public void UnlockBlasterIcon()
    {
		ammoBG.SetActive (true); 
		singleFireIcon.SetActive (true);
        if (gameController.showTutorial)
        {
            DoTutorialScreen("Press {0}{1}</color> to fire Blaster", "blaster", "Right Trigger", "Left Mouse Button");
            TutorialSingleFireScreen();
        }
	}
	public void UnlockAutoFireIcon()
    {
		differentFireModeBG.SetActive (true); 
		ammoText.SetActive (true); 
		autoFireIcon.SetActive (true);
		if (gameController.showTutorial)
        {
            if (gameController.isMouseActive)
                DoTutorialScreen("Press {0}{1} or Click Mouse Wheel</color> to switch to the Machine Gun", "auto", "D-Pad Right", "2 Key");
            else
                DoTutorialScreen("Press {0}{1} or Right Bumper </color> to switch to the Machine Gun", "auto", "D-Pad Right", "2 Key");
            TutorialAutoFireScreen();
        }
    }
	public void UnlockScatterFireIcon()
    {
		scatterFireIcon.SetActive (true);
		if (gameController.showTutorial)
        {
            if (gameController.isMouseActive)
                DoTutorialScreen("Press {0}{1} or Click Mouse Wheel</color> to switch to the Spread Cannon", "scatter", "D-Pad Left", "3 Key");
            else
                DoTutorialScreen("Press {0}{1} or Right Bumper</color> to switch to the Spread Cannon", "scatter", "D-Pad Left", "3 Key");
            TutorialScatterFireScreen();
        }
    }
	public void UnlockEmpExplosionIcon()
    {
		empExplosionIcon.SetActive (true);
		if (gameController.showTutorial)
        {
            DoTutorialScreen("Press {0}{1}</color> to perform an EMP Blast", "empExp", "Y Button", "E Key");
            TutorialEmpExplosionScreen();
        }
    }
	public void UnlockChronoWatchIcon()
    {
		chronoWatchIcon.SetActive (true);
		if (gameController.showTutorial)
        {
            DoTutorialScreen("Press {0}{1}</color> to use Chrono Watch to stop time", "slomo", "B Button", "Q Key");
            TutorialChronoWatchScreen();
        }
    }

	public void UnlockGrenadeLauncherFireIcon(){
		grenadeLauncherIcon.SetActive (true); 
		if (gameController.showTutorial)
		{
			DoTutorialScreen("Press {0}{1} </color> to fire 5 quick Sticky grenades that explode once they all reach their destination", "super", "LB Button", "R Key");
			TutorialGrenadeLauncherScreen (); 
		}
	}
	public void ShowSingleFireIcon()
    {
		uiSingleFire.SetActive (true);
		uiAutoFire.SetActive (false); 
		uiScatterFire.SetActive (false);


		SingleFireIconBG.color = activeColor;
		AutoFireIconBG.color = disableColor; 
		ScatterFireIconBG.color = disableColor;
	
	}
	public void ShowAutoFireIcon()
    {
		uiSingleFire.SetActive (false);
		uiAutoFire.SetActive (true); 
		uiScatterFire.SetActive (false);
	

		UpdateAutoAmmoAmount (); 
		SingleFireIconBG.color = disableColor;
		AutoFireIconBG.color = activeColor;
		ScatterFireIconBG.color = disableColor; 

	}
	public void ShowScatterFireIcon()
    {
		uiSingleFire.SetActive (false);
		uiAutoFire.SetActive (false); 
		uiScatterFire.SetActive (true);


		UpdateScatterAmmoAmount (); 
		SingleFireIconBG.color = disableColor;
		AutoFireIconBG.color = disableColor; 
		ScatterFireIconBG.color = activeColor;

	}
	public void ShowGrenadeLauncherFireIcon(){
		uiSingleFire.SetActive (false);
		uiAutoFire.SetActive (false); 
		uiScatterFire.SetActive (false);
	

		SingleFireIconBG.color = disableColor;
		AutoFireIconBG.color = disableColor; 
		ScatterFireIconBG.color = disableColor;

	}
	public void ShowObjectivePanel()
    {
        if (Input.GetAxis("DpadV") <= -.1)
        {
            objectivePanel.SetActive(); 
        }
	}

    public void AddObjective(BR_Objectives.ObjectiveType _type, string _target)
    {
        objectivePanel.AddObjective(_type, _target);
    }

    public void RemoveObjective(BR_Objectives.ObjectiveType _type)
    {
        objectivePanel.CompleteObjective(_type);
    }

    public void AddChallenge(BR_Challenge.ChallengeType _type, int _count, BR_Challenge.ChallengeReward _reward)
    {
        objectivePanel.AddChallenge(_type, _count, _reward);
    }

    public void RemoveChallenges()
    {
        objectivePanel.RemoveChallenges();
    }

    public void LeftPanelText()
    {
		highScoreText.text = "HighScore: " + gameController.highScore; 
		comboText.text = "" + comboScore; 
	}

	public void ComboScoreMultipler()
    {
		// When combo is false then turn the fill bar to gray
		if (comboMeterBool == false) {
			comboFillBar.color = comboZeroColor; 
		}



		if (comboMeterBool == true)
        {
			curComboMeterTimer -= Time.deltaTime;
			if (comboMeterTimerMax != 2.5f) {
				if (curComboMeterTimer >= 3f) {
					comboFillBar.color = Color.green; 
				}
				if (curComboMeterTimer < 3f) {
					comboFillBar.color = Color.yellow; 
				}
				if (curComboMeterTimer < 1f) {
					comboFillBar.color = Color.red; 
				}
			} else {

				if (curComboMeterTimer >= 2f) {
					comboFillBar.color = Color.green; 
				}
				if (curComboMeterTimer < 1f) {
					comboFillBar.color = Color.yellow; 
				}
				if (curComboMeterTimer < 0.5f) {
					comboFillBar.color = Color.red; 
				}
				
			}
		}
		if (curComboMeterTimer <= 0)
        {
			comboMeterBool = false; 
			curComboMeterTimer = comboMeterTimerMax;
			comboFillBar.color = Color.gray; 
			comboScore = 0;

		}
	}

	// Adjust ComboMeterTimerMax 
	public void ComboMeterTimerMaxADJUSTMENT()
    {
		if (comboScore >= 25) {
			comboMeterTimerMax = 2.5f; 
		}
	}

	// Call when Combo +1 (When enemy dies)
	public void ComboScoreMultiplerADDTIME(BR_EnemyHealth.DMGTYPE _dmgType)
    {
		comboMeterBool = true; 
        if (_dmgType != BR_EnemyHealth.DMGTYPE.TURRET)
		comboScore++; 
		ComboMeterTimerMaxADJUSTMENT (); // Check if the combo is higher if so then adjust comboMeterTimerMax's float.
		curComboMeterTimer = comboMeterTimerMax; 
		if (canTriggerComboTutorial && gameController.showTutorial)
        {
            DoTutorialScreen("Perform quick kills to increase your Combo meter\n \nPress {0}{1}</color>", "Interact","X Button","F Key");
			TutorialComboScreen ();

            canTriggerComboTutorial = false;
        }
        if (objectivePanel.curChallenges.Length != 0)
        {
            for (int x = 0; x < objectivePanel.curChallenges.Length; x++)
            {
                switch (objectivePanel.curChallenges[x].GetComponent<BR_Challenge>().challengeType)
                {
                    case BR_Challenge.ChallengeType.COMBO:
                        objectivePanel.ObjectiveTracker(x,comboScore);
                        break;

                    case BR_Challenge.ChallengeType.KILL:
                        objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.DRONEEXPLD:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.DRONEEXPLD)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.BLASTER:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.SINGLE)
                        objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.AUTO:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.AUTO)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.SCATTER:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.SCATTER)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.MELEE:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.MELEE)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.TURRET:
                        if (_dmgType == BR_EnemyHealth.DMGTYPE.TURRET)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    case BR_Challenge.ChallengeType.SLOMO:
                        if (gameController.isChronoWatchActive == true)
                            objectivePanel.ObjectiveTracker(x, 1);
                        break;

                    default:
                        break;
                }
            }
        }
	}

	public void UpdateAutoAmmoAmount()
    {
		autoAmmoText.text = "" + gameController.curAuto_Ammo; 
		if (gameController.curAuto_Ammo <= 10) {
			autoAmmoText.color = Color.yellow; 
		}
		if (gameController.curAuto_Ammo <= 0) {
			gameController.curAuto_Ammo = 0;
			autoAmmoText.color = Color.red; 
		}
		if (gameController.curAuto_Ammo > 10) {
			autoAmmoText.color = Color.white; 
		}
	}

	public void UpdateScatterAmmoAmount()
    {
		
		scatterAmmoText.text = "" + gameController.curScatter_Ammo; 
		if (gameController.curScatter_Ammo <= 10) {
			scatterAmmoText.color = Color.yellow; 
		}
		if (gameController.curScatter_Ammo <= 0) {
			gameController.curScatter_Ammo = 0;
			scatterAmmoText.color = Color.red; 
		}
		if (gameController.curScatter_Ammo > 10) {
			scatterAmmoText.color = Color.white; 
		}
	}


	public void isDamageTaken()
    {
		isDamaged = true; 
	}

	void FlashColorDamage()
    {
		if (playerController.curHealth <= 25 && playerController.curShield <= 0) {
			damageImage.color = lowHealthColor; 
			damageImageScreenOverlay.color = lowHealthColorScreenOverlay; 
		}
		if (isDamaged) {
			if (playerController.curShield >= 0) {
				
				damageImage.color = shieldflashColor;
				damageImageScreenOverlay.color = shieldflashColorScreenOverlay; 
			} 
			if (playerController.curShield <= 0){
				damageImage.color = healthflashColor; 
				damageImageScreenOverlay.color = healthflashColorScreenOverlay; 
			}
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime); 
			damageImageScreenOverlay.color = Color.Lerp (damageImageScreenOverlay.color, Color.clear, flashSpeed * Time.deltaTime); 

		}
		isDamaged = false;
	}

	public void DoGameOver()
    {
		audioController.PlayGameOverSFX(); 
        thrusterTimerA = 0;
        thrusterTimerB = 0;
        turretTimer = 0;
        objectivePanel.ForceRemoveObjectives();
        objectivePanel.RemoveChallenges();
		if(mainMenuShowPanels != null) {
		mainMenuShowPanels.ShowGameOverPanelDELAY (); 
		GameObject myEventSystem = GameObject.Find("EventSystem"); // Find Game's EventSystem for player's gamepad
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(mainMenuShowPanels.checkpointButton); // Set the gamepad to highlight over checkpoint starting button
		}
		HUDContainer.isPausePanel = true; // Turn off HUD
	}

	public void DoWin()
    {
		audioController.PlayWinSFX();
        thrusterTimerA = 0;
        thrusterTimerB = 0;
        turretTimer = 0;
        objectivePanel.ForceRemoveObjectives();
        objectivePanel.RemoveChallenges();
		gameController.showTutorial = false;

        uiShield.SetActive(false);
        ammoBG.SetActive(false);
        singleFireIcon.SetActive(false);
        differentFireModeBG.SetActive(false);
        ammoText.SetActive(false);
        autoFireIcon.SetActive(false);
        scatterFireIcon.SetActive(false);
	
        empExplosionIcon.SetActive(false);
        chronoWatchIcon.SetActive(false);
		grenadeLauncherIcon.SetActive (false); 
		if (mainMenuShowPanels != null) {
			mainMenuShowPanels.winScreenPanel.SetActive (true); 
			GameObject myEventSystem = GameObject.Find ("EventSystem");
			myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (mainMenuShowPanels.startAgainButton);
		}
		HUDContainer.isPausePanel = true;
	}

	public float GetThrusterAHealthPct()
    {
		return (float)thrustersScriptA.curHealth / thrustersScriptA.maxHealth;
	}
    public void ThrusterAHUD()
    {
        if (thrusterTimerA > 0)
        {
            thrusterIconA.color = Color.red;
            thrusterTimerA -= Time.deltaTime;
            thrusterDamageTextA.enabled = true;
            thrusterDamageTextA.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time, .5f));
        }
        else
        {
            thrusterIconA.color = Color.white;
            thrusterDamageTextA.enabled = false;
        }

		HealthBarREDThrusterA.localScale = new Vector3 (GetThrusterAHealthPct(), 1f, 1f); 
		if (orangeLerpThrusterA)
        {
			HealthBarORANGEThrusterA.localScale = new Vector3 (fromSizeThrusterA -= orangeSpeed, 1f, 1f);
		}
		if (fromSizeThrusterA <= newSizeThrusterA)
        {
			orangeLerpThrusterA = false;
			HealthBarORANGEThrusterA.localScale = HealthBarREDThrusterA.localScale;
		}
		if (thrustersScriptA.curHealth <= 0)
        {
			HealthBarThrusterA_BG.color = Color.red;
            thrusterIconA.color = Color.red;
            thrusterDamageTextA.enabled = false;
		}


	}
	public float GetThrusterBHealthPct()
    {
		return (float)thrustersScriptB.curHealth / thrustersScriptB.maxHealth;
	}
	public void ThrusterBHUD ()
    {
        if (thrusterTimerB > 0)
        {
            thrusterIconB.color = Color.red;
            thrusterTimerB -= Time.deltaTime;
            thrusterDamageTextB.enabled = true;
            thrusterDamageTextB.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time, .5f));
        }
        else
        {
            thrusterIconB.color = Color.white;
            thrusterDamageTextB.enabled = false;
        }
            
        HealthBarREDThrusterB.localScale = new Vector3 (GetThrusterBHealthPct(), 1f, 1f); 
		if (orangeLerpThrusterB)
        {
			HealthBarORANGEThrusterB.localScale = new Vector3 (fromSizeThrusterB -= orangeSpeed, 1f, 1f);
		}
		if (fromSizeThrusterB <= newSizeThrusterB)
        {
			orangeLerpThrusterB = false;
			HealthBarORANGEThrusterB.localScale = HealthBarREDThrusterB.localScale;
		}
		if (thrustersScriptB.curHealth <= 0)
        {
			HealthBarThrusterB_BG.color = Color.red;
            thrusterIconB.color = Color.red;
            thrusterDamageTextB.enabled = false;
        }
        
	}

    public float GetTurretHealthPct()
    {
        return (float)turretScript.curHealth / turretScript.maxHealth;
    }
    public void TurretHUD()
    {
        if (turretTimer > 0)
        {
            turretIcon.color = Color.red;
            turretTimer -= Time.deltaTime;
            turretDamageText.enabled = true;
            turretDamageText.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time, .5f));
        }
        else
        {
            turretIcon.color = Color.white;
            turretDamageText.enabled = false;
        }
        if (turretScript.isActive)
            HealthBarREDTurret.GetComponent<Image>().color = Color.cyan;
        else
            HealthBarREDTurret.GetComponent<Image>().color = Color.gray;

        HealthBarREDTurret.localScale = new Vector3(GetTurretHealthPct(), 1f, 1f);
        if (orangeLerpTurret)
        {
            HealthBarORANGETurret.localScale = new Vector3(fromSizeTurret -= orangeSpeed, 1f, 1f);
        }
        if (fromSizeTurret <= newSizeTurret)
        {
            orangeLerpTurret = false;
            HealthBarORANGETurret.localScale = HealthBarREDTurret.localScale;
        }
        if (turretScript.curHealth <= 0)
        {
            if (!turretScript.isActive)
            {
                HealthBarTurret_BG.color = Color.black;
                turretIcon.color = Color.gray;
            }
            else
            {
                HealthBarTurret_BG.color = Color.red;
                turretIcon.color = Color.red;
            }
            turretDamageText.enabled = false;
        }

    }

    public void ThrusterTakingDamage(SP_Thrusters.THRUSTERTYPE _thruster)
    {
        switch (_thruster)
        {
            case SP_Thrusters.THRUSTERTYPE.THRUSTER_A:
                thrusterTimerA = 1f;
                break;

            case SP_Thrusters.THRUSTERTYPE.THRUSTER_B:
                thrusterTimerB = 1f;
                break;
        }
    }
    public void TurretTakingDamage()
    {
        turretTimer = 1f;
    }

    public float GetTrioGuard01HealthPct()
    {
        BR_EnemyHealth boss1health = BossSlot[0].GetComponent<BR_EnemyHealth>();
        return (float)boss1health.curHealth / boss1health.maxHealth;
    }
	public void Boss01HUD ()
    {
		HealthBarREDBoss01.localScale = new Vector3 (GetTrioGuard01HealthPct(), 1f, 1f);
        if (BossSlot[0].GetComponentInChildren<EnemyShield>() != null)
            HealthBarBlueBoss01.localScale = new Vector3(BossSlot[0].GetComponentInChildren<EnemyShield>().GetShieldPct(), 1f, 1f);
        else
            HealthBarBlueBoss01.localScale = new Vector3(0f, 1f, 1f);
        if (orangeLerpBoss01)
        {
			HealthBarORANGEBoss01.localScale = new Vector3 (fromSizeBoss01 -= orangeSpeed, 1f, 1f);
		}
		if (fromSizeBoss01 <= newSizeBoss01)
        {
			orangeLerpBoss01 = false;
			HealthBarORANGEBoss01.localScale = HealthBarREDBoss01.localScale;
		}
        

          else
            Slot1Marker.SetActive(false);
    }

	public float GetTrioGuard02HealthPct()
    {
        BR_EnemyHealth boss2health = BossSlot[1].GetComponent<BR_EnemyHealth>();
        return (float)boss2health.curHealth / boss2health.maxHealth;
    }
	public void Boss02HUD ()
    {
		HealthBarREDBoss02.localScale = new Vector3 (GetTrioGuard02HealthPct(), 1f, 1f); 
        if(BossSlot[1].GetComponentInChildren<EnemyShield>() != null)
		HealthBarBlueBoss02.localScale = new Vector3 (BossSlot[1].GetComponentInChildren<EnemyShield>().GetShieldPct(),1f,1f);
        else
            HealthBarBlueBoss02.localScale = new Vector3(0f, 1f, 1f);
        if (orangeLerpBoss02) {
			HealthBarORANGEBoss02.localScale = new Vector3 (fromSizeBoss02 -= orangeSpeed, 1f, 1f);
		}
		if (fromSizeBoss02 <= newSizeBoss02)
        {
			orangeLerpBoss02 = false;
			HealthBarORANGEBoss02.localScale = HealthBarREDBoss02.localScale;
		}

	}

	public float GetTrioGuard03HealthPct()
    {
        BR_EnemyHealth boss3health = BossSlot[2].GetComponent<BR_EnemyHealth>();
        return (float)boss3health.curHealth / boss3health.maxHealth;
	}
	public void Boss03HUD ()
    {
		HealthBarREDBoss03.localScale = new Vector3 (GetTrioGuard03HealthPct(), 1f, 1f);
        if (BossSlot[2].GetComponentInChildren<EnemyShield>() != null)
            HealthBarBlueBoss03.localScale = new Vector3(BossSlot[2].GetComponentInChildren<EnemyShield>().GetShieldPct(), 1f, 1f);
        else
            HealthBarBlueBoss03.localScale = new Vector3(0f, 1f, 1f);
        if (orangeLerpBoss03)
        {
			HealthBarORANGEBoss03.localScale = new Vector3 (fromSizeBoss03 -= orangeSpeed, 1f, 1f);
		}
		if (fromSizeBoss03 <= newSizeBoss03)
        {
			orangeLerpBoss03 = false;
			HealthBarORANGEBoss03.localScale = HealthBarREDBoss03.localScale;
		}
	}

    public void DoTutorialScreen (string _string, string _type, string _xboxButton, string _PCKey)
    {
       
        Time.timeScale = 0.000f;
		musicController.audiosource.volume = 0.1f; 

        tutorialScreen.SetActive(true);
        if (gameController.isMouseActive)
            tutorialText.text = string.Format(_string, "<color=orange>",_PCKey);
        else
            tutorialText.text = string.Format(_string, "<color=lime>", _xboxButton);
        inputType = _type;
        waitingForInput = true;
    }

    public void ClearTutorialScreen()
    {
        Time.timeScale = 1;
		musicController.audiosource.volume = 0.4f;
		TutorialClearIcons (); 
        tutorialScreen.SetActive(false);
        waitingForInput = false;
    }

    public void SetThrusterHUD_A(bool _isActive)
    {
        if(_isActive != HealthBarThrusterA.activeInHierarchy)
        {
            HealthBarThrusterA.SetActive(_isActive);
        }
    }

    public void SetThursterHUD_B(bool _isActive)
    {
        if (_isActive != HealthBarThrusterB.activeInHierarchy)
        {
            HealthBarThrusterB.SetActive(_isActive);
        }
    }

    public void SetTurretHUD(bool _isActive)
    {
            HealthBarTurret.SetActive(_isActive);
    }

	public void HUD_HealthLerpEffect()
    {
		if (healthBarOrangeLerp)
        {
			healthFillBarOrange.fillAmount = (healthBarFromSize -= OrangeSpeed); 
		}
		if (healthBarFromSize <= healthBarNewSize)
        {
			healthBarOrangeLerp = false; 
			healthFillBarOrange.fillAmount = healthFillBar.fillAmount; 
		}
	}

	public void HUD_ShieldLerpEffect()
    {
		if (shieldBarOrangeLerp)
        {
			shieldFillBarOrange.fillAmount = (shieldBarFromSize -= OrangeSpeed); 
		}
		if (shieldBarFromSize <= shieldBarNewSize)
        {
			shieldBarOrangeLerp = false; 
			shieldFillBarOrange.fillAmount = shieldFillBar.fillAmount; 
		}
	}

	//-----------------------------------------------------------------------------------------------------------------
	public void DoFromSizeHealth()
    {
		healthBarFromSize = GetHealthPct (); 
	}

	public void DoOrangeLerpHealth ()
    {
		healthBarOrangeLerp = true; 
	}

	public void DoNewSizeHealth()
    {
		healthBarNewSize = GetHealthPct ();
	}

	//-----------------------------------------------------------------------------------------------------------------
	public void DoFromSizeShield()
    {
		shieldBarFromSize = GetShieldPct (); 
	}

	public void DoOrangeLerpShield ()
    {
		shieldBarOrangeLerp = true; 
	}

	public void DoNewSizeShield()
    {
		shieldBarNewSize = GetShieldPct ();
	}

    public void CheckBossChallenge()
    {
        if (objectivePanel.curChallenges.Length != 0)
        {
            objectivePanel.CompleteChallenge(BR_Challenge.ChallengeType.BOSSTIME);
        }
    }

    public void EndWaveHealthCheck()
    {
        if (objectivePanel.curChallenges.Length != 0)
        {
            for (int x = 0; x < objectivePanel.curChallenges.Length; x++)
            {
                switch (objectivePanel.curChallenges[x].GetComponent<BR_Challenge>().challengeType)
                {
                    case BR_Challenge.ChallengeType.HEALTH:
                        if (playerController.curHealth >= playerController.maxHealth * .5)
                            objectivePanel.CompleteChallenge(BR_Challenge.ChallengeType.HEALTH);
                        else
                            objectivePanel.FailChallenge(BR_Challenge.ChallengeType.HEALTH);
                        break;

                    case BR_Challenge.ChallengeType.SHIELD:
                        if (playerController.curShield >= playerController.maxShield * .5)
                            objectivePanel.CompleteChallenge(BR_Challenge.ChallengeType.SHIELD);
                        else
                            objectivePanel.FailChallenge(BR_Challenge.ChallengeType.SHIELD);
                        break;

                    default:
                        if (objectivePanel.curChallenges[x].GetComponent<BR_Challenge>().challengeStatus == BR_Challenge.ChallengeStatus.STARTED)
                        {
                            objectivePanel.curChallenges[x].GetComponent<BR_Challenge>().challengeStatus = BR_Challenge.ChallengeStatus.FAILED;
                            objectivePanel.curChallenges[x].GetComponent<Text>().color = Color.red;
                        }
                        break;
                }
            }
        }
    }

    void DoTrackingMarker()
    {
            if (trackingSlot[0] != null)
            {
                if (!trackingSlot[0].gameObject.GetComponent<BR_EnemyHealth>().isOnScreen)
                {
                    Slot1Marker.SetActive(true);
                    switch (trackingSlot[0].GetComponent<AIAgent>().EnemyType)
                    {
                        case AIAgent.ENEMYTYPE.BOSS1:
                            Boss1TrackerIcon.SetActive(true);
                            break;
                        case AIAgent.ENEMYTYPE.BOSS2:
                            Boss2TrackerIcon.SetActive(true);
                            break;
                        case AIAgent.ENEMYTYPE.TRILOGYEXO:
                            TriGuard1TrackerIcon.SetActive(true);
                            break;
                        default:
                            default1Icon.SetActive(true);
                            break;
                    }
                    Vector2 enemy1Pos = Camera.main.WorldToScreenPoint(trackingSlot[0].transform.position);
                    enemy1Pos.x = Mathf.Clamp(enemy1Pos.x, 40, Screen.width - 40);
                    enemy1Pos.y = Mathf.Clamp(enemy1Pos.y, 40, Screen.height - 40);
                    Slot1Marker.GetComponent<RectTransform>().position = (enemy1Pos);
                }
                else
                {
                    Slot1Marker.SetActive(false);
                    Boss1TrackerIcon.SetActive(false);
                    Boss2TrackerIcon.SetActive(false);
                    TriGuard1TrackerIcon.SetActive(false);
                    default1Icon.SetActive(false);
                }
            }
            else
            {
                Slot1Marker.SetActive(false);
                Boss1TrackerIcon.SetActive(false);
                Boss2TrackerIcon.SetActive(false);
                TriGuard1TrackerIcon.SetActive(false);
                default1Icon.SetActive(false);
            }

            if (trackingSlot[1] != null)
            {
                if (!trackingSlot[1].gameObject.GetComponent<BR_EnemyHealth>().isOnScreen)
                {
                    Slot2Marker.SetActive(true);
                    switch (trackingSlot[1].GetComponent<AIAgent>().EnemyType)
                    {
                        case AIAgent.ENEMYTYPE.TRILOGYEXO:
                            TriGuard2TrackerIcon.SetActive(true);
                            break;
                        default:
                            default2Icon.SetActive(true);
                            break;
                    }
                    Vector2 enemy2Pos = Camera.main.WorldToScreenPoint(trackingSlot[1].transform.position);
                    enemy2Pos.x = Mathf.Clamp(enemy2Pos.x, 40, Screen.width - 40);
                    enemy2Pos.y = Mathf.Clamp(enemy2Pos.y, 40, Screen.height - 40);
                    Slot2Marker.GetComponent<RectTransform>().position = (enemy2Pos);
                }
                else
                {
                    Slot2Marker.SetActive(false);
                    TriGuard2TrackerIcon.SetActive(false);
                    default2Icon.SetActive(false);
                }
            }
            else
            {
                Slot2Marker.SetActive(false);
                TriGuard2TrackerIcon.SetActive(false);
                default2Icon.SetActive(false);
            }
            if (trackingSlot[2] != null)
            {
                if (!trackingSlot[2].gameObject.GetComponent<BR_EnemyHealth>().isOnScreen)
                {
                    Slot3Marker.SetActive(true);
                    switch (trackingSlot[2].GetComponent<AIAgent>().EnemyType)
                    {
                        case AIAgent.ENEMYTYPE.TRILOGYEXO:
                            TriGuard3TrackerIcon.SetActive(true);
                            break;
                        default:
                            default3Icon.SetActive(true);
                            break;
                    }
                    Vector2 enemy3Pos = Camera.main.WorldToScreenPoint(trackingSlot[2].transform.position);
                    enemy3Pos.x = Mathf.Clamp(enemy3Pos.x, 40, Screen.width - 40);
                    enemy3Pos.y = Mathf.Clamp(enemy3Pos.y, 40, Screen.height - 40);
                    Slot3Marker.GetComponent<RectTransform>().position = (enemy3Pos);
                }
                else
                {
                    Slot3Marker.SetActive(false);
                    TriGuard3TrackerIcon.SetActive(false);
                    default3Icon.SetActive(false);
                }
            }
            else
            {
                Slot3Marker.SetActive(false);
                TriGuard3TrackerIcon.SetActive(false);
                default3Icon.SetActive(false);
            }
            if (trackingSlot[3] != null)
            {
                if (!trackingSlot[3].gameObject.GetComponent<BR_EnemyHealth>().isOnScreen)
                {
                    Slot4Marker.SetActive(true);
                    Vector2 enemy4Pos = Camera.main.WorldToScreenPoint(trackingSlot[3].transform.position);
                    enemy4Pos.x = Mathf.Clamp(enemy4Pos.x, 40, Screen.width - 40);
                    enemy4Pos.y = Mathf.Clamp(enemy4Pos.y, 40, Screen.height - 40);
                    Slot4Marker.GetComponent<RectTransform>().position = (enemy4Pos);
                }
                else
                {
                    Slot4Marker.SetActive(false);
                }
            }
            else
            {
                Slot4Marker.SetActive(false);
            }
            if (trackingSlot[4] != null)
            {
                if (!trackingSlot[4].gameObject.GetComponent<BR_EnemyHealth>().isOnScreen)
                {
                    Slot5Marker.SetActive(true);
                    Vector2 enemy5Pos = Camera.main.WorldToScreenPoint(trackingSlot[4].transform.position);
                    enemy5Pos.x = Mathf.Clamp(enemy5Pos.x, 40, Screen.width - 40);
                    enemy5Pos.y = Mathf.Clamp(enemy5Pos.y, 40, Screen.height - 40);
                    Slot5Marker.GetComponent<RectTransform>().position = (enemy5Pos);
                }
                else
                {
                    Slot5Marker.SetActive(false);
                }
            }
            else
            {
                Slot5Marker.SetActive(false);
            }
    }


	public void TutorialClearIcons(){
			meleeMode.SetActive(false);
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false);
			hackingMode.SetActive (false);
			turretMode.SetActive (false);
			powerPanelMode.SetActive (false);
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
			


	}
	public void TutorialMeleeScreen(){
		
		meleeMode.SetActive(true);

		if (meleeMode.activeInHierarchy == true) {
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 

		}
	}
	public void TutorialSingleFireScreen(){
		singleFireMode.SetActive(true);

		if (singleFireMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 

		}
	}
	public void TutorialAutoFireScreen(){
		autoFireMode.SetActive(true);

		if (autoFireMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 

		}
	}
	public void TutorialScatterFireScreen(){
		scatterFireMode.SetActive(true);

		if (scatterFireMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false);
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
		
		}
	}
	public void TutorialChronoWatchScreen(){
		chronoWatchMode.SetActive(true);

		if (chronoWatchMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			empDashMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false);
			thrusterRoomMode.SetActive (false); 

		}
	}
	public void TutorialEmpDashScreen(){
		empDashMode.SetActive(true);

		if (empDashMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empExplosionMode.SetActive(false); 
			comboMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false);
			thrusterRoomMode.SetActive (false); 
		
		}
	}
	public void TutorialEmpExplosionScreen(){
		empExplosionMode.SetActive(true);

		if (empExplosionMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			comboMode.SetActive(false);
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
		
		}
	}
	public void TutorialComboScreen(){
		comboMode.SetActive(true);

		if (comboMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			hackingMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
	
		}
	}

	public void TutorialHackingScreen(){
		hackingMode.SetActive(true);

		if (hackingMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
		
		}
	}
	public void TutorialTurretScreen(){
		turretMode.SetActive(true);

		if (turretMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			hackingMode.SetActive (false); 
			powerPanelMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
		
		}
	}

	public void TutorialPowerPanelScreen(){
		powerPanelMode.SetActive(true);

		if (powerPanelMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			hackingMode.SetActive (false); 
			turretMode.SetActive (false); 
			secretRoomMode.SetActive (false); 
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 
		
		}
	}
	public void TutorialGrenadeLauncherScreen(){
		grenadeLauncherMode.SetActive(true);

		if (grenadeLauncherMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			hackingMode.SetActive (false); 
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false);
			secretRoomMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 

		}
	}
	public void TutorialSecretRoomScreen(){
		secretRoomMode.SetActive(true);

		if (secretRoomMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			hackingMode.SetActive (false); 
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false);
			grenadeLauncherMode.SetActive (false); 
			thrusterRoomMode.SetActive (false); 

		}

	}
	public void TutorialThrusterRoomScreen(){
		thrusterRoomMode.SetActive(true);

		if (thrusterRoomMode.activeInHierarchy == true) {
			meleeMode.SetActive(false); 
			singleFireMode.SetActive(false); 
			autoFireMode.SetActive(false); 
			scatterFireMode.SetActive(false); 
			chronoWatchMode.SetActive(false);
			empDashMode.SetActive(false); 
			empExplosionMode.SetActive(false); 
			comboMode.SetActive (false);
			hackingMode.SetActive (false); 
			turretMode.SetActive (false); 
			powerPanelMode.SetActive (false);
			grenadeLauncherMode.SetActive (false); 
			secretRoomMode.SetActive (false); 

		}

	}


    public void ChallengScoreText(int _value, BR_Challenge.ChallengeReward _reward)
    {
        challengeScore.enabled = true;
        challengeScore.text = "Challenge Success+" + _value + "!";
        switch (_reward)
        {
            case BR_Challenge.ChallengeReward.BRONZE:
                challengeScore.color = new Color32(205, 127, 50, 255);
                return;

            case BR_Challenge.ChallengeReward.SILVER:
                challengeScore.color = new Color32(192, 192, 192, 255);
                return;

            case BR_Challenge.ChallengeReward.GOLD:
                challengeScore.color = new Color32(255, 215, 0, 255);
                return;
        }
    }

	public void TutorialResetStatEmpDash(){
		playerController.curEmpDashCoolDownTimer = 1.3f; 
	}
	public void TutorialResetStatEmpExplosion(){
		playerController.curEmpExplosionCoolDownDuration = 9f; 
	}
	public void TutorialResetStatChronoWatch(){
		
		gameController.curChronoWatchDuration = 0.2f; 
		Invoke ("ResetStatChronoWatch", 0.2f); 
	}
	public void TutorialResetStatGrenadeLauncher(){
		gameController.cur_SuperCooldown = 29f;
	}

	public void ResetStatChronoWatch(){
		
		gameController.curChronoCoolDur = 14f;
	}



}
