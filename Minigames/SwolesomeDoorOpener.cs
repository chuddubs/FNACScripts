using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwolesomeDoorOpener : MonoBehaviour
{
    public GameObject  sprite;

    private void OnTriggerEnter2D(Collider2D body)
    {
        if (body.GetComponent<Rigidbody2D>() != null)
            sprite.SetActive(false);
    }
}
