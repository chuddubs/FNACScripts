using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fingerboy : Jak
{
    public AudioSource soySteinsOfficeAudio;
    public AudioClip officeFboyAudio;
    public AudioClip officePibblesAudio;
    public AudioSource moveAudio;
    public AudioClip screech;
    public AudioClip bark;
    public AudioClip crawlingNoise;
    public AudioClip doorImpact;
    public GameObject crawlAnimationObj;
    public GameObject runAnimationObj;
    private bool      hallwayRoutineLaunched = false;
    private bool      vent11routineLaunched = false;
    private bool      blockedVent = false;
    private NightManager nm;
    public AudioSource ventAudio;

    private void Start()
    {
        nm = NightManager.Instance;
        ViewManager.Instance.onCamSwitched += OnCameraSwitched;
    }

    private void OnCameraSwitched(int id)
    {
        if (id == 4 && currentSpot.Cam.id == 4)
        {
            soySteinsOfficeAudio.clip = SoyGameController.Instance.IsCaca() ? officePibblesAudio : officeFboyAudio;
            soySteinsOfficeAudio.time = Random.Range(0f, soySteinsOfficeAudio.clip.length /2f);
            soySteinsOfficeAudio.Play();
        }
        else
            soySteinsOfficeAudio.Stop();
    }

    private float GetNewWaitCycle() //get wait cycle in soystein's office
    {
        if (blockedVent)
            return 5f;
        return Mathf.Clamp(55f + (Random.Range(0f, 15f) - 2f * currentAggro), 5f, 70f);
    }

    private float GetVentWaitTime()
    {
        return 13f - Mathf.Clamp(0.3f * currentAggro, 0, 12f);
    }

    private float GetHallwayWaitTime()
    {
        return 15f - Mathf.Clamp(0.3f * currentAggro, 0, 14f);
    }

    private IEnumerator VentRoutine()
    {
        vent11routineLaunched = true;
        ventAudio.Play();
        float timer = GetVentWaitTime();
        while (timer > 0)
        {
            if (ViewManager.Instance.currId == 11)
            {
                yield return CrawlRoutine();
                ventAudio.Stop();
                vent11routineLaunched = false;
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        ventAudio.Stop();
        if (!OfficeController.Instance.ventsDoorOpen)
            Teleport(ViewManager.Instance.GetCamById(4).GetFreeSpotWithinView(this));
        else
            Screamer();
        vent11routineLaunched = false;
    }

    private void ToggleAnim(bool running, bool on)
    {
        bool caca = SoyGameController.Instance.IsCaca();
        Debug.Log("Toggle Pibbles anim : " + (running ? " running" : "crawling"));
        GameObject parent = running ? runAnimationObj : crawlAnimationObj;
        GameObject animObj = parent.transform.GetChild(caca ? 1 : 0).gameObject;
        if (running)
            PlayAudio(caca ? bark : screech);
        Animator animator = animObj.GetComponent<Animator>();
        currentSprite.SetActive(!on);
        parent.SetActive(on);
        animObj.SetActive(on);
        animator.enabled = on;
    }


    private IEnumerator CrawlRoutine()
    {
        ToggleAnim(false, true);
        float timer = 3f;
        bool afterRun = false;
        while (timer > 0)
        {
            if (!OfficeController.Instance.ventsDoorOpen)
            {
                while (timer > 1.6f)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
                OfficeController.Instance.ventsDoorAudio.PlayOneShot(doorImpact, 1f);
                // PlayAudio(doorImpact);
                FeralManager.Instance.Disturb(20f);
                ToggleAnim(false, false);
                Teleport(ViewManager.Instance.GetCamById(4).GetFreeSpotWithinView(this));
                yield break;
            }
            else if (timer <= 1.6f && !afterRun)
            {
                ToggleAnim(false, false);
                ShowHide(false);
                afterRun = true;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Screamer();
    }

    private IEnumerator HallwayRoutine()
    {
        hallwayRoutineLaunched = true;
        int attemptsLeft = 2;
        bool caca = SoyGameController.Instance.IsCaca();
        while (attemptsLeft > 0)
        {
            float timer = GetHallwayWaitTime();
            while (timer > 0)
            {
                if (IsInFlashlight())
                {
                    OfficeController.Instance.PlayFootSteps(0, caca ? bark : screech);
                    // yield return new WaitForSeconds(0.4f);
                    yield return SprintRoutine();
                    hallwayRoutineLaunched = false;
                    yield break;
                }
                timer -= Time.deltaTime;
                yield return null;
            }
            bool rapesonPrio = RapesonNearOffice();
            if (OfficeController.Instance.doorOpen && !rapesonPrio)
            {
                // Screamer();
                OfficeController.Instance.PlayFootSteps(0, caca ? bark : screech);
                yield return SprintRoutine();
                hallwayRoutineLaunched = false;
                yield break;
            }
            attemptsLeft -= rapesonPrio ? 0 : 1;
        }
        Teleport(ViewManager.Instance.GetCamById(4).GetFreeSpotWithinView(this));
        hallwayRoutineLaunched = false;
    }

    private bool RapesonNearOffice()
    {
        Jak rapeson = nm.jaks.FirstOrDefault(j => j.variant == Variant.Rapeson);
        return rapeson.CurrentSpot.Cam.id == -1 || rapeson.CurrentSpot.Cam.id == 0;
    }

    private IEnumerator SprintRoutine()
    {
        ToggleAnim(true, true);
        float timer = 1.7f;
        while (timer > 0)
        {
            if (!OfficeController.Instance.doorOpen)
            {
                ToggleAnim(true, false);
                yield return new WaitForSeconds(0.2f);
                OfficeController.Instance.PlayFootSteps(0, doorImpact);
                FeralManager.Instance.Disturb(20f);
                yield return new WaitForSeconds(doorImpact.length);
                Teleport(ViewManager.Instance.GetCamById(4).GetFreeSpotWithinView(this));
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Screamer();
    }

    private void SkipCam6()
    {
        Teleport(ViewManager.Instance.GetCamById(-3).GetFreeSpotWithinView(this));
    }

    public override void UpdateVisibility()
    {
        base.UpdateVisibility();
        if (currentSpot.Cam.id == 4)
            ShowHide(false);
    }

    public override IEnumerator Move()
    {
        Spot newpos = currentBehavior.GetNewSpotFromBehavior();
        if (currentSpot.Cam.id == 10 && !OfficeController.Instance.ventsDoorOpen)
        {
            blockedVent = true;
            newpos = ViewManager.Instance.GetCamById(4).GetFreeSpotWithinView(this);
        }
        else if (currentSpot.Cam.id == 4)
        {
            if (blockedVent)
            {
                newpos = ViewManager.Instance.GetCamById(8).GetFreeSpotWithinView(this);
                blockedVent = false;
            }
            soySteinsOfficeAudio.Stop();
        }
        if (newpos != currentSpot)
            Teleport(newpos);
        else
            ResetCooldown();
        yield break;
    }

    public override void Teleport(Spot newpos, bool playFootSteps = false)
    {
        base.Teleport(newpos, playFootSteps);
        int id = currentSpot.Cam.id;
        if (id == 4)
            OnCameraSwitched(ViewManager.Instance.currId); //play sound if we're watching cam4
        else if (id > 0 && currentSpot.spriteToUse != SpriteToUse.ClimbingVents)
            moveAudio.PlayOneShot(SoyGameController.Instance.IsCaca() ? bark : screech);
    }

    public override void ResetCooldown()
    {
        if (currentSpot.Cam.id == 4)
            cooldown = GetNewWaitCycle();
        else if (currentSpot.Cam.id == 2)
            cooldown = 3.5f;
        else
            cooldown =  baseCooldown;
        inCooldown = true;
    }


    public override void UpdateJak()
    {
        if (nm.currNight > 0 && nm.currNight <= 2 && nm.currHour < 2)
            return;
        if (currentSpot.Cam.id == 11)
        {
            if (!vent11routineLaunched)
                StartCoroutine(VentRoutine());
        }
        else if (currentSpot.Cam.id == -3)
        {
            if (!hallwayRoutineLaunched)
                StartCoroutine(HallwayRoutine());
        } 
        else if (cooldown <= 0 && currentSpot.Cam.id == 2)
            SkipCam6();
        else
            base.UpdateJak();
    }

    public override void EndNight()
    {
        hallwayRoutineLaunched = false;
        vent11routineLaunched = false;
        blockedVent = false;
        crawlAnimationObj.SetActive(false);
        runAnimationObj.SetActive(false);
        ventAudio.Stop();
        soySteinsOfficeAudio.Stop();
        moveAudio.Stop();
        base.EndNight();
    }
}
