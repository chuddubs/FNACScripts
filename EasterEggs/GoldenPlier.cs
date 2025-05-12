using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldenPlier : MonoBehaviour
{
    public float odds = 2500f;
    public GameObject regularPoster;
    public GameObject darkPoster;
    public GameObject goldenPlier;
    public GameObject jumpscare;
    public AudioClip laugh;
    public AudioClip appear;
    public AudioClip scream;
    private ViewManager vm;
    void Awake()
    {
        vm = ViewManager.Instance;
        vm.onCamSwitched +=  OnCamSwitched;
        NightManager.Instance.onNightEnd += EndNight;
    }

    private void OnCamSwitched(int id)
    {
        if (id != 13 || darkPoster.activeSelf)
            return;
        if (Random.Range(0f, odds) <= 1f)
        {
            darkPoster.SetActive(true);
            regularPoster.SetActive(false);
            StartCoroutine(Golden());
        }
    }

    private IEnumerator Golden()
    {
        AudioManager.Instance.Play(laugh);
        while (vm.currId != 0)
            yield return null;
        OfficeController oc = OfficeController.Instance;
        while (oc.power.outage)
            yield return null;
        AudioManager.Instance.Play(appear);
        goldenPlier.SetActive(true);
        AchievementsManager.Instance.UnlockChievo("secretchievo3");
        Jak[] inOfficeJaks = NightManager.Instance.jaks.Where(j => j.IsInOffice || j.currentSpot.Cam.id == -5).ToArray();
        foreach (Jak jak in inOfficeJaks)
        {
            jak.Interrupt();
            jak.Teleport(vm.GetCamById(-3).GetFreeSpotWithinView(jak));
        }
        float timer = 8f;
        while (timer > 0)
        {
            if (vm.currId != 0)
            {
                Reset();
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        jumpscare.SetActive(true);
        foreach (Jak jak in NightManager.Instance.jaks.Where(j => j.active))
            jak.EndNight();
        AudioManager.Instance.Stop();
        NightManager.Instance.inTransition = true;
        AudioManager.Instance.Play(scream);
        yield return new WaitForSeconds(scream.length);
        SoyGameController.Instance.Quit();
    }

    public void Reset()
    {
        regularPoster.SetActive(true);
        darkPoster.SetActive(false);
        goldenPlier.SetActive(false);
    }

    public void EndNight()
    {
        StopAllCoroutines();
        Reset();
    }
}
