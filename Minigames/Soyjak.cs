using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soyjak : MonoBehaviour
{
	
	public AudioSource tookSoylent;
	
	private ImageAnimation calmAnim, normalAnim, seetheAnim;
	private float timeToChangeStatus, elapsedTime;
	private SoyjakStatus status;
	
    // Start is called before the first frame update
    void Start()
    {
        ImageAnimation[] animations = GetComponents<ImageAnimation>();
		calmAnim = animations[0];
		normalAnim = animations[1];
		seetheAnim = animations[2];
		calmAnim.enabled = true;	
		normalAnim.enabled = false;
		seetheAnim.enabled = false;
		
		timeToChangeStatus = 10f;
		elapsedTime = Random.Range(0f, 3f);
		status = SoyjakStatus.Calm;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
		if (elapsedTime >= timeToChangeStatus)
		{
			ChangeStatus();
			elapsedTime = 0f;
		}
    }
	
	private void ChangeStatus()
	{
		SoyjakStatus newStatus;
		switch (status)
		{
			case SoyjakStatus.Calm:
				newStatus = SoyjakStatus.Normal;
				calmAnim.enabled = false;
				normalAnim.enabled = true;
				seetheAnim.enabled = false;
				break;
			case SoyjakStatus.Normal:
				newStatus = SoyjakStatus.Seethe;
				calmAnim.enabled = false;
				normalAnim.enabled = false;
				seetheAnim.enabled = true;
				break;
			default:
				newStatus = SoyjakStatus.Seethe;
				break;
		}
		status = newStatus;
	}
	
	public void GiveSoylent()
	{
		if (status != SoyjakStatus.Calm)
		{
			status = SoyjakStatus.Calm;
			calmAnim.enabled = true;
			normalAnim.enabled = false;
			seetheAnim.enabled = false;
			elapsedTime = 0f;
			tookSoylent.Play();
			// Debug.Log("Given soylent to " + this);
		}
	}
}

public enum SoyjakStatus
{
	Calm,
	Normal,
	Seethe
}
