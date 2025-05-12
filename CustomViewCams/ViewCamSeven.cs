using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCamSeven : ViewCam
{
    public Spot Chuds;
    public Spot Troons;
    public Spot Swedes;

    public override Spot GetFreeSpotWithinView(Jak jak)
    {
        if (jak.variant == Variant.Chud)
            return Chuds;
        else if (jak.variant == Variant.Troon)
            return Troons;
        return Swedes;
    }

}