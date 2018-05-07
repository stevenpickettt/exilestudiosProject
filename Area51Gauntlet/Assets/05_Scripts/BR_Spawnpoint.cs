using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Spawnpoint : MonoBehaviour {
    // Use this for initialization
    
    public bool isStartingSpawn;
    public bool isValid = true;


    public AIAgent aiAgent;

    public GameObject[] lights;
    public Color brightGreen;
    private Color curColor;

    

    
	void Start () {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, .5f, gameObject.transform.position.z);
        this.GetComponent<Collider>().isTrigger = true;
        //    aiAgent = FindObjectOfType<AIAgent>();
        if(lights.Length !=0)
        curColor = lights[lights.Length - 1].GetComponent<Light>().color;
        
    }

    void OntriggerEnter(Collider collider)
    {
      

        if (collider.tag == "Player" || collider.tag == "Enemy")
            isValid = false;

      

    }

    void OnTriggerStay(Collider collider)
    {

        if (collider.tag == "Player" || collider.tag == "Enemy")
            isValid = false;

        if (lights.Length != 0 && collider.tag == "Enemy")
        {
            aiAgent = collider.GetComponent<AIAgent>();
            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1)
            {
                Debug.Log("Boss is in SpawnPoint Turn ON LIGHTS");
                TurnOnLights();
            }
        }

    }

    void OnTriggerExit(Collider collider)
    {
            isValid = true;

        if (lights.Length != 0 && collider.tag == "Enemy")
        {
            aiAgent = collider.GetComponent<AIAgent>();

            if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.BOSS1)
            {
                Debug.Log("Boss left the SpawnPoint Turn OFF LIGHTS");
                TurnOffLights();
            }
        }
    }

    void TurnOnLights()
    {
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].SetActive(true);
            lights[x].GetComponent<Light>().color = Color.Lerp(curColor, Color.clear, Mathf.PingPong(Time.time, .5f));
        }
    }

    public void TurnOffLights()
    {
        for (int x = 0; x < lights.Length; x++)
        {
            lights[x].SetActive(false);
            lights[x].GetComponent<Light>().color = Color.red;
        }
    }

    public void ChangeCompLightColor()
    {
        //turn them on 
        for (int x = 0; x < lights.Length; x++)
        {
                lights[x].SetActive(true);
            lights[x].GetComponent<Light>().color = brightGreen;
        }
    }

}
