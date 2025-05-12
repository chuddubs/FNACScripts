using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SoysteinHallway : MonoBehaviour
{
    public float walkSpeed = 100f;
    public float schizoSpeed = 80f;
    private float elapsed = 0f;
    public float framerate = 0.15f;
    private Vector2 charPos;
    public RectTransform soystein;
    public GameObject idle;
    public GameObject walking;
    public GameObject schizo;
    public GameObject[] rooms;
    private int roomIndex = 0;
    [Header("Minigame Audio")]
    private AudioSource  audioSource;
    public AudioSource    footsteps;
    public  AudioClip   quietBuzz;
    public  AudioClip   loudBuzz;
    private bool ended = false;
    private bool wasWalking = false;
    private Vector3 currentScale = Vector3.one;

    private KeyCode leftKey;
    private KeyCode rightKey;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        idle = soystein.GetChild(0).gameObject;
        walking = soystein.GetChild(1).gameObject;
        charPos = soystein.anchoredPosition;
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
    }

    private void Update()
    {
        if (ended)
        {
            SwitchSprite(false);
            footsteps.Stop();
            return;
        }
        else
            UpdateFromInputs();
    }

    private void UpdateFromInputs()
    {
        bool isWalking = Input.GetKey(rightKey) || Input.GetKey(leftKey);
        if (isWalking)
        {
            if (!wasWalking)
                footsteps.Play();
            if (Input.GetKey(leftKey) && !Input.GetKey(rightKey)) //going left
            {
                if (currentScale.x == 1)
                    Flip();
                charPos += Vector2.left * walkSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(rightKey) && !Input.GetKey(leftKey)) //going right
            {
                if (currentScale.x == -1)
                    Flip();
                charPos += Vector2.right * walkSpeed * Time.deltaTime;
            }
        }
        else if (wasWalking)
            footsteps.Stop();
        if (charPos.x < -960)
            PrevRoom();
        else if (charPos.x > 960)
            NextRoom();
        if (roomIndex == 3 && charPos.x > 300 && !schizo.activeSelf)
            StartCoroutine(DoEndAnimation());
        wasWalking = isWalking;
        SwitchSprite(isWalking);
        elapsed += Time.deltaTime;
        if (elapsed >= framerate)
        {
            elapsed = 0;
            UpdateVisuals();
        }
    }

    private void PrevRoom()
    {
        if (roomIndex <= 0)
        {
            charPos.x = -960 + 95; 
            return;
        }
        charPos.x = 950;
        roomIndex--;
        SwitchRoom();
    }

    private void NextRoom()
    {
        if (roomIndex > 2)
        {
            charPos.x = 950;
            return;
        }
        charPos.x = -950;
        roomIndex++;
        SwitchRoom();
    }

    private void SwitchRoom()
    {
        for (int i = 0; i < rooms.Length; i++)
            rooms[i].SetActive(i == roomIndex);
    }

    private void UpdateVisuals()
    {
        soystein.anchoredPosition = charPos;
    }

    private void Flip()
    {
        currentScale.x *= -1;
        idle.transform.localScale = currentScale;
        walking.transform.localScale = currentScale;
    }

    private IEnumerator DoEndAnimation()
    {
        schizo.SetActive(true);
        SwitchAudioAndPlay(quietBuzz, true);
        RectTransform schizoPos = schizo.GetComponent<RectTransform>();
        float timer = 0f;
        float endX = charPos.x + 95;
        float schizoY = schizoPos.anchoredPosition.y;
        while (schizoPos.anchoredPosition.x > endX)
        {
            endX = charPos.x + 95;
            if (timer > framerate)
            {
                schizoPos.anchoredPosition = new Vector2(Mathf.Max(schizoPos.anchoredPosition.x - schizoSpeed, endX), schizoY);
                timer = 0f;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        ended = true;
        SwitchAudioAndPlay(loudBuzz, true);
        yield return new WaitForSeconds(0.5f);
        MinigamesManager.Instance.EndMinigame();
    }

    private void SwitchSprite(bool isWalking)
    {
        if (idle.activeSelf != !isWalking)
            idle.SetActive(!isWalking);
        if (walking.activeSelf != isWalking)
            walking.SetActive(isWalking);
    }

    private void SwitchAudioAndPlay(AudioClip clip, bool loop)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
