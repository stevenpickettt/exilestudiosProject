using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour {
    private SP_HUD HUD;
    public WaveSpawner waveSpawer;
	public Animator ShipAnimation;
	AudioSource audiosource;
	AudioController audioController;
    

	public bool PlayerHasThrusterA;
	public bool PlayerHasThrusterB;
	public bool PlayerHasShipAi;
	public bool PlayerHasShipEngine;

	public GameObject ShipThrusterA;
	public GameObject ShipThrusterB;
	public GameObject ShipAi;
	public GameObject ShipEngine;
    public Transform playerLoc;

	public bool ShipIdle;
	public bool ShipTakeOff = false;
    [SerializeField]
    private float winTimer = 3.5f;
    private float curTimer;
	// Use this for initialization
	void Start () 
	{
        HUD = FindObjectOfType<SP_HUD>();
        
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
		ShipAnimation = GetComponent<Animator> ();
        curTimer = winTimer;
		
		//at the start of the game the player has not yer aquired these items.
		
        
    }
	
	// Update is called once per frame
	void Update () 
	{

        if (PlayerHasShipAi)

        {
            Time.timeScale = 1;
            if (!ShipTakeOff)
            ShipAnimation.SetBool("ShipTakeOff", true);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = playerLoc.position;
            player.GetComponent<MeshRenderer>().enabled = false;
            foreach (Transform child in player.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            player.GetComponent<PlayerController>().enabled = false;
            curTimer -= Time.deltaTime;
            if (curTimer <= 0)
                waveSpawer.ResetSpawner();
        }

        ShipThrusterAActive();
	}

	public void ShipThrusterAActive()
	{
		if (PlayerHasThrusterA == true) 
			ShipThrusterA.SetActive (true);
		
		else 
			ShipThrusterA.SetActive (false);

		if (PlayerHasThrusterB== true) 
			ShipThrusterB.SetActive (true);

		else 
			ShipThrusterB.SetActive (false);

		if (PlayerHasShipAi== true) 
			ShipAi.SetActive (true);

		else 
			ShipAi.SetActive (false);

		if (PlayerHasShipEngine == true) 
			ShipEngine.SetActive (true);

		else 
			ShipEngine.SetActive (false);

	}

    void OnTriggerEnter(Collider _target)
    {
        if (_target.tag == "Player")
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            PlayerController player = playerGO.GetComponent<PlayerController>();

            if (player.pickUpSlot != null)
            {
                DJ_PickUp carryItem = player.pickUpSlot.GetComponent<DJ_PickUp>();
                HUD.RemoveObjective(BR_Objectives.ObjectiveType.CARRY);
                switch (carryItem.subTypeName)
                {
                    case DJ_PickUp.SUBTYPE.Engine:
                        PlayerHasShipEngine = true;
                        HUD.AddObjective(BR_Objectives.ObjectiveType.RETURN, waveSpawer.rooms[waveSpawer.curRoom].roomName.ToLower());
                        break;

                    case DJ_PickUp.SUBTYPE.Thruster:
                        if (carryItem.amount == 0)
                            PlayerHasThrusterA = true;
                        else
                            PlayerHasThrusterB = true;
                        if (carryItem.amount == 0)
                            HUD.AddObjective(BR_Objectives.ObjectiveType.REACH, waveSpawer.rooms[waveSpawer.curRoom].roomName.ToLower());
                        else
                            HUD.AddObjective(BR_Objectives.ObjectiveType.RETURN, waveSpawer.rooms[waveSpawer.curRoom].roomName.ToLower());
                        break;

                    case DJ_PickUp.SUBTYPE.AIController:
                        PlayerHasShipAi = true;
                        break;
                }
                waveSpawer.isRewardAvailable = false;
                Destroy(player.pickUpSlot);
                player.pickUpSlot = null;
            }
        }
    }
    public void ResetShip()
    {
        curTimer = winTimer;
        PlayerHasShipEngine = false;
        PlayerHasThrusterA = false;
        PlayerHasThrusterB = false;
        PlayerHasShipAi = false;
        ShipAnimation.SetBool("ShipTakeOff", false);
    }

}
