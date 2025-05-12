using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquirrelManager : Singletroon<SquirrelManager>
{
    public System.Action onNutsRefilled;
    public GameObject   nuttySprite;
    public GameObject   eatingMetalSprite;
    public GameObject   standing;
    public GameObject   deadSprite;
    public Transform[]  eatBarsSpots;
    public GameObject[] cellBars;
    public GameObject[] nuts;
    public Transform [] hideNutSpots;
    private int         nutsInBowl = 8;
    private int         currentNutIndex = 0;
    private bool        cobKnocking = false;

    [Header("Audio")]
    public AudioSource  squirrelAudio;
    public AudioSource  bowlAudio;
    public AudioClip[]  nutsPour;
    public AudioClip    cacabinky;
    public AudioClip    eatingNut;
    public AudioClip    eatingBar;
    public AudioClip    barbreakSound;
    public AudioClip    deathSound;
    public Animator     handle;
    private PlayerCam  playerCam;

    private void Awake()
    {
        playerCam = PlayerCam.Instance;
        NightManager.Instance.onNightStart += StartNight;
        NightManager.Instance.onNightEnd += EndNight;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        NightManager.Instance.onNightStart -= StartNight;
        NightManager.Instance.onNightEnd -= EndNight;
    }

    public void StartNight(int night)
    {
        SetCobKnocking(false);
        RefillBowl();
        if (night == 1)
            StartCoroutine(StartNight1());
        else
        {
            if (night == 0 && FeralManager.Instance.baseAggro[0] == 0 && !NightManager.Instance.IsCobbyActive())
            {
                ChangeSprite(standing);
                standing.SetActive(false);
                return;
            }
            StartCoroutine(EatNuts());
        }
    }

    public void SetCobKnocking(bool on)
    {
        cobKnocking = on;
    }

    private IEnumerator StartNight1()
    {
        ChangeSprite(standing);
        yield return new WaitForSeconds(42);
        StartCoroutine(EatNuts());
    }

    private void ChangeSprite(GameObject spriteToShow)
    {
        nuttySprite.SetActive(nuttySprite == spriteToShow);
        eatingMetalSprite.SetActive(eatingMetalSprite == spriteToShow);
        standing.SetActive(standing == spriteToShow);
        deadSprite.SetActive(deadSprite == spriteToShow);
    }

    private void PlayAudio(AudioClip clip, bool loop = false)
    {
        if (!loop)
        {
            squirrelAudio.PlayOneShot(clip);
            return;
        }
        squirrelAudio.clip = clip;
        squirrelAudio.loop = loop;
        squirrelAudio.Play();
    }

    public void RefillBowl()
    {
        if (nutsInBowl == 8)
            return;
        handle.SetTrigger("turnHandle");
        bowlAudio.clip = nutsPour[UnityEngine.Random.Range(0, nutsPour.Length)];
        bowlAudio.Play();
        for (int i = 0; i < 8 - nutsInBowl; i++)
        {
            nuts[i].SetActive(true);
            nuts[i].GetComponent<Rigidbody>().isKinematic = false;
        }
        currentNutIndex = 0;
        nutsInBowl = 8;
        if (onNutsRefilled != null)
            onNutsRefilled();
    }

    private void HideNut()
    {
        GameObject nut = nuts[currentNutIndex];
        nut.GetComponent<Rigidbody>().isKinematic = true;
        nut.SetActive(false);
        nut.transform.position = hideNutSpots[currentNutIndex].position;
        currentNutIndex = Math.Clamp(currentNutIndex + 1, 0, 7);
        nutsInBowl--;
    }

    private IEnumerator EatNuts()
    {
        float nutDelay;
        bool caca = SoyGameController.Instance.IsCaca();
        while (nutsInBowl > 0 && !cobKnocking)
        {
            nutDelay = GetNutDelay();
            if (!caca)
            {
                ChangeSprite(standing);
                yield return new WaitForSeconds(nutDelay * 0.3125f);
            }
            HideNut();
            ChangeSprite(nuttySprite);
            PlayAudio(caca ? cacabinky : eatingNut);
            yield return new WaitForSeconds(caca ? nutDelay : nutDelay * 0.6875f);
            yield return null;
        }
        StartCoroutine(EatCellBars());
    }

    private float GetNutDelay()
    {
        float aggro = FeralManager.Instance.currentAggro;
        return 4f - aggro * 0.1f;
    }

    private IEnumerator EatCellBars()
    {
        ChangeSprite(standing);
        yield return new WaitForSeconds(2f);
        ChangeSprite(eatingMetalSprite);
        OfficeController officeController = OfficeController.Instance;
        while (officeController.cellBarsHP > 0)
        {
            if (nutsInBowl > 0 && !cobKnocking)
            {
                StartCoroutine(EatNuts());
                yield break;
            }
            ChangeSpotOnBar(eatBarsSpots[UnityEngine.Random.Range(0, eatBarsSpots.Length)]);
            PlayAudio(eatingBar);
            yield return new WaitForSeconds(1f);
            officeController.cellBarsHP -= 3;
            UpdateCellBars(officeController.cellBarsHP);
            yield return null;
        }
        PlayAudio(barbreakSound);
        ChangeSprite(standing);
        yield return new WaitForSeconds(2f);
        ItsLeadPoisoning();
    }

    private void ChangeSpotOnBar(Transform spot)
    {
        eatingMetalSprite.transform.SetParent(spot);
        eatingMetalSprite.transform.localPosition = Vector3.zero;
        eatingMetalSprite.transform.localRotation = Quaternion.identity;
        eatingMetalSprite.transform.localScale = Vector3.one;
    }

    public void UpdateCellBars(int hp)
    {
        if (hp < 1)
        {
            SetCellBarState(cellBars[0], 3);
            SetCellBarState(cellBars[1], 3);
            SetCellBarState(cellBars[2], 3);
        }      
        else if (hp < 12)
        {
            SetCellBarState(cellBars[0], 3);
            SetCellBarState(cellBars[1], 2);
            SetCellBarState(cellBars[2], 3);
        }
        else if (hp < 23)
        {
            SetCellBarState(cellBars[0], 2);
            SetCellBarState(cellBars[1], 2);
            SetCellBarState(cellBars[2], 3);
        } 
        else if (hp < 34)
        {
            SetCellBarState(cellBars[0], 2);
            SetCellBarState(cellBars[1], 2);
            SetCellBarState(cellBars[2], 2);
        }
        else if (hp < 45)
        {
            SetCellBarState(cellBars[0], 2);
            SetCellBarState(cellBars[1], 1);
            SetCellBarState(cellBars[2], 2);
        }
        else if (hp < 56)
        {
            SetCellBarState(cellBars[0], 2);
            SetCellBarState(cellBars[1], 1);
            SetCellBarState(cellBars[2], 1);
        }
        else if (hp < 67)
        {
            SetCellBarState(cellBars[0], 1);
            SetCellBarState(cellBars[1], 1);
            SetCellBarState(cellBars[2], 1);
        }
        else if (hp < 78)
        {
            SetCellBarState(cellBars[0], 1);
            SetCellBarState(cellBars[1], 0);
            SetCellBarState(cellBars[2], 1);
        }
        else if (hp < 89)
        {
            SetCellBarState(cellBars[0], 0);
            SetCellBarState(cellBars[1], 0);
            SetCellBarState(cellBars[2], 1);
        }
        else
        {
            SetCellBarState(cellBars[0], 0);
            SetCellBarState(cellBars[1], 0);
            SetCellBarState(cellBars[2], 0);
        }
    }

    private void SetCellBarState(GameObject cellBar, int state)
    {
        for (int i = 0; i < 4; i++)
            cellBar.transform.GetChild(i).gameObject.SetActive(i == state);
    }

    private void ItsLeadPoisoning()
    {
        PlayAudio(deathSound);
        ChangeSprite(deadSprite);
        EndNight();
    }

    private void Update()
    {
        ToggleMuteAudio(playerCam.currentLookDirection != LookDirection.Right &&
            playerCam.currentLookDirection != LookDirection.FrontToRight);
    }

    private void ToggleMuteAudio(bool muted)
    {
        if (!muted && (squirrelAudio.clip == deathSound || squirrelAudio == barbreakSound))
            return;
        if (squirrelAudio.mute != muted)
            squirrelAudio.mute = muted;
    }

    public void EndNight()
    {
        SetCobKnocking(false);
        squirrelAudio.Stop();
        StopAllCoroutines();
        currentNutIndex = 0;
    }

    public void StopAudio()
    {
        squirrelAudio.Stop();
    }

}
