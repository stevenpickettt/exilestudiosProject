using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_DestroyPS : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        ParticleSystem system = gameObject.GetComponent<ParticleSystem>();
        Destroy(gameObject, 1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
