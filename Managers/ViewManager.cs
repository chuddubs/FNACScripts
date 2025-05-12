using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ViewManager : Singletroon<ViewManager>
{
    public ViewCam[] allviews;
    //the index of the current surveillance camera displayed
    public int currId = 0;
    public GameObject SecurityGUI;
    public GameObject SwitchGlitch;
    public GameObject MoveGlitch;
    public GameObject CameraGlitchEffects;
    public GameObject ventsNightvision;
    private bool switchGlitchShowing = false;
    private bool moveGlitchShowing = false;
    private float switchGlitchCooldown = 0.15f;
    private float moveGlitchCooldown = 0.3f;
    public System.Action<int> onCamSwitched;
    private PlayerCam playerCam;
    private int     last = 13;
    public Transform    zoomedOut;
    public Transform    zoomedIn;
    private bool        zooming = false;
    private SoyGameController gctrler;
    private Coroutine         setViewCamRoutine;

    private void Awake()
    {
        gctrler = SoyGameController.Instance;
        playerCam = PlayerCam.Instance;
    }

    private IEnumerator DisplaySwitchGlitch()
    {
        switchGlitchShowing = true;
        SwitchGlitch.SetActive(true);
        yield return new WaitForSeconds(switchGlitchCooldown);
        SwitchGlitch.SetActive(false);
        switchGlitchShowing = false;
    }

    public void ShowMoveGlitch()
    {
        if (!moveGlitchShowing)
            StartCoroutine(DisplayMoveGlitch());
    }

    private IEnumerator DisplayMoveGlitch()
    {
        moveGlitchShowing = true;
        MoveGlitch.SetActive(true);
        yield return new WaitForSeconds(moveGlitchCooldown);
        MoveGlitch.SetActive(false);
        moveGlitchShowing = false;
    }

    private void GetOnCams(int id)
    {
        if (id > 0)
        {
            OfficeController.Instance.ToggleAmbientLighting(true);
            OfficeController.Instance.ToggleFlashlight(false);
            OfficeController.Instance.power.UpdatePowerUsage(1);
            AudioManager.Instance.PlayOpenCams();
        }
    }

    private void SwitchCam(int id)
    {
        StartCoroutine(DisplaySwitchGlitch());
        if (GetCamById(id).isVents)
            AudioManager.Instance.PlayNightVision();
        else
            AudioManager.Instance.PlayCameraSwitch();
    }

    private void LeaveCams()
    {
        StopCoroutine(DisplaySwitchGlitch());
        StopCoroutine(DisplayMoveGlitch());
        OfficeController.Instance.power.UpdatePowerUsage(-1);
        OfficeController.Instance.ToggleAmbientLighting(false);
        last = currId;
        AudioManager.Instance.PlayLeaveCams();
    }

    public void SetViewCam(int id)
    {
        if (!zooming)
            setViewCamRoutine = StartCoroutine(SetViewCamRoutine(id));
        else if (id == 0 && setViewCamRoutine != null)
        {
            StopCoroutine(setViewCamRoutine);
            setViewCamRoutine = StartCoroutine(SetViewCamRoutine(id));
        }
    }

    private IEnumerator SetViewCamRoutine(int id)
    {
        zooming = true;
        if (currId == 0)
        {
            if (id != 0)
            {
                playerCam.enabled = false;
                yield return ZoomMonitor(zoomedOut, zoomedIn, 0.1f); 
            }
            else
            {
                playerCam.enabled = true;
                yield return ZoomMonitor(Camera.main.transform, zoomedOut, 0.05f); 
            }
            GetOnCams(id);
        }
        else if (id == 0) //leaving cams
            LeaveCams();
        else if (currId != id && !switchGlitchShowing)
            SwitchCam(id);
        ventsNightvision.SetActive(GetCamById(id).isVents);
        CameraGlitchEffects.SetActive(id != 0);
        Camera.main.fieldOfView = GetCamById(id).FOV;
        RenderSettings.fog = id == 0;
        SecurityGUI.SetActive(id != 0);
        if (currId != 0 && id == 0)
        {
            FNACStatic.SetPos(transform, GetCamById(0).cameraPosition);
            yield return ZoomMonitor(zoomedIn, zoomedOut, 0.1f);
        }
        else
            FNACStatic.SetPos(transform, GetCamById(id).cameraPosition);    
        playerCam.enabled = id == 0;
        if (currId != id && onCamSwitched != null)
            onCamSwitched(id);
        if (id == 3)
            OfficeController.Instance.Doe();
        currId = id;
        zooming = false;
    }

    private IEnumerator ZoomMonitor(Transform t1, Transform t2, float duration)
    {
        Transform camPos = GetCamById(0).cameraPosition;
        float t = 0;
        while(t < 1)
        {
                t += Time.smoothDeltaTime / duration;
                camPos.position =   Vector3.Lerp(t1.position, t2.position, t);
                camPos.rotation =   Quaternion.Lerp (t1.rotation, t2.rotation, t);
                yield return null;
        }
    }

    public void SetToLast()
    {
        if (NightManager.Instance.inTransition)
            return;
        SetViewCam(last);
    }

    private void Update()
    {
        if (gctrler.inMenu || gctrler.inMinigame)
            return;
        if (currId == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                OfficeController.Instance.ToggleFlashlight(true);
            if (Input.GetKeyUp(KeyCode.Mouse1))
                OfficeController.Instance.ToggleFlashlight(false);
        }
    }

    public ViewCam GetCamById(int id)
    {
        return allviews.First(vc => vc.id == id);
    }
}
