using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewCamSix : ViewCam
{
    public Spot[] fromCam5;
    public Spot[] fromCam7;
    public Spot[] middle;
    public Spot[] leaving;

    public override Spot GetFreeSpotWithinView(Jak jak)
    {
        List<Jak> jaksInRoom = JaksInRoom;
        List<Spot> eligibles = new List<Spot>();
        if (jak.currentSpot.Cam.id == 5)
            eligibles = fromCam5.Where(spot => !jaksInRoom.Any(jak => jak.CurrentSpot == spot)).ToList();
        else if (jak.currentSpot.Cam.id == 14)
            eligibles = fromCam7.Where(spot => !jaksInRoom.Any(jak => jak.CurrentSpot == spot)).ToList();
        else
            eligibles = middle.Where(spot => !jaksInRoom.Any(jak => jak.CurrentSpot == spot)).ToList();
        if (eligibles.Count > 0)
            return eligibles[Random.Range(0, eligibles.Count)]; //found a new empty spot in this view
        Debug.LogWarning("All spots in this viewCam were taken, jak doesn't move");
        return jak.CurrentSpot; //no unoccupied spot in this view so jak doesn't move
    }
}
