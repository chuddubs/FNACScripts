using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AwardPopupController : MonoBehaviour
{
    public AudioClip chime;
    private Queue<IEnumerator> popupQueue = new Queue<IEnumerator>();
    private IEnumerator currentPopup = null;
    private Animator anim;
    private Sprite keyedSprite;
    private Sprite lockedSprite;
    public Image keyedImage;
    public Image lockedImage;
    public TextMeshProUGUI title;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
    }

    public void ShowPopup(Award award)
    {
        if (currentPopup == null)
        {
            currentPopup = Popup(award);
            StartCoroutine(currentPopup);
        }
        else
            popupQueue.Enqueue(Popup(award));
    }
    

    private IEnumerator Popup(Award award)
    {
        keyedImage.sprite = award.GetKeyedSprite();
        if (award.isSecret == false)
        {
            lockedImage.enabled = true;
            lockedImage.sprite = award.GetLockedSprite();
        }
        else
            lockedImage.enabled = false;
        title.text = award.GetAwardTitle();
        Debug.Log("Showing popup " + title.text);
        anim.enabled = true;
        anim.Play("awardpopup", -1, 0f);
        AudioManager.Instance.Play(chime);
        yield return new WaitForSeconds(3f);
        anim.enabled = false;
        currentPopup = null;
        if (popupQueue.Count > 0)
        {
            currentPopup = popupQueue.Dequeue();
            StartCoroutine(currentPopup);
        }
    }
}
