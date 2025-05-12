using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CustomNightManager : Singletroon<CustomNightManager>
{

    [SerializeField]
    private MainMenuManager mainMenu;

    [Header("UI elements")]
    [SerializeField]
    private DiffSelector[]  selectors;
    public Image  playButton;
    public Image  backButton;
    public Image  presetLeft;
    public Image  presetRight;
    public GameObject  presetGem;
    public GameObject  presetGemHard;
    [SerializeField]
    private TextMeshProUGUI presetNameTxt;
    [SerializeField]
    private Toggle          hardMode;

    [Header("Pre-Night Screen")]
    [SerializeField]
    private Image           preNightBg;
    [SerializeField]
    private Image           preNightFilter;
    [SerializeField]
    private GameObject[]    jakPreNightIcons;

    private int currentPreset = 0;
    private bool            isHard = false;

    private void OnEnable()
    {
        GetJaksFromVersion();
        if (currentPreset == -1)
            SelectPreset("Custom", false);
        else
            SelectPreset(FNACStatic.presetNames[currentPreset], isHard);
    }

    public void GetJaksFromVersion()
    {
        GameVersion version = SoyGameController.Instance.version;
        Jak[] jaks = NightManager.Instance.jaks;
        if (version == GameVersion.Base || version == GameVersion.Caca)
        {
            for (int i = 0; i < 6; i++)
            {
                jaks[i] = NightManager.Instance.jaks.FirstOrDefault(j => j.variant == FNACStatic.jaksBaseVersion[i]);
                selectors[i].SetJak(jaks[i], jaks[i].GetComponentsInChildren<Behavior>().FirstOrDefault(b => b.night == 0));
            }
        }
        selectors[6].isFeral = true;
    }

    // public void PopulateSprites(List<Texture2D> cnSprites)
    // {
    //     PopulatePreNightSprites(cnSprites.Where(s => s.name.StartsWith("cni_")).ToList());
    //     Sprite tile = FNACStatic.CreateSprite(cnSprites.FirstOrDefault(s => s.name == "squareroundedges"));
    //     Sprite arrow =  FNACStatic.CreateSprite(cnSprites.FirstOrDefault(s => s.name == "arrowdiff"));
    //     Sprite gem =  FNACStatic.CreateSprite(cnSprites.FirstOrDefault(s => s.name == "presetwon"));
    //     presetLeft.sprite = arrow;
    //     presetRight.sprite = arrow;
    //     playButton.sprite = tile;
    //     backButton.sprite = tile;
    //     presetGem.GetComponent<Image>().sprite = gem;
    //     presetGemHard.GetComponent<Image>().sprite = gem;
    //     for (int i = 0; i < 7; i++)
    //         selectors[i].AssignUIElements(GetDiffIcons(cnSprites, i));
    //     SelectPreset(FNACStatic.presetNames[0], false);
    // }

    // public void PopulateVideo(VideoClip morningScreen)
    // {
    //     NightManager.Instance.morningScreens[0] = morningScreen;
    // }

    // private void PopulatePreNightSprites(List<Texture2D> cni)
    // {
    //     Sprite bg = FNACStatic.CreateSprite(cni.FirstOrDefault(s => s.name.Contains("background")));
    //     Sprite filter = FNACStatic.CreateSprite(cni.FirstOrDefault(s => s.name.Contains("filter")));
    //     CuckFriendlyToggle cucker = SoyGameController.Instance.GetComponent<CuckFriendlyToggle>();
    //     for (int i = 0; i < 7; i++)
    //     {
    //         Texture2D[] jakSprites = cni.Where(s => s.name.Contains(FNACStatic.jaksTexNameBaseVersion[i])).ToArray();
    //         Texture2D jSprite = jakSprites.FirstOrDefault(s => !s.name.Contains("_cuck_"));
    //         Texture2D cuckSprite = jakSprites.FirstOrDefault(s => s.name.Contains("_cuck_"));
    //         jakPreNightIcons[i].transform.GetChild(0).GetComponent<Image>().sprite = FNACStatic.CreateSprite(jSprite);
    //         if (cuckSprite != null)
    //         {
    //             jakPreNightIcons[i].transform.GetChild(1).GetComponent<Image>().sprite = FNACStatic.CreateSprite(cuckSprite);
    //             Array.Resize(ref cucker.cuckables, cucker.cuckables.Length + 1);
    //             cucker.cuckables[cucker.cuckables.Length - 1] = jakPreNightIcons[i].transform;
    //         }
    //     }
    //     preNightBg.sprite = bg;
    //     preNightFilter.sprite = filter;
    // }

    private void TogglePreNightIcons()
    {
        for (int i = 0; i < 7; i++)
            jakPreNightIcons[i].SetActive(selectors[i].GetDiff() > 0);
    }

    // public Texture2D[] GetDiffIcons(List<Texture2D> cnSprites, int i)
    // {
    //     string prefix;
    //     GameVersion version = SoyGameController.Instance.version;
    //     if (version == GameVersion.Base || version == GameVersion.Caca)
    //         prefix = FNACStatic.jaksTexNameBaseVersion[i];
    //     else
    //         prefix = FNACStatic.jaksTexNameBaseVersion[i]; //will change depending on version
    //     return cnSprites.Where(s => s.name.StartsWith(prefix)).OrderBy(s => s.name).ToArray();
    // }

    public void BtClickBack()
    {
        mainMenu.ToggleVCR(true);
        gameObject.SetActive(false);
    }

    public void BtClickPlay()
    {
        AchievementsManager.Instance.eligible720 = currentPreset == 0 && isHard;
        TogglePreNightIcons();
        mainMenu.ToggleVCR(true);
        BtClickBack();
        mainMenu.BtClickPlay(0);
    }

    public void CheckIfPreset()
    {
        int presetsNumber = FNACStatic.presetNames.Length;
        for (int i = 0; i < presetsNumber; i++)
        {
            string presetName = FNACStatic.presetNames[i];
            bool matches = true;
            bool matchesHard = true;
            for(int j = 0; j < 7; j++)
            {
                if (selectors[j].GetDiff() != FNACStatic.presetValues[presetName][j])
                    matches = false;
                if (selectors[j].GetDiff() != FNACStatic.presetValues[presetName][j] * 2)
                    matchesHard = false;
            }
            if (matches || matchesHard)
            {
                currentPreset = i;
                SelectPreset(presetName, matchesHard);
                return;
            }
        }
        SelectPreset("Custom", false);
    }

    public void SelectPreset(string name, bool hard)
    {
        presetNameTxt.text = name;
        if (name != "Custom")
        {
            for(int i = 0; i < 7; i++)
                selectors[i].SetAggro(FNACStatic.presetValues[name][i] * (hard ? 2 : 1));
            hardMode.isOn = hard;
            isHard = hard;
        }
        else
        {
            currentPreset = -1;
            hardMode.SetIsOnWithoutNotify(false);
            isHard = false;
        }
        hardMode.interactable = name != "Custom";
        TogglePresetGems(currentPreset);
    }

    private void TogglePresetGems(int preset)
    {
        int isWon;
        if (preset < 0)
            isWon = 0;
        else
            isWon = PlayerPrefs.GetInt("WonPreset" + preset.ToString());
        presetGem.SetActive(isWon > 0);
        presetGemHard.SetActive(isWon > 1);
    }

    public void ClickPresetArrow(int i)
    {
        int presetsNumber = FNACStatic.presetNames.Length - 1;
        if (currentPreset + i > presetsNumber)
            currentPreset = 0;
        else if (currentPreset + i < 0)
            currentPreset = presetsNumber;
        else
            currentPreset += i;
        SelectPreset(FNACStatic.presetNames[currentPreset], false);
    }

    public void ToggleHardMode(bool on)
    {
        SelectPreset(FNACStatic.presetNames[currentPreset], on);
    }

    public void BeatPreset()
    {
        if (currentPreset < 0)
            return;
        PlayerPrefs.SetInt("WonPreset" + currentPreset.ToString(), isHard ? 2 : 1);
    }
}
