using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singletroon<AudioManager>
{
    private AudioSource audioSource;
    public AudioClip[]    jakInView;
    public AudioSource  ambience;
    public AudioClip[]  ambienceTracks;
    public AudioClip[]  ambienceTracksCaca;
    public AudioSource  slowburnSource;
    public AudioClip[]  slowburn;
    public Transform[]  slowburnSpots;
    public AudioSource  buzzing;
    public AudioSource  cacaStorySource;
    public AudioClip[]  cacaStories;
    public AudioClip nightTransition;
    public AudioClip openCams;
    public AudioClip cameraSwitch;
    public AudioClip toggleNightVision;
    public  AudioClip leaveCamsSound;
    public AudioClip  gameOverMusic;
    public AudioClip  chudboreaMusic;
    public AudioClip  flickeringLights;
    private Coroutine ambienceRoutine;
    private Coroutine ambienceLerp;
    private Coroutine slowburnRoutine;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Stop()
    {
        CancelInvoke();
        if (cacaStorySource.isPlaying)
            cacaStorySource.Stop();
        if (audioSource.isPlaying)
            audioSource.Stop();
        if (ambienceRoutine != null)
        {
            StopCoroutine(ambienceRoutine);
            ambienceRoutine = null;
        }
        if (ambienceLerp != null)
        {
            StopCoroutine(ambienceLerp);
            ambienceLerp = null;
        }
        if (ambience.isPlaying)
            ambience.Stop();
        if (slowburnRoutine != null)
        {
            StopCoroutine(slowburnRoutine);
            slowburnRoutine = null;
        }
        if (slowburnSource.isPlaying)
            slowburnSource.Stop();
    }

    public void PlayStory()
    {
        cacaStorySource.clip = cacaStories[Random.Range(0, cacaStories.Length)];
        cacaStorySource.time = 0f;
        cacaStorySource.Play();
        QueueNewStory();
    }

    public void StopStory()
    {
        CancelInvoke();
        if (cacaStorySource.isPlaying)
            cacaStorySource.Stop();
    }

    private void QueueNewStory()
    {
        Invoke("PlayStory", cacaStorySource.clip.length + 4f);
    }

    public void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayGameOver()
    {
        bool chud = JumpscaresManager.Instance.chudborea.activeSelf;
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.loop = chud;
        audioSource.clip = chud ? chudboreaMusic : gameOverMusic;
        audioSource.time = Mathf.Min(2f, gameOverMusic.length);
        audioSource.Play();
    }

    public void PlayOpenCams()
    {
        audioSource.PlayOneShot(openCams, 0.75f);
    }

    public void PlayCameraSwitch()
    {
        audioSource.PlayOneShot(cameraSwitch);
    }

    public void PlayNightVision()
    {
        audioSource.PlayOneShot(toggleNightVision);
    }

    public void PlayLeaveCams()
    {
        audioSource.PlayOneShot(leaveCamsSound, 0.25f);
    }

    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void LightFlicker()
    {
        audioSource.PlayOneShot(flickeringLights);
    }

    public void PlayJakInView()
    {
        audioSource.PlayOneShot(jakInView[Random.Range(0, jakInView.Length)]);
    }

    public void PlaySlowburnEffects()
    {
        slowburnRoutine = StartCoroutine(Slowburn());
    }

    private IEnumerator Slowburn()
    {
        while (true)
        {
            slowburnSource.clip = slowburn[Random.Range(0, slowburn.Length)];
            slowburnSource.transform.position = slowburnSpots[Random.Range(0, slowburnSpots.Length)].position;
            slowburnSource.Play();
            yield return new WaitForSeconds(slowburnSource.clip.length);
            yield return new WaitForSeconds(Random.Range(6f, 28f));
        }
    }

    public void PlayAmbienceTrack()
    {
        ambienceRoutine = StartCoroutine(PlayAmbience());
    }

    private IEnumerator PlayAmbience()
    {
        float startVol = 0f;
        if (ambienceLerp != null)
        {
            startVol = ambience.volume;
            StopCoroutine(ambienceLerp);
        }
        ambienceLerp = StartCoroutine(LerpAmbienceVolume(startVol, 0.1f, 1.5f));
        while (true)
        {
            ambience.clip = SoyGameController.Instance.IsCaca() ? ambienceTracksCaca[Random.Range(0, ambienceTracksCaca.Length)] : ambienceTracks[Random.Range(0, ambienceTracks.Length)];
            ambience.time = 0f;
            ambience.Play();
            yield return new WaitForSeconds(ambience.clip.length);
        }
    }

    private IEnumerator LerpAmbienceVolume(float from, float to, float duration)
    {
        if (from == 0f)
            ambience.Play();
        float t = 0;
        while(t < 1)
        {
                t += Time.smoothDeltaTime / duration;
                ambience.volume = Mathf.Lerp(from, to, t);
                yield return null;
        }
        if (to == 0f)
            ambience.Stop();
    }

    public void StopAmbience()
    {
        if (ambienceRoutine != null)
        {
            StopCoroutine(ambienceRoutine);
            ambienceRoutine = null;
        }
        float startVol = 0.1f;
        if (ambienceLerp != null)
        {
            startVol = ambience.volume;
            StopCoroutine(ambienceLerp);
            ambienceLerp = null;
        }
        ambienceLerp = StartCoroutine(LerpAmbienceVolume(startVol, 0.0f, 1.5f));
    }
}
