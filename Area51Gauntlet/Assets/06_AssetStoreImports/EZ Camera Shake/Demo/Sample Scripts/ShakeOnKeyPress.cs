using UnityEngine;
using EZCameraShake;

public class ShakeOnKeyPress : MonoBehaviour
{
    public float Magnitude = 2f;
    public float Roughness = 10f;
    public float FadeOutTime = 5f;

	public bool isCameraShakeOn = false; 
	public float maxTimeBetweenShakes = 1.5f; 
	public float curTimeBetweenShakes = 0f; 

	void Update(){
		if (isCameraShakeOn == true) {
			curTimeBetweenShakes += Time.deltaTime; 
			if (curTimeBetweenShakes >= maxTimeBetweenShakes) {
				curTimeBetweenShakes = 0; 
				isCameraShakeOn = false; 
				Debug.Log("SHAKE ON KEY PRESS!"); 
			}
		}
	}
	public void ShakeTheCamera(){
		if (isCameraShakeOn == true) {
			Magnitude = 3f;
			Roughness = 7f;
			FadeOutTime = 2f;
			CameraShaker.Instance.ShakeOnce (Magnitude, Roughness, 0, FadeOutTime);
		}
	}
	public void ShakeForDroneExplosion(){
		if (isCameraShakeOn == true) {
			Magnitude = 2f;
			Roughness = 6f;
			FadeOutTime = 1f;
			CameraShaker.Instance.ShakeOnce (Magnitude, Roughness, 0, FadeOutTime);
		}
	}

	public void ShakeForEMPDash(){
		Magnitude = 3f;
		Roughness = 2f;
		FadeOutTime = 1f;
		CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
	}
	public void ShakeForEMPExplosion(){
		Magnitude = 2f;
		Roughness = 5f;
		FadeOutTime = 1f;
		CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
	}

	public void ShakeForPlayerHitWithProjectile(){
		Magnitude = 2f;
		Roughness = 3f;
		FadeOutTime = 1f;
		CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
	}
}
