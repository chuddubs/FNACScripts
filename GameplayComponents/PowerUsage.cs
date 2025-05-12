using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUsage : MonoBehaviour
{
    public int          usage = 1;
    public float        powerLeft = 100f;
    private float       baseTimeToDrain = 6f; //seconds to drain 1% of power
    private float       timeBeforeDrain = 6f; //seconds left until losing next percent
    public bool        outage = false;
    public float        decayExponent = 0.8f;
    public AudioClip    outageSfx;
    public AudioClip    restoreSfx;
    public GameObject[] powerBars;
    public GameObject[] usageStates;
    public TextMeshProUGUI   powerDisplay;

    public void UpdatePowerUsage(int d)
    {
        usage = Mathf.Clamp(usage + d, 0, 6);
        for (int i = 0; i < 7; i++)
            usageStates[i].SetActive(i == usage);
    }

    public void SetUsage(int u)
    {
        usage = u;
        UpdatePowerUsage(0);
    }

    public void UpdatePowerDisplay()
    {
        powerDisplay.text = powerLeft.ToString() + " %";
        int bars = Mathf.Clamp(Mathf.CeilToInt(powerLeft / 20), 0, 4);
        for (int i = 0; i < 5; i++)
            powerBars[i].SetActive(i == bars);
        
    }

    public void DrainPower(int drain)
    {
        powerLeft = Mathf.Max(0, powerLeft - drain);
        UpdatePowerDisplay();
    }

    void Update()
    {
        if (usage > 0)
        {
            timeBeforeDrain -= Time.deltaTime;
            if (timeBeforeDrain <= 0)
            {
                DrainPower(1);
                timeBeforeDrain = baseTimeToDrain / Mathf.Pow(usage, decayExponent);
            }
        }
        if (powerLeft <= 0 && !outage)
            Outage();
    }

    public void Outage(bool temporary = false)
    {
        outage = true;
        AudioManager.Instance.Play(outageSfx);
        FeralManager.Instance.Disturb(12f);
        ViewManager.Instance.SetViewCam(0);
        if (temporary)
            OfficeController.Instance.PlierOutage();
        else
        {
            UpdatePowerUsage(-usage);
            UpdatePowerDisplay();
            OfficeController.Instance.ToggleDevices(false);
        }
    }

    public void RestorePower(float remainingPower)
    {
        powerLeft = remainingPower;
        outage = false;
        UpdatePowerUsage(0);
        UpdatePowerDisplay();
    }

    public void Reset()
    {
        outage = false;
        powerLeft = 100f;
        usage = 1;
        timeBeforeDrain = baseTimeToDrain;
        UpdatePowerUsage(0);
        UpdatePowerDisplay();
    }
}
