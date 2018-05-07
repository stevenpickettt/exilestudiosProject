using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_Laser : MonoBehaviour
{

    private PlayerController playerController;
    private LineRenderer lr;
    private BR_GameController gameController;

    public int laserDamage = 10;
	public float destroyTimer = 5;
    public GameObject LaserEffect;
	 

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        playerController = FindObjectOfType<PlayerController>();
        gameController = FindObjectOfType<BR_GameController>();

		Destroy (gameObject, destroyTimer); 
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
		 
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);


			if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }


            if(hit.collider.gameObject.tag == "Player")
            {
                Debug.Log(hit.transform.name);

                GameObject laserImpactGO =  Instantiate(LaserEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(laserImpactGO, .1f);

                if (playerController.GODMODE == false)
                {
                    if (playerController.curShield > 0)
                    {
                        lr.SetPosition(1, hit.point);
                        playerController.ModShield(laserDamage);
                        Debug.Log("We took damage from the laser script");
                    }
                    if (playerController.curShield <= 0)
                    {
                        lr.SetPosition(1, hit.point);
                        playerController.ModHealth(laserDamage);
                        Debug.Log("We took damage from the laser script");
                    }
                    if (playerController.curShield <= 0 && playerController.curHealth <= 0)
                    {
                        gameController.PlayerDeath();
                    }
                }


            }
        }
        else lr.SetPosition(1, transform.forward * 5000);
    }
}
	


