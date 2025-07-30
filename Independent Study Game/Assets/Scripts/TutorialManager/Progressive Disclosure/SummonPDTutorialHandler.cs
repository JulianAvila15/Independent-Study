using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class SummonPDTutorialHandler : MonoBehaviour
{
    //Summon
    public SummonManager summonHandler;
    public GameObject[] pdVisualCues;
    public GameObject enabledpdVisualCue;
    int visualCueNum;
    public GameObject penguinIngredientArrow;
    int ingredientOffSetVisualCue = 5;

    AbilityTutorialProgressiveDisclosureHandler mainAbilityPDHandler;

    [SerializeField] private  CraftingManager craftingManager;
    [SerializeField] private  OrderManager orderMan;

    Helper currentSummon;

    int slotIndex;

    bool summonCurrentlyInUse;

    private Coroutine summonCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        mainAbilityPDHandler = gameObject.GetComponent<AbilityTutorialProgressiveDisclosureHandler>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSummonPDTutorialStep(ref Helper currendPDDataSummon, int designatedSlotIndexNum, bool needSummon)
    {
        currentSummon = currendPDDataSummon;
        slotIndex = designatedSlotIndexNum;
        summonCurrentlyInUse = needSummon;

    }


    //Summon pertinent step handling
    public void HighlightSlot()
    {
        visualCueNum = craftingManager.ingredientsList.IndexOf(orderMan.listOfOrder[CraftingManager.penguinSlot]);


        if (mainAbilityPDHandler.GetCurrentAbilityDataName() == "Penguin")
        {
            pdVisualCues[visualCueNum % ingredientOffSetVisualCue].SetActive(true);
            penguinIngredientArrow.SetActive(true);
        }


    }

    public void ResetSummon()
    {
        if (currentSummon != null)
        {
            switch (mainAbilityPDHandler.GetCurrentAbilityDataName())
            {
                case "Penguin":
                    craftingManager.penguinItemSuccessfullyDropped = false;
                    break;
                case "Messenger":
                    currentSummon.selectedTile.SetActive(true);
                    break;
                default:
                    break;

            }

            currentSummon.helperTransform.position = currentSummon.originPosition;
        }

        mainAbilityPDHandler.AdvanceAbilityStep();
    }


    public void ReplaceIngredient()
    {
        Item randomItem = orderMan.listOfOrder[UnityEngine.Random.Range(0, orderMan.listOfOrder.Count)];

        Slot selectedSlotForReplacement = craftingManager.craftingSlots[slotIndex];


        if (mainAbilityPDHandler.GetCurrentAbilityDataName() == "Penguin")
                Helper.penguinHasShownIngredient = false;

            while (randomItem.itemName == orderMan.listOfOrder[slotIndex].itemName)
            {
                randomItem = craftingManager.ingredientsList[UnityEngine.Random.Range(0, orderMan.numOfIngredientsAvailable)];

            }

            selectedSlotForReplacement.item = randomItem;
            selectedSlotForReplacement.GetComponent<Image>().sprite = randomItem.GetComponent<Image>().sprite;


        


       mainAbilityPDHandler.AdvanceAbilityStep();

    }

    public bool NeedSummon()
    {
        return summonCurrentlyInUse;
    }

    public void IngredientDropped()
    {
        HighlightSlot();
       craftingManager.firstTimeUsePenguin = true;
        craftingManager.nextButton.interactable = craftingManager.prevButton.interactable = false;
    }

    public void SummonAppeared(Helper summon)
    {
       summonCoroutine =  StartCoroutine(WaitForSummonToAppear(summon));

    }


    public void SetSummon(ref Helper summon)
    {
        currentSummon = summon;
    }

    public void WatchSummon()
    {

      summonCoroutine =  StartCoroutine(WaitForSummonToGetDestroyed());
    }

    IEnumerator WaitForSummonToAppear(Helper summonHelper)
    {
        yield return StartCoroutine(
      mainAbilityPDHandler.WaitUntilWithTimeout(() => summonHelper.summonedReachedPDTutorialDestination,5f));

        NullifySummonCoroutine();

      mainAbilityPDHandler.AdvanceAbilityStep();
    }

    //Summon specific end functions
    public void EndIngredientDropped()
    {
        //End IngredientDropped step
            pdVisualCues[visualCueNum % ingredientOffSetVisualCue].SetActive(false);
            penguinIngredientArrow.SetActive(false);
            craftingManager.nextButton.interactable = craftingManager.prevButton.interactable = true;
    }

    IEnumerator WaitForSummonToGetDestroyed()
    {
        yield return StartCoroutine(
    mainAbilityPDHandler.WaitUntilWithTimeout(() => !CraftingManager.helperIsActive, 5f));

        NullifySummonCoroutine();

       mainAbilityPDHandler.AdvanceAbilityStep();
    }


    public void NullifySummonCoroutine()
    {
       if(summonCoroutine!=null)
        {
            StopCoroutine(summonCoroutine);
            summonCoroutine = null;
        }
    }
   
}
