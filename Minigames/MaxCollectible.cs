using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MaxCollectible : MonoBehaviour
{
    public AudioClip pickupSound;
    public MaxPlatformer max;
    public int  value;

    private void OnTriggerEnter2D(Collider2D body)
    {
        max.PickUpCollectible(this);
    }
}
