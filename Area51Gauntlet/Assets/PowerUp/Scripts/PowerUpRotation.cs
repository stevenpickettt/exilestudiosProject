using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpRotation : MonoBehaviour
{

	#region Settings
	public float rotationSpeed = 99.0f;
    [Tooltip("use this for z axis")]
	public bool reverse = false;
    [Tooltip("use this for x axis rotation ")]
    public bool rotateXAxis = false;
    [Tooltip("use this for Y axis rotation ")]
    public bool rotateYAxis = false; 

	#endregion

	void Update ()
	{
	if(this.reverse)
			//transform.Rotate(Vector3.back * Time.deltaTime * this.rotationSpeed);
			transform.Rotate(new Vector3(0f,0f,1f) * Time.deltaTime * this.rotationSpeed);
    
    if (rotateXAxis)
        {
            RotateOnXAxis(); 
        }

        if (rotateYAxis)
        {
            RotateOnYAxis(); 
        }
			
	}

	public void SetRotationSpeed(float speed)
	{
		this.rotationSpeed = speed;
	}

	public void SetReverse(bool reverse)
	{
		this.reverse = reverse;
	}

    public void RotateOnXAxis()
    {
        transform.Rotate(new Vector3(1f, 0f, 0f) * Time.deltaTime * this.rotationSpeed);
    }

    public void RotateOnYAxis()
    {
        transform.Rotate(new Vector3(0f, 1f, 0f) * Time.deltaTime * this.rotationSpeed);
    }
    
}

