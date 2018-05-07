using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_resize : MonoBehaviour {
    float scaleTime;
    float positionTime;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        positionTime += Mathf.Clamp(Time.deltaTime * 5,0,.5f);
        gameObject.transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y+positionTime,0,1), transform.localScale.z);
        if(transform.localScale.y != 1)
        gameObject.transform.Translate(Vector3.up * positionTime,Space.Self);
	}
}
