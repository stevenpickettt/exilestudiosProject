using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_EnemyHealing : MonoBehaviour
{

    [Header("Amount Healed to the enemy:")]
    public int healAmount;

    public ParticleSystem healingParticles;

    // Use this for initialization
    void Start ()
    {
        healingParticles.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}
}
