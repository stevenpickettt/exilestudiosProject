using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW_AnimationController : MonoBehaviour
{
    [Header("Add in all doors")]
    public Animator[] door;
	AudioSource audiosource;
	AudioController audioController;

    [Space]
    public bool doorOpen;

	// Use this for initialization
	void Start ()
    {
		audioController = FindObjectOfType<AudioController>();
		audiosource = GetComponent<AudioSource>();
        doorOpen = false;
	}

    public void OpenDoor()
    {
        if (doorOpen == true)
        {
            if (door != null)
            {
                for (int x = 0; x < door.Length; x++)
                {
                    door[x].SetBool("Opening", true);
                }
                
                audioController.Misc_DoorSFX();
            }
        }
    }
  

    public void RESETALLANIMATION()
    {
        if (door != null)
        {
            for (int x = 0; x < door.Length; x++)
            {
                door[x].SetBool("Opening", false);
            }
        }

        }
}
