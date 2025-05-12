using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
	
	// public GameObject transition;
	// public GameObject overlay;
	
	// private ImageAnimation transitionAnim;
	// private ImageAnimation[] overlayAnims;
	// private ImageAnimation currentlyPlayingOverlayAnim;
	// private float elapsedTime, timeBeforeOverlayAnim;
	// private bool playOverlayAnims;
    // public AudioSource transitionAudio;
	
	
    // // Start is called before the first frame update
    // void Start()
    // {

	// }

    // // Update is called once per frame
    // void Update()
    // {
	// 	if (transitionAnim.IsLastFrame())
	// 	{
    //         transitionAudio.Stop();
	// 		playOverlayAnims = true;
	// 		transition.SetActive(false);
	// 	}
		
	// 	if (!playOverlayAnims)
	// 	{
	// 		return;
	// 	}
		
	// 	elapsedTime += Time.deltaTime;
	// 	if (elapsedTime >= timeBeforeOverlayAnim && currentlyPlayingOverlayAnim == null)
	// 	{
	// 		int index = UnityEngine.Random.Range(0, overlayAnims.Length - 1);
	// 		currentlyPlayingOverlayAnim = overlayAnims[index];
	// 		overlay.SetActive(true);
	// 	}
		
	// 	if (currentlyPlayingOverlayAnim != null && currentlyPlayingOverlayAnim.IsLastFrame())
	// 	{
	// 		elapsedTime = 0f;
	// 		timeBeforeOverlayAnim = UnityEngine.Random.Range(2f, 4f);
	// 		currentlyPlayingOverlayAnim = null;
	// 		overlay.SetActive(false);
	// 	}
    // }
	
	
	// public void End()
	// {
    //     MinigamesManager.Instance.EndMinigame();
	// }
}