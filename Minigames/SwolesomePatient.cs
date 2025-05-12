using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwolesomePatient : MonoBehaviour
{
    public SwolesomePlatformer gameCtrlr;
    public int value = 1;
    public AudioClip interactSound;
    private GameObject idle;
    private GameObject calm;
    private bool isDone = false;

    private void Awake()
    {
        idle = transform.GetChild(0).gameObject;
        calm = transform.GetChild(1).gameObject;
    }
    private void OnTriggerEnter2D(Collider2D body)
    {
        if (body.GetComponent<Rigidbody2D>() != null)
            GetMeds();
    }

    private void GetMeds()
    {
        if (isDone)
            return;
        gameCtrlr.Increment(value);
        isDone = true;
        AudioManager.Instance.Play(interactSound);
        idle.SetActive(false);
        calm.SetActive(true);
    }
}
