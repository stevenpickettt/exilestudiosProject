using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour 
{
	//Means audiosource is refering to the AudioSource componant
	AudioSource audiosource;
	MusicController musicController; 
    [SerializeField]
    private AudioClip[] WaveStartSounds;


    [Header ("Player SFX")]
	[Tooltip("Sound for player projectile")]
	public AudioClip SingleShotProjectile;
	[Tooltip("Sound for player multi shot projectile")]
	public AudioClip MultiShotProjectile;
	[Tooltip("Sound for player Chrono watch")]
	public AudioClip ChronoWatchSFX;
	[Tooltip("Sound for player Emp Dash")]
	public AudioClip EmpDashSFX;
	[Tooltip("Sound for player Emp Explosion")]
	public AudioClip EmpExplosionSFX;
	[Tooltip("Sound for player Melee")]
	public AudioClip MeleeSFX;
	[Tooltip("Sound for player Out of Ammo")]
	public AudioClip NoAmmoSFX;
    [Tooltip("Sound for grenade laucher")]
    public AudioClip GrenadeLaucherSFX;


    [Space]
	[Header ("Healing Turret SFX")]
	[Tooltip("Sound for Healing Turret Active")]
	public AudioClip HealingTurretActiveSFX;

	[Space]
	[Header ("Shooting Turret SFX")]
	[Tooltip("Sound for Healing Turret Active")]
	public AudioClip ShootingTurretActiveSFX;

    [Space]
    [Header ("Pick Up SFX")]
	[Tooltip("Sound for Pick Ups")]
	public AudioClip PickUpSFX;

    [Space]
	[Header ("Enemy SFX")]
	[Tooltip("Sound for Enemy Melee")]
	public AudioClip EnemyMeleeSFX;
	[Tooltip("Sound for Explosive enemy")]
	public AudioClip EnemyExplosiveSFX;
	[Tooltip("Sound for Enemy Exo Guard")]
	public AudioClip EnemyExoGuardAttackSFX;
	//[Tooltip("Sound for Enemy Death")]
	public AudioClip EnemyDeathSFX;
	[Tooltip("Sound for Enemy Spawn")]
	public AudioClip EnemySpawnSFX;
    [Tooltip("Sound for EMPd Enemy")]
    public AudioClip EnemyEmpdSFX;
        


    [Space]
	[Header ("Boss 1 SFX")]
	[Tooltip("Sound for Boss 1 Attack")]
	public AudioClip Boss1AttackSFX;
    [Tooltip("Intro Voice Over")]
    public AudioClip DeathIsImmentSFX;
    [Tooltip("Teleport SFX")]
    public AudioClip TeleportSFX;
	[Tooltip("Death SFX")]
	public AudioClip DeathSFX; 

    [Space]
	[Header ("Boss 2 SFX")]
	[Tooltip("Sound for Boss 2 Attack")]
	public AudioClip Boss2LaserAttackSFX;
	[Header ("PowerPanel Explosion SFX")]
	[Tooltip("Sound for PowerPanel Explsoion")]
	public AudioClip PowerPanelExplosionSFX;
	public AudioClip Boss02RegainShield; 



    [Space]
    [Header ("Misc Sounds")]
	[Tooltip("Sound for Doors")]
	public AudioClip DoorSFX;
	[Tooltip("Sound for obtaining and completing Objectives")]
	public AudioClip ObjectiveSFX;
    [Tooltip("Sound for obtaining and completing Objectives")]
    public AudioClip FailedObjectiveSFX;
    /*
    [Tooltip("Sound to be played at beginning of waves")]
    public AudioClip RedAlertSFX;
    [Tooltip("Sound to be played at beginning of waves")]
    public AudioClip ProtocolFiveSFX;
    [Tooltip("Sound to be played at beginning of waves")]
    public AudioClip TerminateSubjectSFX;
    [Tooltip("Sound to be played at beginning of waves")]
    public AudioClip DataLogSFX;
    */ 

    [Space]
	[Header("Game SFX")]
	[Tooltip("SFX for when the player dies")]
	public AudioClip gameOverSFX;
	[Tooltip("SFX for when the player wins")]
	public AudioClip winSFX;

	[Space]
	[Header("Secret Room")]
	public AudioClip secretSwitchSFX; 
	public AudioClip portalRoomSwitchSFX; 



	// Use this for initialization
	void Start () 
	{ 
		//everytime audiosource is used, it gets checks and uses the AudioSource Componant
		audiosource = GetComponent<AudioSource>();
		musicController = FindObjectOfType<MusicController> (); 
	}
	
	//----------Player----------
	public void Player_SingleShotProjectile()
	{
		audiosource.PlayOneShot (SingleShotProjectile, 0.2f);
	}
	public void Player_MultiShotProjectile()
	{
		audiosource.PlayOneShot (MultiShotProjectile, 0.2f);
	}
	public void Player_ChronoWatchSFX()
	{
		audiosource.PlayOneShot (ChronoWatchSFX, 0.2f);
	}
	public void Player_EmpExplosionSFX()
	{
		audiosource.PlayOneShot (EmpExplosionSFX, 0.5f);
	}
	public void Player_MeleeSFX()
	{
		audiosource.PlayOneShot (MeleeSFX, 0.2f);
	}
	public void Player_EmpDashSFX()
	{
		audiosource.PlayOneShot (EmpDashSFX, 0.2f);
	}
	public void Player_NoAmmoSFX()
	{
		audiosource.PlayOneShot (NoAmmoSFX, 1f);
	}
    public void Player_GrenadeLaucherSFX()
    {
        audiosource.PlayOneShot(GrenadeLaucherSFX, .5f);
    }


    //----------Healing Turret----------
    public void HealingTurret_HealingTurretActiveSFX()
	{
		audiosource.PlayOneShot (HealingTurretActiveSFX, 0.2f);
	}

	//----------Shooting Turret----------
	public void ShootingTurret_ShootingTurretActiveSFX()
	{
		audiosource.PlayOneShot (ShootingTurretActiveSFX, 0.2f);
	}

	//----------Pick Ups----------
	public void PickUos_PickUpSFX()
	{
		audiosource.PlayOneShot (PickUpSFX, 0.2f);
	}
	//----------Enemy ----------
	public void Enemy_EnemyMeleeSFX()
	{
		audiosource.PlayOneShot (EnemyMeleeSFX, 0.2f);
	}
	public void Enemy_EnemyExplosionSFX()
	{
		audiosource.PlayOneShot (EnemyExplosiveSFX, 0.2f);
	}
	public void Enemy_EnemyExoGuardAttackSFX()
	{
		audiosource.PlayOneShot (EnemyExoGuardAttackSFX, 0.2f);
	}
	public void Enemy_EnemyDeathSFX()
	{
		audiosource.PlayOneShot (EnemyDeathSFX, 0.2f);
	}
	public void Enemy_EnemySpawnSFX()
	{
		audiosource.PlayOneShot (EnemySpawnSFX, 0.2f);
	}
    public void Enemy_EnemyEmpdSFX()
    {
       // audiosource.PlayOneShot(EnemyEmpdSFX, 0.5f);
    }

    //----------Boss 1----------
    public void Boss1_Boss1AttackSFX()
	{
		audiosource.PlayOneShot (Boss1AttackSFX, 0.2f);
	}
    public void Boss1_VOSFX()
    {
        audiosource.PlayOneShot(DeathIsImmentSFX, 1f);
    }
    public void Boss1_TeleportSFX()
    {
        audiosource.PlayOneShot(TeleportSFX, 1f);
    }
	public void Boss_DeathSFX(){
		audiosource.PlayOneShot(DeathSFX, 2.5f);
		musicController.audiosource.clip = null; 
		Invoke ("PlayGameSceneSFX", 5f); 

	}

	public void PlayGameSceneSFX(){
		musicController.Misc_InGameSFX (); 
	}

    //----------Boss 2----------
    public void Boss2_Boss2LaserAttackSFX()
	{
		audiosource.PlayOneShot (Boss2LaserAttackSFX, 1f);
	}
	public void Boss2_PowerPanelExplosionSFX()
	{
		audiosource.PlayOneShot (PowerPanelExplosionSFX, 1f);
	}

	public void Boss02_RegainShieldSFX(){
		audiosource.PlayOneShot (Boss02RegainShield, 1f); 
	}

    
	//----------Misc----------
	public void Misc_DoorSFX()
	{
		audiosource.PlayOneShot (DoorSFX, 1f);
	}
	public void Misc_ObjectiveSFX()
	{
		audiosource.PlayOneShot (ObjectiveSFX, 1f);
	}
    public void Failed_ObjectiveSFX()
    {
        audiosource.PlayOneShot(FailedObjectiveSFX, 1f);
    }

	public void PlayGameOverSFX() {
		audiosource.PlayOneShot (gameOverSFX); 
	}

	public void PlayWinSFX(){
		audiosource.clip = winSFX; 
		audiosource.Play (); 
	}

	public void PauseAudioController(){
		audiosource.clip = null; 
	}

    /*
    public void PlayRedAlertSFX()
    {
        audiosource.PlayOneShot(RedAlertSFX, 1f);
    }
    public void PlayProtocolFiveSFX()
    {
        audiosource.PlayOneShot(ProtocolFiveSFX, 1f);
    }
    public void PlayDataLogSFX()
    {
        audiosource.PlayOneShot(DataLogSFX, 1f);
    }
    public void PlayTerminateSubjectSFX()
    {
        audiosource.PlayOneShot(TerminateSubjectSFX, 1f);
    }
    */ 

    //----------WaveStartSounds----------

    public void PlayRandomWaveStartSFX()
    {
        audiosource.PlayOneShot(WaveStartSounds[Random.Range(0, WaveStartSounds.Length)], 2f);
    }

	public void PlayPortalSFX(){
		audiosource.PlayOneShot (portalRoomSwitchSFX); 
	}
	public void PlaySecretSwitchSFX(){
		audiosource.PlayOneShot (secretSwitchSFX); 
	}

}
