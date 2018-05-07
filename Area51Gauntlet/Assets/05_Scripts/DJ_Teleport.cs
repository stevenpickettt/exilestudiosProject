using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_Teleport : MonoBehaviour {


    public GameObject[] TeleportSpawns;
    public float teleportTimer = 5;
    public float curTimer;

    public GameObject TeleportSpawnPS;

    private int lastTPSpawn;

    public int selection;

    AudioSource audiosource;
    AudioController audioController;

    // Use this for initialization
    void Start()
    {

        curTimer = teleportTimer;
        audioController = FindObjectOfType<AudioController>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        StartTeleportTimer();

    }
    void StartTeleportTimer()
    {
        // start the timer 
        curTimer -= Time.deltaTime;

        //if timer is less that or equal to zero...
        if (curTimer <= 0)
        {
            //set array to random int value 
            selection = Random.Range(0, TeleportSpawns.Length);
            //PLAY TELEPORT CLIP 
            audioController.Boss1_TeleportSFX();
            //play teleport particle system
            SpawnInPS();
            

            if (TeleportSpawns[selection].GetComponent<BR_Spawnpoint>().isValid && selection != lastTPSpawn)
            {
                //set object to new random postion
                gameObject.transform.position = TeleportSpawns[selection].transform.position;
                lastTPSpawn = selection;
                SpawnInPS();
                //reset the timer,
                curTimer = teleportTimer;
            }
        }

    }

    void SpawnInPS()
    {
        Instantiate(TeleportSpawnPS, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z), transform.rotation);
        
    }

}

