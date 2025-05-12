using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSquirrel : MonoBehaviour
{
    public float scrollSpeed = 100f;
    public Animator animator;
    public RectTransform squirrel;
    public RectTransform background;
    public RectTransform foreground;
    public RectTransform shoulder;
    public RectTransform asylum;
    public static Vector3 faceleft => new Vector3(-0.75f, 0.75f, 1f);
    private Vector3 faceRight => new Vector3(0.75f, 0.75f, 1f);
    private float clampedForegroundX;
    private float clampedBackgroundX;
    [Header("Minigame Audio")]
    private AudioSource  musicAudioSource;
    private AudioSource  sfxAudioSource;
    public AudioClip    music;
    public AudioClip   completion;
    public AudioClip    footsteps;
    public  AudioClip   jumpSfx;
    private bool ended = false;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private void Awake()
    {
        musicAudioSource = AudioManager.Instance.GetComponent<AudioSource>();
        sfxAudioSource = GetComponent<AudioSource>();
        SwitchAudioAndPlay(musicAudioSource, music, true);
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
    }

    private void Update()
    {
        if (-3840f <= clampedForegroundX && clampedForegroundX <= 0f)
        {
            if (clampedForegroundX <= -3840 && !ended)
            {
                StartCoroutine(DoEndAnimation());
                return;
            }
            else if (!ended)
                UpdateFromInputs();
        }
    }

    //TO DO: refactor this ugly shit
    private void UpdateFromInputs()
    {
        if (Input.GetKeyDown(leftKey) && !Input.GetKey(rightKey)) //start going left
            SetAnimatorState(faceleft, true);
        else if (Input.GetKeyDown(rightKey) && !Input.GetKey(leftKey)) //start going right
            SetAnimatorState(faceRight, true);
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey)) //going left
        {
            SideScroll(Vector2.right);
            foreground.anchoredPosition = new Vector2(clampedForegroundX, foreground.anchoredPosition.y);
            background.anchoredPosition = new Vector2(clampedBackgroundX, background.anchoredPosition.y);
        }
        else if (Input.GetKey(rightKey) && !Input.GetKey(leftKey)) //going right
        {
            SideScroll(Vector2.left);
            foreground.anchoredPosition = new Vector2(clampedForegroundX, foreground.anchoredPosition.y);
            background.anchoredPosition = new Vector2(clampedBackgroundX, background.anchoredPosition.y);
        }
        if (Input.GetKeyUp(rightKey) || Input.GetKeyUp(leftKey)) 
            SetAnimatorState(Vector3.zero, false);
    }

    private IEnumerator DoEndAnimation()
    {
        animator.enabled = false;
        ended = true;
        squirrel.SetParent(shoulder);
        squirrel.localPosition = Vector2.zero;
        asylum.SetParent(background);
        // animator.GetComponent<RectTransform>().SetParent(foreground);
        SwitchAudioAndPlay(sfxAudioSource, jumpSfx, false);
        yield return new WaitForSeconds(2f);
        SwitchAudioAndPlay(sfxAudioSource, footsteps, false);
        animator.enabled = true;
        animator.SetBool("DoEnd", true);
        yield return new WaitForSeconds(1.6f);
            yield return null;
        if (sfxAudioSource.isPlaying)
            sfxAudioSource.Stop();
        SwitchAudioAndPlay(musicAudioSource, completion, false);
        yield return new WaitForSeconds(completion.length);
        MinigamesManager.Instance.EndMinigame();
    }

    private void SetAnimatorState(Vector3 scale, bool enabled)
    {
        if (scale != Vector3.zero)
            animator.GetComponent<RectTransform>().localScale = scale;
        if (enabled)
            SwitchAudioAndPlay(sfxAudioSource, footsteps, true);
        else if (sfxAudioSource.isPlaying)
            sfxAudioSource.Stop();
        animator.enabled = enabled;
    }

    private void SideScroll(Vector2 direction)
    {
        background.anchoredPosition += direction * scrollSpeed * Time.deltaTime;;
        foreground.anchoredPosition += direction * scrollSpeed * 3f * Time.deltaTime;;
        clampedForegroundX = Mathf.Clamp(foreground.anchoredPosition.x, -3840f, 0f);
        clampedBackgroundX = Mathf.Clamp(background.anchoredPosition.x, -1920f, 0f);
    }

    private void SwitchAudioAndPlay(AudioSource source, AudioClip clip, bool loop)
    {
        if (source.isPlaying)
            source.Stop();
        source.loop = loop;
        source.clip = clip;
        source.Play();
    }
}
