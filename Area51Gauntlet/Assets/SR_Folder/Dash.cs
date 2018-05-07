using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour 
{
	public DashStates dashstates;
	public float DashTimer;
	public float MaxDash = 10.0f;

	public Vector3 savedVelocity;
	public  Rigidbody playerBody;
	// Use this for initialization
	void Start () 
	{
		playerBody = GetComponent<Rigidbody> ();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		switch (dashstates) 
		{
		case DashStates.DashIsReady:
			var isDashKeyDown = Input.GetKeyDown (KeyCode.L);
			if (isDashKeyDown) {
				//playerBody = GetComponent<Rigidbody> ();
				savedVelocity = playerBody.velocity;
				//savedVelocity = rigidbody.velocity;
				playerBody.velocity = new Vector3 (playerBody.velocity.x*3F, playerBody.velocity.y);
				//rigidbody.velocity = new Vector3 (rigidbody.Velocity.X * 3f, rigidbody.velocity.Y);
				dashstates = DashStates.DashIsActive;
			}
			break;
		case DashStates.DashIsActive:
			DashTimer += Time.deltaTime * 3;
			if (DashTimer >= MaxDash) {
				DashTimer = MaxDash;
				playerBody.velocity = savedVelocity;
				//rigidbody.velocity = savedVelocity;
				dashstates = DashStates.DashCoolDown;
			}
			break;
		case DashStates.DashCoolDown:
			DashTimer -= Time.deltaTime;
			if (DashTimer <= 0) {
				DashTimer = 0;
				dashstates = DashStates.DashIsActive;
			}
			break;
		}
	}

	public enum DashStates
	{
		DashIsReady,
		DashIsActive,
		DashCoolDown
	}

}
