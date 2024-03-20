using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public List<Item> listOfOrder = new List<Item>(); //List of lists of items
    public Item resultingItem;
    public int numofOrdersCompleted=0,currentOrderIndex = 0, currLevel = 0, numOfOrdersInLevel = 0, maxOrderSize = 4, totalNumofOrders=0;
    public CraftingManager craftingMan; //Need access to the recipe list
    public Image itemSprite1,itemSprite2,itemSprite3,itemSprite4;
   
    public int[] levels;

    public Text basicFeedback;

    // Start is called before the first frame update
    void Start()
    {
        numOfOrdersInLevel = levels[currLevel];
        CreateNewOrder();
        basicFeedback.gameObject.SetActive(false);
        FindTotalNumOfOrders();
    }

    private void FindTotalNumOfOrders()
    {
        for(int i=0;i<levels.Length;i++)
        {
            totalNumofOrders += levels[i];
        }
    }

    void GenerateNewLevel() //Need to pick from recipe list randomly for a certain amount of times per level (increment number of times selected as the amount of levels increase)
    { 
        if (currLevel < levels.Length)
        {
            //Add the order into the new level
            currLevel++;

            //set current num of items to the current level
            numOfOrdersInLevel = levels[currLevel];
        }

    }

    private void CreateNewOrder()
    {
        for (int i = 0; i < maxOrderSize; i++)
        {
            listOfOrder.Add(craftingMan.ingredientsList[UnityEngine.Random.Range(0,craftingMan.ingredientsList.Count)]);
        }
    }


    private bool CheckLists(List<Item> orderList, List<Item> finalCraftedOrder)
    {
        for (int i = 0; i < orderList.Count; i++) if (orderList[i] != finalCraftedOrder[i]) return false; //if the item in the order list does not equal the item in the final crafted order return false

        return true; //otherwise the lists are the same and return true;
    }

    public bool CheckItem()
    {


        if (CheckLists(listOfOrder,craftingMan.finalOrderList))//checks to see if the list of items in current order is equal to the final crafted order
        {
            //Add some basic feedback to let the player know that they have successfully completed the order
            basicFeedback.gameObject.SetActive(true);
            basicFeedback.GetComponent<Text>().text = "✔";
            basicFeedback.GetComponent<Text>().color = new Color(0, 255, 0);
            Debug.Log("Correct Order");


            //increment current item index
            currentOrderIndex++;

            //increment completed items
            numofOrdersCompleted++;

            //clear the order list as you will be making a new one
            listOfOrder.Clear();

            for(int i=0;  i< craftingMan.craftingSlots.Length;i++)
            {
                craftingMan.craftingSlots[i].GetComponent<Image>().sprite = null;
                craftingMan.craftingSlots[i].item = null;
            }

            CreateNewOrder();

            if (currentOrderIndex >= numOfOrdersInLevel)
            {
                Debug.Log("Level has been beaten");
                GenerateNewLevel();
                currentOrderIndex = 0;
            }
             
            return true;
        }
        else
        {
            Debug.Log("Incorrect Order");
            basicFeedback.gameObject.SetActive(true);
            basicFeedback.GetComponent<Text>().text = "X";
            basicFeedback.GetComponent<Text>().color = new Color(255, 0, 0);

        }
      
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (listOfOrder.Count > 0)
        {
            itemSprite1.sprite = listOfOrder[0].GetComponent<Image>().sprite;
            itemSprite2.sprite = listOfOrder[1].GetComponent<Image>().sprite;
            itemSprite3.sprite = listOfOrder[2].GetComponent<Image>().sprite;
            itemSprite4.sprite = listOfOrder[3].GetComponent<Image>().sprite;
        }

    }
}
