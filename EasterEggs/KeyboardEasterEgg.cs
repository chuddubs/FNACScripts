using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardEasterEgg : MonoBehaviour
{
    public KeyCode[] sequence;
    public UnityEvent onSequenceTyped;
    private NightManager nm;
    private SoyGameController sgc;
    
    private int index = 0;

    private void Start()
    {
        nm = NightManager.Instance;
        sgc = SoyGameController.Instance;
    }

    private void OnEnable()
    {
        index = 0;
    }

    private void OnDisable()
    {
        index = 0;
    }
    
    private void Update()
    {
        if (nm.inTransition || sgc.inMenu || sgc.inMinigame || sgc.inPause)
            return;
        if (Input.anyKeyDown)
            index = Input.GetKeyDown(sequence[index]) ? index + 1 : 0;
        if (index >= sequence.Length)
        {
            onSequenceTyped.Invoke();
            index = 0;
            enabled = false;
        }
    }
}
