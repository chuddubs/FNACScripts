using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class SchizoCall : MonoBehaviour
{
    private int[] sequence = new int[6]{0, 0, 0, 0, 0, 0};
    public GameObject      txtCanvas;
    public TextMeshProUGUI seqText;
    public GameObject      scribble;
    private NightManager nm;
    private ViewManager vm;
    private SoyGameController sgc;
    private Coroutine schizo;
    private Coroutine rapeRoutine;
    private int index = 0;
    private bool listeningCamSwitch = false;
    private bool playingCall = false;
    private bool hungUpPhone = false;
    public GameObject[] rapey;
    public AudioClip marge;
    public AudioClip heavyBreathing;
    public AudioClip track;
    public AudioClip cobsteps;
    public AudioClip voiceline;
    public AudioClip pantsBuckle;


    private void Awake()
    {
        nm = NightManager.Instance;
        sgc = SoyGameController.Instance;
        vm = ViewManager.Instance;
        vm.onCamSwitched += OnCamSwitched;
        nm.onNightStart += RollToActivate;
        nm.onNightEnd += AbortCall;
        nm.onNightEnd += Deactivate;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        vm.onCamSwitched -= OnCamSwitched;
        nm.onNightStart -= RollToActivate;
        nm.onNightEnd -= AbortCall;
        nm.onNightEnd -= Deactivate;     
    }

    public void RollToActivate(int n)
    {
        if (Random.Range(0f, 100f) < n * 2f)
            Activate();
    }

    public void Activate()
    {
        InitRandomSequence();
        listeningCamSwitch = true;
        index = 0;
        txtCanvas.SetActive(true);
        scribble.SetActive(false);
        nm.onHangUp += HungUp;
    }

    public void Deactivate()
    {
        listeningCamSwitch = false;
        index = 0;
        txtCanvas.SetActive(false);
        scribble.SetActive(true);
        nm.onHangUp -= HungUp;
        playingCall = false;
        hungUpPhone = false;
    }
    
    private void InitRandomSequence()
    {
        char[] characters = new char[6] {'Ϫ', '2', '4', '5', '6', '7'};
        char lastChar = ' ';
        int [] toInt = {14, 2, 4, 5, 6, 7, 9};
        string result = "";
        int rindex;
        for (int i = 0; i < 6; i++)
        {
            do
            {
                rindex = Random.Range(0, 6);
            } while (characters[rindex] == lastChar);
            lastChar = characters[rindex];
            sequence[i] = toInt[rindex];
            result += characters[rindex];
        }
        seqText.text = result;
    }

    private void HungUp()
    {
        if (playingCall)
            hungUpPhone = true;
    }

    private void AbortCall()
    {
        if (schizo != null)
        {
            nm.StopPhoneCall(false);
            StopCoroutine(schizo);
            schizo = null;
        }
        if (rapeRoutine != null)
        {
            OfficeController.Instance.doorAudio.Stop();
            OfficeController.Instance.doorway.Stop();
            OfficeController.Instance.fromOrToOffice.Stop();
            StopCoroutine(rapeRoutine);
            foreach (GameObject g in rapey)
                g.SetActive(false);
            rapeRoutine = null;
        }
        Deactivate();
    }

    private void OnCamSwitched(int id)
    {
        if (!listeningCamSwitch)
            return;
        if (id > 0)
            index = id == sequence[index] ? index + 1 : 0;
        if (index >= 6)
        {
            listeningCamSwitch = false;
            schizo = StartCoroutine(ReceiveSchizoCall());
        }
    }

    private void Update()
    {
        if (playingCall)
        {
            if (sgc.inMenu || sgc.inMinigame || (nm.inTransition && !hungUpPhone))
                AbortCall();
        }
    }

    private IEnumerator ReceiveSchizoCall()
    {
        while (nm.phoneAudio.isPlaying)
            yield return null;
        yield return new WaitForSeconds(Random.Range(2f, 4f));
        playingCall = true;
        nm.phoneAudio.clip = marge;
        nm.phoneAudio.time = 0f;
        nm.phoneAudio.Play();
        float timer = marge.length;
        while (timer > 0f)
        {
            if (hungUpPhone)
            {
                foreach(Jak jak in nm.jaks)
                    jak.EndNight();
                FeralManager.Instance.EndNight();
                SquirrelManager.Instance.EndNight();
                AudioManager.Instance.StopAmbience();
                AudioManager.Instance.ambience.PlayOneShot(track);
                nm.StopAllCoroutines();
                nm.CancelInvoke("Timer");
                rapeRoutine = StartCoroutine(RapeRoutine());
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        AchievementsManager.Instance.UnlockChievo("secretchievo6");
    }

    private IEnumerator RapeRoutine()
    {
        OfficeController oc = OfficeController.Instance;
        oc.power.powerLeft = 0f;
        oc.power.Outage();
        nm.inTransition = true;
        yield return new WaitForSeconds(2.75f);
        oc.PlayFootSteps(0, cobsteps);
        yield return new WaitForSeconds(2.5f);
        rapey[0].SetActive(true);
        oc.PlayFootSteps(0, heavyBreathing);
        yield return new WaitForSeconds(3f);
        oc.PlayFootSteps(0, cobsteps);
        yield return new WaitForSeconds(1.5f);
        oc.doorway.Stop();
        rapey[0].SetActive(false);
        rapey[1].SetActive(true);
        oc.PlayFootSteps(1, voiceline);
        yield return new WaitForSeconds(voiceline.length);
        rapey[1].SetActive(false);
        rapey[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(oc.MoveDoor(false));
        yield return new WaitForSeconds(0.5f);
        rapey[2].SetActive(false);
        rapey[3].SetActive(true);
        oc.PlayFootSteps(1, pantsBuckle);
        yield return new WaitForSeconds(pantsBuckle.length);   
        rapey[3].SetActive(false);
        rapey[4].SetActive(true);
        oc.PlayFootSteps(1, heavyBreathing);
        yield return new WaitForSeconds(1f);
        oc.PlayFootSteps(1, cobsteps);
        yield return new WaitForSeconds(0.8f);   
        rapey[4].SetActive(false);
        rapey[5].SetActive(true);
        yield return new WaitForSeconds(0.8f);   
        rapey[5].SetActive(false);
        rapey[6].SetActive(true);
        yield return new WaitForSeconds(0.8f);   
        rapey[5].SetActive(false);
        rapey[6].SetActive(true);
        yield return new WaitForSeconds(0.8f);   
        rapey[6].SetActive(false);
        rapey[7].SetActive(true);
        yield return new WaitForSeconds(1.5f);   
        rapey[7].SetActive(false);
        JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Rapeson);
        Deactivate();
        yield break;
    }

}
