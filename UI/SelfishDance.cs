using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelfishDance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject slf;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slf.activeSelf == false)
            slf.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slf.activeSelf == true)
            slf.SetActive(false);
    }
}
