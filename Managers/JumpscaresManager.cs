using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class JumpscaresManager : Singletroon<JumpscaresManager>
{
    public GameObject  jumpScaresObj;
    public VideoClip   swede;
    public VideoClip   swede_caca;
    public AudioClip   swedeScream;
    public AudioClip   swedeScream_caca;
    public GameObject       swedeDeath;
    public VideoClip   chud;
    public VideoClip   chud_caca;
    public AudioClip   chudScream;
    public AudioClip   chudScream_caca;
    public GameObject       chudDeath;
    public VideoClip   fingerboyFront;
    public VideoClip   fingerboySide;
    public VideoClip   pibbles;
    public AudioClip   fingerboyScream;
    public AudioClip   pibblesBark;
    public GameObject       fingerboyDeath;
    public VideoClip   rapeson;
    public VideoClip   rabbison;
    public VideoClip   dollson;
    public AudioClip   rapesonScream;
    public GameObject       rapesonDeath;
    public VideoClip   feral;
    public VideoClip   feral_caca;
    public AudioClip   feralScream;
    public AudioClip   feralScream_caca;
    public GameObject  chudborea;
    public GameObject       feralDeath;
    private VideoPlayer vidPlayer;

    private  Dictionary<Jumpscare, VideoClip> variantToClip;
    private  Dictionary<Jumpscare, VideoClip> variantToCacaClip;
    private  Dictionary<Jumpscare, AudioClip> variantToAudio;
    private  Dictionary<Jumpscare, AudioClip> variantToCacaAudio;
    private  Dictionary<Jumpscare, char> variantToPref;
    private  Dictionary<Jumpscare, char> variantToPref_caca;

    public bool        waiting = false;
    private Coroutine jumpscareRoutine;
    private Coroutine halluRoutine;
    public VideoClip hallucinations;

    private void Awake()
    {
        vidPlayer = GetComponent<VideoPlayer>();
        NightManager.Instance.onNightEnd += EndNight;
        variantToClip = new Dictionary<Jumpscare, VideoClip>
        {
            { Jumpscare.Swede, swede },
            { Jumpscare.Chud, chud },
            { Jumpscare.FingerboyFront, fingerboyFront },
            { Jumpscare.FingerboySide, fingerboySide },
            { Jumpscare.Rapeson, rapeson },
            { Jumpscare.Feral, feral },
            { Jumpscare.Hyperborea, chud},
            { Jumpscare.Dollson, dollson}
        };
        variantToCacaClip = new Dictionary<Jumpscare, VideoClip>
        {
            { Jumpscare.Swede, swede_caca },
            { Jumpscare.Chud, chud_caca },
            { Jumpscare.FingerboyFront, pibbles },
            { Jumpscare.FingerboySide, pibbles },
            { Jumpscare.Rapeson, rabbison },
            { Jumpscare.Feral, feral_caca },
            { Jumpscare.Hyperborea, chud_caca},
            { Jumpscare.Dollson, rabbison}
        };
        variantToAudio = new Dictionary<Jumpscare, AudioClip>
        {
            { Jumpscare.Swede, swedeScream },
            { Jumpscare.Chud, chudScream },
            { Jumpscare.FingerboyFront, fingerboyScream },
            { Jumpscare.FingerboySide, fingerboyScream },
            { Jumpscare.Rapeson, rapesonScream },
            { Jumpscare.Feral, feralScream },
            { Jumpscare.Hyperborea, chudScream},
            { Jumpscare.Dollson, rapesonScream}
        };
        variantToCacaAudio = new Dictionary<Jumpscare, AudioClip>
        {
            { Jumpscare.Swede, swedeScream_caca },
            { Jumpscare.Chud, chudScream_caca },
            { Jumpscare.FingerboyFront, pibblesBark },
            { Jumpscare.FingerboySide, pibblesBark },
            { Jumpscare.Rapeson, rapesonScream },
            { Jumpscare.Feral, feralScream_caca },
            { Jumpscare.Hyperborea, chudScream_caca},
            { Jumpscare.Dollson, rapesonScream}
        };
        variantToPref =  new Dictionary<Jumpscare, char>
        {
            { Jumpscare.Swede, 'I' },
            { Jumpscare.Chud, 'C' },
            { Jumpscare.FingerboyFront, 'h' },
            { Jumpscare.FingerboySide, 's' },
            { Jumpscare.Rapeson, 'R' },
            { Jumpscare.Feral, 'F' },
            { Jumpscare.Hyperborea, 'C'},
            { Jumpscare.Dollson, 'D'}
        };
        variantToPref_caca =  new Dictionary<Jumpscare, char>
        {
            { Jumpscare.Swede, 'v' },
            { Jumpscare.Chud, 'z' },
            { Jumpscare.FingerboyFront, 'w' },
            { Jumpscare.FingerboySide, 'w' },
            { Jumpscare.Rapeson, 'x' },
            { Jumpscare.Feral, 'y' },
            { Jumpscare.Hyperborea, 'z'},
            { Jumpscare.Dollson, 'D'}
        };
        InvokeRepeating("RollForHallus", 20f, 20f);
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        NightManager.Instance.onNightEnd -= EndNight;
    }
    
    public void PlayJumpscare(Jumpscare variant)
    {
        // Debug.Log("Play " + variant + " jumpscare");
        if (jumpscareRoutine != null)
            return;
        if (halluRoutine != null)
            StopCoroutine(halluRoutine);
        jumpscareRoutine = StartCoroutine(PlayJumpscareRoutine(variant));
    }

    private void RollForHallus()
    {
        if (ViewManager.Instance.currId != 0)
            return;
        if (jumpscareRoutine == null && NightManager.Instance.RollForHallus())
            halluRoutine = StartCoroutine(PlayHallucinationsRoutine());
    }

    public void PlayHallucinations()
    {
        if (halluRoutine != null)
        {
            StopCoroutine(halluRoutine);
            halluRoutine = null;
        }
        halluRoutine = StartCoroutine(PlayHallucinationsRoutine());
    }

    private IEnumerator PlayHallucinationsRoutine()
    {
        if (vidPlayer.isPlaying)
            vidPlayer.Stop();
        vidPlayer.clip = hallucinations;
        vidPlayer.Prepare();
        while (!vidPlayer.isPrepared)
            yield return new WaitForEndOfFrame();
        vidPlayer.frame = 0;
        jumpScaresObj.SetActive(true);
        vidPlayer.Play();
        yield return new WaitForSeconds((float)hallucinations.length);
        jumpScaresObj.SetActive(false);
    }

    private void SetGameOverFromVariant(Jumpscare variant)
    {
        chudDeath.SetActive(variant == Jumpscare.Chud);
        swedeDeath.SetActive(variant == Jumpscare.Swede);
        fingerboyDeath.SetActive(variant == Jumpscare.FingerboyFront || variant == Jumpscare.FingerboySide);
        rapesonDeath.SetActive(variant == Jumpscare.Rapeson || variant == Jumpscare.Dollson);
        feralDeath.SetActive(variant == Jumpscare.Feral);
        chudborea.SetActive(variant == Jumpscare.Hyperborea);
    }

    private IEnumerator PlayJumpscareRoutine(Jumpscare variant)
    {
        bool caca = SoyGameController.Instance.IsCaca();
        ViewManager vm = ViewManager.Instance;
        VideoClip vclip = caca ? variantToCacaClip[variant] : variantToClip[variant];
        SetGameOverFromVariant(variant);
        if (vidPlayer.isPlaying)
            vidPlayer.Stop();
        vidPlayer.clip = vclip;
        vidPlayer.Prepare();
        while (!vidPlayer.isPrepared)
            yield return new WaitForEndOfFrame();
        vidPlayer.frame = 0;
        float timer = 15f;
        while (timer > 0f)
        {
            if (vm.currId == 0)
                break;
            timer -= Time.deltaTime;
            yield return null;
        }
        if (vm.currId != 0)
            vm.SetViewCam(0);
        NightManager.Instance.StopPhoneCall(false, true);
        AudioManager.Instance.Play(caca ? variantToCacaAudio[variant] : variantToAudio[variant]);
        NightManager.Instance.inTransition = true;
        if (variant == Jumpscare.Chud || variant == Jumpscare.Hyperborea)
            yield return new WaitForSeconds(1f);
        Camera.main.GetComponent<StressReceiver>().InduceStress(0.75f);
        jumpScaresObj.SetActive(true);
        vidPlayer.Play();
        yield return new WaitForSeconds((float)vclip.length);
        CheckForChievo(caca? variantToPref_caca[variant] : variantToPref[variant]);
        vidPlayer.Stop();
        jumpScaresObj.SetActive(false);
        NightManager.Instance.EndNight(true);
    }

    private void EndNight()
    {
        CancelInvoke("RollForHallus");
        StopAllCoroutines();
        jumpscareRoutine = null;
        halluRoutine = null;
    }

    private void CheckForChievo(char c)
    {
        string prog = PlayerPrefs.GetString("DOCTOS");
        if (!prog.Contains(c))
            prog += c;
        PlayerPrefs.SetString("DOCTOS", prog);
        if (prog.Length >= 12)
            AchievementsManager.Instance.UnlockChievo("chievo19");
    }

}

public enum Jumpscare
{
    Swede,
    Chud,
    FingerboyFront,
    FingerboySide,
    Rapeson,
    Feral,
    Hyperborea,
    Dollson
}
