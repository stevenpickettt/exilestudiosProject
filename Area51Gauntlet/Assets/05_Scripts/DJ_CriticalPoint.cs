using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_CriticalPoint : MonoBehaviour
{

    [SerializeField]
    private BR_EnemyHealth health;

    [SerializeField]
    public AIAgent aiAgent;

    public int CrictialHitMulitplier = 2;

	public GameObject criticalPointPS; 
	public GameObject criticalPointSpawnPoint; 
    

    // Use this for initialization
    void Start()
    {
		
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health.curHealth <= 0)
        {
            Destroy(gameObject);
        }
		

    }

    void OnTriggerEnter(Collider target)
    {
        if (target.tag == "PlayerProjectile")
        {
			Instantiate (criticalPointPS, criticalPointSpawnPoint.transform.position, criticalPointPS.transform.rotation); 
            health.TakeCritcalDamage(target.GetComponent<KW_PlayerProjectile>().bulletType,target.GetComponent<KW_PlayerProjectile>().damageDealt * CrictialHitMulitplier);

            if(aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1)
            health.EnemyContainer.transform.LookAt(aiAgent.playerTransform);

           
        }
    }





}
