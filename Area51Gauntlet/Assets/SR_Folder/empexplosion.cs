using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class empexplosion : MonoBehaviour {

	public float EmpExplosionCoolDownDuration = 60.0f;
	private float CurEmpExplosionDuration;
	public bool IsEmpExplosionActive = false;
	public float EmpExplosionDuration = 1.0f;
	private float CurEmpExplosionDur;
	private bool IsEmpExplosionOffCoolDown = false;

	public int EmpExplosionDamage = 5;
	public float EmpExplosionRange = 1.0f;
	private GameObject EmpExplosionVol;


	// Use this for initialization
	void Start ()
	{
		EmpExplosionVol = GameObject.Find ("EmpExplosion");
		EmpExplosionVol.transform.Translate (new Vector3 (0, 0, EmpExplosionRange));
		EmpExplosionVol.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(IsEmpExplosionActive == true)
		{
			EmpExplosion();
		}

		if (Input.GetButtonDown ("Blast") && EmpExplosionDuration <= 0) 
			EmpExplosionActive ();
		
		if(IsEmpExplosionActive)
			EmpExplosion ();

		//EmpExplosionActive ();
		EmpDExplosionCoolDown ();
	}

	void EmpExplosion()
	{
		//explode damage

		CurEmpExplosionDur -= Time.deltaTime;
		CurEmpExplosionDur = Mathf.Clamp (CurEmpExplosionDur, 0.0f, EmpExplosionDuration);

		if(CurEmpExplosionDur <=0.0f)
		{
			IsEmpExplosionActive = false;
			EmpExplosionVol.SetActive (false);
			IsEmpExplosionOffCoolDown = true;
		}
	}

	void EmpExplosionActive()
	{
		if (Input.GetAxis ("Blast") != 0 && IsEmpExplosionActive == false) 
		{
			//CurEmpExplosionDur = EmpExplosionDuration;
			EmpExplosionVol.SetActive (true);
			IsEmpExplosionActive = true;

		}
			
	}

	void EmpDExplosionCoolDown()
	{
		if(CurEmpExplosionDuration >= 0.0f && IsEmpExplosionOffCoolDown == true)
		{
			EmpExplosionDuration -= Time.deltaTime;
			IsEmpExplosionActive = false;
		}

		if(CurEmpExplosionDuration <= 0.0f)
		{
			CurEmpExplosionDuration = EmpExplosionCoolDownDuration;
			IsEmpExplosionOffCoolDown = false;
		}
	}

	void empExplosionDamageActive()
	{
		if (IsEmpExplosionActive)
		{
			//
			EmpExplosionVol.SetActive(true);
		} 
		else 
		{
			EmpExplosionVol.SetActive(false);
		}
	}

}
