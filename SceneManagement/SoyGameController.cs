using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SoyGameController : Singletroon<SoyGameController>
{
    public GameObject seizureWarning;
    public GameObject disclaimer;
    public GameObject introCinematic;
    public Transform cameraSpot;
    public GameObject gameInterface;
    public GameObject victoryScreen;
    public GameObject creditsScreen;
    public GameObject mainMenu;
    [SerializeField]
    private StreamerFriendlyToggle  cuckToggle;
    [SerializeField]
    private BundlesManager      bundlesManager;
    public bool     cuckVersion = false;
    public GameVersion version;
    public bool     inMenu = true;
    public bool     inMinigame = false;
    public bool     inPause = false;
    private bool skipwarning = false;
    private bool skipdisclaimer = false;
    private bool skipIntro = false;

    private void Awake()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
    }

    private void Start()
    {
        SoySettings.Instance.UpdateSettings();
        StartCoroutine(GameBoot());
    }

    public void SetVersion(GameVersion ver)
    {
        version = ver;
    }

    public void SkipWarning()
    {
        skipwarning = true;
    }

    public void SkipDisclaimer()
    {
        skipdisclaimer = true;
    }

    public void SkipIntro()
    {
        // Debug.Log("Skip Intro");
        skipIntro = true;
    }

    private IEnumerator PlayIntro()
    {
        introCinematic.SetActive(true);
        VideoPlayer vp = introCinematic.GetComponentInChildren<VideoPlayer>();
        vp.Prepare();
        while (!vp.isPrepared)
            yield return null;
        disclaimer.SetActive(false);
        vp.time = 0f;
        vp.Play();
        float timer = 48f;
        while (timer > 0f)
        {
            if (skipIntro)
                break;
            timer -= Time.deltaTime;
            yield return null;
        }
        introCinematic.SetActive(false);
    }

    private IEnumerator GameBoot()
    {
        NightManager.Instance.inTransition = true;
        seizureWarning.SetActive(true);
        // yield return bundlesManager.FetchBundles();
        float timer = 8f;
        while (timer > 0f)
        {
            if (skipwarning)
                break;
            timer -= Time.deltaTime;
            yield return null;
        }
        timer = 8f;
        disclaimer.SetActive(true);
        seizureWarning.SetActive(false);
        while (timer > 0f)
        {
            if (skipdisclaimer)
                break;
            timer -= Time.deltaTime;
            yield return null;
        }
        yield return PlayIntro();
        ToggleStreamerVersion(PlayerPrefs.GetInt("CuckVersion") != 0);
        GoToMenu();
    }

    public void Play(int n)
    {
        inMenu = false;
        mainMenu.SetActive(false);
        gameInterface.SetActive(true);
        NightManager.Instance.StartNight(Math.Min(FNACStatic.feraljakMusic, n));
    }

    public void SkipVictoryScreen()
    {
        victoryScreen.SetActive(false);
        gameInterface.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void GoToMenu(bool wingame = false)
    {
        inMenu = true;
        inMinigame = false;
        NightManager.Instance.inTransition = true;
        PlayerPrefs.SetInt("ReachedNight", Math.Min(FNACStatic.feraljakMusic, PlayerPrefs.GetInt("ReachedNight")));
        PlayerCam.Instance.enabled = false;
        FNACStatic.SetPos(Camera.main.transform, cameraSpot);
        if (wingame)
        {
            PlayerPrefs.SetInt("GameWon", 1);
            PlayVictoryScreen(FNACStatic.feraljakMusic == 2);
        }
        else
        {
            gameInterface.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void PlayVictoryScreen(bool demo = false)
    {
        gameInterface.SetActive(true);
        mainMenu.SetActive(false);
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(0).gameObject.SetActive(demo); //demo
        victoryScreen.transform.GetChild(1).gameObject.SetActive(!demo); //full credits
        if (!demo)
            StartCoroutine(RollCredits());
    }

    private IEnumerator RollCredits()
    {
        Animator anim = creditsScreen.GetComponent<Animator>();
        AudioSource creditsMusic = creditsScreen.GetComponent<AudioSource>();
        creditsScreen.SetActive(true);
        creditsMusic.time = 10f;
        creditsMusic.volume = 1f;
        creditsMusic.Play();
        float timer = anim.GetCurrentAnimatorStateInfo (0).length;
        float fadeOut = 0f;
        while (timer > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                break;
            if (timer < 5)
            {
                creditsMusic.volume = Mathf.Lerp(1f, 0f, fadeOut / 5f);
                fadeOut += Time.deltaTime;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        creditsMusic.Stop();
        creditsScreen.SetActive(false);
        SkipVictoryScreen();
    }
    
    public void ToggleStreamerVersion(bool on)
    {
        cuckVersion = on;
        cuckToggle.ToggleStreamer(cuckVersion);
        Cacafier.Instance.ToggleCaca(IsCaca());
    }

    public bool IsCaca()
    {
        return version == GameVersion.Caca;
    }

    public bool InGame()
    {
        return !inMenu && !inMinigame;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}