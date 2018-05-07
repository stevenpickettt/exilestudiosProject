using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_BreakableBox : MonoBehaviour {


    public bool canDropLoot = true;

    public GameObject[] PickupTypes;

    [Space]
    [Header("ForPickUpSpawning")]
    [Tooltip("50% = .5")]
    public float spawnPercentage;

    private BR_GameController gameController;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider target)
    {


        if (target.tag == "PlayerMelee")
        {

            SpawnPickup();

            Destroy(gameObject);

        }
    }

    void SpawnPickup()
    {
        float percentage = Random.value;
        if (percentage < spawnPercentage)
        {
            if (PickupTypes.Length != 0 && canDropLoot)
            {
                int randIndex = Random.Range(0, PickupTypes.Length);
                //Selects one pick up to spawn.
                switch (PickupTypes[randIndex].GetComponent<DJ_PickUp>().subTypeName)
                {
                    case DJ_PickUp.SUBTYPE.Auto:
                        if (gameController.isAutoUnlocked && gameController.curAuto_Ammo <= 420)
                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    case DJ_PickUp.SUBTYPE.Scatter:
                        if (gameController.isScatterUnlocked && gameController.curScatter_Ammo <= 250)
                        {

                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));

                        }
                        return;

                    case DJ_PickUp.SUBTYPE.Shield:
                        if (gameController.isShieldUnlocked)
                            Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;

                    default:
                        Instantiate(PickupTypes[randIndex], transform.position, Quaternion.Euler(0, 180, 0));
                        return;
                }
            }
        }
    }

}
