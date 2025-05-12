using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiffSelector : MonoBehaviour
{
    [SerializeField]
    private Image    portrait;
    [SerializeField]
    private Image    background;
    [SerializeField]
    private Image    leftArrow;
    [SerializeField]
    private Image    rightArrow;
    [SerializeField]
    private TextMeshProUGUI   diffTxt;
    public List<Sprite> diffSprites = new List<Sprite>();
    private Behavior bhv;
    private Jak      jak;
    public bool isFeral = false;

    // public void AssignUIElements(Texture2D[] diffIcons)
    // {
    //     diffTxt = GetComponentInChildren<TextMeshProUGUI>();
    //     for (int i = 0; i < diffIcons.Length; i++)
    //         diffSprites.Add(FNACStatic.CreateSprite(diffIcons[i]));
    // }

    private void UpdateDiffDisplay(int diff)
    {
        diffTxt.text = diff.ToString();
        portrait.gameObject.SetActive(diff > 0);
        portrait.sprite = diffSprites[(diff - 1) / 4];
    }

    public void SetJak(Jak j, Behavior toAffect)
    {
        jak = j;
        bhv = toAffect;
    }

    public void SetAggro(int newAggro)
    {
        if (isFeral)
            FeralManager.Instance.baseAggro[0] = newAggro;
        else
        {
            if (newAggro == 0)
                bhv.bhvType = BehaviorType.Hidden;
            else
                bhv.bhvType = jak.variant == Variant.Plier ? BehaviorType.RoamingAimless : BehaviorType.RoamingTo;
            bhv.startingAggro = newAggro;
        }
        UpdateDiffDisplay(newAggro);
    }

    public int GetDiff()
    {
        if (isFeral)
            return FeralManager.Instance.baseAggro[0];
        return bhv.startingAggro;
    }

    public void UpdateDiffJak(int i)
    {
        int current = isFeral ? FeralManager.Instance.baseAggro[0] : bhv.startingAggro;
        int newAggro = Math.Clamp(current + i, 0, 20);
        SetAggro(newAggro);
        CustomNightManager.Instance.CheckIfPreset();
    }
}
