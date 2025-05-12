using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Jak : MonoBehaviour
{
    public Variant  variant;
    public int      doorPriority = 0;
    public GameObject   standingNear;
    public GameObject   standingFar;
    public GameObject   staringUp;
    public GameObject   crawlingVents;
    public GameObject   climbingVents;
    public GameObject   peeringVents;
    public GameObject   hallway;
    public GameObject   inOffice;
    public GameObject   close;

    [Range(0f, 100f)]
    public float        baseCooldown;
    public bool         inCooldown;
    public double       cooldown;

    public Behavior currentBehavior;
    [HideInInspector]
    public Spot     currentSpot;
    [HideInInspector]
    public GameObject   currentSprite;
    [HideInInspector]
    public int currentAggro;
    [HideInInspector]
    public bool   climbing = false;
    public bool  active = true;
    private bool  alreadySeenTonight = false;
    private Transform flashlight;
    [HideInInspector]
    [Header("Audio")]
    public AudioSource          audioSource;
    public AudioClip            reachingDoor;
    public AudioClip            leavingDoor;
    public AudioClip            enteringOrLeavingOffice;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        currentSprite = standingNear;
        flashlight = OfficeController.Instance.flashlight.transform;
        currentSpot = NightManager.Instance.inactiveSpot;
        currentBehavior = currentBehavior = GetComponentsInChildren<Behavior>().FirstOrDefault(behavior => behavior.night == 1);
        NightManager.Instance.onNightStart += StartNight;
        NightManager.Instance.onNightEnd += EndNight;
        OfficeController.Instance.onFlashLightToggled += OnLightToggled;
        OfficeController.Instance.onDeskLightToggled += OnLightToggled;
    }

    private void OnLightToggled(bool on)
    {
        if (IsActiveTonight())
            UpdateVisibility();
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        NightManager.Instance.onNightStart -= StartNight;
        NightManager.Instance.onNightEnd -= EndNight;
        OfficeController.Instance.onFlashLightToggled -= OnLightToggled;
        OfficeController.Instance.onDeskLightToggled -= OnLightToggled;
    }

    public virtual void ResetCooldown()
    {
        cooldown = baseCooldown;
        inCooldown = true;
    }

    public void PlayAudio(AudioClip clip, bool loop = false)
    {
        audioSource.clip = clip;
        audioSource.time = 0f;
        if (!loop)
        {
            audioSource.PlayOneShot(clip);
            return;
        }
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void ShowHide(bool enabled)
    {
        transform.GetChild(0).gameObject.SetActive(enabled);
    }


    
    public virtual void StartNight(int n)
    {
        alreadySeenTonight = false;
        currentBehavior = GetComponentsInChildren<Behavior>().FirstOrDefault(behavior => behavior.night == NightManager.Instance.currNight); 
        currentAggro = currentBehavior.startingAggro;
        if (IsActiveTonight())
            Teleport(currentBehavior.GetRandomStartPos());
        else
            Teleport(NightManager.Instance.inactiveSpot);
        active = IsActiveTonight();
        // ShowHide(IsActiveTonight());
    }

    public virtual void GetAggroGainHour(int hour)
    {
        if (hour == 2)
            currentAggro += currentBehavior.aggroGainAt2AM;
        if (hour == 4)
            currentAggro += currentBehavior.aggroGainAt4AM;
    }

    public void GetAggro(int gain)
    {
        currentAggro += gain;
    }

    public virtual void Screamer()
    {
        if (variant == Variant.Chud)
            JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Chud);
        else if (variant == Variant.Imp)
            JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Swede);
        else if (variant == Variant.Rapeson)
            JumpscaresManager.Instance.PlayJumpscare(NightManager.Instance.dollSon ? Jumpscare.Dollson : Jumpscare.Rapeson);
        else if (variant == Variant.Fingerboy)
        {
            if (currentSpot.Cam.id == 11)
                JumpscaresManager.Instance.PlayJumpscare(Jumpscare.FingerboySide);
            else
                JumpscaresManager.Instance.PlayJumpscare(Jumpscare.FingerboyFront);
        }
        audioSource.Stop();
        EndNight();
    }

    public void MakeCameraGlitch(ViewCam newcam)
    {
        if (ViewManager.Instance.currId != 0)
        {   
            //if currently watching the jak's departure or arrival camera
            if (ViewManager.Instance.currId == currentSpot.Cam.id || ViewManager.Instance.currId == newcam.id)
                ViewManager.Instance.ShowMoveGlitch();
        }
    }

    public void MakeLightsGlitch(int from, int to)
    {
        if (from == -5 || to == 0 || to == -5 || (from == 0 && to != -88))
        {
            OfficeController.Instance.FlickerLights();
        }
    }

    public virtual IEnumerator Move()
    {
        Spot newpos = currentBehavior.GetNewSpotFromBehavior();
        if (newpos != currentSpot)
        {
            if (newpos.Cam.isVents && !currentSpot.Cam.isVents) //entering a vent
            {
                climbing = true;
                Teleport(newpos.Cam.GetVentEntryPoint(currentSpot.Cam));
                yield return new WaitForSeconds(2f);
            }
            climbing = false;
            Teleport(newpos);
        }
        else
            ResetCooldown();
    }

    public void PlayFootSteps(Spot newpos)
    {
        if (newpos.Cam.id == -88)
            return;
        if (currentSpot.Cam.id == -1 && newpos.Cam.id != 0)
            OfficeController.Instance.PlayFootSteps(0, leavingDoor);
        else if (newpos.Cam.id == -1)
            OfficeController.Instance.PlayFootSteps(0, reachingDoor);
        else if (newpos.Cam.id == 0 || currentSpot.Cam.id == 0)
            OfficeController.Instance.PlayFootSteps(1, enteringOrLeavingOffice);
        else if (newpos.Cam.id == -10 || currentSpot.Cam.id == -10)
            OfficeController.Instance.PlayFootSteps(2, enteringOrLeavingOffice);
    }

    public virtual void Teleport(Spot newpos, bool playFootSteps = true)
    {
        if (newpos == null)
        {
            Debug.LogError("attempted to move " + gameObject.name + " to invalid location");
            return;
        }
        if (playFootSteps)
            PlayFootSteps(newpos);
        MakeCameraGlitch(newpos.Cam);
        MakeLightsGlitch(currentSpot.Cam.id, newpos.Cam.id);
        ChangeSprite(GetImageForSpot(newpos.spriteToUse).gameObject);
        transform.SetParent(newpos.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        currentSpot = newpos;
        UpdateVisibility();
        // Debug.Log(this + " moved to CAM " + FNACStatic.CamIdToDisplayName[currentSpot.Cam.id]);
        ResetCooldown();
    }

    public virtual void UpdateVisibility()
    {
		if (currentSpot.Cam.IsHallway)
            ShowHide(OfficeController.Instance.flashlight.activeSelf);
        else if (IsInOffice)
            ShowHide(!OfficeController.Instance.InDarkness);
        else
            ShowHide(IsActiveTonight());
    }

    // public void HideInHallwayIfAck()
	// {
	// 	if (OfficeController.Instance.hasTroonACKed && currentSpot.Cam.IsHallway)
	// 		ShowHide(false);
	// }

    public virtual void ChangeSprite(GameObject newSprite)
    {
        if (currentSprite != newSprite)
        {
            currentSprite.SetActive(false);
            currentSprite = newSprite;
            currentSprite.SetActive(true);
        }
    }

    public virtual void AttemptMove()
    {
        int roll = UnityEngine.Random.Range(1, 30);
        if (roll <= currentAggro + (variant != Variant.Plier && currentSpot.Cam.isPlierInRoom ? 10 : 0))
            StartCoroutine(Move());
        else
            ResetCooldown();
    }

    public virtual void UpdateJak()
    {
        if (!IsActiveTonight())
            return;
        if (inCooldown && !Locked())
        {
            if (cooldown <= 0 && !climbing)
                AttemptMove();
            cooldown -= Time.deltaTime;
        }
    }

    private void Update ()
    {
        if (!active)
            return;
        if (!alreadySeenTonight && NightManager.Instance.IsJakNearby(this, false))
        {
            if (IsWatched())
            {
                if (IsInDoorway || IsInOffice || currentSpot.Cam.id == -5 || currentSpot.Cam.id == -10)
                    AudioManager.Instance.PlayJakInView();
                alreadySeenTonight = true;
            }
        }
        UpdateJak();
	}

    public virtual void Interrupt()
    {
        StopAllCoroutines();
        inCooldown = false;
        climbing = false;
    }
    public virtual void EndNight()
    {
        Interrupt();
        audioSource.Stop();
        cooldown = baseCooldown;
        inCooldown = false;
        Teleport(NightManager.Instance.inactiveSpot);
        active = false;
        ShowHide(false);
    }


    public bool IsInFlashlight()
    {
        if (!flashlight.gameObject.activeSelf || CurrentSpot.Cam.id > 0)
            return false;
        else if (currentSpot.Cam.IsHallway && !OfficeController.Instance.doorOpen)
            return false;
        Vector3 flashlightPosXZ = new Vector3(flashlight.position.x, 0f, flashlight.position.z);
        Vector3 spritePositionXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 toSpriteXZ = (spritePositionXZ - flashlightPosXZ).normalized;
        Vector3 flashlightForwardXZ = flashlight.forward;
        flashlightForwardXZ.y = 0;
        return Vector3.Angle(flashlightForwardXZ, toSpriteXZ) <= 35f;
    }

    private bool InFrustrum(Vector3 viewPos)
    {
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;
    }

    public bool IsWatched()
    {
        if (ViewManager.Instance.currId == 0)
        {
            if (currentSpot.Cam.id == 0 || currentSpot.Cam.id == -5)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(currentSprite.transform.position);
                if (currentSpot.Cam.id == -5)
                    return OfficeController.Instance.ventsDoorOpen && !OfficeController.Instance.InDarkness && InFrustrum(viewPos);
                else
                    return !OfficeController.Instance.InDarkness && InFrustrum(viewPos);
            }
            else if (CurrentSpot.Cam.IsHallway)
                return OfficeController.Instance.doorOpen && IsInFlashlight();
        }
        return ViewManager.Instance.currId == currentSpot.Cam.id;
    }

    public bool Locked()
    {
        return currentBehavior.bhvType == BehaviorType.Hidden || currentBehavior.bhvType == BehaviorType.Still || currentSpot.Cam.id == -88;
    }

    public bool IsActiveTonight()
    {
        return currentBehavior.bhvType != BehaviorType.Hidden;
    }

    public bool IsInactive
    {
        get
        {
            return currentSpot.Cam.id == -88;
        }
    }

    public bool IsInOffice
    {
        get
        {
            return currentSpot.Cam.id == 0;
        }
    }

    public bool IsInDoorway
    {
        get
        {
            return currentSpot.Cam.id == -1;
        }
    }

    public Spot CurrentSpot
    {
        get
        {
            return currentSpot;
        }
    }

    private GameObject GetImageForSpot(SpriteToUse sprite)
    {
        switch(sprite) 
        {
            case SpriteToUse.Standing1:
                return standingNear;
            case SpriteToUse.Standing2:
                return standingFar;
            case SpriteToUse.StaringUp:
                return staringUp;
            case SpriteToUse.ClimbingVents:
                return climbingVents;
            case SpriteToUse.CrawlingVents:
                return crawlingVents;
            case SpriteToUse.PeeringVents:
                return peeringVents;
            case SpriteToUse.Hallway:
                return hallway;
            case SpriteToUse.Office:
                return inOffice;
            case SpriteToUse.Close:
                return close;
        }
        return standingNear;
    }
}
public enum Variant
{
    Imp,
    Chud,
    Troon,
    Fingerboy,
    Rapeson,
    Plier
}