using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chudjak : Jak
{
    public GameObject nearTroonSprite;
    public GameObject seethingSprite;
    public GameObject hallway2;
    public GameObject inCam6;
    private Jak       troon;
    private bool      inOfficeRoutineLaunched = false;
    private bool      doorWayRoutineLaunched = false;
    public KeyboardEasterEgg egg;
    private void Start()
    {
        troon = NightManager.Instance.jaks.First(jak => jak.variant == Variant.Troon);
        egg.enabled = false;
    }

    public override void ChangeSprite(GameObject newSprite)
    {
        if (currentSpot.Cam.id == -3 && Random.Range(0, 100f) > 50f)
            newSprite = hallway2;
        base.ChangeSprite(newSprite);
    }

    private IEnumerator DoorwayRoutine()
    {
        doorWayRoutineLaunched = true;
        egg.enabled = true;
        yield return new WaitForSeconds(Random.Range(5f, 8f));
        if (!OfficeController.Instance.doorOpen)
            OfficeController.Instance.OpenCloseDoor(true);
        yield return new WaitForSeconds(0.6f);
        Teleport(ViewManager.Instance.GetCamById(0).GetFreeSpotWithinView(this));
        doorWayRoutineLaunched = false;
    }

    private IEnumerator InOfficeRoutine()
    {
        inOfficeRoutineLaunched = true;
        FeralManager.Instance.Disturb(15f);
        yield return new WaitForSeconds(IsWatched() ? 2f : 3.5f);
        if (!OfficeController.Instance.InDarkness)
        {
                Screamer();
                egg.enabled = false;
                yield break;
        }
        float timer = Random.Range(4f, 6f);
        while (timer > 0f)
        {
            if (!OfficeController.Instance.InDarkness)
            {
                Screamer();
                egg.enabled = false;
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        if (!OfficeController.Instance.doorOpen)
        {
            OfficeController.Instance.OpenCloseDoor(true);
            yield return new WaitForSeconds(0.6f);
        }
        egg.enabled = false;
        Teleport(ViewManager.Instance.GetCamById(1).GetFreeSpotWithinView(this));
        AchievementsManager.Instance.UpdateChievoStat("chievo6", "ChudExp");
        inOfficeRoutineLaunched = false;        
    }

    public override void UpdateJak()
    {
        if (Locked() || !inCooldown)
            return;
        if (IsInDoorway)
        {
            if (!doorWayRoutineLaunched)
                StartCoroutine(DoorwayRoutine());
        }
        else if (currentSpot.Cam.id ==  0)
        {
            if (!inOfficeRoutineLaunched)
                StartCoroutine(InOfficeRoutine());
        } 
        else
            base.UpdateJak();
        if (currentSpot.Cam.id == 6)
            ChangeSprite(troon.CurrentSpot.Cam.id == 6 ? nearTroonSprite : inCam6);
        if (currentSpot.Cam.id == 7)
            ChangeSprite(troon.CurrentSpot.Cam.id == 7 ? seethingSprite : standingNear);
    }

    public void RestoreHyperborea()
    {
        JumpscaresManager.Instance.PlayJumpscare(Jumpscare.Hyperborea);
        audioSource.Stop();
        EndNight();
    }

    public override void Interrupt()
    {
        base.Interrupt();
        doorWayRoutineLaunched = false;
        inOfficeRoutineLaunched = false;
        egg.enabled = false;
    }
}
