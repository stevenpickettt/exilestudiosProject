using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_TrilogyProjectile : MonoBehaviour
{

    [Header("Changeable values:")]
    public float destroyTime;
    public float destroyTimer;
    public float MoveSpeed;

    [Header("Damage of bullet:")]
    [Tooltip("How much damage the player will take.")]
    public int damageDealt;
    
    // Use this for initialization

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        destroyTime += Time.deltaTime;
        transform.position += transform.forward * Time.deltaTime * MoveSpeed;
        if (destroyTime >= destroyTimer)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Environment")
        {
            Destroy(gameObject);
        }


        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }

        if (other.tag == "ThrusterA")
        {
            Destroy(gameObject);
        }


    }
}
