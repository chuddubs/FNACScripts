using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ViewCam : MonoBehaviour
{
    public Transform cameraPosition;
    public int id = 0;
    [Range(0,120f)]
    public float FOV = 60;
    public ViewCam[] neighbors_backtrack;
    public ViewCam[] neighbors_progress;

    //list of positions where jaks can be seen by this camera
    [HideInInspector]
    public Spot[] spots;
    [Header("For Vents Camera")]
    public bool      isVents;
    public Spot[]      ventExits;

    private void Awake()
    {
        spots = GetComponentsInChildren<Spot>();
        if (spots == null || spots.Length == 0)
            Debug.LogError(this.name + "(cam " + id +") has no spots in children");
    }

    private Spot SwapJaksAtDoorway(Jak jak)
    {
        Spot doorwaySpot = spots[0];
        Jak atDoor = NightManager.Instance.jaks.FirstOrDefault(jak => jak.CurrentSpot.Cam == this);
        if (atDoor != null)
        {
            if (jak.doorPriority > atDoor.doorPriority)
                atDoor.Teleport(jak.currentSpot, false);
            else 
                return jak.currentSpot;
        }
        return doorwaySpot;
    }

    public virtual Spot GetFreeSpotWithinView(Jak jak)
    {
        List<Spot> eligibles;
        if (id == -1)
            return SwapJaksAtDoorway(jak);
        else
            eligibles = spots.Where(s => !s.IsVentExit).Where(spot => !JaksInRoom.Any(jak => jak.CurrentSpot == spot)).ToList();
        if (eligibles.Count > 0)
            return eligibles[Random.Range(0, eligibles.Count)]; //found a new empty spot in this view
        // Debug.LogWarning("All spots in this viewCam were taken, jak doesn't move");
        return jak.CurrentSpot; //no unoccupied spot in this view so jak doesn't move
    }

    public Spot GetVentEntryPoint(ViewCam cam)
    {
        foreach (Spot ventExit in ventExits)
        {
            if (ventExit.Cam == cam)    //jak is seen entering vent by another cam
                return ventExit;
        }
        return ventExits.FirstOrDefault(s => s.Cam == this); //jak is seen peering into this vent by this vent's cam.
    }

    public List<Jak> JaksInRoom
    {
        get
        {
            return NightManager.Instance.jaks.Where(jak => jak.CurrentSpot.Cam == this).ToList();
        }
    }

    public bool isPlierInRoom
    {
        get
        {            
            return JaksInRoom.Any(jak => jak.variant == Variant.Plier);
        }
    }

    public bool IsInOffice
    {
        get
        {
            return id == 0 || id == -5;
        }
    }

    public bool IsHallway
    {
        get
        {
            return id <= -1 && id >= -3;
        }
    }
}
