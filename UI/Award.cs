using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Award : MonoBehaviour
{
    public bool unlocked = false;
    public bool isSecret = false;
    public string chievoName;
    private Color locked = new Color(29f/255f, 29f/255f, 29f/255f, 1f);
    private Color keyed = new Color(101f/255f, 101f/255f, 101f/255f, 1f);
    private GameObject keyedImage;
    private GameObject lockedImage;

    public void Init()
    {
        keyedImage = transform.GetChild(0).gameObject;
        if (!isSecret)
            lockedImage = transform.GetChild(1).gameObject;
        LockUnlock(AchievementsManager.Instance.IsChievoUnlocked(chievoName));
    }

    public void LockUnlock(bool _unlocked)
    {
        unlocked = _unlocked;
        this.GetComponent<Image>().color = unlocked ? keyed : locked;
        if (isSecret)
        {
            keyedImage.SetActive(_unlocked);
            transform.GetChild(1).gameObject.SetActive(unlocked);
            this.GetComponent<Image>().enabled = unlocked;
            gameObject.SetActive(_unlocked);
            // if (!unlocked)
            //     gameObject.SetActive(false);
        }
        else
            lockedImage.SetActive(!unlocked);
    }

    public Sprite GetKeyedSprite()
    {
        return keyedImage.GetComponent<Image>().sprite;
    }

    public Sprite GetLockedSprite()
    {
        return lockedImage.GetComponent<Image>().sprite;
    }

    public string GetAwardTitle()
    {
        return transform.GetChild(isSecret ? 1 : 2).GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }
}
