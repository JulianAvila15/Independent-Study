using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite closedSprite;
    public Sprite openSprite;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Open();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Close();
    }

    private void Open()
    {
        spriteRenderer.sprite = openSprite;
    }

    private void Close()
    {
        spriteRenderer.sprite = closedSprite;
    }
}
