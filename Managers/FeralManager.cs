using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeralManager : Singletroon<FeralManager>
{
    public System.Action onTablet;
    public GameObject[] feralSprites;
    public int[] baseAggro = {10, 1, 3, 4, 5, 8};
    public int[] aggro2am = {0, 3, 5, 6, 7, 11};
    public int[] aggro4am = {0, 8, 11, 13, 15, 18};
    public int currentAggro = 1;
    private float sleepMeter = 0; //100 = awake
    private bool awake = false;
    private bool hasTablet = false;
    private bool isSeething = false;
    private int timesWokeUp = 0;
    public GameObject  tablet;
    
    [Header("Audio")]
    public AudioSource tabletAudio;
    public AudioSource feralJakAudio;
    private AudioSource thump;
    public AudioClip   snoring;
    public AudioClip   seething;
    public AudioClip   tantrum;
    public AudioClip   tabletFlip;
    public AudioClip   tabletBootUp;
    public AudioClip   tabletDrop;
    public AudioClip   tabletBroken;
    public AudioClip[] tabletSongs;
    public AudioClip[] cacaSongs;

    private PlayerCam  playerCam;

    private void Awake()
    {
        playerCam = PlayerCam.Instance;
        NightManager.Instance.onNightStart += StartNight;
        NightManager.Instance.onNightEnd += EndNight;
        thump = feralSprites[feralSprites.Length - 1].transform.GetChild(0).GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        NightManager.Instance.onNightEnd -= EndNight;
    }

    private void ToggleBrokenTablet(bool broken)
    {
        tablet.transform.GetChild(0).gameObject.SetActive(broken);
    }

    public void StartNight(int night)
    {
        timesWokeUp = 0;
        hasTablet = false;
        isSeething = false;
        currentAggro = baseAggro[night];
        tablet.SetActive(true);
        ToggleBrokenTablet(false);
        FallAsleep();
    }

    public void RaiseAggro(int night, int hour)
    {
        if (hour == 2)
            currentAggro = aggro2am[night];
        else if (hour == 4)
            currentAggro = aggro4am[night];
    }

    private void FallAsleep()
    {
        awake = false;
        sleepMeter = 0;
        UpdateSleepState();
        InvokeRepeating("DecreaseSleepMeter", 5f, 5f);
        StartCoroutine(Snore());
    }

    private IEnumerator Snore()
    {
        yield return new WaitForSeconds(Random.Range(2f, 10f));
        if (awake)
            yield break;
        float origVolume = feralJakAudio.volume;
        feralJakAudio.volume = 0.33f;
        PlayAudio(feralJakAudio, snoring, true);
        float elapsed = 0f;
        while (elapsed < 15f)
        {
            if (feralJakAudio.clip != snoring)
            {
                feralJakAudio.volume = origVolume;
                yield break;
            }
            feralJakAudio.volume = Mathf.Lerp(0.33f, 0f, elapsed / 15f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        feralJakAudio.Stop();
        feralJakAudio.volume = origVolume;
    }

    private void DecreaseSleepMeter()
    {
        if (!awake)
            sleepMeter = Mathf.Max(0, sleepMeter - 1f);
        if (!hasTablet)
            UpdateSleepState();
    }

    public void Disturb(float noiseAmount)
    { 
        sleepMeter = AdjustDisturbValueFromAggro(noiseAmount);
        UpdateSleepState();
    }

    private float AdjustDisturbValueFromAggro(float noise)
    {
        if (!IsActiveTonight())
            return (0);
        if (noise == 100f)
            return 100f;
        return sleepMeter + (noise + 0.025f *  currentAggro);
    }

    private void UpdateSleepState()
    {
        if (awake)
            return;
        if (sleepMeter <= 20)
            ShowSleepSprite(0);
        else if (sleepMeter <= 50)
            ShowSleepSprite(1);
        else if (sleepMeter <= 75)
            ShowSleepSprite(2);
        else if (sleepMeter <= 97)
            ShowSleepSprite(3);
        else if (sleepMeter <= 99)
            ShowSleepSprite(4);
        else if (!awake && !hasTablet)
            StartCoroutine(WakeUp());
    }

    public void GiveTablet()
    {
        if (!awake || hasTablet)
            return;
        if (onTablet != null)
            onTablet();
        if (timesWokeUp > 3 || isSeething)
        {
            PlayAudio(tabletAudio, tabletBroken);
            return;
        }
        hasTablet = true;
        StartCoroutine(TabletGiven());
    }

    public IEnumerator TabletGiven()
    {
        feralJakAudio.Stop();
        tablet.SetActive(false);
        PlayAudio(tabletAudio, tabletFlip);
        yield return new WaitForSeconds(tabletFlip.length);
        PlayAudio(tabletAudio, tabletBootUp);
    }

    private void DropTablet()
    {
        NightManager.Instance.UpdateCalm(-1);
        hasTablet = false;
        ToggleBrokenTablet(timesWokeUp > 2);
        PlayAudio(tabletAudio, tabletDrop);
        NightManager.Instance.RaiseGlobalAggro(-5);
        tablet.SetActive(true);
    }

    private IEnumerator WakeUp()
    {
        awake = true;
        timesWokeUp++;
        ShowSleepSprite(5);
        if (SoyGameController.Instance.IsCaca())
            PlayAudio(feralJakAudio, tantrum, true);
        else
            PlayAudio(feralJakAudio, seething, true);
        CancelInvoke("DecreaseSleepMeter");
        float timeToGiveTablet = 20;
        while (timeToGiveTablet > 0)
        {
            if (hasTablet)
            {
                StartCoroutine(WatchTabletRoutine());
                yield break;
            }
            timeToGiveTablet -= Time.deltaTime;
            yield return null;
        }
        if (OfficeController.Instance.cellBarsHP > 0 && !isSeething)
            StartCoroutine(Seethe());
        else
            Screamer();
    }

    private IEnumerator Seethe()
    {
        isSeething = true;
        NightManager.Instance.UpdateCalm(1);
        ShowSleepSprite(7);
        float origVolume = feralJakAudio.volume;
        feralJakAudio.volume = 1f;
        if (SoyGameController.Instance.IsCaca())
            PlayAudio(feralJakAudio, tantrum, true);
        else
        {
            PlayAudio(feralJakAudio, seething, true);
            thump.Play();
        }
        NightManager.Instance.RaiseGlobalAggro(4);
        while (isSeething)
        {
            if (OfficeController.Instance.cellBarsHP <= 0)
            {
                if (SoyGameController.Instance.IsCaca())
                    thump.Stop();
                feralJakAudio.volume = origVolume;
                Screamer();
                yield break;
            }
            yield return null;
        }
        if (SoyGameController.Instance.IsCaca())
            thump.Stop();
        feralJakAudio.volume = origVolume;
    }

    private void PlayAudio(AudioSource source, AudioClip clip, bool loop = false, bool randomStartTime = false)
    {
        source.Stop();
        source.clip = clip;
        source.loop = loop;
        if (!randomStartTime)
        {
            source.time = 0f;
            if (!loop)
            {
                source.PlayOneShot(clip);
                return;
            }
        }
        else
            source.time = Random.Range(0f, clip.length / 2f);
        source.Play();
    }

    private IEnumerator StartTabletSong()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        NightManager.Instance.UpdateCalm(1);
        NightManager.Instance.RaiseGlobalAggro(5);
        if (SoyGameController.Instance.IsCaca())
            PlayAudio(tabletAudio, cacaSongs[Random.Range(0, cacaSongs.Length)], true, true);
        else
            PlayAudio(tabletAudio, tabletSongs[Random.Range(0, SoyGameController.Instance.cuckVersion? tabletSongs.Length - 2 : tabletSongs.Length)], true, true);

    }

    private IEnumerator WatchTabletRoutine()
    {
        ShowSleepSprite(6);
        StartCoroutine(StartTabletSong());
        yield return new WaitForSeconds(30f);
        DropTablet();
        FallAsleep();
        awake = false;
    }

    private void Screamer()
    {
        JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Feral);
        EndNight();
    }

    private void ShowSleepSprite(int index)
    {
        for (int i = 0; i < feralSprites.Length; i++)
            feralSprites[i].SetActive(i == index);
    }

    private void Update()
    {
        ToggleMuteAudio(playerCam.currentLookDirection != LookDirection.Right &&
            playerCam.currentLookDirection != LookDirection.FrontToRight);
    }

    private void ToggleMuteAudio(bool muted)
    {
        if (feralSprites[7].activeSelf)
        {
            if (!SoyGameController.Instance.IsCaca())
                thump.mute = muted;
            if (muted)
                return;
        }
        if (feralJakAudio.mute != muted)
            feralJakAudio.mute = muted;
    }

    public void EndNight()
    {
        StopAudio();
        CancelInvoke("DecreaseSleepMeter");
        StopAllCoroutines();
        hasTablet = false;
        awake = false;
        isSeething = false;
        sleepMeter = 0;
        UpdateSleepState();
        feralSprites[0].SetActive(false);
    }

    public void StopAudio()
    {
        if (!SoyGameController.Instance.IsCaca())
        {
            AudioSource thump = feralSprites[feralSprites.Length - 1].transform.GetChild(0).GetComponent<AudioSource>();
            thump.Stop();
        }
        tabletAudio.Stop();
        feralJakAudio.Stop();
    }

    public bool IsActiveTonight()
    {
        return !(NightManager.Instance.currNight == 0 && baseAggro[0] == 0);
    }
}
