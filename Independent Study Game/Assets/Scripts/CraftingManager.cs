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
    public int setIndex = 0;

    public  GameObject grayImage;
    public static bool helperIsActive = false;
    public static string helperName;
    public static int penguinSlot;//Gets the slot the penguin has chosen to color green
    public GameObject mainGame;

    public void OnMouseDownItem(Item item)
    {
        if (currentItem == null)
        {
            currentItem = item;
            customCursor.gameObject.SetActive(true);
            customCursor.sprite = currentItem.GetComponent<Image>().sprite;

        }

        DataMiner.numOfIngredientClicks++;
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
       
        
        if(Helper.shownIngredient==false&& mainGame.activeInHierarchy==true)
        {
            if (finalOrderList[penguinSlot]!=null)
            {
                craftingSlots[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                orderManager.listOfOrder[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                Helper.shownIngredient = true;
                helperName = "other";
            } 
        }

    




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
                nearestSlot.GetComponent<Slot>().item = currentItem;
                finalOrderList[nearestSlot.index] = currentItem;
                currentItem = null;
                TimeManager.ResetAFKTimer();

            }
            

        }


        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);


        if (setIndex == 0)
            {
                prevButton.gameObject.SetActive(false);
            }


        if (setIndex == Mathf.FloorToInt(orderManager.numOfIngredientsAvailable / (setsOfIngredients.Length)))
        {
            nextButton.gameObject.SetActive(false);
        }


    }

    public void CheckForCompletedOrder()
    {
        if(!finalOrderList.Contains(null))
            TimeManager.ResetAFKTimer();

        orderManager.CheckItem();
        
            Debug.Log("true");

        if (Helper.shownIngredient == false && mainGame.activeInHierarchy == true)
        {
            
                craftingSlots[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                orderManager.listOfOrder[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                Helper.shownIngredient = true;
                helperName = "other";
            
        }

        ClearList(); //if the order is crafted correctly clear the final order list

        DataMiner.numOfCrafts++;
    }

    public void ClearList()
    {
        for(int i = 0; i<finalOrderList.Count;i++)
        {
            finalOrderList[i] = null;
        }
    }

    public void OnClickSlot(int slotIndex)
    {
           
            finalOrderList[slotIndex] = null;
            craftingSlots[slotIndex].item = null;
        //Debug.Log("slot Clicked");
        craftingSlots[slotIndex].GetComponent<Image>().sprite = null;
        craftingSlots[slotIndex].item = currentItem;
           
       
      
    }

   public int GetAvailableSlot()
    {
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            if (finalOrderList[i] == null)
            {
                return i;
            }
        }

        return -1; // Or throw an exception if no available slot is found
    }


    public void CreateSlot(int slotIndex)
        { 
        finalOrderList[slotIndex] = orderManager.listOfOrder[slotIndex];
        craftingSlots[slotIndex].GetComponent<Slot>().item = orderManager.listOfOrder[slotIndex];
        craftingSlots[slotIndex].GetComponent<Image>().sprite = orderManager.listOfOrder[slotIndex].GetComponent<Image>().sprite;
        }



}
