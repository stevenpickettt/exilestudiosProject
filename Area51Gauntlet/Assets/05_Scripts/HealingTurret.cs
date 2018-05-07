using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTurret : MonoBehaviour {

    // healing turret 
    private bool isHealingCoolDown = false;
    [SerializeField]
    public float maxHealCoolDown = 5f;
    [SerializeField]
    private float curHealCoolDown;
    [SerializeField]
    private bool isHealingActive = false;
    public float maxHealingDuration = 5f;
    [SerializeField]
    private float curHealingDuration;
    public GameObject healingVolume;

    [Tooltip("Put how much health will be gained. (In negatives)")]
    public int healingAmount; 

    private BR_EnemyHealth enemyHealth;
    private AIAgent aiAgent;


    // Use this for initialization
    void Start()
    {
        enemyHealth = FindObjectOfType<BR_EnemyHealth>();
        aiAgent = FindObjectOfType<AIAgent>();


    }
	
	// Update is called once per frame
	void Update ()
    {
        HealingPulse();
        HealingCoolDown();	
	}

    /*void OnTriggerEnter(Collider target)
    {
        if(target.tag == "Enemy")
        {
            Debug.Log("an enemy has entered the healing volume");

            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.DEFAULT)
            {
                enemyHealth.HealDamage(healingAmount);
            }
        }
    }*/

    public void HealingPulse()
    {
        // turns on the volume for healing radius
        healingVolume.SetActive(true);
        //sets bool to true
        isHealingActive = true;
        //starts timer for how long the volume will last and clamps value
        curHealingDuration -= Time.deltaTime;
        curHealingDuration = Mathf.Clamp(curHealingDuration, 0f, maxHealingDuration);

        // checks to see if timer is at zero, if so set the volume to false 
        if (curHealingDuration <= 0.0f)
        {
            //sets the bool to false
            isHealingActive = false;
            //turns on the cool down timer
            isHealingCoolDown = true;
            //turns off the healing volume 
            healingVolume.SetActive(false);

        }

    }

    void HealingCoolDown()
    {
        //if the cool down is greater than or equal to zero start the cooldown timer 
        if (curHealCoolDown >= 0f && isHealingCoolDown == true)
        {
            curHealCoolDown -= Time.deltaTime;
            isHealingActive = false;
        }
        if (curHealCoolDown < 0.0f)
        {
            curHealCoolDown = maxHealCoolDown;
            curHealingDuration = maxHealingDuration;
            isHealingCoolDown = false;
        }
    }
}
