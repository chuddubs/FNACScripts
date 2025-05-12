using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MinigamesManager : Singletroon<MinigamesManager>
{
    public GameObject[] minigames;
    public GameObject   schizoGame;
    public GameObject retroFilter;
    public GameObject redLines;
    public AudioSource redLinesAudio;
    public GameObject whiteLines;
    private GameObject currMinigame;
    private ImageAnimation[] whiteLinesAnims;
    private ImageAnimation currentlyPlayingWhiteLinesAnim;
    private float elapsedTime = 0f;
    private float timeBeforeOverlayAnim = 0f;
    public bool inMinigame = false;
    private bool schizo = false;
    private char[] gaems => new char[6]{'M', 'A', 'R', 'G', 'E', '?'};

    private void Awake()
    {
		whiteLinesAnims = whiteLines.GetComponentsInChildren<ImageAnimation>(true);
        // Debug.Log("Found " + whiteLinesAnims.Length + " white lines anims");
    }

    private IEnumerator PlayTransition(Action callback = null)
    {
        redLinesAudio.Play();
		redLines.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        redLines.SetActive(false);
        redLinesAudio.Stop();
        if (callback != null)
            callback();
    }

    public void LaunchMinigame(bool _schizo = false)
    {
        schizo = _schizo;
        retroFilter.SetActive(true);
        StartCoroutine(PlayTransition(BeforeMinigame));
    }

    private void BeforeMinigame()
    {
        if (schizo)
            currMinigame = Instantiate(schizoGame, transform);
        else
            currMinigame = Instantiate(minigames[NightManager.Instance.currNight - 1], transform);
        currMinigame.transform.SetSiblingIndex(0);
        elapsedTime = 0f;
		timeBeforeOverlayAnim = UnityEngine.Random.Range(2f, 4f);
        inMinigame = true;
        whiteLines.SetActive(true);
    }

    private void Update()
    {
        if (!inMinigame)
            return;
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= timeBeforeOverlayAnim && currentlyPlayingWhiteLinesAnim == null)
		{
			int index = UnityEngine.Random.Range(0, whiteLinesAnims.Length);
            // Debug.Log("selected white lines anim #" + index);
			currentlyPlayingWhiteLinesAnim = whiteLinesAnims[index];
            currentlyPlayingWhiteLinesAnim.gameObject.SetActive(true);
			// whiteLines.SetActive(true);
		}
		if (currentlyPlayingWhiteLinesAnim != null && currentlyPlayingWhiteLinesAnim.IsLastFrame())
		{
			elapsedTime = 0f;
			timeBeforeOverlayAnim = UnityEngine.Random.Range(2f, 4f);
            currentlyPlayingWhiteLinesAnim.gameObject.SetActive(false);
			currentlyPlayingWhiteLinesAnim = null;
			// whiteLines.SetActive(false);
		}
    }

    public void EndMinigame(bool playTrans = true)
    {
        inMinigame = false;
		whiteLines.SetActive(false);
        Destroy(currMinigame);
        retroFilter.SetActive(false);
        if (playTrans)
            StartCoroutine(PlayTransition(AfterMinigame));
        else
            AfterMinigame();
    }

    private void CheckForChievo(int i)
    {
        if (schizo)
            i = 5;
        string prog = PlayerPrefs.GetString("ItsAnEnding");
        if (!prog.Contains(gaems[i]))
            prog += gaems[i];
        int questionMarkIndex = prog.IndexOf('?');
        if (questionMarkIndex != -1)
        {
            prog = prog.Remove(questionMarkIndex, 1);
            prog += "?";
        }
        PlayerPrefs.SetString("ItsAnEnding", prog);
        if (prog.Length == 6)
            AchievementsManager.Instance.UnlockChievo("chievo5");
    }

    private void AfterMinigame()
    {
        int nIndex = NightManager.Instance.currNight + (schizo ? 0 : 1);
        CheckForChievo(nIndex - 1);
        bool wingame = nIndex > FNACStatic.feraljakMusic;
        if (wingame)
            nIndex = FNACStatic.feraljakMusic;
        int oldProg = PlayerPrefs.GetInt("ReachedNight");
        if (oldProg < nIndex)
            PlayerPrefs.SetInt("ReachedNight", nIndex);
        SoyGameController.Instance.GoToMenu(wingame);
    }

}
