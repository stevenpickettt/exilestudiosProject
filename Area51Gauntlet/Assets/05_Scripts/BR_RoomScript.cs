using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_RoomScript : MonoBehaviour {
    public int roomNumber;
    public string roomName;
    public bool isRepairRoom = false;
    public GameObject RespawnPoint;
    public GameObject[] spawnPoints;
    public GameObject[] teleportPoints;
    public GameObject friendlyPoints;
    public bool hasPlayerEntered = false;
    [SerializeField]
    private bool showVolume = false;
    private BR_GameController gameController;
    KW_AnimationController anim;
    SP_HUD HUD;
    WaveSpawner waveSpawner;
    [SerializeField]
    GameObject nextRoom;
    [SerializeField]
    GameObject FriendlyTurret;

    public new enum friendlyTurret { NONE, Activate, Remove};
    public friendlyTurret friendlyState = friendlyTurret.NONE; 

    public new enum doorType { Hack, Key, Exit, WaveComplete};
    public doorType curDoorType = doorType.Key;
    public BR_Hacking hackingVolume;

    float textTimer= 2.5f;
    bool startTextTimer = false;

    void Start()
    {
        HUD = FindObjectOfType<SP_HUD>();
        anim = gameObject.GetComponent<KW_AnimationController>();
        gameController = FindObjectOfType<BR_GameController>();
        waveSpawner = gameController.spawner;
    }

    void Update()
    {
        if (waveSpawner == null)
            waveSpawner = gameController.spawner;
        if (curDoorType == doorType.Hack)
        {
            if (hackingVolume.hackingComplete && !anim.doorOpen)
            {
                OpenDoor();
            }
        }
        if (startTextTimer)
        {
            if (textTimer > 0)
            {
                textTimer -= Time.deltaTime;
            }
            else if (hasPlayerEntered)
            {
                HUD.RoomNameHeaderText.text = "";
                HUD.RoomNameText.text = roomName;
                startTextTimer = false;
            }
        }
    }

    void OnTriggerEnter(Collider target)
    {
        if (waveSpawner != null)
        {
            if (waveSpawner.curRoom == roomNumber && !gameController.DoDebug)
            {
                if (target.tag == "Player" && !hasPlayerEntered)
                {
                    GameObject aliveFriendly = GameObject.FindGameObjectWithTag("FriendlyTurret");
                    if (friendlyState == friendlyTurret.Activate && aliveFriendly == null)
                        Instantiate(FriendlyTurret, friendlyPoints.transform.position, FriendlyTurret.transform.localRotation);
                    if (roomNumber != 0 && !isRepairRoom)
                    {
                        HUD.RemoveObjective(BR_Objectives.ObjectiveType.REACH);
                        HUD.RemoveObjective(BR_Objectives.ObjectiveType.RETURN);
                    }
                    else if (roomNumber == 0)
                        HUD.objectivePanel.ForceRemoveObjectives();
                    
                    hasPlayerEntered = true;
                    Debug.Log("Player Entered " + gameObject.name);
                    gameController.HitCheckPoint(RespawnPoint.transform);
                }
                if (target.tag == "Player")
                {
                    HUD.RoomNameHeaderText.text = roomName;
                    startTextTimer = true;
                }
            }
        }
        else
            waveSpawner = gameController.spawner;
    }
    public void OpenDoor()
    {
        if (!anim.doorOpen)
        {
            if (curDoorType == doorType.Hack)
            {
                HUD.RemoveObjective(BR_Objectives.ObjectiveType.HACK);
            }
            anim.doorOpen = true;
            anim.OpenDoor();
        }
    }

    public void ResetDoor()
    {
        if (curDoorType == doorType.Hack)
            hackingVolume.ResetHack();
        hasPlayerEntered = false;
        anim.doorOpen = false;
        anim.RESETALLANIMATION();
    }

    void OnTriggerExit(Collider target)
    {
        if (curDoorType == doorType.Hack && anim.doorOpen)
        {
            if (target.tag == "Player" && hackingVolume.hackingComplete)
            {
                HUD.curHackingVol = null;
                if (friendlyState == friendlyTurret.Remove)
                {
                    GameObject friendlyTurret = GameObject.FindGameObjectWithTag("FriendlyTurret");
                    if (friendlyTurret != null)
                    { 
                        Destroy(friendlyTurret);
                        HUD.SetTurretHUD(false);
                    }
                }
            }
        }
            if (target.tag == "Player")
            {
                if (curDoorType == doorType.Exit && !anim.doorOpen)
                {
                    OpenDoor();
                }
                HUD.RoomNameText.text = "";
            if (nextRoom != null)
                nextRoom.SetActive(true);

            }
        }
}
