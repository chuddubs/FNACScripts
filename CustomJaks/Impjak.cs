using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Impjak : Jak
{
    private bool      officeRoutineLaunched = false;
    private bool      doorwayRoutineLaunched = false;
    private bool      ventRoutineLaunched = false;
    public GameObject   flashlitVentSprite;
    public GameObject   flashlitDoorSprite;
    
    public AudioClip  shriek;

    private bool    recoilingVent = false;
    private bool    recoilingDoor = false;

    private IEnumerator DoorwayRoutine()
    {
        doorwayRoutineLaunched = true;
        List<Jak> higherPrio = NightManager.Instance.jaks.Where(j => j.active &&  j.doorPriority > doorPriority).ToList();
        float timer = Random.Range(9f, 12f);
        while (timer >= 0)
        {
            if (OfficeController.Instance.doorOpen)
            {
                if (IsInFlashlight() && !recoilingDoor)
                {
                    recoilingDoor = true;
                    ChangeSprite(flashlitDoorSprite);
                    OfficeController.Instance.doorAudio.PlayOneShot(shriek, 1f);
                    yield return new WaitForSeconds(0.5f);
                    recoilingDoor = false;
                    Teleport(ViewManager.Instance.GetCamById(12).GetFreeSpotWithinView(this));
                    doorwayRoutineLaunched = false;
                    yield break;
                }
                else if (timer <= 2.5f && !higherPrio.Any(j => j.IsInOffice))
                {
                    Teleport(ViewManager.Instance.GetCamById(0).GetFreeSpotWithinView(this));
                    doorwayRoutineLaunched = false;
                    yield break;
                }
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Teleport(ViewManager.Instance.GetCamById(12).GetFreeSpotWithinView(this));
        doorwayRoutineLaunched = false;
    }


    private IEnumerator VentRoutine()
    {
        ventRoutineLaunched = true;
        float timer = Random.Range(9f, 12f);
        while (timer >= 0)
        {
            if (OfficeController.Instance.ventsDoorOpen)
            {
                if (IsInFlashlight() && !recoilingVent)
                {
                    recoilingVent = true;
                    OfficeController.Instance.ventsDoorAudio.PlayOneShot(shriek, 1f);
                    ChangeSprite(flashlitVentSprite);
                    yield return new WaitForSeconds(0.5f);
                    recoilingVent = false;
                    Teleport(ViewManager.Instance.GetCamById(15).GetFreeSpotWithinView(this));
                    ventRoutineLaunched = false;
                    yield break;
                }
                else if (timer <= 2.5f)
                {
                    Teleport(ViewManager.Instance.GetCamById(0).GetFreeSpotWithinView(this));
                    ventRoutineLaunched = false;
                    yield break;
                }
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Teleport(ViewManager.Instance.GetCamById(15).GetFreeSpotWithinView(this));
        ventRoutineLaunched = false;
    }

    private IEnumerator OfficeRoutine()
    {
        officeRoutineLaunched = true;
        float timer = IsWatched() ? 1.5f : 2.5f;
        while (timer >= 0)
        {
            if (IsInFlashlight() && !recoilingDoor)
            {
                recoilingDoor = true;
                PlayAudio(shriek);
                if (!OfficeController.Instance.doorOpen)
                    OfficeController.Instance.OpenCloseDoor(true);
                ChangeSprite(flashlitDoorSprite);
                yield return new WaitForSeconds(0.5f);
                recoilingDoor = false;
                Teleport(ViewManager.Instance.GetCamById(1).GetFreeSpotWithinView(this));
                officeRoutineLaunched = false;
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Screamer();
        officeRoutineLaunched = false;
    }

    public override void UpdateJak()
    {
        if (Locked() || !inCooldown)
            return;
        if (IsInDoorway)
        {
            if (!doorwayRoutineLaunched)
                StartCoroutine(DoorwayRoutine());
        }
        else if (currentSpot.Cam.id == 0)
        {
            if (!officeRoutineLaunched)
                StartCoroutine(OfficeRoutine());
        }
        else if (currentSpot.Cam.id == -5)
        {
            if (!ventRoutineLaunched)
                StartCoroutine(VentRoutine());
        }
        else
            base.UpdateJak();
    }

    public override void Interrupt()
    {
        base.Interrupt();
        doorwayRoutineLaunched = false;
        ventRoutineLaunched = false;
        officeRoutineLaunched = false;
        recoilingDoor = false;
        recoilingVent = false;
    }

    public override void EndNight()
    {
        Interrupt();
        base.EndNight();
    }
}
