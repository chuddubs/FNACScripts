using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HighlightOnHover : MonoBehaviour
{
    private NightManager nm;
    private SoyGameController  sg;
    private Outline outline;

    private void Awake()
    {
        nm = NightManager.Instance;
        sg = SoyGameController.Instance;
        outline = GetComponent<Outline>();
        // FNACStatic.SetLayerRecursively(transform, LayerMask.NameToLayer("Default"));
    }

    private void OnMouseOver()
    {
        outline.enabled = CanHiglight();
        // FNACStatic.SetLayerRecursively(transform, LayerMask.NameToLayer(CanHiglight() ? "Outlined" : "Default"));
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
        // FNACStatic.SetLayerRecursively(transform, LayerMask.NameToLayer("Default"));
    }

    private bool CanHiglight()
    {
        return !nm.inTransition && !sg.inMenu && !sg.inMinigame && !sg.inPause;
    }


}
