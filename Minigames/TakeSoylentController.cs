using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TakeSoylentController : MonoBehaviour
{
	
	public Doll doll;
	public Image truck;
	public Image soylita;
	public Image rapeson;
	
	public Vector2 truckDirection;
	public float truckFramerate;
	
	public float waitingDuration;
	public float movingDuration;
	public float murderingDuration;
	public float leavingDuration;
	public float dollFramerateIncreaseMoving, dollFramerateIncreaseMurdering;
	
	
	private RectTransform truckTransform;
	private ImageAnimation soylitaCryingAnim, soylitaDeathAnim;
	private float sequenceElapsedTime, elapsedTime;
	private MurderSequenceStatus status;
	
	
    // Start is called before the first frame update
    void Start()
    {
		rapeson.enabled = false;
		ImageAnimation[] soylitaAnims = soylita.GetComponents<ImageAnimation>();
		soylitaCryingAnim = soylitaAnims[0];
		soylitaDeathAnim = soylitaAnims[1];
		soylitaCryingAnim.enabled = true;
		soylitaDeathAnim.enabled = false;
		
        truckTransform = truck.GetComponent<RectTransform>();
		sequenceElapsedTime = 0f;
		elapsedTime = 0f;
		
		status = MurderSequenceStatus.Waiting;
    }

    // Update is called once per frame
    void Update()
    {
		switch (status)
		{
			case MurderSequenceStatus.Waiting:
				break;
			case MurderSequenceStatus.Moving:
				doll.framerate += dollFramerateIncreaseMoving;
				MoveTruck();
				break;
			case MurderSequenceStatus.Murdering:
				doll.framerate += dollFramerateIncreaseMurdering;
				Kill();
				break;
			case MurderSequenceStatus.Leaving:
				Leave();
				break;
			case MurderSequenceStatus.End:
				End();
				break;
		}
		
		sequenceElapsedTime += Time.deltaTime;
		if (sequenceElapsedTime >= GetMurderSequenceDuration()) 
		{
			sequenceElapsedTime = 0;
			elapsedTime = 0;
			status = GetNextMurderSequenceStatus();
		}
    }
	
	private void MoveTruck()
	{
		if (elapsedTime < truckFramerate) 
		{
			elapsedTime += Time.deltaTime;
			return;
		}
		elapsedTime = 0f;
		
		Vector3 localPosition = truckTransform.transform.localPosition;
		localPosition.Set(localPosition.x + truckDirection.x, localPosition.y + truckDirection.y, localPosition.z);
		truckTransform.transform.localPosition = localPosition;
	}
	
	private void Kill()
	{
		rapeson.enabled = true;
		soylitaCryingAnim.enabled = false;
		soylitaDeathAnim.enabled = true;
		if (soylitaDeathAnim.IsLastFrame())
		{
			truck.GetComponent<RectTransform>().SetSiblingIndex(2);
		}
	}
	
	private void Leave()
	{
		rapeson.enabled = false;
		MoveTruck();
	}
	
	private void End()
	{
		if (sequenceElapsedTime == 0f) {	// End the minigame only once
			GetComponent<TakeSoylentMinigameManager>().End();
		}
	}
	
	private float GetMurderSequenceDuration()
	{
		float duration;
		switch (status)
		{
			case MurderSequenceStatus.Waiting:
				duration = waitingDuration;
				break;
			case MurderSequenceStatus.Moving:
				duration = movingDuration;
				break;
			case MurderSequenceStatus.Murdering:
				duration = murderingDuration;
				break;
			case MurderSequenceStatus.Leaving:
				duration = leavingDuration;
				break;
			case MurderSequenceStatus.End:
				duration = 0;
				break;
			default:
				duration = 0;
				break;
		}
		return duration;
	}
	
	private MurderSequenceStatus GetNextMurderSequenceStatus()
	{
		MurderSequenceStatus nextStatus = status;
		switch (status)
		{
			case MurderSequenceStatus.Waiting:
				nextStatus = MurderSequenceStatus.Moving;
				break;
			case MurderSequenceStatus.Moving:
				nextStatus = MurderSequenceStatus.Murdering;
				break;
			case MurderSequenceStatus.Murdering:
				nextStatus = MurderSequenceStatus.Leaving;
				break;
			case MurderSequenceStatus.Leaving:
				nextStatus = MurderSequenceStatus.End;
				break;
			case MurderSequenceStatus.End:
				nextStatus = MurderSequenceStatus.Finish;
				break;
		}
		return nextStatus;
	}

}

public enum MurderSequenceStatus
{
	Waiting,
	Moving,
	Murdering,
	Leaving,
	End,
	Finish
}