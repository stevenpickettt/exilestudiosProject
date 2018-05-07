
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {


	//for EMP Boost
	public float EmpBoostCoolDownDur = 60.0f;
	private float EmpCurDashDur;
	public bool IsEmpDashActive = false;
	public float EmpBoostDur = 2.0f;
	private float CurEmpDuration;
	private bool IsEmpDashOffCoolDown = false;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{


	

		if(IsEmpDashActive == true)
		{
			EmpDashAmount();
		}
	}
		

	void EmpDashAmount()
	{
		//movement speed = max speed

		CurEmpDuration -= Time.deltaTime;
		CurEmpDuration = Mathf.Clamp (CurEmpDuration, 0.0f, CurEmpDuration);

		if(CurEmpDuration <=0.0f)
		{
			IsEmpDashActive = false;
			CurEmpDuration = CurEmpDuration;
			Time.timeScale = 1f;
			IsEmpDashOffCoolDown = true;
		}
	}


	void EMPdashActive()
	{
		if (Input.GetAxis ("Dash") != 0 && IsEmpDashActive == false)
			IsEmpDashActive = true;
	}
		

	void EmpDashCoolDown()
	{
		if(EmpCurDashDur >= 0.0f && IsEmpDashOffCoolDown == true)
		{
			EmpBoostDur -= Time.deltaTime;
			IsEmpDashActive = false;
		}
		if(EmpCurDashDur <= 0.0f)
		{
			EmpCurDashDur = EmpBoostCoolDownDur;
			IsEmpDashOffCoolDown = false;
		}
	}

	//void EmpSpeedIncrease()
	//{
	//	if(gameController.IsEmpDashActive)
	//		//movement speed +2 w/e
	//	else
	//		//movement speed = normal
	//	}




}
