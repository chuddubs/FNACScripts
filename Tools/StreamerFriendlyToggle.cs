using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreamerFriendlyToggle : MonoBehaviour
{
    public Transform[] streamerTogglables;
    public Toggle      toggle;

    public void ToggleStreamer(bool toggleOn)
    {
        toggle.isOn = toggleOn;
        foreach (Transform t in streamerTogglables)
        {
            t.GetChild(0).gameObject.SetActive(!toggleOn);
            t.GetChild(1).gameObject.SetActive(toggleOn);
        }
    }
}
