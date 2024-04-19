using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CraftingManager : MonoBehaviour
{
    private Item currentItem;
    public Image customCursor;

    public Slot[] craftingSlots; //slots created for crafting slots

    public List<Item> ingredientsList, finalOrderList;
    public Slot result;
    public Item resultingItem;
    public OrderManager orderManager;
    public Button craftButton;

    public GameObject[] setsOfIngredients;
    public Button nextButton, prevButton;
    private int setIndex = 0;

    
    

    public void OnMouseDownItem(Item item)
    {
        if (currentItem == null)
        {
            currentItem = item;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.GetComponent<Image>().sprite;

        }
    }

    public void SetIndexIncrease()
    {
        setIndex++;

        foreach(GameObject x in setsOfIngredients)
        {
            x.SetActive(false);
        }

        setsOfIngredients[setIndex].SetActive(true);

    }

    public void SetIndexDecrease()
    {
        setIndex--;

        foreach (GameObject x in setsOfIngredients)
        {
            x.SetActive(false);
        }

        setsOfIngredients[setIndex].SetActive(true);
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
                finalOrderList[nearestSlot.index] = currentItem;
                currentItem = null;


            }
        }


        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);
        if (setIndex == 0)
            {
                prevButton.gameObject.SetActive(false);
            }

            if (setIndex == (orderManager.numOfIngredientsAvailable/setsOfIngredients.Length)-1)
            {
            
                nextButton.gameObject.SetActive(false);
            }

    }

    public void CheckForCompletedOrder()
    {
        if (orderManager.CheckItem())
        {
            ClearList(); //if the order is crafted correctly clear the final order list
            Debug.Log("true");
         }
    }

    public void ClearList()
    {
        for(int i = 0; i<finalOrderList.Count;i++)
        {
            finalOrderList[i] = null;
        }
    }

    public void OnClickSlot(Slot tempSlot)
    {
        tempSlot.item = null;
        ingredientsList[tempSlot.index] = null;
        tempSlot.gameObject.SetActive(false);
        tempSlot.item = currentItem;
    }

   
 

}
