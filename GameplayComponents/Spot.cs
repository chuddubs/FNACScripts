using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    private ViewCam cam;

    private void Awake()
    {
        cam = transform.parent.GetComponent<ViewCam>();
    }

    public SpriteToUse spriteToUse;
    public bool IsVentExit
    {
        get
        {
            return spriteToUse == SpriteToUse.ClimbingVents || spriteToUse == SpriteToUse.PeeringVents;
        }
    }

    public ViewCam Cam
    {
        get
        {
            return cam;
        }
    }
}


public enum SpriteToUse
{
    Standing1, //near
    Standing2, //far
    StaringUp, //wherever you are
    CrawlingVents, //swede and fingerboy only
    ClimbingVents, //swede only
    PeeringVents,   //unused at the moment
    Hallway,
    Office,
    Close //only torso drawn (troon or chud leaving cam 6)
}



