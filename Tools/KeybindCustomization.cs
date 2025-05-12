using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KeybindCustomization : MonoBehaviour
{
    public OptionsAndSubOptionsComunicator optionsAndSubOptionsComms;
    public Button up;
    public Button down;
    public Button left;
    public Button right;
    public enum KeyBindToEdit
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    private KeyBindToEdit toEdit = KeyBindToEdit.None;

    public void ListenForUp()
    {
        toEdit = KeyBindToEdit.Up;
    }

    public void ListenForDown()
    {
        toEdit = KeyBindToEdit.Down;
    }

    public void ListenForLeft()
    {
        toEdit = KeyBindToEdit.Left;
    }

    public void ListenForRight()
    {
        toEdit = KeyBindToEdit.Right;
    }

    public void OnEnable()
    {
        UpdateDisplayedKeys();
    }

    private void UpdateDisplayedKeys()
    {
        up.GetComponentInChildren<Text>().text = TranslateKeybind("UpKey");
        down.GetComponentInChildren<Text>().text = TranslateKeybind("DownKey");
        left.GetComponentInChildren<Text>().text = TranslateKeybind("LeftKey");
        right.GetComponentInChildren<Text>().text = TranslateKeybind("RightKey");
    }

    private string TranslateKeybind(string pref)
    {
        return ((KeyCode)PlayerPrefs.GetInt(pref)).ToString();
    }

    private void Update()
    {
        if (toEdit == KeyBindToEdit.None)
            return;
        if (Input.anyKeyDown)
        {
            for (int i = 0; i < 323; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    SetNewBind(i);
                    break;
                }
            }
        }
    }

    private void SetNewBind(int k)
    {
        if ((KeyCode)k != KeyCode.Escape)
        {
            switch (toEdit)
            {
                case KeyBindToEdit.Up:
                    SoySettings.Instance.upKey = (KeyCode)k;
                    PlayerPrefs.SetInt("UpKey", k);
                    break;
                case KeyBindToEdit.Down:
                    SoySettings.Instance.downKey = (KeyCode)k;
                    PlayerPrefs.SetInt("DownKey", k);
                    break;
                case KeyBindToEdit.Left:
                    SoySettings.Instance.leftKey = (KeyCode)k;
                    PlayerPrefs.SetInt("LeftKey", k);
                    break;
                case KeyBindToEdit.Right:
                    SoySettings.Instance.rightKey = (KeyCode)k;
                    PlayerPrefs.SetInt("RightKey", k);
                    break;
            }
        }
        toEdit = KeyBindToEdit.None;
        UpdateDisplayedKeys();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    public void ClickBackButton()
    {
        toEdit = KeyBindToEdit.None;
        optionsAndSubOptionsComms.BackToOptions(2);
        gameObject.SetActive(false);
    }

}
