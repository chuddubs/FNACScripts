using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OfficeController : Singletroon<OfficeController>
{
    public System.Action onDoor;
    [HideInInspector]
    public PowerUsage   power;
    [SerializeField]
    private SecurityGUI securityGUI;
    public GameObject   computerScreen;
    public AudioSource  computerBuzz;
    private Coroutine   plierOutage;

    [Header("Desk light")]
    public GameObject   deskLight;
    public AudioSource  deskLightAudioSource;
    public AudioClip    lightOnSound;
    public AudioClip    lightOffSound;

    public System.Action<bool> onDeskLightToggled;
    [Header("Flashlight")]
    public GameObject   flashlight;
    public GameObject   flashlightGUI;
    public GameObject   flashlightOffUI;
    public AudioClip    flashLightOnSound;
    public AudioClip    flashLightOffSound;
    public System.Action<bool> onFlashLightToggled;
    private float       flashLightBaseIntensity;

    [Header("Door")]
    public bool         doorOpen = true;
    private bool        isMovingDoor = false;
    public DoorMovement doorController;
    public AudioSource  doorAudio;
    public AudioClip    doorSoundOpen;
    public AudioClip    doorSoundClose;

    [Header("Vents Door")]
    public bool         ventsDoorOpen = true;
    private bool        isMovingVentsDoor = false;
    public DoorMovement ventsDoorController;
    public AudioSource  ventsDoorAudio;
    public AudioClip    ventsDoorSoundOpen;
    public AudioClip    ventsDoorSoundClose;

    [Header("Generator Door")]
    public bool         genDoorOpen = true;
    public DoorMovement genDoorController;
    private bool        isMovingGenDoor = false;
    public AudioSource  genDoorAudio;
    public Button[]     genDoorButtons;
    public GameObject   ACK;
    public int          cellBarsHP = 100;

    [Header("Footsteps AudioSources")]
    public AudioSource  doorway;
    public AudioSource  fromOrToOffice;
    public AudioSource  fromOrToFerals;

    [Header("Easter Eggs or something")]
    public GameObject   thoughjak;
    private void Awake()
    {
        power = GetComponent<PowerUsage>();
        flashLightBaseIntensity = flashlight.GetComponent<Light>().intensity;
        onDeskLightToggled += ToggleAmbientLighting;
        NightManager.Instance.onNightEnd += EndNight;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;
        onDeskLightToggled -= ToggleAmbientLighting;
        NightManager.Instance.onNightEnd -= EndNight;
    }

    #region DOOR
    public void OpenCloseDoor()
    {
        if (NightManager.Instance.inTransition)
            return;
        if (onDoor != null)
            onDoor();
        OpenCloseDoor(!doorOpen);
    }

    public void OpenCloseDoor(bool open)
    {
        if (power.powerLeft <= 0 && !open)
            return;
        if (isMovingDoor || doorOpen == open)
            return;
        StartCoroutine(MoveDoor(open));
        power.UpdatePowerUsage(open ? -1 : 1);
    }

    private float DelayIfJakGoingThruDoor(bool open)
    {
        if (!open)
            return 0.2f;
        if (NightManager.Instance.jaks.Any(jak => jak.CurrentSpot.Cam.id == -1))
            return 0.6f;
        return 0.2f;
    }

    public IEnumerator MoveDoor(bool open, bool silent = false)
    {
        isMovingDoor = true;
        if (!silent)
        {
            doorAudio.clip = open ? doorSoundOpen : doorSoundClose;
            doorAudio.Play();
            FeralManager.Instance.Disturb(15f);
        }
        doorController.AnimDoor(open);
        yield return new WaitForSeconds(DelayIfJakGoingThruDoor(open));
        doorOpen = open;
        isMovingDoor = false;
    }
    #endregion

    #region VENTS DOOR
    public void OpenCloseVentsDoor()
    {
        if (NightManager.Instance.inTransition)
            return;
        if (onDoor != null)
            onDoor();
        OpenCloseVentsDoor(!ventsDoorOpen);
    }

    public void OpenCloseVentsDoor(bool open)
    {
        if (power.powerLeft <= 0 && !open)
            return;
        if (isMovingVentsDoor || ventsDoorOpen == open)
            return;
        StartCoroutine(MoveVentsDoor(open));
        power.UpdatePowerUsage(open ? -1 : 1);
    }

    private IEnumerator MoveVentsDoor(bool open, bool silent = false)
    {
        isMovingVentsDoor = true;
        if (!silent)
        {
            ventsDoorAudio.PlayOneShot( open ? ventsDoorSoundOpen : ventsDoorSoundClose);
            FeralManager.Instance.Disturb(15f);
        }
        ventsDoorController.AnimDoor(open);
        yield return new WaitForSeconds(0.1f);
        isMovingVentsDoor = false;
        ventsDoorOpen = open;
    }

    #endregion

    #region GEN DOOR
    public void OpenCloseGenDoor(bool open)
    {
        if (power.powerLeft <= 0 && open)
            return;
        if (isMovingGenDoor || genDoorOpen == open)
            return;
        StartCoroutine(MoveGenDoor(open));
        power.UpdatePowerUsage(open ? -1 : 1);
    }

    private IEnumerator MoveGenDoor(bool open, bool silent = false)
    {
        if (open && RollForThoughjak())
        {
            thoughjak.SetActive(true);
            if (ViewManager.Instance.currId == 3)
                AchievementsManager.Instance.UnlockChievo("secretchievo1");
        }
        isMovingGenDoor = true;
        genDoorButtons[0].interactable = false;
        genDoorButtons[1].interactable = false;
        genDoorController.AnimDoor(open);
        securityGUI.ToggleGenDoorButton(open);
        if (!silent)
            genDoorAudio.PlayOneShot(open ? doorSoundOpen : doorSoundClose, 0.65f);
        yield return new WaitForSeconds(0.25f);
        genDoorButtons[0].interactable = true;
        genDoorButtons[1].interactable = true;
        isMovingGenDoor = false;
        genDoorOpen = open;
        if (!open)
            thoughjak.SetActive(false);
        yield break;
    }

    private bool RollForThoughjak()
    {
        return UnityEngine.Random.Range(0f, 200f) <= 1f;
    }

    #endregion

    public bool flickering;
    private Coroutine flickeringRoutine;
    public void FlickerLights()
    {
        if (!InDarkness)
        {
            if (flickering)
                StopCoroutine(flickeringRoutine);
            flickeringRoutine = StartCoroutine(Flicker());
            AudioManager.Instance.LightFlicker();
        }
        
    }

    private bool wasDarkBeforeFlicker = false;

    public IEnumerator Flicker()
    {
        // Debug.Log("Start Flickering");
        flickering = true;
        wasDarkBeforeFlicker = !deskLight.activeSelf;
        if (ViewManager.Instance.currId == 0)
            ToggleDarkness(true);
        deskLight.GetComponent<LightFlicker>().flicker = true;
        flashlight.GetComponent<LightFlicker>().flicker = true;
        if (flashlight.activeSelf)
        {
            flashlightGUI.SetActive(false);
            flashlightOffUI.SetActive(true);
        }
        yield return new WaitForSeconds(0.8f);
        StopFlickering();
    }

    public void StopFlickering()
    {
        if (flashlightOffUI.activeSelf)
        {
            flashlightGUI.SetActive(true);
            flashlightOffUI.SetActive(false);
        }
        deskLight.GetComponent<LightFlicker>().flicker = false;
        deskLight.GetComponent<Light>().intensity = 0.5f;
        flashlight.GetComponent<LightFlicker>().flicker = false;
        flashlight.GetComponent<Light>().intensity = flashLightBaseIntensity;
        ToggleDarkness(wasDarkBeforeFlicker);
        flickering = false;
        // Debug.Log("End Flickering");
    }


    #region LIGHT


    public void ToggleLight()
    {
        if (NightManager.Instance.inTransition)
            return;
        ToggleLight(!deskLight.activeSelf);
    }

    private void ToggleLight(bool on, bool silent = false)
    {
        if (on && (flickering  || NightManager.Instance.inTransition))
            return;
        if (deskLight.activeSelf == on || NightManager.Instance.inTransition)
            return;
        if (power.powerLeft > 0 || !on)
        {
            computerScreen.SetActive(on);
            deskLight.SetActive(on);
            if (!silent)
            {
                computerBuzz.mute = !on;
                deskLightAudioSource.PlayOneShot(on ? lightOnSound : lightOffSound);
                FeralManager.Instance.Disturb(on ? 7f : 0f);
            }
            power.UpdatePowerUsage(on ? 1 : -1);
            if (onDeskLightToggled != null)
                onDeskLightToggled(on);

        }
    }

    private Color greyLight => new Color(96f / 255f, 95f / 255f, 95f / 255f);
    private Color blackLight => new Color(12f / 255f, 12f / 255f, 12f / 255f);
    // private Color cacaLight => new Color(42f / 255f, 42f / 255f, 42f / 255f);

    public void ToggleAmbientLighting(bool bright)
    {
        // bool caca = SoyGameController.Instance.IsCaca() && ViewManager.Instance.currId != 0;
        RenderSettings.ambientLight = bright || deskLight.activeSelf ? greyLight : blackLight;//(caca ? cacaLight : blackLight);
        RenderSettings.ambientIntensity = 0;
    }

    public void ToggleDarkness(bool dark)
    {

        RenderSettings.ambientLight = dark ? blackLight : greyLight;
        RenderSettings.ambientIntensity = 0;
    }

    #endregion

    #region FLASHLIGHT

    public void ToggleFlashlight(bool on, bool silent = false)
    {
        if (on && (flickering  || NightManager.Instance.inTransition || SoyGameController.Instance.inPause))
            return;
        if (flashlight.activeSelf == on)
            return;
        
        if (power.powerLeft > 0 || !on)
        {
            if (on && PlayerPrefs.GetInt("TutorialDone") == 0)
            {
                NightManager.Instance.tutorial.SetActive(false);
                PlayerPrefs.SetInt("TutorialDone", 1);
            }
            flashlightGUI.SetActive(on);
            flashlightOffUI.SetActive(false);
            if (!silent)
            {
                deskLightAudioSource.PlayOneShot(on ? flashLightOnSound : flashLightOffSound);
                FeralManager.Instance.Disturb(on ? 2f : 0f);
            }
            flashlight.SetActive(on);
            power.UpdatePowerUsage(on ? 1 : -1);
            RenderSettings.fogDensity = on ? 0.1f : 0.12f;
            if (onFlashLightToggled != null)
                onFlashLightToggled(on);
        }
    }
    #endregion

    public void PlayFootSteps(int aSource, AudioClip footsteps)
    {
        // Debug.Log("Play footsteps for " + (office ? "Office" : "Doorway"));
        if (aSource == 0)
            doorway.PlayOneShot(footsteps);
        else if (aSource == 1)
            fromOrToOffice.PlayOneShot(footsteps);
        else if (aSource == 2)
            fromOrToFerals.PlayOneShot(footsteps);
    }

    public void PlierOutage()
    {
        plierOutage = StartCoroutine(TempPowerOutage());
        if (PlayerPrefs.GetInt("PlierNeg") < 15)
            AchievementsManager.Instance.UpdateChievoStat("chievo8", "PlierNeg");
    }

    public IEnumerator TempPowerOutage()
    {
        // Debug.Log("Temporary power outage");
        computerScreen.SetActive(false);
        bool doorWasOpen = doorOpen;
        bool ventsDoorWasOpen = ventsDoorOpen;
        bool genDoorWasOpen = genDoorOpen;
        bool lightWasOn = deskLight.activeSelf;
        float powerWasAt = power.powerLeft;
        if (flickering)
            StopFlickering();
        ToggleDevices(false);
        power.powerLeft = 0;
        power.SetUsage(0);
        yield return new WaitForSeconds(17f);
        genDoorAudio.PlayOneShot(power.restoreSfx);
        yield return new WaitForSeconds(3f);
        power.RestorePower(powerWasAt);
        OpenCloseDoor(doorWasOpen);
        OpenCloseVentsDoor(ventsDoorWasOpen);
        OpenCloseGenDoor(genDoorWasOpen);
        ToggleLight(lightWasOn);
        computerScreen.SetActive(lightWasOn);
    }

    public void ToggleDevices(bool on, bool silent = false)
    {
        if (!silent)
            computerBuzz.mute = !on;
        // Debug.Log("Toggling devices: " + on);
        StartCoroutine(MoveDoor(!on, silent));
        StartCoroutine(MoveVentsDoor(!on, silent));
        StartCoroutine(MoveGenDoor(!on, silent));
        ToggleLight(on, silent);
        ToggleFlashlight(on, silent);
    }

    public void ResetPower()
    {
        power.Reset();
        power.usage = 1;
        power.outage = false;
        power.UpdatePowerUsage(0);
    }

    public void StartNight()
    {
        // Debug.Log("Office Controller start night");
        ToggleAmbientLighting(false);
        cellBarsHP = 100;
        SquirrelManager.Instance.UpdateCellBars(100);
        ACK.SetActive(false);
        ToggleDevices(false, true);
        deskLight.SetActive(true);
        ToggleAmbientLighting(true);
        computerScreen.SetActive(true);
    }

    private void EndNight()
    {
        if (plierOutage != null)
            StopCoroutine(plierOutage);
        computerBuzz.Stop();
        power.usage = 0;
        doorway.Stop();
        fromOrToOffice.Stop();
        ToggleAmbientLighting(true);
    }

    public void Doe()	
	{	
		if (thoughjak.activeSelf && genDoorOpen)
            AchievementsManager.Instance.UnlockChievo("secretchievo1");
	}	

    public bool InDarkness
    {
        get
        {
            return !flashlight.activeSelf && !deskLight.activeSelf;  
        }
    }
}
