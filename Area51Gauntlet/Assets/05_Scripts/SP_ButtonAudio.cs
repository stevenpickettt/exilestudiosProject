using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI; 
public class SP_ButtonAudio : MonoBehaviour, ISelectHandler , IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
	private SP_MainMenuAudio mainMenuAudio; 

	public GameObject difficultyImage;

	void Start(){
		mainMenuAudio = FindObjectOfType<SP_MainMenuAudio> ();
		mainMenuAudio = mainMenuAudio.GetComponent<SP_MainMenuAudio> (); 
		if(difficultyImage != null){
			difficultyImage.SetActive (false); 
		}

	}




	public void OnPointerEnter(PointerEventData eventData)
	{
		mainMenuAudio.TaskOnHighlight (); 

		if(difficultyImage != null){
			difficultyImage.SetActive (true); 
		}

	}


	public void OnPointerExit(PointerEventData eventData){
		if(difficultyImage != null){
			difficultyImage.SetActive (false); 
		}
	}



	public void OnSelect(BaseEventData eventData)
	{
		if(difficultyImage != null){
			difficultyImage.SetActive (true); 
		}
		
		mainMenuAudio.TaskOnClick (); 

	}
	public void OnDeselect(BaseEventData eventData){
		if(difficultyImage != null){
			difficultyImage.SetActive (false); 

		}
	}



}