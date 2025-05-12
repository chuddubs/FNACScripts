using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Gapejak : MonoBehaviour
{
    public float odds = 10000f;
    private bool isActive = false;
    public Jak  rapeson;
    private ViewManager vm;
    private Spot        currentSpot;
    void Start()
    {
        vm = ViewManager.Instance;
        vm.onCamSwitched += OnCamSwitched;
        currentSpot = NightManager.Instance.inactiveSpot;
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
            return;
        vm.onCamSwitched -= OnCamSwitched;
    }

    private void OnCamSwitched(int id)
    {
        if (isActive && id != currentSpot.Cam.id)
            Teleport(NightManager.Instance.inactiveSpot);
        else if (id > 0 && Random.Range(0f, odds) < 1f)
        {
            if (id == 4)
                return;
            if (id == 2 && rapeson.CurrentSpot.Cam.id == -11)
                return;
            if (rapeson.CurrentSpot.Cam.id != id)
            {
                Spot s = PickEmptyStandingSpot(vm.GetCamById(id));
                if (s != null)
                {
                    Teleport(s);
                    AchievementsManager.Instance.UnlockChievo("secretchievo4");
                }
            }
        }
            
    }

    private Spot PickEmptyStandingSpot(ViewCam cam)
    {
        List<Spot> eligibles;
            eligibles = cam.spots.Where(s => s.spriteToUse == SpriteToUse.Standing1 || s.spriteToUse == SpriteToUse.Standing2)?.Where(spot => !cam.JaksInRoom.Any(jak => jak.CurrentSpot == spot)).ToList();
        if (eligibles.Count > 0)
            return eligibles[Random.Range(0, eligibles.Count)]; //found a new empty standing spot in this view
        return null;
    }

    private void Teleport(Spot newpos)
    {
        if (ViewManager.Instance.currId == currentSpot.Cam.id)
            ViewManager.Instance.ShowMoveGlitch();
        transform.SetParent(newpos.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        currentSpot = newpos;
        isActive = newpos.Cam.id != -88;
    }

    private void Update()
    {
        if (!isActive)
            return;
        if (rapeson.currentSpot.Cam == currentSpot.Cam)
            Teleport(NightManager.Instance.inactiveSpot);   
    }

}
