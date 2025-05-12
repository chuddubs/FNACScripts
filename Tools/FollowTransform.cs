using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform toFollow;
    public bool x = false;
    public bool y = false;
    public bool z = false;

    Vector3 pos;
    void Update()
    {
        pos = transform.position;
        if (x)
            pos.x = toFollow.position.x;
        if (y)
            pos.y = toFollow.position.y;
        if (z)
            pos.z = toFollow.position.z;
        transform.position = pos;
    }
}
