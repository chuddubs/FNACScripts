using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    public GameObject popup1;
    public GameObject popup2;
    public OptionsAndSubOptionsComunicator com;

    public void TogglePopup1(bool on)
    {
        popup1.SetActive(on);
        if (!on)
        {
            this.gameObject.SetActive(false);
            com.BackToOptions(4);
        }

    }
    public void TogglePopup2(bool on)
    {
        popup2.SetActive(on);
        this.gameObject.SetActive(false);
        com.BackToOptions(4);
    }

    public void ResetProg()
    {
        PlayerPrefs.SetInt("ReachedNight", 1);
        PlayerPrefs.SetInt("TutorialDone", 0);
        PlayerPrefs.SetInt("GameWon", 0);
        PlayerPrefs.SetInt("WonPreset0", 0);
        PlayerPrefs.SetInt("WonPreset1", 0);
        PlayerPrefs.SetInt("WonPreset2", 0);
        PlayerPrefs.SetInt("WonPreset3", 0);
        PlayerPrefs.SetInt("WonPreset4", 0);
        PlayerPrefs.SetInt("WonPreset5", 0);
        PlayerPrefs.SetInt("WonPreset6", 0);
        popup1.SetActive(false);
        popup2.SetActive(true);
    }

    public void ResetAwardProg()
    {
        PlayerPrefs.SetString("ItsAnEnding", "");
        PlayerPrefs.SetString("DOCTOS", "");
        PlayerPrefs.SetString("Feral", "");
        PlayerPrefs.SetInt("ChudExp", 0);
        PlayerPrefs.SetInt("TroonExp", 0);
        PlayerPrefs.SetInt("PlierNeg", 0);
        PlayerPrefs.SetString("beatn1", "");
        PlayerPrefs.SetString("beatn2", "");
        PlayerPrefs.SetString("beatn3", "");
        PlayerPrefs.SetString("beatn4", "");
        PlayerPrefs.SetString("beatn5", "");
        PlayerPrefs.SetString("chievo5", "");
        PlayerPrefs.SetString("chievo6", "");
        PlayerPrefs.SetString("chievo7", "");
        PlayerPrefs.SetString("chievo8", "");
        PlayerPrefs.SetString("chievo9", "");
        PlayerPrefs.SetString("chievo10", "");
        PlayerPrefs.SetString("chievo11", "");
        PlayerPrefs.SetString("chievo12", "");
        PlayerPrefs.SetString("chievo13", "");
        PlayerPrefs.SetString("chievo14", "");
        PlayerPrefs.SetString("chievo15", "");
        PlayerPrefs.SetString("chievo16", "");
        PlayerPrefs.SetString("chievo17", "");
        PlayerPrefs.SetString("chievo18", "");
        PlayerPrefs.SetString("chievo19", "");
        PlayerPrefs.SetString("secretchievo1", "");
        PlayerPrefs.SetString("secretchievo2", "");
        PlayerPrefs.SetString("secretchievo3", "");
        PlayerPrefs.SetString("secretchievo4", "");
        PlayerPrefs.SetString("secretchievo5", "");
        PlayerPrefs.SetString("secretchievo6", "");
        popup2.SetActive(false);
        this.gameObject.SetActive(false);
        com.BackToOptions(4);
    }
}