using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MaxChuddy : MonoBehaviour
{
    public MaxPlatformer max;
    public Animator     animator;
    private bool        killing = false;

    private void OnTriggerEnter2D(Collider2D body)
    {
        KillMax();
    }

    private void KillMax()
    {
        if (!killing)
            StartCoroutine(Kill());
        killing = true;
    }

    private IEnumerator Kill()
    {
        max.SetDeath();
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        max.audioSource.PlayOneShot(max.rifle);
        yield return new WaitForSeconds(0.2f);
        max.GetShot();
    }
}
