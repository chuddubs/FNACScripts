using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class SwolesomeRoomLooper : MonoBehaviour
{
    public AudioSource  audiosource;
    public RawImage renderTexture;
    public GameObject teleporter;
    private bool      ending = false;
    private void OnTriggerEnter2D(Collider2D body)
    {
        if (!ending && body.GetComponent<Rigidbody2D>() != null)
        {
            ending = true;
            teleporter.SetActive(true);
            StartCoroutine(FadeEnd());
        }
    }

    private IEnumerator FadeEnd()
    {
        yield return new WaitForSeconds(5f);
        float t= 0;
        while(t < 1)
        {
                t += Time.smoothDeltaTime / 15f;
                audiosource.volume = Mathf.Lerp(1f, 0f, t);
                renderTexture.color = Color.Lerp(Color.white, Color.black, t);
                yield return null;
        }
        MinigamesManager.Instance.EndMinigame(false);
    }
}
