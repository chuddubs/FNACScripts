using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class FNACStatic
{
    public static int feraljakMusic = 5;
    public static Dictionary<int, string> CamIdToDisplayName { get; } = new Dictionary<int, string>
    {
        { -88, "Hidden"},
        { -11, "Rapeson Cell"},
        { -10, "Feral Cell"},
        { -5, "Office Vents"},
        { -3, "Hallway Back"},
        { -2, "Hallway Middle"},
        { -1, "Hallway Door"},
        { 0, "Office"},
        { 1, "1A: West cells" },
        { 2, "2 : Cobson's cell" },
        { 3, "3A: Power generator"},
        { 4, "4 : Dr.  Soystein's office\n[AUDIO FEED ONLINE]"},
        { 5, "5 : Cafeteria"},
        { 6, "6 : Hallway"},
        { 7, "7 : Janitor closet"},
        { 8, "8A: Ventilation (NW)"},
        { 9, "9 : Ventilation (E)"},
        {10, "10: Ventilation (W)"},
        {11, "11: Ventilation (SW)"},
        {12, "1B: West wing hallway"},
        {13, "3B: Entrance Lobby"},
        {14, "Ϫ: Infirmary"},
        {15, "8B: Ventilation (NE)"}
    };

    public static Dictionary<int, string> CamIdToCacaDisplayName { get; } = new Dictionary<int, string>
    {
        { -88, "Hidden"},
        { -11, "Rapeson Cell"},
        { -10, "Feral Cell"},
        { -5, "Office Vents"},
        { -3, "Hallway Back"},
        { -2, "Hallway Middle"},
        { -1, "Hallway Door"},
        { 0, "Office"},
        { 1, "1A: Bwoders bedwooms" },
        { 2, "2 : Badhwoom" },
        { 3, "3A: Prowre genrador"},
        { 4, "4 : Dada's bedwoom\n[AUIDO FEDE OLNIEN]"},
        { 5, "5 : Kitchwne"},
        { 6, "6 : Hlalwya"},
        { 7, "7 : Colset"},
        { 8, "8A: Vetnilashon (NW)"},
        { 9, "9 : Vetnilashon (E)"},
        {10, "10: Vetnilashon (W)"},
        {11, "11: Vetnilashon (SW)"},
        {12, "1B: Left hnad halway"},
        {13, "3B: Livwnig roob"},
        {14, "Ϫ: Baba's bedwoom"},
        {15, "8B: Vetnilashon (NE)"}
    };

    public static Dictionary<string, string> chievoToPass {get; } = new Dictionary<string, string>
    {
        {"beatn1", "BEATN1"},
        {"beatn2", "BEATN2"},
        {"beatn3", "BEATN3"},
        {"beatn4", "BEATN4"},
        {"beatn5", "BEATN5"},
        {"chievo5", "ALLMGEMS"},
        {"chievo6", "REPEL_CHUD"},
        {"chievo7", "REPEL_TROON"},
        {"chievo8", "NEGLECT_PLIER"},
        {"chievo9", "NO_FLASHLIGHT"},
        {"chievo10", "CHUD_ENDING"},
        {"chievo11", "CALM"},
        {"chievo12", "MUH_SHEKELS"},
        {"chievo13", "CARTERPEDO"},
        {"chievo14", "POWERSAVER"},
        {"chievo15", "NO_NUT_NIGHT"},
        {"chievo16", "CADOSQUIRREL"},
        {"chievo17", "XE_ACKED"},
        {"chievo18", "THERESAPARTY"},
        {"chievo19", "ALLJUMPS"},
        {"secretchievo1", "THOUGH"},
        {"secretchievo2", "YWNBAW"},
        {"secretchievo3", "GOLDEN"},
        {"secretchievo4", "OMINOUS"},
        {"secretchievo5", "GEMBORALD"},
        {"secretchievo6", "SKITZO"}
    };

    #region VIDEO SETTINGS
    public static Dictionary<int, Vector2> resolutionFromPrefs { get; } = new Dictionary<int, Vector2>
    {
        { 0, new Vector2(2560, 1440)},
        { 1, new Vector2(1920, 1080)},
        { 2, new Vector2(1600, 900)},
        { 3, new Vector2(1366, 768)},
        { 4, new Vector2(1280, 720)},
        { 5, new Vector2(960, 540)}
    };

    public static Dictionary<int, int> refreshRateFromPrefs { get; } = new Dictionary<int, int>
    {
        { 0, 60},
        { 1, 75},
        { 2, 120},
        { 3, 144}
    };

    public static Dictionary<int, AntialiasingQuality> antiAliasingFromPrefs { get; } = new Dictionary<int, AntialiasingQuality>
    {
        { 1, AntialiasingQuality.Low},
        { 2, AntialiasingQuality.Medium},
        { 3, AntialiasingQuality.High}
    };

    #endregion
    public static Dictionary<string, string> devslogs { get; } = new Dictionary<string, string>
    {
        {"devpw", "devname"},
    };

    public static Dictionary<string, int> chievoMaxStat { get; } = new Dictionary<string, int>
    {
        {"ChudExp", 110},
        {"TroonExp", 59},
        {"PlierNeg", 15}
    };

    public static Variant[] jaksBaseVersion { get; } = new Variant[]
    {
        Variant.Rapeson,
        Variant.Chud,
        Variant.Imp,
        Variant.Plier,
        Variant.Troon,
        Variant.Fingerboy
    };

    public static string[] jaksTexNameBaseVersion { get; } = new string[]
    {
        "Cob",
        "Chud",
        "Imp",
        "Plier",
        "Troon",
        "Fboy",
        "Feral"
    };

    public static string[] presetNames { get; } = new string[]
    {
        "Golden Cobby",
        "Kway",
        "Eat Ze Bugs",
        "In Your Walls",
        "The 'cuck",
        "Groomcord",
        "Take Your Meds"
    };

    public static Dictionary<string, int[]> presetValues { get; } = new Dictionary<string, int[]>
    {
        {"Golden Cobby", new int[]{10, 10, 10, 10, 10, 10, 10}},
        {"Kway", new int[]{0, 0, 10, 10, 0, 0, 10}},
        {"Eat Ze Bugs", new int[]{0, 5, 0, 0, 5, 10, 10}},
        {"In Your Walls", new int[]{5, 0, 10, 5, 0, 10, 5}},
        {"The 'cuck", new int[]{0, 10, 10, 0, 10, 0, 0}},
        {"Groomcord", new int[]{10, 5, 0, 0, 5, 0, 0}},
        {"Take Your Meds", new int[]{5, 0, 0, 5, 0, 10, 5}}
        
    };

    public static void SetPos(Transform c, Transform p)
    {
        c.SetParent(p);
        c.localPosition = Vector3.zero;
        c.localRotation = Quaternion.identity;
        c.localScale = Vector3.one;
    }

    public static void SetLayerRecursively(this Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
 
        for (int i = 0, count = parent.childCount; i < count; i++)
        {
            parent.GetChild(i).SetLayerRecursively(layer);
        }
    }

    public static Sprite CreateSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    } 
}
public enum GameVersion
{
    Base,
    Caca
};