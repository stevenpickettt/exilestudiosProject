using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
    public enum SpawnState { SPAWNING, WAITING, COUNTING, DONE };
    public enum ThrustWaveInfo { NONE, ThrusterA, ThrusterB, EndBoth,EndThrusterA,EndThrusterB, Both};
    public enum TutorialWave { NO, YES, END};
    public BR_GameController.GameDifficulty waveDifficulty;

    AudioSource audiosource;
    AudioController audioController;

    [SerializeField]
    private AudioClip[] waveSpawnerStartSFXs;


    [System.Serializable]
    public class ChallengeSetup
    {
        public BR_Challenge.ChallengeType challengeType;
        public int count;
        public BR_Challenge.ChallengeReward rewardType;
    }
    [System.Serializable]
    public class enemySetup
    {
        public GameObject enemy;
        public int cost;
        public float chanceToSpawn;
    }
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public float waveCountdownTime = 5f;
        public int waveAllowance;
        public int aliveValue;
        public enemySetup[] enemyList;
        public GameObject[] Bosses;
        public bool newRoom;
        public bool rewardWave;
        public bool forceSpawn;
        public bool returnShipItem;
        public BR_Challenge.ChallengeType challengeType;
        public int count;
        public BR_Challenge.ChallengeReward rewardType;
        public ThrustWaveInfo thrusterWave = ThrustWaveInfo.NONE;
        public TutorialWave tutorialWave = TutorialWave.NO;
    }

    //public Text waveUI;
    bool canAssignChallenges = false;
    public GameObject[] aliveEnemies;
    public bool onFinalCount;

    public Wave[] waves;

    public BR_RoomScript[] rooms;

    public ChallengeSetup[] Challenges;
    public int curWave = 0;
    public int actualWave;
    int checkpointActualWave = 1;
    public int checkpointWave;
    int deathWave =-1;
    public int aliveCost;
    private int count;
    private float randomFloat;
    private bool isSpawningComplete = false;
    public bool isRewardAvailable = false;
    private bool canUpdateThursterObjective = true ;
    private bool canAddTutorialObjective = true;
    

    private bool forceSpawnCurWave = false;

    float actionTimer;
    float checkValue;
    public int curSelection;

    public int curRoom = 0;
    private int curWavePrice;
    private int usedPoints;
    public bool isBossWave;
    private bool isBossAlive;
    public bool wasBossSpawned;

    public float waveCountdown;
    private bool challengeSet = false;
    bool debugSetActive = false;

    public SpawnState state = SpawnState.WAITING;

    BR_GameController gameController;

    BR_ObjectivePanel objPanel;

    [SerializeField]
    GameObject keyPrefab;
    [SerializeField]
    GameObject hackPrefab;
    [SerializeField]
    public GameObject[] rewardPrefabs;
    public int curReward;
    Vector3 lastEnemyLoc;
    GameObject player;
    GameObject reward;
    ShipScript ship;
    void Start()
    {
        actualWave = 1;
        
        gameController = FindObjectOfType<BR_GameController>();
        objPanel = FindObjectOfType<BR_ObjectivePanel>();
        waveCountdown = waves[0].waveCountdownTime;
        curWavePrice = waves[0].waveAllowance;

        audioController = FindObjectOfType<AudioController>();
        audiosource = GetComponent<AudioSource>();
        ship = FindObjectOfType<ShipScript>();
        ship.waveSpawer = gameObject.GetComponent<WaveSpawner>();
        if (waves[curWave].forceSpawn)
        {
            forceSpawnCurWave = true;
            waveCountdown = 0;
        }
        
    }

    void Update()
    {
		gameController.HUD.waveSystem.text = "Wave: " + WaveSystemTextPct (); 
        if (actionTimer <= 0)
        {
            switch (state)
            {
                case SpawnState.DONE:
                    break;

                case SpawnState.WAITING:
                    Waiting(waves[curWave]);
                    break;

                case SpawnState.SPAWNING:
                    WaveSpawn(waves[curWave]);
                    break;

                case SpawnState.COUNTING:
                    EnemyCheck(waves[curWave]);
                    break;
            }
            
        }
        else
        {
            if (gameController.isChronoWatchActive)
                actionTimer -= Time.deltaTime * 4;
            else
                actionTimer -= Time.deltaTime;
        }
        if (isBossAlive)
            BossCheck();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N)&& rooms[curRoom].hasPlayerEntered)
        {
            aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy"); 
            for(int x = 0; x < aliveEnemies.Length; x++)
            {
                Destroy(aliveEnemies[x]);
            }
            lastEnemyLoc = player.transform.position;

            WaveCompleted();
            gameController.HUD.objectivePanel.ForceRemoveObjectives();

            //HUD.objectivePanel.ForceRemoveObjectives();
        }
    }

	public float WaveSystemTextPct(){
		return(float)actualWave; 
	}

    void Waiting(Wave _wave)
    {
        if (gameController.rooms[curRoom].hasPlayerEntered)
        {
            if (!isRewardAvailable)
            {
                if (waveCountdown <= 8.5f && _wave.tutorialWave != TutorialWave.NO && canAddTutorialObjective && gameController.showTutorial)
                {
                    gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.TUTORIAL, "tutorial");
                    canAddTutorialObjective = false;
                }
                if (_wave.thrusterWave != ThrustWaveInfo.NONE && canUpdateThursterObjective)
                {
                    switch (_wave.thrusterWave)
                    {
                        case ThrustWaveInfo.ThrusterA:
                            gameController.HUD.SetThrusterHUD_A(true);
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "the first Thruster!");
                            if (gameController.showTutorial)
                            {
                                gameController.HUD.DoTutorialScreen("You need that Thruster! Protect it from the enemies\n\n Press {0}{1}</color> to continue", "Interact", "X Button", "F Key");
							gameController.HUD.TutorialThrusterRoomScreen();
                            }
                            canUpdateThursterObjective = false;
                            break;

                        case ThrustWaveInfo.EndThrusterA:
                            gameController.HUD.SetThrusterHUD_A(true);
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "the first Thruster!");
                            canUpdateThursterObjective = false;
                            break;

                        case ThrustWaveInfo.ThrusterB:
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "the second Thruster!");
                            gameController.HUD.SetThursterHUD_B(true);
                            canUpdateThursterObjective = false;
                            break;

                        case ThrustWaveInfo.EndThrusterB:
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "the second Thruster!");
                            gameController.HUD.SetThursterHUD_B(true);
                            canUpdateThursterObjective = false;
                            break;

                        case ThrustWaveInfo.Both:
                            gameController.HUD.SetThrusterHUD_A(true);
                            gameController.HUD.SetThursterHUD_B(true);
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "BOTH Thrusters!");
                            canUpdateThursterObjective = false;
                            break;

                        case ThrustWaveInfo.EndBoth:
                            gameController.HUD.SetThrusterHUD_A(true);
                            gameController.HUD.SetThursterHUD_B(true);
                            gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PROTECT, "BOTH Thrusters!");
                            canUpdateThursterObjective = false;
                            break;
                    }
                }
                if (waveCountdown <= 0)
                {
                    gameController.HUD.RemoveChallenges();
                    gameController.HUD.waveUI.text = null;
                    gameController.HUD.waveUI.color = Color.white;
                    gameController.HUD.challengeScore.text = null;
                    gameController.HUD.challengeScore.enabled = false;
                    // insert audio function here 
                    if (waves[curWave].waveName != " ")
                        audioController.PlayRandomWaveStartSFX();

                    state = SpawnState.SPAWNING;
                }
                else
                {
                    if (isBossWave)
                    {
                        gameController.HUD.waveUI.text = "Boss incoming in " + Mathf.RoundToInt(waveCountdown) + " seconds";
                        gameController.HUD.waveUI.color = Color.red;
                    }
                    else
                        gameController.HUD.waveUI.text = "Prepare for enemies in " + Mathf.RoundToInt(waveCountdown) + " seconds";
                    //Debug.Log("Starting " + waves[curWave].waveName + " in " + Mathf.RoundToInt(waveCountdown) + " seconds");
                    waveCountdown -= Time.deltaTime;
                }
            }
            else
                Debug.Log("Reward is available");
        }
        else
        {
            gameController.HUD.waveUI.text = null;
            Debug.Log("Player has not entered room"+ rooms[curRoom].roomName.ToLower()+ " # "+ curRoom);
        }
    }



    void WaveSpawn(Wave _wave)
    {
        if (!challengeSet && _wave.challengeType != BR_Challenge.ChallengeType.NONE && curWave >= deathWave)
        {
            switch (_wave.challengeType)
            {
                default:
                    gameController.HUD.AddChallenge(_wave.challengeType, _wave.count, _wave.rewardType);
                    break;

                case BR_Challenge.ChallengeType.RANDOM:
                    int rand = Random.Range(0, Challenges.Length);
                    gameController.HUD.AddChallenge(Challenges[rand].challengeType, Challenges[rand].count, Challenges[rand].rewardType);
                    break;
            }
            challengeSet = true;
        }
        
        Debug.Log("Spawning Wave: " + _wave.waveName);
        BossSpawn(_wave);
       
        if (forceSpawnCurWave)
            ForceSpawn(_wave);
        else
            NormalSpawn(_wave);

    }

    void ForceSpawn(Wave _wave)
    {
        if (forceSpawnCurWave)
        {
            
                
            GameObject[] UseSP = rooms[curRoom].spawnPoints;
            for (int FS = 0; FS < _wave.enemyList.Length; FS++)
            {
                SpawnEnemy(_wave.enemyList[FS].enemy, UseSP[FS], 0);
            }
            state = SpawnState.COUNTING;
        }
    }

    void BossSpawn(Wave _wave)
    {
        if (_wave.Bosses.Length != 0)
        {
            if (!wasBossSpawned)
            {
                int y = 0;
                GameObject[] UseSP = rooms[curRoom].spawnPoints;
                for (int x = 0; y < _wave.Bosses.Length;x++)
                {
                    if (UseSP[x].GetComponent<BR_Spawnpoint>().isValid)
                    {
                        SpawnBoss(_wave.Bosses[y], UseSP[x], y);
                        y++;
                    }
                }
                string _target = _wave.Bosses[0].name;
                AIAgent.ENEMYTYPE _curEnemyType = gameController.HUD.BossSlot[0].GetComponent<AIAgent>().EnemyType;
                switch (_curEnemyType)
                {
                    default:
                        gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.KILL, _target);
                        break;

                    case AIAgent.ENEMYTYPE.TRILOGYEXO:
                        gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.KILL, _target + "s");
                        break;
                }
                isBossAlive = true;
                wasBossSpawned = true;
                
            }
        }
    }

    void NormalSpawn(Wave _wave)
    {
        
        if (usedPoints < curWavePrice)
        {
            checkValue = 0;
            aliveCost = 0;
            if (count <= 0 || count > 4)
            {
                float randfloat = Random.value;
                randomFloat = randfloat;
                count = 0;
            }

            
            for (int curEnemy = 0; curEnemy < _wave.enemyList.Length; curEnemy++)
            {
                float curEnemy_chanceToSpawn = _wave.enemyList[curEnemy].chanceToSpawn / 100;
                if (randomFloat >= checkValue && randomFloat < curEnemy_chanceToSpawn + checkValue)
                {
                    curSelection = curEnemy;
                    
                }
                checkValue += curEnemy_chanceToSpawn;
            }
           // Debug.Log("Random Float was " + randomFloat + " CurSelection = " + curSelection);
            BR_EnemyCost[] aliveEnemyCost = FindObjectsOfType<BR_EnemyCost>();
            for (int x = 0; x < aliveEnemyCost.Length; x++)
            {
                aliveCost += aliveEnemyCost[x].enemyCost;
            }
            AIAgent.ENEMYTYPE curCheck = _wave.enemyList[curSelection].enemy.GetComponent<AIAgent>().EnemyType;
            int typeAlive = 0;
            bool allowSpawn = false;
            aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int x = 0; x < aliveEnemies.Length; x++)
            {
                if (aliveEnemies[x].GetComponent<AIAgent>().EnemyType == curCheck)
                    typeAlive++;
            }
            switch (curCheck)
            {
                default:
                    if (typeAlive < 2)
                        allowSpawn = true;
                    break;

                case AIAgent.ENEMYTYPE.DEFAULT:
                    if (typeAlive < 10)
                        allowSpawn = true;
                    break;
            }
            if (aliveCost + _wave.enemyList[curSelection].cost > _wave.aliveValue || !allowSpawn)
            {
                Debug.Log(_wave.waveName + ": Tried to spawn " + _wave.enemyList[curSelection].enemy.name + " failed: Too Expensive-" + _wave.enemyList[curSelection].cost);
                Debug.Log(_wave.waveName + ": current alive cost=" + aliveCost + " wave alive cost" + _wave.aliveValue);
                count++;
                actionTimer = .5f;
            }
            else 
            {
                GameObject[] curSpawnPoints;
                if (_wave.thrusterWave == ThrustWaveInfo.Both || _wave.thrusterWave == ThrustWaveInfo.EndBoth)
                {
                    curSpawnPoints = new GameObject[rooms[curRoom].spawnPoints.Length + rooms[curRoom - 1].spawnPoints.Length];
                    rooms[curRoom].spawnPoints.CopyTo(curSpawnPoints, 0);
                    rooms[curRoom - 1].spawnPoints.CopyTo(curSpawnPoints, rooms[curRoom].spawnPoints.Length);
                }
                else
                    curSpawnPoints = rooms[curRoom].spawnPoints;

                GameObject curSP = curSpawnPoints[Random.Range(0, curSpawnPoints.Length)];
                if (curSP.GetComponent<BR_Spawnpoint>().isValid)
                {
                    SpawnEnemy(_wave.enemyList[curSelection].enemy, curSP, _wave.enemyList[curSelection].cost);
                    usedPoints += _wave.enemyList[curSelection].cost;
                    actionTimer = 1f;
                    count = 0;
                }
                else
                {
                    Debug.Log("Spawnpoint " + curSP.name + " is inValid");
                    actionTimer = .5f;
                }
            }
            
        }
        else
        {
            Debug.Log(_wave.waveName + " Spawning Complete");
            state = SpawnState.COUNTING;
        }
    }

    void BossCheck()
    {
        if (gameController.isPlayerAlive)
        {
            if (gameController.HUD.BossSlot[0] == null && gameController.HUD.BossSlot[1] == null && gameController.HUD.BossSlot[2] == null)
            {
                Debug.Log("All Bosses Killed");
                gameController.HUD.RemoveObjective(BR_Objectives.ObjectiveType.KILL);
                gameController.HUD.CheckBossChallenge();
                for (int x = 0; x < rooms[curRoom].teleportPoints.Length; x++)
                {
                    rooms[curRoom].teleportPoints[x].GetComponent<BR_Spawnpoint>().ChangeCompLightColor();
                }
                state = SpawnState.COUNTING;
                isBossAlive = false;
            }
        }
    }

    void EnemyCheck(Wave _wave)
    {
        aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (aliveEnemies.Length <= 5 && gameController.isPlayerAlive)
        {
            for (int x = 0; x < aliveEnemies.Length;x++)
            {
                gameController.HUD.trackingSlot[x] = aliveEnemies[x];
            }
            switch (aliveEnemies.Length)
            {
                default:
                    gameController.HUD.waveUI.text = aliveEnemies.Length + " enemies remaining";
                    lastEnemyLoc = aliveEnemies[0].transform.position;
                    actionTimer = .5f;
                    break;

                case 1:
                    gameController.HUD.waveUI.text = aliveEnemies.Length + " enemy remains";
                    if (waves[curWave].newRoom || waves[curWave].rewardWave)
                    {
                        lastEnemyLoc = aliveEnemies[0].transform.position;
                        aliveEnemies[0].GetComponent<BR_EnemyHealth>().canDropLoot = false;
                    }
                        actionTimer = .5f;
                    break;

                case 0:
                    gameController.HUD.trackingSlot = new GameObject[5];
                    gameController.HUD.waveUI.text = null;
                    WaveCompleted();
                    break;
            }
        }
        else
        {
            gameController.HUD.trackingSlot = new GameObject[5];
            onFinalCount = false;
            gameController.HUD.waveUI.text = null;
            Debug.Log(_wave.waveName + " Enemies Remaining: " + aliveEnemies.Length);
            actionTimer = 1f;
        }
    }

    void WaveCompleted()
    {
        state = SpawnState.DONE;
        if (waves[curWave].tutorialWave == TutorialWave.END)
            gameController.HUD.RemoveObjective(BR_Objectives.ObjectiveType.TUTORIAL);
        challengeSet = false;
        gameController.HUD.EndWaveHealthCheck();
        if (!waves[curWave].forceSpawn)
            actualWave++;
        switch (waves[curWave].thrusterWave)
        {
            case ThrustWaveInfo.EndThrusterA:
                gameController.HUD.RemoveObjective(BR_Objectives.ObjectiveType.PROTECT);
                gameController.HUD.SetThrusterHUD_A(false);
                canUpdateThursterObjective = true;
                SP_Thrusters[] thrusters = FindObjectsOfType<SP_Thrusters>();
                for (int x = 0; x < thrusters.Length; x++)
                {
                    if(thrusters[x].CurrentThruster == SP_Thrusters.THRUSTERTYPE.THRUSTER_A)
                    thrusters[x].SpawnPickup();
                }
                gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.THRUSTER, "thruster");
                break;

            case ThrustWaveInfo.EndThrusterB:
                gameController.HUD.RemoveObjective(BR_Objectives.ObjectiveType.PROTECT);
                gameController.HUD.SetThursterHUD_B(false);
                canUpdateThursterObjective = true;
                SP_Thrusters[] thruster = FindObjectsOfType<SP_Thrusters>();
                for (int x = 0; x < thruster.Length; x++)
                {
                    if (thruster[x].CurrentThruster == SP_Thrusters.THRUSTERTYPE.THRUSTER_B)
                        thruster[x].SpawnPickup();
                }
                gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.THRUSTER, "thruster");
                break;

            case ThrustWaveInfo.EndBoth:
                gameController.HUD.SetThrusterHUD_A(false);
                gameController.HUD.SetThursterHUD_B(false);
                gameController.HUD.RemoveObjective(BR_Objectives.ObjectiveType.PROTECT);
                SP_Thrusters[] thrustersB = FindObjectsOfType<SP_Thrusters>();
                for (int x = 0; x < thrustersB.Length; x++)
                {
                    thrustersB[x].SpawnPickup();
                }
                break;
        }
        if (curWave + 1 != waves.Length)
        {
            Debug.Log(waves[curWave].waveName + " Completed!");
            isSpawningComplete = false;
            if (waves[curWave].rewardWave && curWave >= deathWave)
            {
                reward = Instantiate(rewardPrefabs[curReward], lastEnemyLoc, new Quaternion(0, 180, 0, 0));
                gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PICKUP, reward.GetComponent<DJ_PickUp>().subTypeName.ToString());
                curReward++;
                isRewardAvailable = true;
            }
            if (waves[curWave].returnShipItem)
                isRewardAvailable = true;
            
            if (waves[curWave].newRoom)
            {
                if (debugSetActive)
                {
                    rooms[curRoom].OpenDoor();
                }
                else if (rooms[curRoom].curDoorType == BR_RoomScript.doorType.Hack || rooms[curRoom].curDoorType == BR_RoomScript.doorType.Key)
                {
                    GameObject drop;
                    string dropString;
                    switch (rooms[curRoom].curDoorType)
                    {
                        case BR_RoomScript.doorType.Hack:
                            drop = hackPrefab;
                            dropString = "code card";
                            break;

                        default:
                            drop = keyPrefab;
                            dropString = "Key";
                            break;
                    }
                    GameObject dropItem = Instantiate(drop, lastEnemyLoc, new Quaternion(0, 180, 0, 0));
                    gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.PICKUP, dropString);
                    dropItem.GetComponent<DJ_PickUp>().amount = curRoom;
                }
                else if (rooms[curRoom].curDoorType == BR_RoomScript.doorType.WaveComplete || rooms[curRoom].curDoorType == BR_RoomScript.doorType.Exit)
                {
                    if (rooms[curRoom].curDoorType == BR_RoomScript.doorType.WaveComplete)
                        rooms[curRoom].OpenDoor();
                }
                GameObject friendlyTurret = GameObject.FindGameObjectWithTag("FriendlyTurret");
                if (friendlyTurret != null)
                {
                    if (friendlyTurret.GetComponent<BR_Friendly>().isActive)
                        gameController.HUD.objectivePanel.CompleteObjective(BR_Objectives.ObjectiveType.PROTECT);
                    else
                        gameController.HUD.objectivePanel.FailObjective(BR_Objectives.ObjectiveType.ACTIVATE);
                    friendlyTurret.GetComponent<BR_Friendly>().done = true;
                }
                curRoom++;
                checkpointActualWave = actualWave;
                checkpointWave = curWave +1;
                Debug.Log("CurRoom = " + curRoom);
            }

            if (curWave + 1 == waves.Length)
                state = SpawnState.DONE;
            else
                curWave++;
            Wave nextwave = waves[curWave];
            if (isBossWave)
            {
                Debug.Log("Resetting Boss Wave");
                isBossWave = false;
                wasBossSpawned = false;
            }
            waveCountdown = waves[curWave].waveCountdownTime;
            usedPoints = 0;
            if (nextwave.forceSpawn)
            {
                forceSpawnCurWave = true;
                curWavePrice = 0;
            }
            else
            {
                forceSpawnCurWave = false;
                curWavePrice = waves[curWave].waveAllowance;
            }
            
            if (nextwave.Bosses.Length != 0)
                isBossWave = true;
            if (debugSetActive)
            {
                if(reward != null)
                Destroy(reward);
            }
            state = SpawnState.WAITING;
        }
    }

    void SpawnEnemy(GameObject _enemy, GameObject _sp, int _cost)
	{
        Debug.Log(waves[curWave].waveName +" Spawning Enemy: " + _enemy.name + " at SP: " + _sp.name);
        GameObject enemy = Instantiate(_enemy, _sp.transform.position, _enemy.transform.localRotation);
        if (enemy.gameObject.GetComponent<BR_EnemyCost>() != null)
            enemy.gameObject.GetComponent<BR_EnemyCost>().enemyCost = _cost;
    }

    void SpawnBoss(GameObject _enemy, GameObject _sp, int _bossSlot)
    {
        Debug.Log(waves[curWave].waveName + " Spawning Enemy: " + _enemy.name + " at SP: " + _sp.name);
        GameObject enemy = Instantiate(_enemy, _sp.transform.position, new Quaternion(0, Random.Range(0, 359), 0, 0));
        enemy.GetComponent<BR_EnemyHealth>().bossSlot = _bossSlot;
        if (enemy.gameObject.GetComponent<DJ_Teleport>() != null)
            enemy.gameObject.GetComponent<DJ_Teleport>().TeleportSpawns = rooms[curRoom].teleportPoints;
        gameController.HUD.BossSlot[_bossSlot] = enemy;
        gameController.HUD.trackingSlot[_bossSlot] = enemy;
        if (enemy.GetComponent<AIAgent>().EnemyType == AIAgent.ENEMYTYPE.BOSS2)
        {
            DJ_PowerPanel[] powerPanels = FindObjectsOfType<DJ_PowerPanel>();
            for (int x = 0; x < powerPanels.Length; x++)
            {
                powerPanels[x].SetActive(enemy);
            } 
        }
    }

    public void CheckpointWaves()
    {
        state = SpawnState.DONE;
        if (curWave > deathWave)
            deathWave = curWave;
        actualWave = checkpointActualWave;
        curWave = checkpointWave;
        rooms[curRoom].hasPlayerEntered = false;

        Reset();
    }

    public void ResetSpawner()
    {

        state = SpawnState.DONE;
        
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        gameController.PlayerWin();
        curWave = 0;
        curRoom = 0;
        curReward = 0;
        checkpointWave = 0;
        actualWave = 1;
        for (int room = 0; room < rooms.Length; room++)
        {
            rooms[room].ResetDoor();
            
        }
        Reset();
    }

    void Reset()
    {
        gameController.HUD.trackingSlot = new GameObject[5];
        isBossWave = false;
        isSpawningComplete = false;
        isBossAlive = false;
        wasBossSpawned = false;
        waveCountdown = waves[curWave].waveCountdownTime;
        usedPoints = 0;
        isRewardAvailable = false;
        canUpdateThursterObjective = true;
        SP_DestoryObject[] destroys = FindObjectsOfType<SP_DestoryObject>();
        for (int volumes = 0; volumes < destroys.Length; volumes++)
        {
            Destroy(destroys[volumes].gameObject);
        }
        GameObject[] killAll = GameObject.FindGameObjectsWithTag("Enemy");
        for (int ai = 0; ai < killAll.Length; ai++)
        {
            Destroy(killAll[ai]);
        }
        GameObject[] killPickUps = GameObject.FindGameObjectsWithTag("PickUp");
        for (int pu = 0; pu < killPickUps.Length; pu++)
        {
            if (killPickUps[pu].GetComponent<DJ_PickUp>().subTypeName != DJ_PickUp.SUBTYPE.RegenVol)
            Destroy(killPickUps[pu]);
        }
        if (waves[curWave].forceSpawn)
        {
            forceSpawnCurWave = true;
            curWavePrice = 0;
            waveCountdown = 0;
        }
        else
            curWavePrice = waves[curWave].waveAllowance;
        state = SpawnState.WAITING;

        BR_Spawnpoint[] spawnpoints = FindObjectsOfType<BR_Spawnpoint>();

        for (int x = 0; x < spawnpoints.Length; x++)
        {
            spawnpoints[x].TurnOffLights();
        }
        DJ_PowerPanel[] powerPanels = FindObjectsOfType<DJ_PowerPanel>();
        for (int x = 0; x < powerPanels.Length; x++)
        {
            powerPanels[x].HealthBar.SetActive(false);
            powerPanels[x].isAlive = false;
            powerPanels[x].powerPanelLight.SetActive(false);
            powerPanels[x].isDamagable = false;
            powerPanels[x].isPulsing = false;
            powerPanels[x].destoryedPS.SetActive(false);
            powerPanels[x].curHealth = powerPanels[x].maxHealth;
        }
        Destroy(GameObject.FindGameObjectWithTag("FriendlyTurret"));
        gameController.HUD.SetTurretHUD(false);
        gameController.HUD.turretPanel.SetActive(false);
        
    }
    bool allowSpawn(AIAgent.ENEMYTYPE _enemyType)
    {
        int typeAlive = 0;
        for (int x = 0; x < aliveEnemies.Length; x++)
        {
            if (aliveEnemies[x].GetComponent<AIAgent>().EnemyType == _enemyType)
                typeAlive++;
        }
        switch(_enemyType)
        {
            default:
                switch(waveDifficulty)
                {
                    case BR_GameController.GameDifficulty.HARD:
                        if (typeAlive >= 3)
                            return false;
                        else
                            return true;

                    default:
                        if (typeAlive >= 2)
                            return false;
                        else
                            return true;
                }

            case AIAgent.ENEMYTYPE.DEFAULT:
                return true;
        }
    }

    public void DebugWaveSet(int _targetRoom)
    {
        player.GetComponent<PlayerController>().TeleportPlayer(rooms[_targetRoom].RespawnPoint.transform.position);
        debugSetActive = true;
        if(_targetRoom >0)
        {
            gameController.UnlockBlaster();
            gameController.UnlockAuto();
            gameController.curAuto_Ammo += 125;
            gameController.HUD.UpdateAutoAmmoAmount();
            gameController.TurnOnShild();
            player.GetComponent<PlayerController>().AddShield(player.GetComponent<PlayerController>().maxShield);
            
        }
        if (_targetRoom > 1)
        {
            gameController.UnlockChronoWatch();
        }
        if (_targetRoom > 2)
        {
            gameController.UnlockEmpExplosion();
            gameController.ship.PlayerHasThrusterA = true;
        }
        if (_targetRoom > 4)
        {
            gameController.UnlockScatter();
            gameController.curScatter_Ammo += 75;
            gameController.HUD.UpdateScatterAmmoAmount();
            gameController.ship.PlayerHasThrusterB = true;
            
        }
        if (_targetRoom > 7)
            gameController.ship.PlayerHasShipEngine = true;
        for (int x = 0; curRoom< _targetRoom; x++)
        {
            WaveCompleted();
            gameController.HUD.objectivePanel.RemoveAllObjectives();
        }
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        for(int x = 0; x < pickups.Length; x++)
        {
            Destroy(pickups[x]);
        }
        isRewardAvailable = false;
        debugSetActive = false;
        gameController.HUD.RoomNameText.text = "";
        gameController.HUD.AddObjective(BR_Objectives.ObjectiveType.REACH, rooms[curRoom].GetComponent<BR_RoomScript>().roomName.ToLower());
        gameController.showTutorial = true;
    }
}
