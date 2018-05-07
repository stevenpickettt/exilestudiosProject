using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerAnimationMachine : MonoBehaviour {

	public enum PLAYERSTATE { IDLE = 0, WALK = 1, DASH = 2, DEATH = 3, DRAWINGWEAPON = 4, POWERUP = 5, TAKINGITEM =6, MELEE =7}
	public PLAYERSTATE CurrentState = PLAYERSTATE.IDLE;

	private Animator myAnimator; 
	private PlayerController playerController; 
	private SP_HUD HUD; 
	private BR_GameController gameController; 
	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator> (); 
		playerController = FindObjectOfType<PlayerController> (); 
		HUD = FindObjectOfType<SP_HUD> (); 
		gameController = FindObjectOfType<BR_GameController> (); 

		ChangeState (PLAYERSTATE.IDLE); 
	}
	
	// Update is called once per frame
	void Update () {
		



		
	}
	public IEnumerator Idle()
	{
		
		while(CurrentState == PLAYERSTATE.IDLE)
		{
			
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isMelee", false);

			myAnimator.SetBool ("isIdle", true); 

			if (Input.GetAxis ("VerticalMenu") != 0 || Input.GetAxis ("HorizontalMenu") != 0) {
				ChangeState (PLAYERSTATE.WALK); 
			} 

			if (Input.GetAxis ("SloMo") != 0 && gameController.isChronoWatchActive == false && gameController.chronoWatchOn == true && gameController.isChronoWatchCoolDown == false && playerController.allSSfired && !HUD.tutorialScreen.activeInHierarchy && !playerController.isEmpExplosionActive && !playerController.isEmpDashActive) {
				ChangeState (PLAYERSTATE.POWERUP); 
				Invoke ("ChangeToIdle", 1.8f); 

			}

			yield return null;
		}

	}
	public IEnumerator Melee()
	{

		while(CurrentState == PLAYERSTATE.MELEE)
		{
			
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isIdle", false); 


			myAnimator.SetBool ("isMelee", true); 

			yield return null;
		}

	}
	public IEnumerator Walk()
	{

		while(CurrentState == PLAYERSTATE.WALK)
		{
			
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isIdle", false);
			myAnimator.SetBool ("isMelee", false);


			myAnimator.SetBool ("isWalking", true); 
			if (Input.GetButtonDown ("Dash") && !playerController.isEmpDashActive && !playerController.isEmpDashCoolDown && Time.timeScale != 0 && gameController.doDash == true && playerController.allSSfired && !HUD.tutorialScreen.activeInHierarchy && !playerController.isEmpExplosionActive && !gameController.isChronoWatchActive) {
				ChangeState (PLAYERSTATE.DASH); 

			}

			if (Input.GetAxis ("VerticalMenu") == 0 && Input.GetAxis ("HorizontalMenu") == 0) {
				ChangeState (PLAYERSTATE.IDLE); 
			} 
			if (Input.GetAxis("Melee") != 0 && playerController.curMeleeDuration <= 0 && Time.timeScale > 0.01f && !playerController.isMeleeActive)
			{
				ChangeState (PLAYERSTATE.MELEE); 

			}
		

			if (Input.GetAxis ("Blast") != 0 && playerController.isEmpExplosionActive == false && playerController.isEmpExplosionCoolDownActive == false && gameController.empBlastOn == true && Time.timeScale != 0 && playerController.allSSfired && !HUD.tutorialScreen.activeInHierarchy && !playerController.isEmpDashActive && !gameController.isChronoWatchActive) {
				ChangeState (PLAYERSTATE.POWERUP);
			}

			if (Input.GetAxis ("SloMo") != 0 && gameController.isChronoWatchActive == false && gameController.chronoWatchOn == true && gameController.isChronoWatchCoolDown == false && playerController.allSSfired && !HUD.tutorialScreen.activeInHierarchy && !playerController.isEmpExplosionActive && !playerController.isEmpDashActive) {
				ChangeState (PLAYERSTATE.POWERUP); 
				Invoke ("ChangeToIdle", 1.8f); 

			}
			 

			yield return null;
		}
	}

	public IEnumerator Dash()
	{

		while(CurrentState == PLAYERSTATE.DASH)
		{
			myAnimator.SetBool ("isDash", true); 
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isIdle", false);
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isMelee", false);

			Invoke ("ChangeToIdle", 0.3f);

			yield return null;
		}
	}
	public IEnumerator Death()
	{

		while(CurrentState == PLAYERSTATE.DEATH)
		{
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false);  
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isIdle", false);
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isMelee", false);

			myAnimator.SetBool ("isDeath", true); 

			yield return null;
		}
	}
	public IEnumerator DrawingWeapon()
	{

		while(CurrentState == PLAYERSTATE.DRAWINGWEAPON)
		{
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isIdle", false); 
			myAnimator.SetBool ("isMelee", false);

			myAnimator.SetBool ("isDrawingWeapon", true); 

			yield return null;
		}
	}

	public IEnumerator PowerUp()
	{

		while(CurrentState == PLAYERSTATE.POWERUP)
		{
			myAnimator.SetBool ("isTakingItem", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isIdle", false); 
			myAnimator.SetBool ("isMelee", false);

			myAnimator.SetBool ("isPowerUp", true); 

			yield return null;
		}
	}

	public IEnumerator TakingItem()
	{

		while(CurrentState == PLAYERSTATE.TAKINGITEM)
		{
			myAnimator.SetBool ("isPowerUp", false); 
			myAnimator.SetBool ("isDrawingWeapon", false); 
			myAnimator.SetBool ("isDeath", false); 
			myAnimator.SetBool ("isDash", false); 
			myAnimator.SetBool ("isWalking", false); 
			myAnimator.SetBool ("isIdle", false); 
			myAnimator.SetBool ("isMelee", false);

			myAnimator.SetBool ("isTakingItem", true); 

			yield return null;
		}
	}
		
	public void ChangeState(PLAYERSTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch(NewState)
		{
		case PLAYERSTATE.IDLE:
			StartCoroutine (Idle());
			break;

		case PLAYERSTATE.WALK:
			StartCoroutine (Walk());
			break;

		case PLAYERSTATE.DASH:
			StartCoroutine (Dash());
			break;

		case PLAYERSTATE.DEATH:
			StartCoroutine(Death());
			break;

		case PLAYERSTATE.DRAWINGWEAPON:
			StartCoroutine(DrawingWeapon());
			break; 

		case PLAYERSTATE.POWERUP:
			StartCoroutine(PowerUp());
			break;

		case PLAYERSTATE.TAKINGITEM:
			StartCoroutine(TakingItem());
			break;

		case PLAYERSTATE.MELEE:
			StartCoroutine (Melee ()); 
			break; 


		}
	}

	public void ChangeToIdle(){
		ChangeState (PLAYERSTATE.IDLE); 
	}



}
