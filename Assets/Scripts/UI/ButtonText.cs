using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text theText;

    private void Start()
    {
        theText = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        theText.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = Color.black;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = Color.white;
    }


    

}