using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Hacking : MonoBehaviour {
    public bool allowHacking;
    public bool hackingComplete;
    bool isHacking;
    public float maxHackingDuration= 3f;
    public float curHacking;
    private bool playerInVolume;
    public  GameObject directionLight;
    SP_HUD HUD;
    WaveSpawner spawner;

    private Color curColor; 

    // Use this for initialization
    void Start () {
        HUD = FindObjectOfType<SP_HUD>();
 
		HUD.HackingColor ();

        curColor = directionLight.GetComponent<Light>().color;
       
    }
	
	// Update is called once per frame
	void Update () {
        if (spawner == null)
            spawner = FindObjectOfType<WaveSpawner>();
        curHacking = Mathf.Clamp(curHacking, 0, maxHackingDuration);
        if (!isHacking && !hackingComplete && allowHacking)
            curHacking -= Time.deltaTime;
        if (allowHacking && !hackingComplete)
        {
            directionLight.SetActive(true);
            directionLight.GetComponent<Light>().color = Color.Lerp(curColor, Color.clear, Mathf.PingPong(Time.time, .5f));
        }

        // directionLight.enabled = true;

        else
            directionLight.GetComponent<Light>().color = Color.clear;

        if (hackingComplete)
        {
            directionLight.SetActive(true);
            directionLight.GetComponent<Light>().color = Color.green;
        }

        if (playerInVolume && allowHacking && !hackingComplete)
        {
			if (Input.GetButton ("Interact")) {
				curHacking += Time.deltaTime;
				isHacking = true;
				if (curHacking >= maxHackingDuration) {
					HUD.HackingCompleteColor (); 
					allowHacking = false;
					if (!spawner.rooms[spawner.curRoom].isRepairRoom)
						HUD.AddObjective(BR_Objectives.ObjectiveType.REACH, spawner.rooms[spawner.curRoom].roomName.ToLower());
                    hackingComplete = true;
                }
			}
            else
				isHacking = false;
			
        }
        else if (!hackingComplete)
        {
            curHacking -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider _target)
    {
        if (allowHacking && !hackingComplete && _target.tag == "Player")
        {
			HUD.HackingColor (); 
            HUD.curHackingVol = gameObject.GetComponent<BR_Hacking>();
            playerInVolume = true;
        }
    }
    /*
    void OnTriggerStay(Collider _target)
    {
        if (allowHacking && !hackingComplete && _target.tag == "Player")
        {
            if (Input.GetButton("Interact"))
            {
                curHacking += Time.deltaTime;
                isHacking = true;
                if (curHacking >= maxHackingDuration)
                {
                    hackingComplete = true;
                    allowHacking = false;
                }
            }
            else
                isHacking = false;
        }
        else if (!hackingComplete)
        {
            curHacking -= Time.deltaTime;
        }
    }
    */
    void OnTriggerExit(Collider _target)
    {
        if (allowHacking && !hackingComplete && _target.tag == "Player")
        {
            isHacking = false;
            HUD.curHackingVol = null;
            playerInVolume = false;
        }
    }

    public void ResetHack()
    {
		HUD.HackingColor (); 
        hackingComplete = false;
        isHacking = false;
        allowHacking = false;
        curHacking = 0;

    }
}
