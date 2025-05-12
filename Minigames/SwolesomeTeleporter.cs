using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwolesomeTeleporter : MonoBehaviour
{
    public Transform teleportTo;

    private void OnTriggerEnter2D(Collider2D body)
    {
        if (body.GetComponent<Rigidbody2D>() != null)
            body.transform.position = new Vector3(teleportTo.position.x, body.transform.position.y, body.transform.position.z);
    }
}
