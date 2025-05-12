using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SoySceneController : MonoBehaviour
{
    public bool isInitialized = false;
    public Action onInitialized;
    protected SoySceneManager.SceneType sceneType;

    public SoySceneManager.SceneType GetSceneType()
    {
        return sceneType;
    }

    public abstract void Initialize();
    public abstract void Launch();
}
