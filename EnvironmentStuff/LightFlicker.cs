using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// Written by Steve Streeting 2017
// License: CC0 Public Domain http://creativecommons.org/publicdomain/zero/1.0/

/// <summary>
/// Component which will flicker a linked light while active by changing its
/// intensity between the min and max values given. The flickering can be
/// sharp or smoothed depending on the value of the smoothing parameter.
///
/// Just activate / deactivate this component as usual to pause / resume flicker
/// </summary>
public class LightFlicker : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 15;
    public bool flicker;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;


    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset() {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start() {

         smoothQueue = new Queue<float>(smoothing);
         // External or internal light?
         if (light == null) {
            light = GetComponent<Light>();
         }
         maxIntensity = light.intensity;
        // StartCoroutine(Beep());
    }

    // private IEnumerator Beep()
    // {
    //     while (true)
    //     {
    //         light.intensity = maxIntensity;
    //         yield return new WaitForSeconds(0.25f);
    //         light.intensity = 0f;
    //         yield return new WaitForSeconds(1f);
    //         yield return null;
    //     }
    // }

    void Update()
    {
        if (!flicker)
            return;

        // pop off an item if too big
        while (smoothQueue.Count >= smoothing) {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calculate new smoothed average
        float avg = lastSum / (float)smoothQueue.Count;
        light.intensity = avg <= maxIntensity * 0.5f ? 0f : avg;
        // light.intensity = avg <= maxIntensity * 0.5f ? 0f : maxIntensity;
        // OfficeController.Instance.ToggleAmbientLighting(avg > maxIntensity * 0.7f);
    }

}