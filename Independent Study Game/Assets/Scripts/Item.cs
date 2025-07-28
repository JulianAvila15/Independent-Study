using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
public class Item : MonoBehaviour, IPointerDownHandler 
{
    public string itemName; //What the item is
    Color defaultColor;
    private void Start()
    {
        defaultColor = gameObject.GetComponent<Image>().color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<CraftingManager>().OnMouseDownItem(this);
    }


    private void OnMouseEnter()
    {
        gameObject.GetComponent<Image>().color = Color.gray;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Image>().color = defaultColor;
    }
}
