using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rapeson : Jak
{
    private Coroutine knockingRoutine;
    private bool      knockingOnDoor = false;
    private Coroutine inOfficeRoutine;
    private Coroutine inDoorWaySeenRoutine;
    private bool      finnaRape = false;
    private bool      inOfficeRoutineLaunched = false;
    private bool      inFeralCellRoutineLaunched = false;

    private AudioSource doorAudioSource;
    [SerializeField]
    private AudioClip   knockSound1;
    [SerializeField]
    public AudioClip   knockSound2;
    public AudioClip[] dollsonLines;
    private bool       lastWentFeral = true;
    [SerializeField]
    private GameObject backTurned;
    public GameObject inCamSix;
    [SerializeField]
    private GameObject peeking;
    [SerializeField]
    private Spot        spotFeralHole;
    [SerializeField]
    private AudioClip   heavyBreathing;
    public GameObject   rabbiHole;
    private NightManager nm;
    private PlayerCam pc;
    private void Start()
    {
        pc = PlayerCam.Instance;
        nm = NightManager.Instance;
        doorAudioSource = OfficeController.Instance.doorAudio;
        SquirrelManager.Instance.onNutsRefilled += Rape;
        OfficeController.Instance.onDoor += Rape;
        FeralManager.Instance.onTablet += Rape;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        SquirrelManager.Instance.onNutsRefilled -= Rape;
        OfficeController.Instance.onDoor -= Rape;
        FeralManager.Instance.onTablet -= Rape;
    }

    private void ToggleDollson(bool dollson)
    {
        standingNear.transform.GetChild(0).gameObject.SetActive(!dollson);
        standingNear.transform.GetChild(1).gameObject.SetActive(dollson);
        staringUp.transform.GetChild(0).gameObject.SetActive(!dollson);
        staringUp.transform.GetChild(1).gameObject.SetActive(dollson);
        hallway.transform.GetChild(0).gameObject.SetActive(!dollson);
        hallway.transform.GetChild(1).gameObject.SetActive(dollson);
        inOffice.transform.GetChild(0).gameObject.SetActive(!dollson);
        inOffice.transform.GetChild(1).gameObject.SetActive(dollson);
        peeking.transform.GetChild(0).gameObject.SetActive(!dollson);
        peeking.transform.GetChild(1).gameObject.SetActive(dollson);
        inCamSix.transform.GetChild(0).gameObject.SetActive(!dollson);
        inCamSix.transform.GetChild(1).gameObject.SetActive(dollson);
    }

    private void HideIfFirstTwoCacaNights(bool earlyRabbi)
    {
        active = !earlyRabbi;
        currentAggro = earlyRabbi ? 0 : currentBehavior.startingAggro;
    }

    public override void StartNight(int n)
    {
        lastWentFeral = true;
        finnaRape = false;
        bool earlyRabbbi = SoyGameController.Instance.IsCaca() && NightManager.Instance.currNight != 0 && NightManager.Instance.currNight < 3;
        if (SoyGameController.Instance.IsCaca())
            rabbiHole.SetActive(!earlyRabbbi);
        currentBehavior = GetComponentsInChildren<Behavior>().FirstOrDefault(behavior => behavior.night == NightManager.Instance.currNight); 
        if (earlyRabbbi)
            currentAggro = 0;
        else
            currentAggro = currentBehavior.startingAggro;
        active = IsActiveTonight() && !earlyRabbbi;
        if (active)
        {
            ToggleDollson(nm.dollSon);
            if (n == 4)
                StartCoroutine(WaitFor2AMRoutine());
            else
            {
                Teleport(currentBehavior.GetRandomStartPos());
                inCooldown = true;
                cooldown = baseCooldown;
            }
        }
        else
            Teleport(NightManager.Instance.inactiveSpot);
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

    private IEnumerator KnockOnDoor()
    {
        knockingOnDoor = true;
        NightManager.Instance.UpdateCalm(1);
        SquirrelManager.Instance.SetCobKnocking(true);
        NightManager.Instance.RaiseGlobalAggro(3, this);
        int dollLine = 0;
        float nextKnockTime = Time.time + Random.Range(0.5f, 1.5f);
        while (true)
        {
            if (OfficeController.Instance.doorOpen)
            {
                NightManager.Instance.RaiseGlobalAggro(-3, this);
                Teleport(ViewManager.Instance.GetCamById(0).GetFreeSpotWithinView(this));
                knockingOnDoor = false;
                SquirrelManager.Instance.SetCobKnocking(false);
                yield break;
            }
            else if (Time.time >= nextKnockTime)
            {
                AudioClip knockSound;
                if (nm.dollSon)
                    knockSound  = dollsonLines[dollLine];
                else
                    knockSound = Random.value < 0.5f ? knockSound1 : knockSound2;
                doorAudioSource.clip = knockSound;
                doorAudioSource.Play();
                OfficeController.Instance.power.DrainPower(4);
                FeralManager.Instance.Disturb(10f);
                nextKnockTime = Time.time + knockSound.length + Random.Range(0.5f, 1.5f);
                dollLine = dollLine >= dollsonLines.Length - 1 ? 0 : dollLine + 1;
            }
            yield return null;
        }
    }


    private void StopKnocking()
    {
        if (knockingRoutine != null)
        {
            StopCoroutine(knockingRoutine);
            knockingRoutine = null;
        }
        NightManager.Instance.UpdateCalm(-1);
        knockingOnDoor = false;
    }


    public override void GetAggroGainHour(int hour)
    {
        if (!active)
            return;
        if (hour == 2)
        {
            currentAggro += currentBehavior.aggroGainAt2AM;
            if (nm.currNight == 0 || nm.currNight > 2)
                currentBehavior.bhvType = BehaviorType.RoamingTo;
        }
        if (hour == 4)
            currentAggro += currentBehavior.aggroGainAt4AM;
    }

    public override IEnumerator Move()
    {
        Spot newpos;
        if (currentSpot.Cam.id == 13 && !lastWentFeral)
        {
                if (ViewManager.Instance.currId == 13)
                    ViewManager.Instance.ShowMoveGlitch();
                Teleport(NightManager.Instance.inactiveSpot);
                yield return new WaitForSeconds(20f - Mathf.Clamp(currentAggro / 2f, 0, 19f));
                Teleport(spotFeralHole);
        }
        else if ((newpos = currentBehavior.GetNewSpotFromBehavior()) != currentSpot)
        {
            Teleport(newpos);
            MakeCameraGlitch(newpos.Cam);
        }
    }

    public override void Teleport(Spot newpos, bool playFootSteps = true)
    {
        if (ViewManager.Instance.currId == 2 &&
            (currentSpot.Cam.id == -11 || newpos.Cam.id == -11))
            ViewManager.Instance.ShowMoveGlitch();
        base.Teleport(newpos, playFootSteps);
        if (newpos.Cam.id == -11 && newpos != currentBehavior.startSpots[0])
            ChangeSprite(backTurned);
        if (newpos == spotFeralHole)
            ChangeSprite(peeking);
        if (newpos.Cam.id == -10)
            lastWentFeral = true;
        else if (newpos.Cam.IsHallway)
            lastWentFeral = false;
    }

    private IEnumerator OfficeRoutine()
    {
        inOfficeRoutineLaunched = true;
        FeralManager.Instance.Disturb(15f);
        float lingerDuration = Random.Range(10f, 15f);
        float timeSideways = 0;
        while (lingerDuration > 0)
        {
            if (pc.currentLookDirection != LookDirection.Front)
                timeSideways += Time.deltaTime;
            else
                timeSideways = 0;
            if (timeSideways > 2f)
            {
                Screamer();
                inOfficeRoutineLaunched = false;
                yield break;
            }
            lingerDuration -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(lingerDuration);
        if (!OfficeController.Instance.doorOpen) //this should never be the case doe
        {
            OfficeController.Instance.OpenCloseDoor(true);
            yield return new WaitForSeconds(0.6f);
        }
        Teleport(currentBehavior.GetRandomStartPos());
        audioSource.Stop();
        cooldown = baseCooldown;
        inCooldown = true;
        inOfficeRoutineLaunched = false;
    }

    public void Rape()
    {
        if (IsInOffice)
        {
            StopOfficeRoutine();
            StopKnocking();
            StopDoorwaySeenRoutine();
            Screamer();
        }
    }

    private void StopDoorwaySeenRoutine()
    {
        if (inDoorWaySeenRoutine != null)
        {
            StopCoroutine(inDoorWaySeenRoutine);
            inDoorWaySeenRoutine = null;
        }
        finnaRape = false;
    }

    private void StopOfficeRoutine()
    {
        if (inOfficeRoutine != null)
        {
            StopCoroutine(inOfficeRoutine);
            inOfficeRoutine = null;
        }
        inOfficeRoutineLaunched = false;
    }

    private IEnumerator InFeralCellRoutine()
    {
        inFeralCellRoutineLaunched = true;
        yield return new WaitForSeconds(30f - Mathf.Clamp(currentAggro / 2f, 0, 29f));
        if (OfficeController.Instance.InDarkness)
            Teleport(currentBehavior.GetRandomStartPos());
        else
        {
            Teleport(CurrentSpot.Cam.GetFreeSpotWithinView(this));
            if (OfficeController.Instance.cellBarsHP == 0)
            {
                while (true)
                {
                    if (Random.Range(0f, 30f) < currentAggro)
                    {
                        Screamer();
                        inOfficeRoutineLaunched = false;
                        yield break;
                    }
                    else
                        yield return new WaitForSeconds((float)baseCooldown);
                }
            }
            else
            {
                float lingerDuration = baseCooldown;
                while (lingerDuration > 0)
                {
                    lingerDuration -= Time.deltaTime;
                    yield return null;
                }
                Teleport(currentBehavior.GetRandomStartPos());
            }         
        }
        inFeralCellRoutineLaunched = false;
    }


    private void UpdateOffice()
    {
        if (!inOfficeRoutineLaunched)
            inOfficeRoutine = StartCoroutine(OfficeRoutine());
        if (!OfficeController.Instance.InDarkness || ViewManager.Instance.currId != 0)
            Screamer();
    }

    private IEnumerator InDoorwaySeen()
    {
        finnaRape = true;
        float timer = 2f;
        while (timer > 0)
        {
            if (!OfficeController.Instance.doorOpen)
            {
                finnaRape = false;
                yield break;
            }
            else if (OfficeController.Instance.InDarkness)
            {
                finnaRape = false;
                Teleport(ViewManager.Instance.GetCamById(0).GetFreeSpotWithinView(this));
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Screamer();
    }

    private void UpdateDoorway()
    {
        if (!audioSource.isPlaying)
            PlayAudio(heavyBreathing, true);
        if (!OfficeController.Instance.doorOpen)
        {
            if (!knockingOnDoor)
                inOfficeRoutine = StartCoroutine(KnockOnDoor());
        }
        else
        {
            if (IsInFlashlight() && !finnaRape)
            {
                inCooldown = false;
                inDoorWaySeenRoutine = StartCoroutine(InDoorwaySeen());
            }
            if (knockingOnDoor)
                StopKnocking();
            if (inCooldown)
                TickCooldown();
       }
    }

    public override void UpdateJak()
    {
        if (finnaRape)
            return;
        if (IsInDoorway)
            UpdateDoorway();
        else if (currentSpot.Cam.id == -10)
        {
            if (!inFeralCellRoutineLaunched)
                StartCoroutine(InFeralCellRoutine());
        }
        else if (IsInOffice)
            UpdateOffice();
        else if (inCooldown && !Locked() && (currentSpot.Cam.id == -11 || !IsWatched()))
            TickCooldown();
        if (currentSpot.Cam.id == 6)
            ChangeSprite(inCamSix);
    }

    private void TickCooldown()
    {
        if (cooldown <= 0)
            AttemptMove();
        cooldown -= Time.deltaTime;
    }

    public override void Interrupt()
    {
        base.Interrupt();
        StopKnocking();
        StopOfficeRoutine();
        StopDoorwaySeenRoutine();
        StopAllCoroutines();
        inFeralCellRoutineLaunched = false;
        finnaRape = false;
    }
}
