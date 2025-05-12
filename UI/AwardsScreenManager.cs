using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AwardsScreenManager : MonoBehaviour
{
    private RectTransform rect;
    private void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        ResizeForSecrets();
    }

    private void ResizeForSecrets()
    {
        int nSecretsUnlocked = AchievementsManager.Instance.allAwards.Where(aw => aw.isSecret && aw.unlocked).Count();
        if (nSecretsUnlocked >= 5)
        {
            rect.localPosition = new Vector2(0,0);
            rect.anchoredPosition = new Vector2(0,0);
            rect.sizeDelta = new Vector2(0,0);
        }
        else if (nSecretsUnlocked >= 3)
        {
            rect.localPosition = new Vector2(0,180);
            rect.anchoredPosition = new Vector2(0,180);
            rect.sizeDelta = new Vector2(0,-360);
        }
        else if (nSecretsUnlocked >= 1)
        {
            rect.localPosition = new Vector2(0,350);
            rect.anchoredPosition = new Vector2(0,350);
            rect.sizeDelta = new Vector2(0,-700);
        }
        else
        {
            rect.localPosition = new Vector2(0,520);
            rect.anchoredPosition = new Vector2(0,520);
            rect.sizeDelta = new Vector2(0,-1040);
        }
    }


}
