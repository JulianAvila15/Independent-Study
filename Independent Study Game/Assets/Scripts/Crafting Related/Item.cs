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
    public Image imageOfItem;
    Color defaultColor;

    private void Awake()
    {
        imageOfItem = gameObject.GetComponent<Image>();
        defaultColor = imageOfItem.color;
        
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
