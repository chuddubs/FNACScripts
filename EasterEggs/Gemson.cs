using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gemson : MonoBehaviour
{
    public GameObject gemsonSprite;
    public AudioClip cobsteps;
    public AudioClip heavyBreathing;
    public AudioClip godHere;
    private AudioSource audioSource;
    private KeyboardEasterEgg egg;
    private NightManager nm;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        egg = GetComponent<KeyboardEasterEgg>();
        nm = NightManager.Instance;
        nm.onNightStart += TurnOnEgg;
        nm.onNightEnd += TurnOffEgg;
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
            return;
        nm.onNightStart -= TurnOnEgg;
        nm.onNightEnd -= TurnOffEgg;
    }

    private void TurnOffEgg()
    {
        egg.enabled = false;
    }

    private void TurnOnEgg(int i)
    {
        egg.enabled = true;
    }

    public void GemScare()
    {
        AchievementsManager.Instance.UnlockChievo("secretchievo5");
        foreach(Jak jak in nm.jaks)
            jak.EndNight();
        FeralManager.Instance.EndNight();
        SquirrelManager.Instance.EndNight();
        nm.StopPhoneCall(false, true);
        nm.StopAllCoroutines();
        nm.CancelInvoke("Timer");
        ViewManager.Instance.SetViewCam(0);
        StartCoroutine(GemsonRoutine());
    }

    private IEnumerator GemsonRoutine()
    {
        JumpscaresManager.Instance.PlayHallucinations();
        OfficeController.Instance.power.powerLeft = 0f;
        OfficeController.Instance.power.Outage();
        nm.inTransition = true;
        yield return new WaitForSeconds(2.75f);
        OfficeController.Instance.PlayFootSteps(1, cobsteps);
        yield return new WaitForSeconds(1.5f);
        gemsonSprite.SetActive(true);
        audioSource.clip = heavyBreathing;
        audioSource.Play();
        yield return new WaitForSeconds(heavyBreathing.length * 0.34f);
        audioSource.Stop();
        audioSource.PlayOneShot(godHere);
        yield return new WaitForSeconds(2.5f);
        gemsonSprite.SetActive(false);
        JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Rapeson);
    }
}
