using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject PlayerObject;
    [SerializeField]
    private float heightFromPlayer = 10f;

    // Update is called once per frame
    void Update()
    {
        if (PlayerObject != null)
        {
            DoMovement();
        }
        else
        {
            FindPlayer();
            Debug.Log("No PlayerObject Found");
        }
    }
    
    void DoMovement()
    {
        transform.position = new Vector3(PlayerObject.transform.position.x, PlayerObject.transform.position.y + heightFromPlayer, PlayerObject.transform.position.z);
    }

    public void FindPlayer()
    {
        PlayerObject = GameObject.FindGameObjectWithTag("Player");
    }
}
