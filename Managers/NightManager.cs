using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class NightManager : Singletroon<NightManager>
{
    public bool dollSon;
    public System.Action<int> onNightStart;
    public System.Action onNightEnd;
    public System.Action onHangUp;
    public Spot inactiveSpot;
    public int currHour = 0;
    public int currNight = 1;
    private int currCalmMeter = 0;
    public GameObject calmMeter;
    public GameObject[] calmDisplays;
    public Jak[] jaks;
    public GameObject     cobCellPoster;
    public GameObject     feralCellPoster;
    public TextMeshProUGUI nightDisplay;
    public TextMeshProUGUI timeDisplay;
    public GameObject       powerDisplays;
    public VideoClip[]       morningScreens;
    public VideoClip[]       morningScreens_caca;
    public CanvasGroup  nightTransition;
    public CanvasGroup  blackScreen;
    public CanvasGroup  gameOverScreen;
    // public GameObject   devcheats;
    public GameObject   tutorial;
    public GameObject   prenight;
    public GameObject[] prenightImgs;
    public GameObject   gameOverPp;
    public GameObject   skipGameOverButton;
    public bool         skippedGameOver = false;
    // [HideInInspector]
    public bool        inTransition = true;
    private string[]    hoursText = {"12 AM", "1 AM", "2 AM", "3 AM", "4 AM", "5 AM", "6 AM"};
    private string[]    hoursText_Caca = {"8:" , "3:", "4:", "5:", "6:", "7:"};
    private string[]    minutesText_Caca = {"00 PM", "10 PM", "20 PM", "30 PM", "40 PM", "50 PM", "CACA"};
    private string[]    dateText = {"HWABAG", "28/3/88", "29/3/88", "30/3/88", "31/3/88", "1/4/88"};

    [Header("Dolly Doll's manly voice")]
    public AudioSource phoneAudio;
    public AudioClip[] calls;
    public AudioClip[] calls_caca;
    public AudioClip   hangUpSfx;
    public SchizoCall schizoCall;
    private SoyGameController sgc;
    void Start()
    {
        sgc = SoyGameController.Instance;
    }

    public void Fade(float from, float to, float duration)
    {
        StartCoroutine(LerpCanvasGroup(blackScreen, from, to, duration));
    }

    public void StartNight(int index)
    {
        currNight = index;
        dollSon = SoyGameController.Instance.IsCaca() == false && currNight == 5 && (UnityEngine.Random.Range(0f, 10f) <= 1f);

        currHour = 0;
        currCalmMeter = 0;
        UpdateCalm(0);
        TogglePosters();
        powerDisplays.SetActive(true);
        calmMeter.SetActive(true);
        blackScreen.alpha = 1f;
        ViewManager.Instance.SetViewCam(0);
        bool caca = SoyGameController.Instance.IsCaca();
        nightDisplay.text = caca ? "C/A/CA" : dateText[currNight];
        timeDisplay.text = caca ? hoursText_Caca[currNight].ToString() + minutesText_Caca[0].ToString() : hoursText[Math.Clamp(currHour, 0, 6)];
        OfficeController.Instance.StartNight();
        StartCoroutine(StartNightFade());
    }

    public bool IsCobbyActive()
    {
        Jak cob = jaks.FirstOrDefault(j => j.variant == Variant.Rapeson);

        return cob != null && cob.GetComponentsInChildren<Behavior>().FirstOrDefault(behavior => behavior.night == 0).startingAggro == 0;
    }

    private void TogglePosters()
    {

        bool rapeyActive = true;
        if (currNight == 0 && !IsCobbyActive())
            rapeyActive = false;
        bool on = currNight <= 2 && !rapeyActive;
        cobCellPoster.SetActive(on);
        feralCellPoster.SetActive(on);
    }

    private void TogglePreNightImgByIndex()
    {
        for (int i = 0; i < 6; i++)
            prenightImgs[i].SetActive(i == currNight);

    }

    private void PlayPhoneCall()
    {
        if (currNight == 0)
            return;
        phoneAudio.clip = SoyGameController.Instance.IsCaca() ? calls_caca[currNight - 1] : calls[currNight - 1];
        phoneAudio.time = 0f;
        phoneAudio.Play();
    }

    public void StopPhoneCall(bool hangUp = true, bool force = false)
    {
        if (phoneAudio.isPlaying)
        {
            if (phoneAudio.time < 5 && !force)
                phoneAudio.time = 5f;
            else
            {
                phoneAudio.Stop();
                if (hangUp && !force)
                {
                    phoneAudio.PlayOneShot(hangUpSfx);
                    if (onHangUp != null)
                        onHangUp();
                }
            }
        }
    }

    private IEnumerator StartNightFade()
    {
        inTransition = true;
        PlayerCam.Instance.enabled = false;
        prenight.SetActive(true);
        TogglePreNightImgByIndex();
        schizoCall.RollToActivate(currNight);
        yield return LerpCanvasGroup(blackScreen, 1f, 0f, 1f);
        while (true)
        {
            if (Input.anyKeyDown)
                break;
            yield return null;
        }
        yield return LerpCanvasGroup(blackScreen, 0f, 1f, 1.5f);
        prenight.SetActive(false);
        // devcheats.SetActive(true);
        tutorial.SetActive(PlayerPrefs.GetInt("TutorialDone") == 0);
        OfficeController.Instance.ResetPower();
        OfficeController.Instance.computerBuzz.mute = false;
        OfficeController.Instance.computerBuzz.Play();
        if (onNightStart != null)
            onNightStart(currNight);
        yield return LerpCanvasGroup(blackScreen, 1f, 0f, 1f);
        PlayerCam.Instance.enabled = true;
        inTransition = false;
        InvokeRepeating("Timer", 80f, 80f);
        yield return new WaitForSeconds(3f);
        PlayPhoneCall();
        AudioManager.Instance.PlaySlowburnEffects();
    }

    private void Timer()
    {        
        currHour++;
        if (currHour >= 6)
        {
            EndNight();
            CancelInvoke("Timer");
            return;
        }
        NextHour();
        // Debug.Log("Next Hour");
    }
    
    private void NextHour()
    {
        timeDisplay.text = SoyGameController.Instance.IsCaca() ? hoursText_Caca[currNight] + minutesText_Caca[currHour] : hoursText[currHour];
        if (currHour == 2 || currHour == 4)
        {
            UpdateCalm(1);
            foreach(Jak jak in jaks)
                jak.GetAggroGainHour(currHour);
            FeralManager.Instance.RaiseAggro(currNight, currHour);
        }
    }

    public void RaiseNearbyAggro(Jak source, int gain)
    {
        foreach (Jak j in jaks.Where(jak => jak.CurrentSpot.Cam == source.CurrentSpot.Cam))
            j.currentAggro += j == source ? 0 : gain;
    }

    public void RaiseGlobalAggro(int gain, Jak source = null)
    {
        foreach (Jak j in jaks.Where(jak => jak.IsActiveTonight()))
            j.currentAggro += (source == null || source != j) ? gain : 0;
    }

    public void EndNight(bool gameOver = false, bool schizoGame = false)
    {
        // devcheats.SetActive(false);
        if (phoneAudio.isPlaying)
            phoneAudio.Stop();
        AudioManager.Instance.StopStory();
        CancelInvoke("Timer");
        StopAllCoroutines();
        if (!gameOver && !schizoGame)
            AchievementsManager.Instance.Survived(currNight);
        if (onNightEnd != null)
            onNightEnd();
        // currNight = Math.Clamp(currNight + (gameOver ? 0 : 1), 1, FNACStatic.numberNights);
        if (gameOver)
            StartCoroutine(PlayGameOver());
        else
        {
            if (schizoGame)
                EndNightSchizo();
            else   
                StartCoroutine(NightTransitionScreen());
        }
    }

    public void SkipGameOver()
    {
        skippedGameOver = true;
    }

    private IEnumerator PlayGameOver()
    {
        // Debug.Log("got killed");
        inTransition = true;
        powerDisplays.SetActive(false);
        calmMeter.SetActive(false);
        skipGameOverButton.SetActive(false);
        FNACStatic.SetPos(Camera.main.transform, SoyGameController.Instance.cameraSpot);
        GameObject chudBorea = JumpscaresManager.Instance.chudborea;
        bool chudEnding = chudBorea.activeSelf;
        ViewManager.Instance.ShowMoveGlitch();
        if (!chudEnding)
            yield return LerpCanvasGroup(blackScreen, 0f, 1f, 2.5f);
        gameOverScreen.alpha = 1f;
        if (!chudEnding)
            StartCoroutine(LerpCanvasGroup(blackScreen, 1f, 0f, 2.5f));
        AudioManager.Instance.PlayGameOver();
        if (chudEnding)
        {
            VideoPlayer vp = chudBorea.GetComponent<VideoPlayer>();
            vp.Prepare();
            while (!vp.isPrepared)
                yield return null;
            vp.Play();
            AchievementsManager.Instance.UnlockChievo("chievo10");
        }
        gameOverPp.SetActive(!chudEnding);
        float timer = 4f;
        while (!skippedGameOver)
        {
            if (timer < 0f && !skipGameOverButton.activeSelf)
                skipGameOverButton.SetActive(true);
            else
                timer -= Time.deltaTime;
            yield return null;
        }
        skippedGameOver = false;
        gameOverScreen.alpha = 0f;
        // yield return LerpCanvasGroup(gameOverScreen, 1f, 0f, 0.5f);
        gameOverPp.SetActive(false);
        skipGameOverButton.SetActive(false);
        AudioManager.Instance.Stop();
        AudioManager.Instance.buzzing.Stop();
        SoyGameController.Instance.GoToMenu();
    }

    public void        ExitNight(float delay = 0.15f)
    {
        // devcheats.SetActive(false);
        inTransition = true;
        StopPhoneCall(false, true);
        CancelInvoke("Timer");
        StopAllCoroutines();
        ViewManager.Instance.SetViewCam(0);
        AudioManager.Instance.Stop();
        if (onNightEnd != null)
            onNightEnd();  
        powerDisplays.SetActive(false);
        calmMeter.SetActive(false);
        skipGameOverButton.SetActive(false);
        // FNACStatic.SetPos(Camera.main.transform, SoyGameController.Instance.cameraSpot);
        AudioManager.Instance.Stop();
        Invoke("DelayBackToMenu", delay);
    }

    private void DelayBackToMenu()
    {
        SoyGameController.Instance.GoToMenu();
    }

    private void EndNightSchizo()
    {
        ViewManager.Instance.SetViewCam(0);
        AudioManager.Instance.Stop();  
        MinigamesManager.Instance.LaunchMinigame(true);
    }

    private IEnumerator NightTransitionScreen()
    {
        // AudioManager.Instance.PlayNightTransition();
        // blackScreen.alpha = 1f;
        Debug.Log("NightTransitionScreen");
        VideoPlayer vp = nightTransition.GetComponent<VideoPlayer>();
        inTransition = true;
        if (vp.isPlaying)
            vp.Stop();
        vp.clip = SoyGameController.Instance.IsCaca() ? morningScreens_caca[currNight] : morningScreens[currNight];
        vp.Prepare();
        while (!nightTransition.GetComponent<VideoPlayer>().isPrepared)
            yield return new WaitForEndOfFrame();
        Debug.Log("Done preparing video");
        yield return LerpCanvasGroup(blackScreen, 0f, 1f, 2f);
        ViewManager.Instance.SetViewCam(0);
        AudioManager.Instance.Stop();
        AudioManager.Instance.buzzing.Stop();
        nightTransition.GetComponent<VideoPlayer>().frame = 0;
        nightTransition.GetComponent<VideoPlayer>().Play();
        RawImage img = nightTransition.GetComponent<RawImage>();
        img.enabled = true;
        yield return LerpCanvasGroup(blackScreen, 1f, 0f, 1f);
        yield return new WaitForSeconds((float)vp.clip.length - 1f);
        if (currNight > 0)
            AchievementsManager.Instance.UnlockChievo("beatn" + currNight.ToString());
        yield return LerpCanvasGroup(nightTransition, 0f, 1f, 1.5f);
        nightTransition.GetComponent<VideoPlayer>().Stop();
        img.enabled = false;
        if (currNight > 0)
            MinigamesManager.Instance.LaunchMinigame();
        else
        {
            CustomNightManager.Instance.BeatPreset();
            SoyGameController.Instance.GoToMenu(false);
        }
        StartCoroutine(LerpCanvasGroup(blackScreen, 1f, 0f, 1.5f));
    }

    bool atLeastOnejakWasNearby = false;
    bool oneJakInOffice = false;

    public bool RollForHallus()
    {
        float chance = Math.Max(currNight, 1) * 0.67f * (IsAnyJakCloseForHallus() ? 1.5f : 1f);
        return UnityEngine.Random.Range(0f, 100f) <= chance;
    }

    public bool IsAnyJakCloseForHallus()
    {
        return jaks.Any(j => IsJakNearby(j) && j.currentSpot.Cam.id != -3);
    }

    public bool IsJakNearby(Jak j, bool includeCams = true)
    {
        if (!includeCams)
            return j.currentSpot.Cam.id <= 0 && j.currentSpot != inactiveSpot && j.currentSpot.Cam.id != -11;
        return j.currentSpot.Cam.id == 7 || j.currentSpot.Cam.id == 11 ||
        (j.currentSpot.Cam.id <= 0 && j.currentSpot != inactiveSpot && j.currentSpot.Cam.id != -11);
    }

    private void CheckAnyJakNearby()
    {
        bool isAnyJakNearby = jaks.Any(j => IsJakNearby(j));
        if (!atLeastOnejakWasNearby && isAnyJakNearby)
            AudioManager.Instance.PlayAmbienceTrack();
        if (atLeastOnejakWasNearby && !isAnyJakNearby)
           AudioManager.Instance.StopAmbience();
        atLeastOnejakWasNearby = isAnyJakNearby;
    }

    private void CheckAnyJakOffice()
    {
        bool isAnyJakinOffice = jaks.Any(j => j.IsInOffice && j.variant != Variant.Rapeson && j.variant != Variant.Troon);
        if (!oneJakInOffice && isAnyJakinOffice)
            AudioManager.Instance.buzzing.Play();
        if (oneJakInOffice && !isAnyJakinOffice)
            AudioManager.Instance.buzzing.Stop();
        oneJakInOffice = isAnyJakinOffice;
    }

    private void Update()
    {
        // if (inTransition || !sgc.InGame())  
        //     return;
        // if (Input.GetKeyDown(KeyCode.X))
        //     EndNight();
        // if (Input.GetKeyDown(KeyCode.C))
        //     Timer();
        // if (Input.GetKeyDown(KeyCode.V))
        //     OfficeController.Instance.FlickerLights();
        // if (Input.GetKeyDown(KeyCode.O))
        //     OfficeController.Instance.power.Outage(true);

        CheckAnyJakNearby();
        CheckAnyJakOffice();
    }

    public void UpdateCalm(int n)
    {
        currCalmMeter = Mathf.Clamp(currCalmMeter + n, 0, 6);
        for (int i = 0; i < 7; i++)
            calmDisplays[i].SetActive(i == currCalmMeter);
    }

    private IEnumerator LerpCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        while(t < 1)
        {
                t += Time.smoothDeltaTime / duration;
                cg.alpha = Mathf.Lerp(from, to, t);
                yield return null;
        }
    }
}
