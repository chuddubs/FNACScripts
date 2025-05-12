using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// using Steamworks;

public class AchievementsManager : Singletroon<AchievementsManager>
{
    public Award[] allAwards;
    public AwardPopupController   awardPopup;
    private bool usedFlashLight = false;
    private bool refilledNuts = false;
    private bool eligibleForStench = false;
    private bool survivedParty = false;
    public bool eligible720 = false;
    int partyMembers = 0;
    private NightManager nm;
    private SoyGameController sgc;
    private ViewManager vm;
    private OfficeController oc;

    private void OnEnable()
    {
        sgc = SoyGameController.Instance;
        nm = NightManager.Instance;
        vm = ViewManager.Instance;
        oc = OfficeController.Instance;
        nm.onNightStart += OnNightStart;
        SquirrelManager.Instance.onNutsRefilled += OnNutsRefilled;
        OfficeController.Instance.onFlashLightToggled += OnFlashLightToggled;
        // bool steamInit = SteamManager.Initialized;
        foreach(Award award in allAwards)
        {
            award.Init();
            award.LockUnlock(/*steamInit && */IsChievoUnlocked(award.chievoName));
        }
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded)
            return;
        nm.onNightStart -= OnNightStart;
        SquirrelManager.Instance.onNutsRefilled -= OnNutsRefilled;
        OfficeController.Instance.onFlashLightToggled -= OnFlashLightToggled;
    }

    private void OnNutsRefilled()
    {
        refilledNuts = true;
    }

    private void OnNightStart(int n)
    {
        usedFlashLight = false;
        eligibleForStench = false;
        survivedParty = PlayerPrefs.GetString("chievo18") == "THERESAPARTY";
    }

    public void Survived(int night)
    {
        if (night == 0)
        {
            return;
        }
        if (OfficeController.Instance.cellBarsHP == 100)
            FedSquirrelAllNight(night);
        if (night == 5 && OfficeController.Instance.power.powerLeft >= 50)
            UnlockChievo("chievo14");
        if (night > 2 && refilledNuts == false)
            UnlockChievo("chievo15");
        if (usedFlashLight == false)
            UnlockChievo("chievo9");
        if (eligibleForStench == true)
            UnlockChievo("chievo17");
        if (survivedParty == true)
            UnlockChievo("chievo18");

    }

    private void FedSquirrelAllNight(int n)
    {
        string s = "FERAL";
        char c = s[n - 1];
        string prog = PlayerPrefs.GetString("Feral");
        if (!prog.Contains(c))
            prog += c;
        PlayerPrefs.SetString("Feral", prog);
        if (prog.Length >= 5)
            UnlockChievo("chievo16");
    }

    private void OnFlashLightToggled(bool on)
    {
        if (on)
            usedFlashLight = true;
    }

    public void OnTroonAck()
    {
        if (nm.currHour < 2)
            eligibleForStench = true;
    }

    private void Update()
    {
        if (nm.inTransition || !sgc.InGame())
            return;
        if (!survivedParty)
        {
            partyMembers = vm.GetCamById(0).JaksInRoom.Count;
            if (partyMembers >= 3 || (partyMembers == 2 && oc.ACK.activeSelf))
                survivedParty = true;
        }
    }

    public void UnlockChievo(string chievoName)
    {
        string pass = FNACStatic.chievoToPass[chievoName];
        // if (SteamManager.Initialized)
        //     UnlockChievoSteam(pass);
        if (PlayerPrefs.GetString(chievoName) != pass)
            PlayerPrefs.SetString(chievoName, pass);
        else
            return;
        Award award = allAwards.FirstOrDefault(aw => aw.chievoName == chievoName);
        awardPopup.ShowPopup(award);
        award.LockUnlock(true);
    }

    // public void UnlockChievoSteam(string chievoName)
    // {
    //     SteamUserStats.GetAchievement(chievoName, out bool hasChievo);
    //     if (!hasChievo)
    //     {
    //         SteamUserStats.SetAchievement(chievoName);
    //         SteamUserStats.StoreStats();
    //         Award award = allAwards.FirstOrDefault(aw => aw.chievoName == chievoName);
    //         award.LockUnlock(true);
    //     }
    // }


    public void UpdateChievoStat(string chievoName, string statName)
    {
        // if (SteamManager.Initialized)
        //     UpdateChievoStatSteam(FNACStatic.chievoToPass[chievoName]);
        // else
        // {
        
            int count = PlayerPrefs.GetInt(statName);
            count++;
            PlayerPrefs.SetInt(statName, count);
            if (count >= FNACStatic.chievoMaxStat[statName])
                UnlockChievo(chievoName);   
        // }
    }

    
    // public void UpdateChievoStatSteam(string chievoName)
    // {
    //     SteamUserStats.GetAchievement(chievoName, out bool hasChievo);
    //     if (hasChievo)
    //         return;
    //     SteamUserStats.GetStat(chievoName, out int count);
    //     count++;
    //     SteamUserStats.SetStat(chievoName, count);
    //     SteamUserStats.StoreStats();
    //     if (count >= FNACStatic.chievoMaxStat[chievoName])
    //         UnlockChievo(chievoName);
    // }

    public bool IsChievoUnlocked(string chievoName)
    {
        return PlayerPrefs.GetString(chievoName) == FNACStatic.chievoToPass[chievoName];
        // SteamUserStats.GetAchievement(chievoName, out bool hasChievo);
        // return hasChievo;
    }

    public bool AllChievosWon()
    {
        return allAwards.Where(aw => !aw.unlocked).Count() == 0;
    }
}
