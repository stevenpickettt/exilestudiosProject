using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_PlayerShooting : MonoBehaviour
{
    private bool isFiring;

    [Header ("The Normal Bullet firing:")]
    public KW_PlayerProjectile normalBullet;

    public float normal_TimeBetweenShots;
    public float normal_ShotCounter;


    [Header ("The Auto Bullet firing:")]

    public KW_PlayerProjectile autoBullet;

    public float autoShot_TimeBetweenShots;
    public float autoShot_shotCounter;

    [Header("The ScattershotBullet firing:")]

    public KW_PlayerProjectile scatterBullet;

    public float scatterShot_TimeBetweenShots;
    public float scatterShot_shotCounter;

    [Header ("Where does the bullet comes from:")]
    public Transform firePoint;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

            FireNormalBullet();

        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
        }
	}

    void FireNormalBullet()
    {

        if (isFiring == true)
        {
            normal_ShotCounter -= Time.deltaTime;
            if (normal_ShotCounter <= 0)
            {
                normal_ShotCounter = normal_TimeBetweenShots;
                KW_PlayerProjectile newBullet = Instantiate(normalBullet, firePoint.position, firePoint.rotation) as KW_PlayerProjectile;
            }
        }
        else
        {
                normal_ShotCounter = 0;
        }
    }

    void FireAutoBullet()
    {

        if (isFiring == true)
        {
            autoShot_shotCounter -= Time.deltaTime;
            if (autoShot_shotCounter <= 0)
            {
                autoShot_shotCounter = autoShot_TimeBetweenShots;
                KW_PlayerProjectile newBullet = Instantiate(autoBullet, firePoint.position, firePoint.rotation) as KW_PlayerProjectile;
            }
        }
        else
        {
            autoShot_shotCounter = 0;
        }
    }

    void FireScatterShotBullet()
    {

        if (isFiring == true)
        {
            scatterShot_shotCounter -= Time.deltaTime;
            if (scatterShot_shotCounter <= 0)
            {
                scatterShot_shotCounter = scatterShot_TimeBetweenShots;
                KW_PlayerProjectile newBullet = Instantiate(scatterBullet, firePoint.position, firePoint.rotation) as KW_PlayerProjectile;
            }
        }
        else
        {
            scatterShot_shotCounter = 0;
        }
    }
}
