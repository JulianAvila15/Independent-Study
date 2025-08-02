using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{

  [SerializeField]  ManagerofManagers managerHub;

    public Item currentItem;
    public Image customCursor;

    public Slot[] craftingSlots; //slots created for crafting slots
    public Image[] imageCraftingSlots;
    public Vector3[] craftingSlotPositions;

    public List<Item> ingredientsList, finalOrderList;
    public Slot result;
    public Item resultingItem;
    public Button craftButton;
    public int numberOfCraftingSlots;

    public GameObject[] setsOfIngredients;
    public Button nextButton, prevButton;
    public int setIndex = 0;

    public GameObject grayImage;
    public static bool helperIsActive = false;
    public static string helperName;

    public GameObject mainGame;

  [SerializeField]  private int selectedSlotIndex = 0;
   [SerializeField] private int selectedSlotIndexDivisor = 4;
  [SerializeField]  int prevIngredientSetCount = -1, prevSetIndex = -1;

    public static bool canClickCraftButton;

  public int nearestSlotIndex = -1;

    public enum slotIndexes
    {
        firstSlotIndex = 0,
        secondSlotIndex =1,
        thirdSlotIndex =2,
        fourthSlotIndex=3
    }

    private void Awake()
    {
        if (managerHub == null)
            Debug.Break();

        if (managerHub != null && managerHub.craftingManager == null)
            managerHub.craftingManager = gameObject.GetComponent<CraftingManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

        numberOfCraftingSlots = craftingSlots.Length;//initialize the number of crafting 
        
        for(int ingredientListIndex=1; ingredientListIndex<setsOfIngredients.Length;ingredientListIndex++)
        {
            setsOfIngredients[ingredientListIndex].SetActive(false);
        }

        //get slot images and slot info
        for(int currSlotNum = 0; currSlotNum<craftingSlots.Length;currSlotNum++)
        {
            if (craftingSlots[currSlotNum] != null)
            {
                imageCraftingSlots[currSlotNum] = craftingSlots[currSlotNum].GetComponent<Image>();
                craftingSlotPositions[currSlotNum] = craftingSlots[currSlotNum].GetComponent<Transform>().position;
            }
        }
    }
    public void OnMouseDownItem(Item item)
    {
        currentItem = item;
        if (CanPickUpItem())
        {

            customCursor.gameObject.SetActive(true);
            customCursor.sprite = item.GetComponent<Image>().sprite;
            DataMiner.numOfIngredientClicks++;
        }

    }

    private void UpdateIngredientPanel()
    {
        for (int i = 0; i < setsOfIngredients.Length; i++)
            setsOfIngredients[i].SetActive(i == setIndex);

        int maxIndex = Mathf.FloorToInt(managerHub.orderManager.numOfIngredientsAvailable / (setsOfIngredients.Length-1)); //the max index you can have at a time is the the floor of: the number of ingredients available to the player divided by the total number of sets of ingredients

        nextButton.gameObject.SetActive(setIndex < maxIndex-1);
        prevButton.gameObject.SetActive(setIndex > 0);
    }

    public void SetIndexIncrease()
    {
        setIndex++;
        UpdateIngredientPanel();
        managerHub.timeManager.ResetAFKTimer();
    }

    public void SetIndexDecrease()
    {
        setIndex--;

        UpdateIngredientPanel();
        managerHub.timeManager.ResetAFKTimer();

    }

 
    // Update is called once per frame
    private void Update()
    {

        if (mainGame.activeInHierarchy)
        {

            if (Input.GetMouseButtonUp(0) && currentItem != null)
                DropItem();

            MaybeUpdateIngredientPanel();
        }
    }

    private void MaybeUpdateIngredientPanel()
    {
        int currentSetCount = managerHub.orderManager.numOfIngredientsAvailable;

        if (currentSetCount != prevIngredientSetCount || setIndex != prevSetIndex)
        {
            prevIngredientSetCount = currentSetCount;
            prevSetIndex = setIndex;
            UpdateIngredientPanel();
        }
    }

    private void DropItem()
    {
        if (CanDropItem())
        {
            customCursor.gameObject.SetActive(false);
         
            float shortestDistance = float.MaxValue;
            float finalDist = 0.0f;
            float sufficentDist = 100.00f; //How far the item must be from the crafting slot
            foreach (Slot potentialNearestSlot in craftingSlots)
            {
                float dist = Vector2.Distance(Input.mousePosition, potentialNearestSlot.transform.position);

                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestSlotIndex = Array.IndexOf(craftingSlots,potentialNearestSlot) ;
                    finalDist = dist;
                }
            }

            if (ItemClosestToCraftingSlot(ref finalDist ,ref sufficentDist))//item is close enough to the crafting slot (if the progressive disclosure version is still in tutorial mode make sure they can only put the right ingredients in the right slot)
            {
                if (managerHub.summonManager.CanSafetlyDropItemWhenPenguinIsActive(nearestSlotIndex)&&nearestSlotIndex==managerHub.summonManager.penguinSlot)
                    managerHub.summonManager.FillPenguinSlot();
                else
                    FillInSlot(nearestSlotIndex,false);
               

            }
            else //if item is clicked
            {
                if (managerHub.summonManager.CanSafetlyDropItemWhenPenguinIsActive())//if the penguin has shown the ingredient and the ingredient clicked on is the correct one
                {
                    managerHub.summonManager.FillPenguinSlot();
                }
                else if (GetAvailableSlot() > -1 && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered )//fill next available slot if there is one
                {
                    FillInSlot(GetAvailableSlot(),false);
                }
                else if(!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                {
                    if (Helper.penguinHasShownIngredient && selectedSlotIndex % selectedSlotIndexDivisor == managerHub.summonManager.penguinSlot)//skip iteration 
                        selectedSlotIndex++;


                    if (craftingSlots[selectedSlotIndex % selectedSlotIndexDivisor].item == null || craftingSlots[selectedSlotIndex % selectedSlotIndexDivisor].item != currentItem)
                    {
                        FillInSlot(selectedSlotIndex % selectedSlotIndexDivisor,false);

                        selectedSlotIndex++;
                    }
                }
            }



            if (managerHub.tutorialManager.IntroTutorialOccuring()) //if the tutorial type is progressive disclosure, guide them through the order by specifying which ingredients to drag and don't allow clicking the ones not correct
            {
                managerHub.introPDManager.HighlightIngredientandCorrespondingSlot();
            }


            currentItem = null;
            managerHub.timeManager.ResetAFKTimer();
        }
    }

  public void FillInSlot(int selectedSlotIndex, bool isAutoFill)
    {
        Item itemToFillSlot;

        if (isAutoFill)
            itemToFillSlot = managerHub.orderManager.listOfOrder[selectedSlotIndex];
        else
            itemToFillSlot = currentItem;

        craftingSlots[selectedSlotIndex].gameObject.SetActive(true);
        imageCraftingSlots[selectedSlotIndex].sprite = itemToFillSlot.imageOfItem.sprite;
       craftingSlots[selectedSlotIndex].item = itemToFillSlot;
        finalOrderList[selectedSlotIndex] = itemToFillSlot;
    }


    public void CheckForCompletedOrder()
    {
        if (!finalOrderList.Contains(null))//if list is not empty
        {

            if (CanCraft())
            {
                if (!finalOrderList.Contains(null))
                    managerHub.timeManager.ResetAFKTimer();

                managerHub.orderManager.CheckItem();

                DataMiner.numOfCrafts++;

                ClearList(); //if the order is crafted correctly clear the final order list

                selectedSlotIndex = 0;
            }
        }
        
    }

    public void ClearList()
    {
        for(int i = 0; i<finalOrderList.Count;i++)
        {
            finalOrderList[i] = null;
        }
    }

    public void RemoveItemFromSlot(int selectedSlotIndex)
    {
        
        finalOrderList[selectedSlotIndex] = null;
        craftingSlots[selectedSlotIndex].item = null;
        craftingSlots[selectedSlotIndex].GetComponent<Image>().sprite = null;
       
        
    }

    public void OnClickSlot(int slotIndex)
    {
        if(CanRemoveIngredientFromSlot())
        RemoveItemFromSlot(slotIndex);
    }

   public int GetAvailableSlot()
    {
        for (int craftingSlotNumber = 0; craftingSlotNumber < numberOfCraftingSlots; craftingSlotNumber++)
        {
            if(managerHub.summonManager.penguinSlot==craftingSlotNumber && Helper.penguinHasShownIngredient)
            {
                continue;
            }
            else if (finalOrderList[craftingSlotNumber] == null)
            {
                return craftingSlotNumber;
            }
        }

        return -1; // Or throw an exception if no available slot is found
    }


    public bool NeedToFillSlot(int selectedSlotIndex)
    {
     return   (craftingSlots[selectedSlotIndex].item == null) || (craftingSlots[selectedSlotIndex].item.itemName != managerHub.orderManager.listOfOrder[selectedSlotIndex].itemName);
    }

    bool CanRemoveIngredientFromSlot()
    {
        return (managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure) || (DataMiner.numOfSuccessfulCrafts >= 1);
    }

    bool CanDropItem()
    {
        return (currentItem != null && (managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || managerHub.introPDManager.IntroPDCraftEventOccuring() || managerHub.tutorialManager.NoPDTutorialOccuring() || managerHub.summonManager.PenguinInCurrentUse()));
    }

    bool CanPickUpItem()
    {
        return (managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || managerHub.introPDManager.IntroPDCraftEventOccuring() || managerHub.tutorialManager.NoPDTutorialOccuring() || managerHub.summonManager.PenguinInCurrentUse());
    }

    bool ItemClosestToCraftingSlot(ref float finalDist, ref float sufficentDist)
    {
        return finalDist < sufficentDist && (managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || IntroProgressiveDisclosureHandler.introPDTutorialFinished);
    }

    bool CanCraft()
    {
        return !(managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure) || canClickCraftButton == true && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered;
    }

}
