using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_Enemy_Projectile : MonoBehaviour {

    [Header("Changeable values:")]
    public float destroyTime;
    public float destroyTimer;
    public float MoveSpeed;

    [Header("Damage of bullet:")]
    [Tooltip("How much damage the player will take.")]
    public int damageDealt;

    [Header("ForTrilogyGuard")]
    public  bool isTrilogyBullet;
    public int damageMultipler;
    public Color highDamage;
    private Renderer rend;

    BR_isTrilogyGuard[] TrilogyGuards;


    // Use this for initialization
    void Start()
    {
        Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
        TrilogyGuards = FindObjectsOfType<BR_isTrilogyGuard>();


        rend = GetComponent<Renderer>();

        if (isTrilogyBullet)
        {
            DoTrilogyProjectile();
        }

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
        
        if(other.tag == "ThrusterA")
        {
            Destroy(gameObject); 
        }
       
    }

    void DoTrilogyProjectile()
    {
        if(TrilogyGuards.Length <= 3)
        {
            Behaviour halo = (Behaviour)gameObject.GetComponent("Halo");
            switch (TrilogyGuards.Length)
            {
                case 1:
                    damageDealt = damageDealt * damageMultipler * 2;
                    rend.material.color = highDamage; 
                    halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
                    break;
                case 2:
                    damageDealt = damageDealt * damageMultipler;
                    rend.material.color = highDamage; 
                    break;
            } 
        }

    }
}
