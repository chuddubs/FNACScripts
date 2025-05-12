using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SoySettings : Singletroon<SoySettings>
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public AudioMixer mainMixer;
    void Awake()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        InitalizePrefsIfNone();
        UpdateKeybinds();
        ApplySettingsFromPrefs();
    }

    private void InitalizePrefsIfNone()
    {
        if (!PlayerPrefs.HasKey("0Resolution"))
            PlayerPrefs.SetInt("0Resolution", 1);
        if (!PlayerPrefs.HasKey("1FullScreen/Windowed"))
            PlayerPrefs.SetInt("1FullScreen/Windowed", 0);
        if (!PlayerPrefs.HasKey("2RefreshRate"))
            PlayerPrefs.SetInt("2RefreshRate", 0);
        if (!PlayerPrefs.HasKey("3Anti-Aliasing"))
            PlayerPrefs.SetInt("3Anti-Aliasing", 2);
        if (!PlayerPrefs.HasKey("4Master Volume"))
            PlayerPrefs.SetInt("4Master Volume", 7);
        if (!PlayerPrefs.HasKey("5Music Volume"))
            PlayerPrefs.SetInt("5Music Volume", 11);
        if (!PlayerPrefs.HasKey("6Sound Effects Volume"))
            PlayerPrefs.SetInt("6Sound Effects Volume", 11);
        if (!PlayerPrefs.HasKey("7Allow full turns"))
            PlayerPrefs.SetInt("7Allow full turns", 1);
        if (!PlayerPrefs.HasKey("ReachedNight"))
            PlayerPrefs.SetInt("ReachedNight", 1);
        else if (PlayerPrefs.GetInt("ReachedNight") > FNACStatic.feraljakMusic)
            PlayerPrefs.SetInt("ReachedNight", FNACStatic.feraljakMusic);
        if (!PlayerPrefs.HasKey("TutorialDone"))
            PlayerPrefs.SetInt("TutorialDone", 0);
        if (!PlayerPrefs.HasKey("GameWon"))
            PlayerPrefs.SetInt("GameWon", 0);
        if (!PlayerPrefs.HasKey("ItsAnEnding"))
            PlayerPrefs.SetString("ItsAnEnding", "");
        if (!PlayerPrefs.HasKey("DOCTOS"))
            PlayerPrefs.SetString("DOCTOS", "");
        if (!PlayerPrefs.HasKey("DOCTOS"))
            PlayerPrefs.SetString("DOCTOS", "");
        if (!PlayerPrefs.HasKey("UpKey"))
            PlayerPrefs.SetInt("UpKey", (int)KeyCode.W);
        if (!PlayerPrefs.HasKey("DownKey"))
            PlayerPrefs.SetInt("DownKey", (int)KeyCode.S);
        if (!PlayerPrefs.HasKey("LeftKey"))
            PlayerPrefs.SetInt("LeftKey", (int)KeyCode.A);
        if (!PlayerPrefs.HasKey("RightKey"))
            PlayerPrefs.SetInt("RightKey", (int)KeyCode.D);
        if (!PlayerPrefs.HasKey("CuckVersion"))
            PlayerPrefs.SetInt("CuckVersion", 0);
        if (!PlayerPrefs.HasKey("Feral"))
            PlayerPrefs.SetString("Feral", "");
        if (!PlayerPrefs.HasKey("WonPreset0"))
            PlayerPrefs.SetInt("WonPreset0", 0);
        if (!PlayerPrefs.HasKey("WonPreset1"))
            PlayerPrefs.SetInt("WonPreset1", 0);
        if (!PlayerPrefs.HasKey("WonPreset2"))
            PlayerPrefs.SetInt("WonPreset2", 0);
        if (!PlayerPrefs.HasKey("WonPreset3"))
            PlayerPrefs.SetInt("WonPreset3", 0);
        if (!PlayerPrefs.HasKey("WonPreset4"))
            PlayerPrefs.SetInt("WonPreset4", 0);
        if (!PlayerPrefs.HasKey("WonPreset5"))
            PlayerPrefs.SetInt("WonPreset5", 0);
        if (!PlayerPrefs.HasKey("WonPreset6"))
            PlayerPrefs.SetInt("WonPreset6", 0);

        // chievos progress tracking
        if (!PlayerPrefs.HasKey("ChudExp"))
            PlayerPrefs.SetInt("ChudExp", 0);
        if (!PlayerPrefs.HasKey("TroonExp"))
            PlayerPrefs.SetInt("TroonExp", 0);
        if (!PlayerPrefs.HasKey("PlierNeg"))
            PlayerPrefs.SetInt("PlierNeg", 0);

        // chievos
        if (!PlayerPrefs.HasKey("beatn1"))
            PlayerPrefs.SetString("beatn1", "");
        if (!PlayerPrefs.HasKey("beatn2"))
            PlayerPrefs.SetString("beatn2", "");
        if (!PlayerPrefs.HasKey("beatn3"))
            PlayerPrefs.SetString("beatn3", "");
        if (!PlayerPrefs.HasKey("beatn4"))
            PlayerPrefs.SetString("beatn4", "");
        if (!PlayerPrefs.HasKey("beatn5"))
            PlayerPrefs.SetString("beatn5", "");
        if (!PlayerPrefs.HasKey("chievo5"))
            PlayerPrefs.SetString("chievo5", "");
        if (!PlayerPrefs.HasKey("chievo6"))
            PlayerPrefs.SetString("chievo6", "");
        if (!PlayerPrefs.HasKey("chievo7"))
            PlayerPrefs.SetString("chievo7", "");
        if (!PlayerPrefs.HasKey("chievo8"))
            PlayerPrefs.SetString("chievo8", "");
        if (!PlayerPrefs.HasKey("chievo9"))
            PlayerPrefs.SetString("chievo9", "");
        if (!PlayerPrefs.HasKey("chievo10"))
            PlayerPrefs.SetString("chievo10", "");
        if (!PlayerPrefs.HasKey("chievo11"))
            PlayerPrefs.SetString("chievo11", "");
        if (!PlayerPrefs.HasKey("chievo12"))
            PlayerPrefs.SetString("chievo12", "");
        if (!PlayerPrefs.HasKey("chievo13"))
            PlayerPrefs.SetString("chievo13", "");
        if (!PlayerPrefs.HasKey("chievo14"))
            PlayerPrefs.SetString("chievo14", "");
        if (!PlayerPrefs.HasKey("chievo15"))
            PlayerPrefs.SetString("chievo15", "");
        if (!PlayerPrefs.HasKey("chievo16"))
            PlayerPrefs.SetString("chievo16", "");
        if (!PlayerPrefs.HasKey("chievo17"))
            PlayerPrefs.SetString("chievo17", "");
        if (!PlayerPrefs.HasKey("chievo18"))
            PlayerPrefs.SetString("chievo18", "");
        if (!PlayerPrefs.HasKey("chievo19"))
            PlayerPrefs.SetString("chievo19", "");
        if (!PlayerPrefs.HasKey("secretchievo1"))
            PlayerPrefs.SetString("secretchievo1", "");
        if (!PlayerPrefs.HasKey("secretchievo2"))
            PlayerPrefs.SetString("secretchievo2", "");
        if (!PlayerPrefs.HasKey("secretchievo3"))
            PlayerPrefs.SetString("secretchievo3", "");
        if (!PlayerPrefs.HasKey("secretchievo4"))
            PlayerPrefs.SetString("secretchievo4", "");
        if (!PlayerPrefs.HasKey("secretchievo5"))
            PlayerPrefs.SetString("secretchievo5", "");
        if (!PlayerPrefs.HasKey("secretchievo6"))
            PlayerPrefs.SetString("secretchievo6", "");
    }

    private void UpdateKeybinds()
    {
        upKey = (KeyCode)PlayerPrefs.GetInt("UpKey");
        downKey = (KeyCode)PlayerPrefs.GetInt("DownKey");
        leftKey = (KeyCode)PlayerPrefs.GetInt("LeftKey");
        rightKey = (KeyCode)PlayerPrefs.GetInt("RightKey");
    }

    private void ApplySettingsFromPrefs()
    {
        UpdateVideoSettings();
        UpdateAudioSettings();
        UpdateGameplaySettings();
    }

    private void UpdateVideoSettings()
    {
        Vector2 res = FNACStatic.resolutionFromPrefs[PlayerPrefs.GetInt("0Resolution")];
        bool fullScreen = PlayerPrefs.GetInt("1FullScreen/Windowed") == 0;
        int refreshRate = FNACStatic.refreshRateFromPrefs[PlayerPrefs.GetInt("2RefreshRate")];
        Screen.SetResolution((int)res.x, (int)res.y, fullScreen, refreshRate);
        int antialiasing = PlayerPrefs.GetInt("3Anti-Aliasing");
        Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = antialiasing != 0? AntialiasingMode.FastApproximateAntialiasing : AntialiasingMode.None;
        if (antialiasing != 0)
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = FNACStatic.antiAliasingFromPrefs[antialiasing];
    }

    public void UpdateAudioSettings()
    {
        float masterVolume = PlayerPrefs.GetInt("4Master Volume");
        float musicVolume = PlayerPrefs.GetInt("5Music Volume");
        float SFX = PlayerPrefs.GetInt("6Sound Effects Volume");
        mainMixer.SetFloat("MasterVolume", VolumeToAttenuation(masterVolume));
        mainMixer.SetFloat("MusicVolume", VolumeToAttenuation(musicVolume));
        mainMixer.SetFloat("SFXVolume", VolumeToAttenuation(SFX));
    }

    private float VolumeToAttenuation(float volume)
    {
        if (volume == 0)
            return -80f;
        volume = Mathf.Lerp(0.001f, 1f, Mathf.InverseLerp(0, 11, volume));
        return Mathf.Log(volume) * 20f;
    }

    private void UpdateGameplaySettings()
    {
        PlayerCam cam = PlayerCam.Instance;
        if (cam != null)
        {
            cam.allowFullTurns = PlayerPrefs.GetInt("7Allow full turns") == 0;
        }
    }
}
