using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    public float angle = 50f;
    [Range(1f, 100f)]
    public float speed = 10f;

    private Quaternion startRotation;
    private Quaternion targetRotation;
    private int direction = 1; // 1 for right, -1 for left

    private void Awake()
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0f, angle * direction, 0f) * startRotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
        {
            direction *= -1;
            targetRotation = Quaternion.Euler(0f, angle * direction, 0f) * startRotation;
        }
    }
}