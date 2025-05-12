using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainMenuManager : MonoBehaviour
{
    public SoyGameController gameCtrlr;
    public TextMeshProUGUI   gameTitle;
    public TextMeshProUGUI   versionTitle;
    public GameObject        cacaNo;
    public GameObject        cacaYes;
    public GameObject        cuckToggle;
    public GameObject   wonAward;
    public GameObject   customAward;
    public GameObject   obsessedAward;
    public GameObject   bg;
    public GameObject   splitSecBg;
    private Coroutine   bgAnimation;
    public AudioClip    baseMusic;
    public AudioClip    cacaMusic;
    public AudioSource  musicAudioSource;
    public AudioSource  sfxAudioSource;
    public MenuCreator settingsController;
    public GameObject[]  playNightButtons;
    public GameObject    customNightButton;
    public GameObject    customNightMenu;
    public GameObject    awardsMenu;

    private void OnEnable()
    {
        SetVersionTitle();
        UpdateFromProgress();
        ToggleCaca(SoyGameController.Instance.IsCaca());
    }

    public void UpdateFromProgress()
    {
        wonAward.SetActive(PlayerPrefs.GetInt("GameWon") == 1);
        customAward.SetActive(PlayerPrefs.GetInt("WonPreset0") == 2);
        obsessedAward.SetActive(AchievementsManager.Instance.AllChievosWon());
        customNightButton.SetActive(PlayerPrefs.GetInt("GameWon") == 1);
        bgAnimation = StartCoroutine(AnimateBg());
        UpdateNightSelectorList();
    }

    private void PlayMusic()
    {
        musicAudioSource.clip = SoyGameController.Instance.IsCaca() ? cacaMusic : baseMusic;
        musicAudioSource.Play();
    }

    private void SetVersionTitle()
    {
        if (SoyGameController.Instance.IsCaca())
        {
            gameTitle.text = "Fibe Nites Ad Cwobsnons";
            versionTitle.text = "Cacaborea Edition";
        }
        else
        {
            gameTitle.text = "Five Nights At Cobson's";
            versionTitle.text = FNACStatic.feraljakMusic == 2 ? "Free Demo Version" : "";
        }
        
    }

    private void UpdateNightSelectorList()
    {
        int nIndex = Math.Min(FNACStatic.feraljakMusic, PlayerPrefs.GetInt("ReachedNight"));
        for (int i = 0; i < 5; i++)
            playNightButtons[i].SetActive(i < nIndex);
    }

    private IEnumerator AnimateBg()
    {
        while (musicAudioSource == null)
            yield return new WaitForEndOfFrame();
        musicAudioSource.time = 0;
        musicAudioSource.Play();
        float min = 0.15f;
        float max = 8f;
        // Bias factor controls the strength of the bias towards lower values
        float biasFactor = 2f; // Adjust this value as needed
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Pow(UnityEngine.Random.Range(0f, 1f), biasFactor) * (max - min) + min);
            splitSecBg.SetActive(true);
            int n = UnityEngine.Random.Range(2, 12);
            while (n > 0)
            {
                n--;
                yield return new WaitForEndOfFrame();
            }
            splitSecBg.SetActive(false);
            yield return null;
        }
    }

    public void BtClickPlay(int n)
    {
        musicAudioSource.Stop();
        gameCtrlr.Play(n);
    }

    public void BtClickCustomNight()
    {
        ToggleVCR(false);
        customNightMenu.SetActive(true);
    }

    public void BtClickAwards()
    {
        ToggleVCR(false);
        awardsMenu.SetActive(true);
    }

    public void BtClickAwardsBack()
    {
        ToggleVCR(true);
        awardsMenu.SetActive(false);
    }

    public void BtClickSettings()
    {
        settingsController.ShowMenu();
    }

    public void BtClickCredits()
    {
        SoyGameController.Instance.PlayVictoryScreen();
    }

    public void BtClickExit()
    {
        SoyGameController.Instance.Quit();
    }

    public void ToggleSubMenuPostProc(bool on)
    {
        Volume pp = GetComponentInChildren<Volume>();
        CRTVolume crt;
        pp.profile.TryGet(out crt);
        if (crt != null)
        {
            crt.Curve.overrideState = true;
            crt.Curve.value = on ? 0f : 1f;
        }
    }

    public void ToggleVCR(bool on)
    {
        ToggleSubMenuPostProc(!on);
        Volume pp = GetComponentInChildren<Volume>();
        VCRVolume vcr;
        pp.profile.TryGet(out vcr);
        if (vcr != null)
            vcr.active = on;
    }

    public void ToggleCaca(bool on)
    {
        SoyGameController.Instance.version = on ? GameVersion.Caca : GameVersion.Base;
        SetVersionTitle();
        cacaNo.SetActive(!on);
        cacaYes.SetActive(on);
        if (on)
            SoyGameController.Instance.ToggleStreamerVersion(true);
        PlayMusic();
        Cacafier.Instance.ToggleCaca(on);
        cuckToggle.SetActive(!on);
    }

    private void OnDisable()
    {
        if (bgAnimation != null)
        {
            StopCoroutine(bgAnimation);
            bgAnimation = null;   
        }
    }
}
