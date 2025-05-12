
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Behavior : MonoBehaviour
{
    [Range(0, 7)]
    public int night = 1;
    public BehaviorType bhvType;
    [Header("Aggro (leave all at 0 if Hidden or Still)")]
    [Range(0, 20)]
    public int startingAggro = 0;
    public int aggroGainAt2AM = 0;
    public int aggroGainAt4AM = 0;

    [Tooltip("Only set 1 if Behavior is Idle or Still")]
    public ViewCam[]   camsWhereSeen;
    [Tooltip("Only set 1 if Behavior is Still")]
    public Spot[]       startSpots;
    // [Tooltip("number  of spots this jak will jump back to when forced to backtrack")]
    // public int backtrack_steps = 1;

    private Jak jak;
    
    private void Awake ()
    {
        jak = transform.parent.GetComponent<Jak>();
    }
    public Spot GetRandomStartPos()
    {
        List<Spot> eligibles = startSpots.Where(spot => !spot.Cam.JaksInRoom.Any(jak => jak.CurrentSpot == spot))?.ToList();
        if (eligibles.Count == 0)
        {
            foreach (Spot s in startSpots)
            {
                Spot[] freeSpotsInCam = s.Cam.spots.Where(spot => !spot.Cam.JaksInRoom.Any(jak => jak.CurrentSpot == spot))?.ToArray();
                if (freeSpotsInCam.Length > 0)
                {
                    foreach (Spot f in freeSpotsInCam)
                        eligibles.Add(f);
                }
            }
            if (eligibles.Count == 0)
                return startSpots[0];
        }
        return eligibles[Random.Range(0, eligibles.Count)];
    }

    //-1: force backtrack
    //0: coinflip backtrack or progress
    //1: progress
    public Spot GetNewSpotFromBehavior()
    {
        if (bhvType == BehaviorType.Hidden || bhvType == BehaviorType.Still)
            return jak.CurrentSpot;
        if (bhvType == BehaviorType.IdleSameCam)
            return jak.CurrentSpot.Cam.GetFreeSpotWithinView(jak);
        if (bhvType == BehaviorType.RoamingAimless)
        {
            if (Random.value < 0.5f)
                return GetBackTrackSpot();
        }
        return GetProgressSpot();
    }

    //picks a progress spot in one of the jak's allowed cameras
    public Spot GetProgressSpot()
    {
        // Debug.Log("Progressing");
        ViewCam currView = jak.CurrentSpot.Cam;
        ViewCam[] eligibles;
        ViewCam picked;
        if (currView != null && currView.neighbors_progress != null && currView.neighbors_progress.Length > 0)
        {
            eligibles = currView.neighbors_progress.Where(cam => camsWhereSeen.Contains(cam)).ToArray();
            if (eligibles.Length > 0)
            {
                picked = eligibles[Random.Range(0, eligibles.Length)];
                return picked.GetFreeSpotWithinView(jak);
            }
        }
        Debug.LogWarning("Found no progress spot");
        return jak.CurrentSpot;
    }

    //picks a backtrack spot in one of the jak's allowed cameras
    public Spot GetBackTrackSpot()
    {
        // Debug.Log("Backtracking");
        ViewCam currView = jak.CurrentSpot.Cam;
        ViewCam[] eligibles;
        ViewCam picked;
        if (currView != null && currView.neighbors_backtrack != null && currView.neighbors_backtrack.Length > 0)
        {
            eligibles = currView.neighbors_backtrack.Where(cam => camsWhereSeen.Contains(cam)).ToArray();
            if (eligibles.Length > 0)
            {
                picked = eligibles[Random.Range(0, eligibles.Length)];
                return picked.GetFreeSpotWithinView(jak);
            }
        }
        Debug.LogWarning("Found no backtrack spot");
        return jak.CurrentSpot;
    }
}

public enum BehaviorType
{
        Hidden,
        Still,
        IdleSameCam,    //hanges spots within same viewcam
        RoamingAimless, //changes spots between several viewcams
        RoamingTo,      //changes spots towards goal spots
}
