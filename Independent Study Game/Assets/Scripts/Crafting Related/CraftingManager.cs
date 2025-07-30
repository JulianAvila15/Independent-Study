using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public Item currentItem;
    public Image customCursor;

    public Slot[] craftingSlots; //slots created for crafting slots

    public List<Item> ingredientsList, finalOrderList;
    public Slot result;
    public Item resultingItem;
    public OrderManager orderManager;
    public TutorialManager tutorialManager;
  [SerializeField]   private IntroProgressiveDisclosureHandler pdIntroManager;
    public Button craftButton;
    public static int numberOfCraftingSlots;

    public GameObject[] setsOfIngredients;
    public Button nextButton, prevButton;
    public int setIndex = 0;

    public GameObject grayImage;
    public static bool helperIsActive = false;
    public static string helperName;
    public static int penguinSlot = -1;//Gets the slot the penguin has chosen to color blue
    private int penguinProspectiveSlotNum = 1;
    public GameObject mainGame;
    public GameManager gameManager;
    public int highlightedSlotIndex_ProgressiveDisclosureTutorial = 0; //for the tutorial, it specifies the index of the ingredient in the order that needs to be satisfied 
    private int selectedSlotIndex = 0;

    public AbilityTutorialProgressiveDisclosureHandler abilityPDManager;

    static public bool canClickCraftButton = false;

    public bool firstTimeUsePenguin = false;

    public bool penguinItemSuccessfullyDropped = false;

    int prevIngredientSetCount=-1,prevSetIndex = -1;

    Color penguinButtonColor;

   public GameObject penguinButton;
    public Image penguinImage;
    // Start is called before the first frame update
    void Start()
    {
        numberOfCraftingSlots = craftingSlots.Length;//initialize the number of crafting 

        abilityPDManager = tutorialManager.abilityPDHandler;

        penguinButtonColor = penguinButton.GetComponent<Image>().color;
    }
    public void OnMouseDownItem(Item item)
    {
        currentItem = item;
        if (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || ((pdIntroManager.progressiveDisclosureStep == (int)IntroProgressiveDisclosureHandler.PDStepsIntro.DuringCraftEvent && orderManager.listOfOrder[highlightedSlotIndex_ProgressiveDisclosureTutorial] == currentItem) || (IntroProgressiveDisclosureHandler.introPDTutorialFinished&&!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)|| penguinSlot>0&&orderManager.listOfOrder[penguinSlot] == currentItem))
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

        int maxIndex = Mathf.FloorToInt(orderManager.numOfIngredientsAvailable / (setsOfIngredients.Length-1));

        nextButton.gameObject.SetActive(setIndex < maxIndex-1);
        prevButton.gameObject.SetActive(setIndex > 0);
    }

    public void SetIndexIncrease()
    {
        setIndex++;
        UpdateIngredientPanel();
        TimeManager.ResetAFKTimer();
    }

    public void SetIndexDecrease()
    {
        setIndex--;

        UpdateIngredientPanel();
        TimeManager.ResetAFKTimer();

    }

 
    // Update is called once per frame
    private void Update()
    {

        if (mainGame.activeInHierarchy)
        {
            if (penguinSlot > 0)
                FillPenguinSlot();




            if (orderManager.penguinUnlocked&&(!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered||abilityPDManager.GetStepTutorialType()!=TutorialStepType.FlashButton))
            {

                HandlePenguinButtonEnableorDisable();
            }

            if (Input.GetMouseButtonUp(0) && currentItem != null)
                DropItem();

            MaybeUpdateIngredientPanel();
        }
    }

    private void HandlePenguinButtonEnableorDisable()
    { 

        if (finalOrderList[penguinProspectiveSlotNum] != null && orderManager.listOfOrder[penguinProspectiveSlotNum].itemName == finalOrderList[penguinProspectiveSlotNum].itemName)
        {
            penguinButton.GetComponent<Button>().interactable = false;
          penguinImage.color = penguinButton.GetComponent<Image>().color = penguinButton.GetComponent<Button>().colors.disabledColor;
        }
        else
        {
            penguinButton.GetComponent<Button>().interactable = true;
            penguinButton.GetComponent<Image>().color = penguinButtonColor;
            penguinImage.color = Color.white;
        }
    }

    private void MaybeUpdateIngredientPanel()
    {
        int currentSetCount = orderManager.numOfIngredientsAvailable;

        if (currentSetCount != prevIngredientSetCount || setIndex != prevSetIndex)
        {
            prevIngredientSetCount = currentSetCount;
            prevSetIndex = setIndex;
            UpdateIngredientPanel();
        }
    }

    private void DropItem()
    {
        if (currentItem != null && (gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || ((pdIntroManager.progressiveDisclosureStep == (int)IntroProgressiveDisclosureHandler.PDStepsIntro.DuringCraftEvent && orderManager.listOfOrder[highlightedSlotIndex_ProgressiveDisclosureTutorial] == currentItem || (IntroProgressiveDisclosureHandler.introPDTutorialFinished&&!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered) || (penguinSlot > 0&&(orderManager.listOfOrder[penguinSlot] == currentItem))))))
        {
            customCursor.gameObject.SetActive(false);
            Slot nearestSlot = null;
            float shortestDistance = float.MaxValue;
            float finalDist = 0.0f;
            float sufficentDist = 100.00f; //How far the item must be from the crafting slot
            foreach (Slot x in craftingSlots)
            {
                float dist = Vector2.Distance(Input.mousePosition, x.transform.position);

                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestSlot = x;
                    finalDist = dist;
                }
            }

            Debug.Log("trying to drop item");

            if (finalDist < sufficentDist&&(gameManager.tutorialType!=GameManager.TutorialType.progressiveDisclosure||IntroProgressiveDisclosureHandler.introPDTutorialFinished))//item is close enough to the crafting slot (if the progressive disclosure version is still in tutorial mode make sure they can only put the right ingredients in the right slot)
            {

                

               if (penguinSlot<0 || ((nearestSlot != craftingSlots[penguinSlot] && currentItem != orderManager.listOfOrder[penguinSlot])||  (currentItem == orderManager.listOfOrder[penguinSlot])))
                {

                    nearestSlot.gameObject.SetActive(true);
                    nearestSlot.GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                    nearestSlot.GetComponent<Slot>().item = currentItem;
                    finalOrderList[nearestSlot.index] = currentItem;

                    if(penguinSlot > 0&&nearestSlot ==craftingSlots[penguinSlot])
                    penguinItemSuccessfullyDropped = true;
                }

            }
            else
            {


                if (penguinSlot > 0 && Helper.penguinHasShownIngredient && currentItem != null && craftingSlots[penguinSlot].item == null && orderManager.listOfOrder[penguinSlot].itemName == currentItem.itemName)//if the penguin has shown the ingredient and the ingredient clicked on is the correct one
                {
                    craftingSlots[penguinSlot].gameObject.SetActive(true);
                    craftingSlots[penguinSlot].GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                    craftingSlots[penguinSlot].item = currentItem;
                    finalOrderList[penguinSlot] = currentItem;
                    penguinItemSuccessfullyDropped = true;
                    Debug.Log(firstTimeUsePenguin);
                    Debug.Log("penguin is a virgin");


                }
                else if (GetAvailableSlot() > -1 && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered )//fill next available slot if there is one
                {
                    craftingSlots[GetAvailableSlot()].gameObject.SetActive(true);
                    craftingSlots[GetAvailableSlot()].GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                    craftingSlots[GetAvailableSlot()].item = currentItem;
                    finalOrderList[GetAvailableSlot()] = currentItem;
                }
                else if(!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                {
                    if (Helper.penguinHasShownIngredient && selectedSlotIndex % 4 == penguinSlot)//skip iteration 
                        selectedSlotIndex++;


                    if (craftingSlots[selectedSlotIndex % 4].item == null || craftingSlots[selectedSlotIndex % 4].item != currentItem)
                    {

                        craftingSlots[selectedSlotIndex % 4].gameObject.SetActive(true);
                        craftingSlots[selectedSlotIndex % 4].GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                        craftingSlots[selectedSlotIndex % 4].item = currentItem;
                        finalOrderList[selectedSlotIndex % 4] = currentItem;


                        selectedSlotIndex++;

                        Debug.Log("All slots filled");

                    }


                }
            }



            if (gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure && !IntroProgressiveDisclosureHandler.introPDTutorialFinished) //if the tutorial type is progressive disclosure, guide them through the order by specifying which ingredients to drag and don't allow clicking the ones not correct
            {
                if (highlightedSlotIndex_ProgressiveDisclosureTutorial < numberOfCraftingSlots - 1)//Guide player to complete the order till complete
                {
                    pdIntroManager.introPDVisualCues[pdIntroManager.visualCueNum].SetActive(false);
                    pdIntroManager.slotBGs[highlightedSlotIndex_ProgressiveDisclosureTutorial].GetComponent<Image>().color = new Color(255, 255, 255);//set color back to white
                    highlightedSlotIndex_ProgressiveDisclosureTutorial++;
                }
                else if (!IntroProgressiveDisclosureHandler.introPDTutorialFinished) //order is complete 
                {
                    tutorialManager.tutorialPanel.SetActive(true);
                }
            }
            currentItem = null;
            TimeManager.ResetAFKTimer();
        }
    }

    private void FillPenguinSlot()
    {
        if (craftingSlots[penguinSlot].item != null && finalOrderList[penguinSlot].itemName != orderManager.listOfOrder[penguinSlot].itemName)//if the item that is in that specific crafting slot is not the correct ingredient remove it
            craftingSlots[penguinSlot].GetComponent<Image>().color = Color.yellow;
        else if (mainGame.activeInHierarchy == true)//if the penguin has shown the ingredient
        {
            if (finalOrderList[penguinSlot] != null && orderManager.listOfOrder[penguinSlot].itemName == finalOrderList[penguinSlot].itemName)//if the correct item is in the in the penguin slot
            {

                craftingSlots[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                orderManager.listOfOrder[penguinSlot].GetComponent<Image>().color = new Color(255, 255, 255);
                Helper.penguinHasShownIngredient = false;
                if (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                    penguinSlot = -1;

                if (firstTimeUsePenguin)
                {
                    abilityPDManager.AdvanceAbilityStep();
                    firstTimeUsePenguin = false;
                }
            }
        }
    }

    public void CheckForCompletedOrder()
    {
        if (!finalOrderList.Contains(null))//if list is not empty
        {

            if (!(gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure) || canClickCraftButton == true&&!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
            {

                if (!finalOrderList.Contains(null))
                    TimeManager.ResetAFKTimer();

                orderManager.CheckItem();

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

    public void RemoveItemFromSlot(int slotIndex)
    {
        finalOrderList[slotIndex] = null;
        craftingSlots[slotIndex].item = null;
        craftingSlots[slotIndex].GetComponent<Image>().sprite = null;
       
        
    }

    public void OnClickSlot(int slotIndex)
    {
        if((gameManager.tutorialType!=GameManager.TutorialType.progressiveDisclosure)||(DataMiner.numOfSuccessfulCrafts>=1))
        RemoveItemFromSlot(slotIndex);
    }

   public int GetAvailableSlot()
    {
        for (int craftingSlotNumber = 0; craftingSlotNumber < numberOfCraftingSlots; craftingSlotNumber++)
        {
            if(penguinSlot==craftingSlotNumber && Helper.penguinHasShownIngredient)
            {
                continue;
            }
            else if (finalOrderList[craftingSlotNumber] == null)
            {
                return craftingSlotNumber;
            }
        }

        Debug.Log("Has shown ingredient? " + Helper.penguinHasShownIngredient);

        return -1; // Or throw an exception if no available slot is found
    }


    public void CreateSlot(int slotIndex)
        { 
        finalOrderList[slotIndex] = orderManager.listOfOrder[slotIndex];
        craftingSlots[slotIndex].GetComponent<Slot>().item = orderManager.listOfOrder[slotIndex];
        craftingSlots[slotIndex].GetComponent<Image>().sprite = orderManager.listOfOrder[slotIndex].GetComponent<Image>().sprite;
        }



}
