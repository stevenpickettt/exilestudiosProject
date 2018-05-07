using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastHandler : MonoBehaviour {

    TurretController controller;
    private LineRenderer lineRenderer; // For laser effect
    AIAgent aiAgent;
    BR_Friendly friendly;
    WaveSpawner spawner;
    ShootingSystem shootingSystem;

    [Header("Points")]
    [Tooltip("Point from which raycast should start")]
    public Transform startPoint;

    void Start() {
        aiAgent = this.GetComponent<AIAgent>();
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.FRIENDLYTURRET)
            friendly = this.GetComponent<BR_Friendly>();
        lineRenderer = startPoint.GetComponent<LineRenderer>();
        controller = this.GetComponent<TurretController>();
        shootingSystem = this.GetComponent<ShootingSystem>();
        spawner = FindObjectOfType<WaveSpawner>();
    }

    void FixedUpdate() {
        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGTURRET)
        {
            if (shootingSystem.target != null)
            {
                lineRenderer.enabled = true;
                if (startPoint && lineRenderer)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, controller._Shooting.range))
                    {
                        if (aiAgent.EnemyType == AIAgent.ENEMYTYPE.REGTURRET)
                        {
                            if (hit.collider.gameObject.tag == "Player")
                            {
                                lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));//if raycast hit somewhere then stop laser effect at that point
                                controller._Shooting.Fire(hit.point, hit.collider.gameObject);//if hit some point then shoot through shootingSystem Fire Function
                            }
                        }
                    }
                    else
                    {
                        lineRenderer.SetPosition(1, new Vector3(0, 0, controller._Shooting.range));//if not hit, laser till range 
                    }
                }
            }
            lineRenderer.enabled = false;
        }
        else
        {
            if (startPoint && lineRenderer && friendly.isActive && spawner.state != WaveSpawner.SpawnState.WAITING)
            {
               RaycastHit hit;
                if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, controller._Shooting.range))
                {
                        if (hit.collider.gameObject.tag != "Player" || hit.collider.gameObject.tag != "Environment" || hit.collider.gameObject.tag == "PickUp")
                        {
                            lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));//if raycast hit somewhere then stop laser effect at that point
                            controller._Shooting.Fire(hit.point, hit.collider.gameObject);//if hit some point then shoot through shootingSystem Fire Function
                        }
                    }
                else
                {
                    lineRenderer.SetPosition(1, new Vector3(0, 0, controller._Shooting.range));//if not hit, laser till range 
                }
            }
            else
                lineRenderer.enabled = false;
        }
    }

	public void TurretLaser_Status(bool val){
	
		lineRenderer.enabled = val;
	}
}
