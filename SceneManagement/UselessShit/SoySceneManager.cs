using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SoySceneManager : Singletroon<SoySceneManager>
{
    public VideoPlayer   loadingScreen;
    /// <summary>
    /// Name list of scenes waiting for loading 
    /// </summary>
    private List<string> waitingScenesNameList = new List<string>();
    
    /// <summary>
    /// Boolean true if a scene is being loaded by SceneManager or if a loaded scene is still treated by OnSceneLoaded method
    /// </summary>
    private bool isLoadingScene = false;
    
    /// <summary>
    /// List of scenes waiting for unload
    /// </summary>
    private List<ToUnloadScene> toUnloadSceneList = new List<ToUnloadScene>();
    
    /// <summary>
    /// Name list of loaded scenes
    /// </summary>
    public List<string> loadedSceneNamesList = new List<string>();

    /// <summary>
    /// Structure containing a scene name and a scene Type 
    /// </summary>
    public struct ToUnloadScene{
        public string sceneName;
        public SceneType sceneType;
    }


    /// <summary>
    /// Type of scene used for managment of scene unloading 
    /// </summary>
    public enum SceneType{
        Undefined,
        /// <summary>
        /// The scene stay loaded until application end
        /// </summary>
        PersistentScene,
        /// <summary>
        /// The scene contains graph and physics components and locators for ProductZone scene binding 
        /// </summary>
        GameScene
    }

    void Start()
    {
        // DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Method called when SceneManager has ended scene loading
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SoySceneManager OnSceneLoaded(" + scene.name + ")");
        if(scene.name != null && scene.name !="")
        {
            // Set the scene as active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.name));

            // Get scene controller of the new scene
            SoySceneController[] sceneControllerSet = FindObjectsOfType<SoySceneController>();
            SoySceneController sceneController = sceneControllerSet.First((_sceneController) => _sceneController.gameObject.scene == scene);
            sceneController.Initialize();
            SceneType sceneType = sceneController.GetSceneType();
            if(sceneType != SceneType.PersistentScene)
            {
                toUnloadSceneList.Add(new ToUnloadScene{
                    sceneName = scene.name,
                    sceneType = sceneType
                });
            }
            if(waitingScenesNameList.Count != 0)
            {
                string waitingSceneName = waitingScenesNameList[0];
                waitingScenesNameList.RemoveAt(0);
                SetScene(waitingSceneName);
            }
            else
                isLoadingScene = false;        
            sceneController.Launch();
        }
        else
        {
            Debug.LogWarning("SoySceneManager OnSceneLoaded(" + scene.name + ") : Impossible to treat loaded scene");
            isLoadingScene = false;
        }
    }

    /// <summary>
    /// Unload scenes which are waiting for unloading
    /// </summary>
    private IEnumerator UnloadScenesCoroutine(string newSceneName, Action callback = null)
    {
        for(int i = 0; i < toUnloadSceneList.Count; i++)
        {
            if(newSceneName != toUnloadSceneList[i].sceneName)
            {
                yield return UnloadSceneCoroutine(toUnloadSceneList[i]);
                i--;
            }
        }
        if(callback != null){
            callback();
        }
    }


    /// <summary>
    /// Call the coroutine to launch the loading of the scene
    /// </summary>
    /// <param name="nameScene">
    /// The name of the Scene to load
    /// </param>
    public void SetScene(string nameScene)
    {
        Debug.Log("SoySceneManager SetScene(" + nameScene + ") isLoadingScene=" + isLoadingScene );
        if(!isLoadingScene)
        {
            isLoadingScene = true;
            StartCoroutine(LoadSceneCoroutine(nameScene));
        }
        else if(!loadedSceneNamesList.Contains(nameScene))
            waitingScenesNameList.Add(nameScene);
    }


    /// <summary>
    /// Coroutine to load scene asynchronously
    /// </summary>
    /// <param name="nameScene">
    /// The name of the Scene to load
    /// </param>
    IEnumerator LoadSceneCoroutine(string nameScene)
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.Play();
        if(!loadedSceneNamesList.Contains(nameScene))
        {
            loadedSceneNamesList.Add(nameScene);
            yield return SceneManager.LoadSceneAsync(nameScene,LoadSceneMode.Additive);
            isLoadingScene = false;
        }
        else
        {
            Debug.LogError("SoySceneManager SetSceneCoroutine(" + nameScene + ") : Impossible to load the scene , the scene is already loaded");
            isLoadingScene = false;
        }
        loadingScreen.Stop();
        loadingScreen.gameObject.SetActive(false);
        StartCoroutine(UnloadScenesCoroutine(nameScene));
    }


    /// <summary>
    /// Coroutine to unload scene asynchronously
    /// </summary>
    /// <param name="toUnloadScene">
    /// The name of the Scene to load
    /// </param>
    private IEnumerator UnloadSceneCoroutine(ToUnloadScene toUnloadScene)
    {
        // if the scene is loaded
        if(loadedSceneNamesList.Contains(toUnloadScene.sceneName))
        {
            yield return SceneManager.UnloadSceneAsync(toUnloadScene.sceneName);
            loadedSceneNamesList.Remove(toUnloadScene.sceneName);
            toUnloadSceneList.Remove(toUnloadScene);
        }
        else{
            Debug.LogError("SoySceneManager UnloadScene(" + toUnloadScene.sceneName + ") : Impossible to unload this scene, the scene is not loaded.");
            yield break;
        }
    }
}
