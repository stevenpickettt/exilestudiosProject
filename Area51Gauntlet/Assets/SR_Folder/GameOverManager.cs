using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

	public float StartDelay = 5.0f;
	public PlayerController PlayerRef;
	public Animator anim;
	float RestartTimer;
	float PlayerCurHealth;


	void Awkae()
	{
		anim = GetComponent <Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{



		
	}

	public void GameOverUI()
	{
		if (PlayerRef.curHealth <= 0.0f) 
		{
			anim.SetTrigger("GameOver");

			RestartTimer += Time.deltaTime;

			if(RestartTimer >= StartDelay)
			{
				//re loads the scene upon death. WE can replace this with the last checkpoint
				SceneManager.LoadScene ("DJ_TestScene");
			}
		}
	}
}
