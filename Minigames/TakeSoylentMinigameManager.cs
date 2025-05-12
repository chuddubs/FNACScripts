using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TakeSoylentMinigameManager : MonoBehaviour
{
	public AudioSource backgroundAudio, hwabagAudio;

    void Start()
    {
        backgroundAudio.Play();
        hwabagAudio.Play();
	}

	
	public void End()
	{

		hwabagAudio.Stop();
		backgroundAudio.Stop();
        MinigamesManager.Instance.EndMinigame();
    }
}