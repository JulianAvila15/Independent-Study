using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CraftingManager : MonoBehaviour
{
    private Item currentItem;
    public Image customCursor;

    public Slot[] craftingSlots; //slots created for crafting slots

    public List<Item> itemList;
    public string[] recipes;
    public Item[] recipeList;
    public Slot result;

    public void OnMouseDownItem(Item item)
    {
        if (currentItem == null)
        {
            currentItem = item;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.GetComponent<Image>().sprite;

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) //if the player drops the item in the specific slot
        {
            if (currentItem != null)
            {
                customCursor.gameObject.SetActive(false);
                Slot nearestSlot = null;
                float shortestDistance = float.MaxValue;

                foreach (Slot x in craftingSlots)
                {
                    float dist = Vector2.Distance(Input.mousePosition, x.transform.position);

                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        nearestSlot = x;
                    }
                }
                nearestSlot.gameObject.SetActive(true);
                nearestSlot.GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                itemList[nearestSlot.index] = currentItem;
                currentItem = null;

                CheckForCompletedRecipies();
            }
        }
    }

    void CheckForCompletedRecipies()
    {
        result.gameObject.SetActive(false);
        result.item = null;

        string currentRecipeString = "";

        foreach (Item i in itemList)
        {
            if (i != null)
            {
                currentRecipeString += i.itemName;
            }
            else
            {
                currentRecipeString += "null";
            }
        }

        for(int i=0; i<recipes.Length;i++)
        {
            if (recipes[i] == currentRecipeString)
            {
                result.gameObject.SetActive(true);
                result.GetComponent<Image>().sprite = recipeList[i].GetComponent<Image>().sprite;
                result.item = recipeList[i];
            }
        }
    }

    public void OnClickSlot(Slot tempSlot)
    {
        tempSlot.item = null;
        itemList[tempSlot.index] = null;
        tempSlot.gameObject.SetActive(false);
        CheckForCompletedRecipies();
    }

 

}
