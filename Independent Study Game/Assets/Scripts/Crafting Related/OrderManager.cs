using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{


    public List<Item> listOfOrder = new List<Item>(4); //List of lists of items
    [SerializeField] private List<Image> listOfOrderImages = new List<Image>(4);
    public Item resultingItem;
    public int numofOrdersCompleted = 0, currentOrderIndex = 0, currLevel = 0, numOfOrdersInLevel = 0, maxOrderSize = 4, totalNumofOrders = 0, numOfIngredientsAvailable=5, levelsNecessary = 6;

[SerializeField]    ManagerofManagers managerHub;

    public Image[] itemDisplayedSprites;

    public int[] ordersToCompletePerLevel;

    public Slider progressBar;

    public GameObject newLevelProgressed,ingredientsUnlockedAlert;

    public Helper helper;


    public GameObject penguinButton, messengerButton, dragonButton, timingButton, collectingButton;

    public bool penguinUnlocked=false,messengerUnlocked=false,dragonUnlocked=false;

    public Image[] feedBackImages;

    public GameObject mainGame;

    private void Awake()
    {
        if (managerHub != null && managerHub.orderManager == null)
            managerHub.orderManager = gameObject.GetComponent<OrderManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        numOfOrdersInLevel = ordersToCompletePerLevel[currLevel];
        CreateNewOrder();
        FindTotalNumOfOrders();

        if (managerHub.gameManager.progressType == GameManager.ProgressFeedbackType.progressBar)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = ordersToCompletePerLevel[currLevel];
        }

    }

    IEnumerator BasicFeedback()
    {
       
        if (CheckLists(listOfOrder, managerHub.craftingManager.finalOrderList))//checks to see if the list of items in current order is equal to the final crafted order
        {
            feedBackImages[0].gameObject.SetActive(true);
        }
        else
        {

            feedBackImages[1].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        if(feedBackImages[0].gameObject.activeInHierarchy)
            feedBackImages[0].gameObject.SetActive(false);
        else

            feedBackImages[1].gameObject.SetActive(false);
    }

    IEnumerator NewLevelFeedback()
    {
        newLevelProgressed.SetActive(true);

        if ((currLevel%2)+1==1)
            ingredientsUnlockedAlert.SetActive(true);
        else
            ingredientsUnlockedAlert.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        
            managerHub.timeManager.ResetAFKTimer();
        newLevelProgressed.SetActive(false);

        if (managerHub.gameManager.isPausedByFocusLoss)
            managerHub.gameManager.Pause();
        GenerateNewLevel();
    }

    private void FindTotalNumOfOrders()
    {
        for (int i = 0; i < ordersToCompletePerLevel.Length; i++)
        {
            totalNumofOrders += ordersToCompletePerLevel[i];
        }
    }

    void GenerateNewLevel() //Need to pick from recipe list randomly for a certain amount of times per level (increment number of times selected as the amount of levels increase)
    {

        if (currLevel < ordersToCompletePerLevel.Length)
        {
            managerHub.timeManager.SetTimeCompletedLevel(currLevel);
            
            //Add the order into the new level
            currLevel++;

           if((currLevel % 2)== 1)
            {
                numOfIngredientsAvailable += 5;
            }


            //set current num of items to the current level
            numOfOrdersInLevel = ordersToCompletePerLevel[currLevel];

            switch (currLevel)
            {
                case 2:
                    penguinButton.SetActive(true);
                    penguinUnlocked = true;
                    break;
                case 4:
                    collectingButton.SetActive(true);
                    break;
                case 6:
                    messengerButton.SetActive(true);
                    break;
                case 8:
                    dragonButton.SetActive(true);
                    break;
                case 10:
                    timingButton.SetActive(true);
                    break;
                default:
                    break;

            }

        }
    }

    private void CreateNewOrder()
    {
        Item addedItem = null;

        for (int orderIndex = 0; orderIndex < maxOrderSize; orderIndex++)
        {
            addedItem = managerHub.craftingManager.ingredientsList[UnityEngine.Random.Range(0, numOfIngredientsAvailable - 1)];

            listOfOrder[orderIndex] = addedItem;

            itemDisplayedSprites[orderIndex].sprite =listOfOrder[orderIndex].imageOfItem.sprite;
        }
    }


    private bool CheckLists(List<Item> orderList, List<Item> finalCraftedOrder)
    {
        return orderList.SequenceEqual(finalCraftedOrder, new ItemComparer());
    }

    public void CheckItem()
    {           

        if (CheckLists(listOfOrder, managerHub.craftingManager.finalOrderList))//checks to see if the list of items in current order is equal to the final crafted order
        {
            //Add some basic feedback to let the player know that they have successfully completed the order
            StartCoroutine(BasicFeedback());

            DataMiner.numOfSuccessfulCrafts++;


            //increment current item index
            currentOrderIndex++;

            //increment completed items
            numofOrdersCompleted++;

            //clear the order list as you will be making a new one
            ClearOrder();

            CreateNewOrder();

            if (currentOrderIndex >= numOfOrdersInLevel)
            {
                StartCoroutine(NewLevelFeedback());
                currentOrderIndex = 0;
            }

         
        }
        else
        {
            ClearOrder();
            StartCoroutine(BasicFeedback());

                DataMiner.numOfFailedCrafts++;
        }

       
    }

    private void ClearOrder()
    {

        for (int slotIndex = 0; slotIndex < managerHub.craftingManager.craftingSlots.Length; slotIndex++)
        {
            managerHub.craftingManager.imageCraftingSlots[slotIndex].sprite= null;
            managerHub.craftingManager.craftingSlots[slotIndex].item = null;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (newLevelProgressed.activeInHierarchy)
            managerHub.gameManager.pauseButton.gameObject.SetActive(false);
        else
            managerHub.gameManager.pauseButton.gameObject.SetActive(true);
    }

    public class ItemComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item item1, Item item2)
        {
            return item1.itemName == item2.itemName;
        }

        public int GetHashCode(Item obj)
        {
            throw new NotImplementedException();
        }
    }

    }

