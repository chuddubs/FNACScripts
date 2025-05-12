using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class SoySceneController_Main : SoySceneController
{
    public string   gameSceneName;
    public string   titleScreenSceneName;

    void Awake()
    {
        SoySceneManager.Instance.loadingScreen = FindObjectOfType<VideoPlayer>();
        SoySceneManager.Instance.loadingScreen.Prepare();
    }

    void Start()
    {
        Initialize();
        Launch();
    }


    public override void Initialize()
    {
        sceneType = SoySceneManager.SceneType.PersistentScene;

        isInitialized = true;
    }

    public override void Launch()
    {
        if (gameSceneName != null)
            SoySceneManager.Instance.SetScene(gameSceneName);
        if (titleScreenSceneName != null)
            SoySceneManager.Instance.SetScene(titleScreenSceneName);
    }
}
