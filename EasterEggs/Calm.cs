using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Collider))]
public class Calm : MonoBehaviour
{
    public Sprite baseImage;
    public Sprite cacaImage;
    private KeyboardEasterEgg egg;
    public GameObject movie;
    private NightManager nm;
    
    private void Awake()
    {
        egg = GetComponent<KeyboardEasterEgg>();
        nm = NightManager.Instance;
        nm.onNightStart += SwitchPosterVersion;
    }

    private void SwitchPosterVersion(int n)
    {
        bool caca = SoyGameController.Instance.IsCaca();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (caca)
            sr.sprite = cacaImage;
        else
            sr.sprite = baseImage;
    }

    private void OnDestroy()
    {
        nm.onNightStart -= SwitchPosterVersion;
    }

    private void OnMouseOver()
    {
        egg.enabled = !nm.inTransition && !SoyGameController.Instance.inPause;
    }

    private void OnMouseExit()
    {
        egg.enabled = false;
    }

    public void CalmScene()
    {
        if (!movie.activeSelf)
        {
            movie.SetActive(true);
            nm.inTransition = true;
            StartCoroutine(PlayCalm());
        }
    }

    private IEnumerator PlayCalm()
    {
        VideoPlayer vp = movie.GetComponent<VideoPlayer>();
        vp.Prepare();
        while (!vp.isPrepared)
            yield return null;
        NightManager.Instance.phoneAudio.Stop();
        NightManager.Instance.ExitNight((float)vp.clip.length);
        vp.time = 0f;
        vp.Play();
        yield return new WaitForSeconds((float)vp.clip.length);
        movie.SetActive(false);
        yield return new WaitForSeconds(1f);
        AchievementsManager.Instance.UnlockChievo("chievo11");
    }

    void Update()
    {
        if (nm.inTransition)
        {
            if (egg.enabled)
                egg.enabled = false;
        }
    }
}
