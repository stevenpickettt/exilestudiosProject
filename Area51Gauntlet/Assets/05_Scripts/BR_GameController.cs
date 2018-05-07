using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_GameController : MonoBehaviour {

    public static BR_GameController controller;
    public enum DebugRoom {NONE, BOSS1, THRUSTER1, THRUSTER2, TRIGUARDS, BOSS2};
    public DebugRoom DebugStartRoom = DebugRoom.NONE;
    public enum GameDifficulty { EASY, NORMAL, HARD};
    public GameDifficulty gameDifficulty = GameDifficulty.NORMAL;


	//[HideInInspector]
    public float tutorialTimer; // 
	public bool showTutorial = true; // Toggle for the Tutorial Panel during the Game
	public bool doDash = false; // So the player doesn't dash when the tutorial / pause panel is active. 
	public float canDashTimer = 0f; // Timer for when the player is done when the pause menu to able to dash
	public bool canDashSwitch = false; // Bool for when the timer starts. 
    
	AudioSource audiosource;
	AudioController audioController;


    [SerializeField]
    private PlayerController playerPrefab;
    PlayerController player;
	public SP_HUD HUD;

    [SerializeField]
    GameObject startingSpawn;
    //Score variables 

    public int highScore;


    public KW_AnimationController anim;

    //weapon controller
    public enum WEAPON { NONE = 0, SINGLE = 1, SCATTER = 2, AUTO = 3 };
    public WEAPON CurrentWeapon = WEAPON.NONE;
    public WEAPON LastWeapon = WEAPON.NONE;

    public bool isBlasterUnlocked = false;
    public bool isScatterUnlocked = false;
    public bool isAutoUnlocked = false;
    public bool isShieldUnlocked = false;
    public bool isGLUnlocked = false;

	public bool isEmpExplosionUnlocked = false; 

    public int curScatter_Ammo;
    public int startingScatter_Ammo =50;

    public int curAuto_Ammo;
    public int startingAuto_Ammo = 100;

    private bool canTriggerMeleeTutorial = true;
    public bool canTriggerDashTutorial = true;
    private bool canTriggerHackTutorial = true;
    public BR_RoomScript[] rooms;


	[Space]
	[Header("Chrono-Watch")]
    //for chrono watch
    public float chronoWatchCoolDownDuration = 10.0f;
	[HideInInspector] public float curChronoCoolDur;
    public bool isChronoWatchActive = false;
    [Tooltip("This value will be multiplied by 4.")]
    public float chronoWatchDuration = 5.0f;
    public float curChronoWatchDuration; // Need public for HUD. 
	[HideInInspector] public bool isChronoWatchCoolDown = false;
    public float sloMoTimeScale = .25f;

    [Header("Super")]
    public float superCooldown = 120f;
    public float cur_SuperCooldown;
    public int superShots = 5; 


    private Transform respawnPoint;

    public ShipScript ship;

    public bool isMouseActive =false;
    Vector2 lastMouseLoc;

    public WaveSpawner spawner;
    [SerializeField]
    private WaveSpawner[] AllSpawners;

    public float RS_X;
    public float RS_y;

	[Space]
	//Thrusters
	private GameObject ThrusterObjectA; 
	private GameObject ThrusterObjectB;

    [Space]
    public bool empBlastOn;
    public bool chronoWatchOn;

    private bool hasCompletedThrusters = false;
    public bool DoDebug;
    float debugTimer = .1f;

    public bool isPlayerAlive;

    //public GameOverManager GOM;
    // Use this for initialization
    void Start ()
    {

		ShowPanels UI = FindObjectOfType<ShowPanels>();
		if (UI != null)
		{
			

			gameDifficulty = UI.difficultySettings;
			DebugStartRoom = UI.DebugRoomSet;
			UI.tutorialToggleGameObject.SetActive (true); // Turning on Tutorial Button in the game scene.  
		}
		switch (gameDifficulty)
		{
		case GameDifficulty.EASY:
			spawner = AllSpawners[0];
                chronoWatchCoolDownDuration = 10f;
                playerPrefab.maxEmpDashCoolDownDuration = 1.5f;
                playerPrefab.maxEmpExplosionCoolDownDuration = 7.5f;
            break;

		default:
			spawner = AllSpawners[1];
                chronoWatchCoolDownDuration = 15f;
                playerPrefab.maxEmpDashCoolDownDuration = 2.5f;
                playerPrefab.maxEmpExplosionCoolDownDuration = 10f;
            break;

		case GameDifficulty.HARD:
			spawner = AllSpawners[2];
                chronoWatchCoolDownDuration = 20f;
                playerPrefab.maxEmpDashCoolDownDuration = 3.5f;
                playerPrefab.maxEmpExplosionCoolDownDuration = 15f;
			break;
		}

        if (DebugStartRoom != DebugRoom.NONE)
            DoDebug = true;
        spawner.gameObject.SetActive(true);
        spawner.rooms = rooms;
        audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
        ship = FindObjectOfType<ShipScript>();
        respawnPoint = startingSpawn.transform;
		HUD = FindObjectOfType<SP_HUD> (); // Reference for HUD PREFAB
		curChronoWatchDuration = chronoWatchDuration;
        RespawnPlayer();
        // Find the Thrusters
        ThrusterObjectA = GameObject.FindGameObjectWithTag ("ThrusterA");
		ThrusterObjectB = GameObject.FindGameObjectWithTag ("ThrusterB");


       
}	

	// Update is called once per frame
	void Update () {
        if (DoDebug)
        {
            showTutorial = false;
            debugTimer -= Time.deltaTime;
            if (debugTimer<= 0)
            {
                switch (DebugStartRoom)
                {
                    case DebugRoom.BOSS1:
                        spawner.DebugWaveSet(1);
                        break;

                    case DebugRoom.THRUSTER1:
                        spawner.DebugWaveSet(2);
                        break;

                    case DebugRoom.THRUSTER2:
                        spawner.DebugWaveSet(4);
                        break;

                    case DebugRoom.TRIGUARDS:
                        spawner.DebugWaveSet(7);
                        break;

                    case DebugRoom.BOSS2:
                        spawner.DebugWaveSet(10);
                        break;
                }
                DoDebug = false;
            }
        }
		if (showTutorial && tutorialTimer <= 10f && spawner.curRoom == 0) 
				tutorialTimer += Time.deltaTime;
        if (tutorialTimer >= 3f && canTriggerMeleeTutorial && showTutorial)
        {
            HUD.DoTutorialScreen("Press {0}{1}</color> to Melee", "melee","Left Trigger", "Right Mouse Button");
			HUD.TutorialMeleeScreen (); 
            canTriggerMeleeTutorial = false;
        }
        if (tutorialTimer >= 7f && canTriggerDashTutorial && showTutorial)
        {
            HUD.DoTutorialScreen("Press {0}{1}</color> to perform a Dash", "dash","A Button","Space Bar");
			HUD.TutorialEmpDashScreen (); 
            canTriggerDashTutorial = false;
        }
        CheckInput();
        if (isChronoWatchActive == true)
            SloMo();
        TurnOnChrono();
        ChronoWatchCoolDown(); 
        if (isBlasterUnlocked && player.allSSfired) 
			DoWeaponSelection ();
        if (isGLUnlocked)
            GrenadeLauncherInput();
    }

     void DoWeaponSelection()
    {
        if (Input.GetButtonDown("Cycle") && !HUD.tutorialScreen.activeInHierarchy)
        {
            switch (CurrentWeapon)
            {
                case WEAPON.SINGLE:
                    if (isAutoUnlocked)
                    {
                        HUD.ShowAutoFireIcon();
                        CurrentWeapon = WEAPON.AUTO;
                    }
                    break;

                case WEAPON.AUTO:
                    if (isScatterUnlocked)
                    {
                        HUD.ShowScatterFireIcon();
                        CurrentWeapon = WEAPON.SCATTER;
                    }
                    else
                    {
                        HUD.ShowSingleFireIcon();
                        CurrentWeapon = WEAPON.SINGLE;
                    }
                    break;

                case WEAPON.SCATTER:
                    HUD.ShowSingleFireIcon();
                    CurrentWeapon = WEAPON.SINGLE;
                    break;
            }
        }
        if (Input.GetAxis("DpadV") >= .1 && CurrentWeapon != WEAPON.SINGLE && isBlasterUnlocked && !HUD.tutorialScreen.activeInHierarchy)
        {
            CurrentWeapon = WEAPON.SINGLE;
            Debug.Log("Current Weapon = " + CurrentWeapon);
			HUD.ShowSingleFireIcon ();

        }

        if (Input.GetAxis("DpadH") >= .1 && CurrentWeapon != WEAPON.AUTO && isAutoUnlocked && !HUD.tutorialScreen.activeInHierarchy)
        {
            CurrentWeapon = WEAPON.AUTO;
            Debug.Log("Current Weapon = " + CurrentWeapon);
            HUD.ShowAutoFireIcon ();
			 
        }
        
        if (Input.GetAxis("DpadH") <= -.1&& CurrentWeapon != WEAPON.SCATTER && isScatterUnlocked && !HUD.tutorialScreen.activeInHierarchy)
        {
            CurrentWeapon = WEAPON.SCATTER;
            Debug.Log("Current Weapon = " + CurrentWeapon);
			HUD.ShowScatterFireIcon ();

        }
    }
    
    public void UnlockBlaster()
    {
        
        isBlasterUnlocked = true;
        HUD.UnlockBlasterIcon();
        CurrentWeapon = WEAPON.SINGLE;
        HUD.ShowSingleFireIcon();
        RewardPickedUp();
    }

    public void UnlockAuto()
    {
        if (!isAutoUnlocked)
        {
            
            curAuto_Ammo = startingAuto_Ammo;
            HUD.UnlockAutoFireIcon();
            isAutoUnlocked = true;
            RewardPickedUp();
        }
    }
	public void UnlockScatter()
	{
        if (!isScatterUnlocked)
        {
            curScatter_Ammo = startingScatter_Ammo;
            HUD.UnlockScatterFireIcon();
            isScatterUnlocked = true;
            RewardPickedUp();
        }
    }

    public void TurnOnShild()
    {
        isShieldUnlocked = true;
        HUD.UnlockShieldIcon();
        RewardPickedUp();
    }

    public void UnlockEmpExplosion()
    {
        
        empBlastOn = true;
        HUD.UnlockEmpExplosionIcon();
        RewardPickedUp();
    }

    public void UnlockChronoWatch()
    {
        
        HUD.UnlockChronoWatchIcon();
            chronoWatchOn = true;
        RewardPickedUp();
    }

	public void UnlockGrenadeLauncher(){
		HUD.UnlockGrenadeLauncherFireIcon (); 
		isGLUnlocked = true; 
		RewardPickedUp (); 
	}

    void RewardPickedUp()
    {
        HUD.RemoveObjective(BR_Objectives.ObjectiveType.PICKUP);
        spawner.isRewardAvailable = false;
    }

    void SloMo()
    {
        // slow down time 
		Time.timeScale = sloMoTimeScale;
		HUD.chronoWatchScreen.SetActive (true); 
        //activate the timer for watch duration and clamp value
        curChronoWatchDuration -= Time.deltaTime; 
        curChronoWatchDuration = Mathf.Clamp(curChronoWatchDuration, 0.0f, chronoWatchDuration);
         

        // if timer is up turn off SloMo and reset the timer and activate the cooldown
        if(curChronoWatchDuration <= 0.0f)
        {
            isChronoWatchActive = false;
            
            Time.timeScale = 1f;
			HUD.chronoWatchScreen.SetActive (false); 
            isChronoWatchCoolDown = true;  

        }
    }

    void TurnOnChrono()
    {

        // if the rb button is held and the chrono is not already active then activate sloMo 
		if (Input.GetAxis("SloMo") != 0 && isChronoWatchActive == false && chronoWatchOn == true && isChronoWatchCoolDown == false && player.allSSfired && !HUD.tutorialScreen.activeInHierarchy && !player.isEmpExplosionActive && !player.isEmpDashActive)
        {
			
		
            isChronoWatchActive = true;
			audioController.Player_ChronoWatchSFX();


        }
    }

    void ChronoWatchCoolDown()
    {
        // if the cool down is greater than or equal to zero start timer
        if (curChronoCoolDur >= 0.0f && isChronoWatchCoolDown == true)
        {
            curChronoCoolDur += Time.deltaTime;
            isChronoWatchActive = false; 
			HUD.chronoWatchFillBar.color = HUD.regenColor; 

        }

		if(curChronoCoolDur >= chronoWatchCoolDownDuration)
        {
			isChronoWatchCoolDown = false; 
            curChronoCoolDur = 0.0f;
			curChronoWatchDuration = chronoWatchDuration;
			HUD.chronoWatchFillBar.color = HUD.chronoWatchColor;
            
        }
    }

    void CheckInput()
    {
        if (isMouseActive)
        {
            if (Input.GetAxis("HorizontalRS") != 0 || Input.GetAxis("VerticalRS") != 0 || Input.GetAxis("HorizontalLS") != 0 || Input.GetAxis("VerticalLS") != 0) 
            {
                isMouseActive = false;
                Cursor.visible = false;
            }
        }
        else
        {
            if (lastMouseLoc != (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
				
            {
                lastMouseLoc = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
                isMouseActive = true;
                Cursor.visible = true;
            }
        }
        RS_X = Input.GetAxis("HorizontalRS");
        RS_y = Input.GetAxis("VerticalRS");
    }

    public void ModHighScore(int value)
    {
		
		if (value > 0) {
			highScore += value * HUD.comboScore;
		}
		else{
            highScore += value;
		}

		// Score never goes under 0. 
		if (highScore <= 0) {
			highScore = 0; 
		}

    }

	public void ModHighScoreChallenges(int value, BR_Challenge.ChallengeReward _reward)
	{
		highScore += value;
        HUD.ChallengScoreText(value, _reward);
	}
	

    public void HitCheckPoint(Transform _spawn)
    {
        
        respawnPoint = _spawn;
        Debug.Log("New Respawn point is " + respawnPoint.name);
    }

    public void PlayerDeath()
    {
        isPlayerAlive = false;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        spawner.CheckpointWaves();
        ModHighScore(-1000);
        
        HUD.DoGameOver(); 

    }

    public void RespawnPlayer()
    {
		// Reset HUD
		HUD.gameObject.SetActive (true); 
		HUD.isDamaged = false;
        HUD.RemoveChallenges();
        //---------------------------------



		if (isAutoUnlocked && curAuto_Ammo <= startingAuto_Ammo) {
			
			curAuto_Ammo = startingAuto_Ammo;
			HUD.UpdateAutoAmmoAmount (); 
		}
		if (isScatterUnlocked && curAuto_Ammo <= startingScatter_Ammo) {
			
			curScatter_Ammo = startingScatter_Ammo;
			HUD.UpdateScatterAmmoAmount (); 
		}
		player = Instantiate(playerPrefab, respawnPoint.position, respawnPoint.rotation);
        HUD.AddObjective(BR_Objectives.ObjectiveType.REACH, rooms[spawner.curRoom].GetComponent<BR_RoomScript>().roomName.ToLower());

        if (!HUD.thrustersScriptA.GetComponent<SP_Thrusters>().done)
            HUD.thrustersScriptA.SetHealth();
        if (!HUD.thrustersScriptB.GetComponent<SP_Thrusters>().done)
            HUD.thrustersScriptB.SetHealth();

        isPlayerAlive = true;



    }

    public void PlayerWin()
    {
        HUD.thrustersScriptA.ResetThruster();
        HUD.thrustersScriptB.ResetThruster();
        //RewardPickedUp();
        ship.ResetShip();
        CurrentWeapon = WEAPON.NONE;
        empBlastOn = false;
        chronoWatchOn = false;
        isBlasterUnlocked = false;
        isScatterUnlocked = false;
        isAutoUnlocked = false;
        isShieldUnlocked = false;
        isEmpExplosionUnlocked = false;
		isGLUnlocked = false; 
        respawnPoint = startingSpawn.transform;
        highScore = 0;
		HUD.DoWin (); 
    }

    public void OpenDoor(int _roomNumber)
    {
        HUD.RemoveObjective(BR_Objectives.ObjectiveType.PICKUP);
        HUD.AddObjective(BR_Objectives.ObjectiveType.REACH, spawner.rooms[spawner.curRoom].roomName.ToLower());
        rooms[_roomNumber].OpenDoor();
    }

    public void AllowHacking(int _roomNumber)
    {
        HUD.RemoveObjective(BR_Objectives.ObjectiveType.PICKUP);
        if(canTriggerHackTutorial && showTutorial)
        {
            HUD.DoTutorialScreen("Walk up to the door panel and hold\n{0}{1}</color> to Hack the door open", "Interact", "X Button", "F Key");
            HUD.TutorialHackingScreen();
            canTriggerHackTutorial = false;
        }
        HUD.AddObjective(BR_Objectives.ObjectiveType.HACK,"door");
        rooms[_roomNumber].hackingVolume.allowHacking = true;
    }

    void GrenadeLauncherInput()
    {
		if (cur_SuperCooldown < superCooldown && !player.isSuperActive) {
			cur_SuperCooldown += Time.deltaTime;
			HUD.grenadeLauncherFillBar.color = HUD.regenColor; 
		} else {
			HUD.grenadeLauncherFillBar.color = HUD.grenadeLauncherColor; 
			HUD.DoSuperLerp (); 
		}

        if (Input.GetButtonDown("Super"))
        {
            if (!player.isEmpExplosionActive && !player.isEmpDashActive && !isChronoWatchActive && !player.isSuperActive && cur_SuperCooldown >= superCooldown)
            {
                LastWeapon = CurrentWeapon;
                CurrentWeapon = WEAPON.NONE;
                player.StartSuper();

                cur_SuperCooldown = 0;
            }
        }
    }
}

