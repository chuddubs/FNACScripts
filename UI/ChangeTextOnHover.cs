using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ChangeTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private string baseString;
    private string moddedString;
    public string prefix = "";
    public string suffix = "";
    public string replaceBy = "";

    public void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseString = text.text;
        moddedString = replaceBy.Length == 0 ? prefix + baseString + suffix : replaceBy;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = moddedString;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = baseString;
    }
}
