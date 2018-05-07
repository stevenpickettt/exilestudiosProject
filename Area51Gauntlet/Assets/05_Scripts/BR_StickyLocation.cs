using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_StickyLocation : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter( Collider other)
    {
        if (other.tag != "PlayerProjectile" && other.tag != "EnemyProjectile" && other.tag != "PickUp" && other.tag != "Untagged")
        {
            this.transform.parent = other.transform;
        }
    }
}
