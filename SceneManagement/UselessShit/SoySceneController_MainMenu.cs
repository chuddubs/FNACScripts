using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoySceneController_MainMenu : SoySceneController
{
    public override void Initialize()
    {
        sceneType = SoySceneManager.SceneType.GameScene;

        isInitialized = true;
    }

    public override void Launch()
    {
    }

    public void Reinitialize(bool isTransitionScene)
    {
    }
}
