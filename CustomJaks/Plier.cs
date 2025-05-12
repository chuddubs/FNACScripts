using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plier : Jak
{
    private NightManager nm;
    private bool         wasJustAtGenerator = false;
    private bool         atGeneratorRoutineLaunched;
    private bool         backtracking = false;
    private bool         officeRoutineLaunched = false;
    [SerializeField]
    private AudioClip    plierVoiceline;
    [SerializeField]
    private AudioClip    plierVoicelinecaca1;
    [SerializeField]
    private AudioClip    plierVoicelinecaca2;
    [SerializeField]
    private AudioClip    plierGenVoiceline;

    private void Start()
    {
        nm = NightManager.Instance;
    }

    private IEnumerator WaitFor2AMRoutine()
    {
        Teleport(NightManager.Instance.inactiveSpot);
        ShowHide(false);
        active = false;
        while (NightManager.Instance.currHour < 2)
            yield return null;
        active = true;
        Teleport(currentBehavior.GetRandomStartPos());
        inCooldown = true;
        cooldown = baseCooldown;
        ShowHide(true);
    }

    public override void StartNight(int n)
    {
        wasJustAtGenerator = Random.value < 0.5f;
        currentBehavior = GetComponentsInChildren<Behavior>().FirstOrDefault(behavior => behavior.night == NightManager.Instance.currNight);
        currentAggro = currentBehavior.startingAggro;
        if (IsActiveTonight())
        {
            if (NightManager.Instance.currNight > 0 && NightManager.Instance.currNight != 5)
            {
                StartCoroutine(WaitFor2AMRoutine());
                return;
            }
            else
            {
                active = true;
                Teleport(currentBehavior.GetRandomStartPos());
            }
        }
        else
            Teleport(NightManager.Instance.inactiveSpot);
        ShowHide(IsActiveTonight());
    }

    public override IEnumerator Move()
    {
        currentBehavior.bhvType = currentSpot.Cam.IsHallway ? BehaviorType.RoamingTo : BehaviorType.RoamingAimless;
        Spot newpos = currentSpot.Cam.id == 13 && wasJustAtGenerator ? ViewManager.Instance.GetCamById(14).GetFreeSpotWithinView(this) : currentBehavior.GetNewSpotFromBehavior();
        if (newpos != currentSpot)
            Teleport(newpos);
        else
            ResetCooldown();
        wasJustAtGenerator = false;
        yield break;
    }

    private IEnumerator AtGeneratorRoutine()
    {
        atGeneratorRoutineLaunched = true;
        yield return new WaitForSeconds(20f);
        wasJustAtGenerator = true;
        if (OfficeController.Instance.genDoorOpen && OfficeController.Instance.power.powerLeft > 0 && !OfficeController.Instance.power.outage)
        {
            Teleport(NightManager.Instance.inactiveSpot);
            MakeCameraGlitch(CurrentSpot.Cam);
            yield return new WaitForSeconds(2f); //time during which 'plier is messing with the generator
            OfficeController.Instance.genDoorAudio.PlayOneShot(plierGenVoiceline, 1f);
            yield return new WaitForSeconds(plierGenVoiceline.length);
            OfficeController.Instance.power.Outage(true);
        }
        Teleport(ViewManager.Instance.GetCamById(13).GetFreeSpotWithinView(this));
        ResetCooldown();
        atGeneratorRoutineLaunched = false;
    }

    private IEnumerator DoorwayRoutine()
    {
        backtracking = true;
        float timer = Random.Range(6f, 8f);
        while (timer > 0)
        {
            if (timer < 3f && OfficeController.Instance.doorOpen)
            {
                backtracking = false;
                yield return Move();
                cooldown = baseCooldown;
                inCooldown = true;
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Teleport(ViewManager.Instance.GetCamById(6).GetFreeSpotWithinView(this));
        backtracking = false;
    }

    private IEnumerator InOfficeRoutine()
    {
        officeRoutineLaunched = true;
        yield return new WaitForSeconds(1.5f);
        if (SoyGameController.Instance.IsCaca())
            AudioManager.Instance.PlayClip(Random.Range(0f, 100f) % 2 == 0 ? plierVoicelinecaca1 : plierVoicelinecaca2);
        else    
            AudioManager.Instance.PlayClip(plierVoiceline);
        NightManager.Instance.UpdateCalm(1);
        NightManager.Instance.RaiseGlobalAggro(2);
        FeralManager.Instance.Disturb(100f);
        yield return new WaitForSeconds(2.4f);
        Teleport(ViewManager.Instance.GetCamById(13).GetFreeSpotWithinView(this));
        officeRoutineLaunched = false;
    }

    public override void UpdateJak()
    {
        if (Locked() || !inCooldown || !active)
            return;
        if (IsInDoorway)
        {
            if (!backtracking)
                StartCoroutine(DoorwayRoutine());
        }
        else if (currentSpot.Cam.id == 3)
        {
            if (!atGeneratorRoutineLaunched)
                StartCoroutine(AtGeneratorRoutine());
        }
        else if (IsInOffice)
        {
            if (!officeRoutineLaunched)
                StartCoroutine(InOfficeRoutine());
        }
        else if (!IsWatched() && !IsInFlashlight())
            base.UpdateJak();
        
    }

    public override void Interrupt()
    {
        base.Interrupt();
        atGeneratorRoutineLaunched = false;
        backtracking = false;
        officeRoutineLaunched = false;
    }
}

