using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private SoyGameController gc;
    private MinigamesManager mgm;
    
    public MenuCreator settingsController;

    private void Start()
    {
        gc = SoyGameController.Instance;
        mgm = MinigamesManager.Instance;
    }

    public void BtClickResume()
    {
        pauseMenu.SetActive(false);
        gc.inPause = false;
    }

    public void BtClickBacktoTitle()
    {
        pauseMenu.SetActive(false);
        gc.inPause = false;
        if (mgm.inMinigame)
            mgm.EndMinigame(false);
        else
            NightManager.Instance.ExitNight();
    }

    public void BtClickSettings()
    {
        settingsController.ShowMenu();
    }

    public void BtClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        if (!gc.inMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf && NightManager.Instance.inTransition)
                    return;
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            gc.inPause = pauseMenu.activeSelf;
        }
    }
}
