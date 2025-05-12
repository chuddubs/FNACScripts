using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MakeKinematic : MonoBehaviour
{
    void Start()
    {
        Invoke("TurnKinematic", 5f);      
    }

    void TurnKinematic()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

}
