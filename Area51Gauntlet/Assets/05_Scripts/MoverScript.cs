using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverScript : MonoBehaviour
{

    // for speed 
    [SerializeField]
    float moveSpeed = 5.0f;

    // for distance in each axis
    [SerializeField]
    Vector3 distance = Vector3.zero;

    //store starting position
    Vector3 startpos = Vector3.zero;

    // offset position will hold what our current offset is based off the time
    Vector3 offset = Vector3.zero;

    [SerializeField]
    bool isPingPong = true;




    // Use this for initialization
    void Start()
    {
        startpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPingPong)
            DoPingPong();
        else
            DoSine();

        //setting the position 
        transform.position = startpos + offset;
    }

    //our ping pong movement pattern sets the offset for this type 
    void DoPingPong()
    {
        // offsets our x position based on time, never being more than distance, movespeed adjusting how fast it is going
        if (distance.x > 0)
            offset.x = Mathf.PingPong(Time.time * moveSpeed, distance.x);
        // offsets our y position based on time, never being more than distance, movespeed adjusting how fast it is going
        //if (distance.y > 0)
        //    offset.y = Mathf.PingPong(Time.time * moveSpeed, distance.y);
        // offsets our z position based on time, never being more than distance, movespeed adjusting how fast it is going
        if (distance.z > 0)
            offset.z = Mathf.PingPong(Time.time * moveSpeed, distance.z);
    }

    // our sine movement pattern sets the offset for this type 
    void DoSine()
    {
        // offsets our x position based on time, never being more than distance, movespeed adjusting how fast it is going
        if (distance.x > 0)
            offset.x = Mathf.Sin(Time.time * moveSpeed) * distance.x;
        // offsets our y position based on time, never being more than distance, movespeed adjusting how fast it is going
        //if (distance.y > 0)
        //    offset.y = Mathf.Sin(Time.time * moveSpeed) * distance.y;
        // offsets our z position based on time, never being more than distance, movespeed adjusting how fast it is going
        if (distance.z > 0)
            offset.z = Mathf.Sin(Time.time * moveSpeed) * distance.z;
    }


}
