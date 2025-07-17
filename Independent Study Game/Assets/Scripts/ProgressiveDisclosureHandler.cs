using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class ProgressiveDisclosureHandler : MonoBehaviour
{
    public string[] sentences;
    public int sentenceIndex, abilitySentenceIndex;
    public float typingSpeed;
    public TutorialManager tutorialManager;
    public GameManager gameManager;
    public CraftingManager craftingManager;
    public GameObject[] slotBGs;
    public  bool isForSummon;
    public int introTutorialCallOutNumber = 0;
    Color tutorialPanelColor;
    public GameObject pdCraftButton;
    public GameObject[] introTutorialCallOuts, introPDVisualCues;
    public int visualCueNum=0;
    static public bool introPDTutorialFinished=false;
    [SerializeField] private int[] showIntroTutorialSpeechBubbleAtSentenceIndex;
    public int progressiveDisclosureStep = 0;

    public OrderManager orderMan;

    [SerializeField] private GameObject pdTutorialPanelAbility;

   public GameObject pDAbilityContinueButton;
    public enum PDStepsIntro
    {
        BeginCraftEvent=3,
        DuringCraftEvent=4,
        ClickCraftButtonEvent=6,
        DescribeFeedBack=7
    }
    public static PDStepsIntro progressive_DisclosureSteps;

    [SerializeField] private AbilityTutorialData[] abilityTutorialDataArray;
    [SerializeField] private AbilityTutorialData currentAbilityTutorialData;//get the steps and data about the current ability's progressive disclosure data
    private int currentAbilityTutorialStepIndex; //get the current ability tutorial step index
    public HashSet<string> completedTutorials=new HashSet<string>();

    private Coroutine currentStepCoroutine;

    public Abilities currentAbilityUsed;

    AbilityTutorialStepData current_step;

   static public bool abilityTutorialTriggered = false;

    private void Awake()
    {
        orderMan = craftingManager.orderManager;
    }

    private void Start()
    {
        if (!gameManager.inTestingMode)
        {
            if (tutorialManager.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure)
            {

                tutorialManager = tutorialManager.GetComponent<TutorialManager>();
                tutorialManager.continueButton.gameObject.SetActive(false);

                isForSummon = false;
                StartCoroutine(Type());
            }

            tutorialPanelColor = tutorialManager.tutorialPanel.GetComponent<Image>().color;
        }
        else
            introPDTutorialFinished=true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!introPDTutorialFinished) //if beginning tutorial is not completed yet
        {
            if (gameManager.craftingManager.highlightedSlotIndex_ProgressiveDisclosureTutorial <=CraftingManager.numberOfCraftingSlots && !tutorialManager.tutorialPanel.activeInHierarchy)//visual queues for progressive disclosure
            {
                visualCueNum = craftingManager.ingredientsList.IndexOf(gameManager.orderManager.listOfOrder[gameManager.craftingManager.highlightedSlotIndex_ProgressiveDisclosureTutorial]);
                introPDVisualCues[visualCueNum].gameObject.SetActive(true);
            }
            else
            {
                HideAllVisualCue();
            }

            if (progressiveDisclosureStep == (int)PDStepsIntro.DuringCraftEvent && !tutorialManager.tutorialPanel.activeInHierarchy)
            {
                slotBGs[craftingManager.highlightedSlotIndex_ProgressiveDisclosureTutorial].GetComponent<Image>().color = new Color(255, 255, 0);
            }
            else
            {
                slotBGs[craftingManager.highlightedSlotIndex_ProgressiveDisclosureTutorial].GetComponent<Image>().color = new Color(255, 255, 255);
            }

            if (progressiveDisclosureStep == (int)PDStepsIntro.ClickCraftButtonEvent)
            {
                tutorialManager.continueButton.gameObject.SetActive(false);
                pdCraftButton.SetActive(true);

                tutorialManager.tutorialPanel.GetComponent<Image>().color = new Color(tutorialPanelColor.r, tutorialPanelColor.g, tutorialPanelColor.b, 0);
                introPDVisualCues[4].SetActive(true);
                introPDVisualCues[5].SetActive(true);
            

                if (gameManager.orderManager.currentOrderIndex == 1)
                {
                    pdCraftButton.SetActive(false);
                    tutorialManager.tutorialPanel.GetComponent<Image>().color = tutorialPanelColor;
                    introPDVisualCues[4].SetActive(false);
                    introPDVisualCues[5].SetActive(false);

                    NextSentence();

                }

            }

         
        }

        if (!craftingManager.craftButton.gameObject.activeInHierarchy)
            craftingManager.craftButton.gameObject.SetActive(true);

    }

    IEnumerator Type()
    {


        while (!tutorialManager.tutorialPanel.activeInHierarchy && !introPDTutorialFinished)
        {
            yield return null; 
        }

        introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text = "";


            foreach (char letter in sentences[sentenceIndex])
            {
                introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }


        if (!tutorialManager.tutorialPanel.activeInHierarchy && progressiveDisclosureStep!=(int)PDStepsIntro.DescribeFeedBack)
            StopCoroutine(Type());

        if (progressiveDisclosureStep == (int)PDStepsIntro.ClickCraftButtonEvent)
        {
            CraftingManager.canClick = true;
        }

        if (progressiveDisclosureStep < (int)PDStepsIntro.ClickCraftButtonEvent || progressiveDisclosureStep == (int)PDStepsIntro.DescribeFeedBack)
        {
            tutorialManager.continueButton.gameObject.SetActive(true);
        }
      
    }

    public void NextSentence()//would be parameterized to include the specific number of sentences per speech bubble
    {
        tutorialManager.continueButton.gameObject.SetActive(false);

        if (!introPDTutorialFinished)//intro tutorial has not completed
        {


            //change speech bubble if the sentenceIndex of the sentence is a specific number
            if (showIntroTutorialSpeechBubbleAtSentenceIndex.Contains(progressiveDisclosureStep))
            {
                introTutorialCallOuts[introTutorialCallOutNumber].SetActive(false);

                if (progressiveDisclosureStep == (int)PDStepsIntro.BeginCraftEvent)
                {
                    tutorialManager.tutorialPanel.SetActive(false);
                }
                introTutorialCallOutNumber++;
                introTutorialCallOuts[introTutorialCallOutNumber].SetActive(true);
            }

            if (sentenceIndex < sentences.Length - 1)//instead of sentences.length-1 it would only go up to how many sentences there would be per speech bubble
            {
                sentenceIndex++;

                introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text = "";
                progressiveDisclosureStep++;
                StartCoroutine(Type());
            }
            else
            {
                introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text = "";//clear text
                tutorialManager.tutorialPanel.gameObject.SetActive(false); //set tutorial panel unactive
                introPDTutorialFinished = true;//tutorial has finished so inform the time manager
                if (sentenceIndex < introTutorialCallOuts.Length)
                    sentenceIndex++;
                HideAllVisualCue();
                this.gameObject.SetActive(false);
            }
        }
    }

    public void HideAllVisualCue()
    {
        for (int i = 0; i < introPDVisualCues.Length; i++)
        {
            if (introPDVisualCues[i].activeInHierarchy)
                introPDVisualCues[i].SetActive(false);
        }
    }


    public void SetCurrentAbilityData(int currentAbilityIndex)
    {
        
            currentAbilityTutorialData = abilityTutorialDataArray[currentAbilityIndex];
        if (currentAbilityTutorialData.steps.Count > 0)
        {
            abilityTutorialTriggered = true;
            Debug.Log(abilityTutorialTriggered);
            TriggerAbilityTutorial(currentAbilityTutorialData);
        }
    }

    public void TriggerAbilityTutorial(AbilityTutorialData triggeredAbility)
    {
        if(!gameObject.activeInHierarchy)
        gameObject.SetActive(true);
        if (!HasCompletedTutorial(triggeredAbility.abilityName))
        {
            
            currentAbilityTutorialData = triggeredAbility;
            currentAbilityTutorialStepIndex= 0;
            gameObject.SetActive(true);
            StartStep();
        }

    }

    private void StartStep()
    {
        var step = currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex];
        current_step = step;

        switch (step.stepType)
        {
            case TutorialStepType.ShowText:
                //show text and display speech bubble
                ShowText(ref step);
                break;
            case TutorialStepType.FlashButton:
                step.clicked = false;
                FlashButton(ref step);
                break;
            case TutorialStepType.HighlightSlot:
                //highlight slot it will affect
                currentAbilityTutorialData.selectedSlotColor = craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex].GetComponent<Image>().color;
                HighlightSlot();
                break;
            case TutorialStepType.SummonedAppeared:
                //Summon appeared
                //StartCoroutine(AssignSummonReference());
                currentAbilityTutorialData.summon = currentAbilityUsed.createdObject.GetComponent<Helper>();
                Debug.Log("summon set");
                SummonAppeared(currentAbilityTutorialData.summon);
                break;
            case TutorialStepType.IngredientDropped:
                //Ingredient Dropped
                IngredientDropped();
                break;
            case TutorialStepType.ReplaceIngredient:
                ReplaceIngredient();
                break;
            case TutorialStepType.waitForWalkRight:
                //wait for player to walk right
               
                WaitForWalkRightOrLeft(true);
                break;
            case TutorialStepType.waitForWalkLeft:
                //wait for player to walk right
                WaitForWalkRightOrLeft(false);
                break;
            case TutorialStepType.waitForJump:
                WaitForJump();
                break;
            case TutorialStepType.waitForStopOnGreen:
                //wait for player to walk right
                WaitForStopOnGreen();
                break;
        
            
            
                // Add others as needed
        }


    }

    private void ReplaceIngredient()
    {
        Item randomItem = orderMan.listOfOrder[UnityEngine.Random.Range(0, orderMan.listOfOrder.Count)];

        Slot selectedSlotForReplacement = craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex];

        while (randomItem.itemName == orderMan.listOfOrder[currentAbilityTutorialData.slotIndex].itemName) 
        {
            randomItem = orderMan.listOfOrder[UnityEngine.Random.Range(0, orderMan.listOfOrder.Count)];
            
        }
        Debug.Log("trying to replace ingredient");
        selectedSlotForReplacement.item = randomItem;
        selectedSlotForReplacement.GetComponent<Image>().sprite = randomItem.GetComponent<Image>().sprite;
        Debug.Log(craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex].item.itemName);


    }

    private void WaitForStopOnGreen()
    {
        //logic to wait for what happens when the player lands on green
    }

    private void WaitForJump()
    {
        //logic that is executed when the player jumps x number of times
    }

    private void WaitForWalkRightOrLeft(bool walkLeftorWalkRight) //temp argument to signify if the player has walked x number of pixels right or left
    {
      //logic that is executed when the player walks x number to the right
    }

    private void IngredientDropped()
    {
        if (pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.gameObject.SetActive(false);
        craftingManager.firstTimeUsePenguin = true;

        //Ingredient Dropped logic
    }

    private void SummonAppeared(Helper summon)
    {
        StartCoroutine(WaitForSummonToAppear(summon));
       
    }

    IEnumerator AssignSummonReference()
    {
        yield return null; // wait one frame
        Helper summon = currentAbilityTutorialData.summon = currentAbilityUsed.createdObject.GetComponent<Helper>();

        if (summon == null)
            Debug.LogError("Helper script still not found on createdObject.");
    }

    IEnumerator WaitForSummonToAppear(Helper summonHelper)
    {
       yield return new WaitUntil(() => summonHelper.summonedReachedPDTutorialDestination);
        AdvanceAbilityStep();
    }

    public void SetSummon(ref Helper summon)
     {
        currentAbilityTutorialData.summon = summon;
     }

    private void HighlightSlot()
    {
        //logic to highlight slot or slots the summoned ability will fill

        Item selectedSlotItem = orderMan.listOfOrder[currentAbilityTutorialData.slotIndex];

        if (!pdCraftButton.activeInHierarchy)
            pdCraftButton.SetActive(true);

        Debug.Log(selectedSlotItem.itemName);
        Debug.Log(craftingManager.ingredientsList.IndexOf(selectedSlotItem) % 5);

        introPDVisualCues[craftingManager.ingredientsList.IndexOf(selectedSlotItem)%5].SetActive(true);

        craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex].GetComponent<Image>().color = Color.yellow;
    }

    private void FlashButton(ref AbilityTutorialStepData step)
    {
        
        pdTutorialPanelAbility.SetActive(true);
        pDAbilityContinueButton.SetActive(false);
        pdTutorialPanelAbility.GetComponent<Image>().raycastTarget=false;
        currentStepCoroutine=StartCoroutine(AnimateFlashButton(step.targetObject, step));
    }

    private void ShowText(ref AbilityTutorialStepData step)
    {
        if (!pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.SetActive(true);
        Debug.Log(step.sentence);
        currentStepCoroutine = StartCoroutine(TypeForAbility(step.callOut, step.sentence));
    }


    public void AdvanceAbilityStep()
    {
        EndStep();
        if (currentAbilityTutorialStepIndex >= currentAbilityTutorialData.steps.Count - 1)
        {
            MarkTutorialComplete(currentAbilityTutorialData.abilityName);
            EndAbilityTutorial(); // Optionally disable this handler/UI
        }
        else
        {

            currentAbilityTutorialStepIndex++;
            StartStep();
        }
    }

    public void EndStep()
    {
        if (currentAbilityTutorialStepIndex<=currentAbilityTutorialData.steps.Count-1)
        {
            
            switch(current_step.stepType)
            {
                case TutorialStepType.ShowText:
                    EndShowTextStep();
                    break;
                case TutorialStepType.FlashButton:
                    EndFlashButtonStep();
                    break;
                case TutorialStepType.HighlightSlot:
                    //highlight slot it will affect
                    EndHighlightSlot();
                    break;
                case TutorialStepType.SummonedAppeared:
                    //Summon appeared
                    EndSummonAppeared();
                    break;
                case TutorialStepType.IngredientDropped:
                    //Ingredient Dropped
                    EndIngredientDropped();
                    break;

                case TutorialStepType.waitForWalkRight:
                    //wait for player to walk right

                    EndWaitForWalkRightOrLeft(true);
                    break;
                case TutorialStepType.waitForWalkLeft:
                    //wait for player to walk right
                    EndWaitForWalkRightOrLeft(false);
                    break;
                case TutorialStepType.waitForJump:
                    EndWaitForJump();
                    break;
                case TutorialStepType.waitForStopOnGreen:
                    //wait for player to walk right
                    EndWaitForStopOnGreen();
                    break;
            }

            if (current_step.summonCanContinue)
                currentAbilityTutorialData.summon.summonedCanContinuePDTutorial = true;
            else if (currentAbilityTutorialData.summon != null)
                currentAbilityTutorialData.summon.summonedCanContinuePDTutorial =false;

            StopCoroutine(currentStepCoroutine);
        }
    }

    private void EndWaitForStopOnGreen()
    {
        //End WaitForStopOnGreen step
    }

    private void EndWaitForJump()
    {
        //End WaitForJump step
    }

    private void EndWaitForWalkRightOrLeft(bool v)
    {
       //End WaitForWalkRIghtOrLeft step
    }

    private void EndIngredientDropped()
    {
        //End IngredientDropped step
    }

    private void EndSummonAppeared()
    {
        //End SummonAppeared step
        Debug.Log("summon appeared");
    }

    private void EndHighlightSlot()
    {

        Item selectedSlotItem = orderMan.listOfOrder[currentAbilityTutorialData.slotIndex];
        //End HightlightSlot step by setting slot color back to what it was
        craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex].GetComponent<Image>().color= currentAbilityTutorialData.selectedSlotColor;
        introPDVisualCues[craftingManager.ingredientsList.IndexOf(selectedSlotItem) % 5].SetActive(false);
    }

    private void EndFlashButtonStep()
    {
        Debug.Log("Button clicked");
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].clicked = true;
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].targetObject.GetComponent<Image>().color= currentAbilityTutorialData.buttonColor;
    }

    private void EndShowTextStep()
    {
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].callOut.SetActive(false);
           

    }

    private void EndAbilityTutorial()
    {
        //add logic to disable panel


        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].clicked = true;
        Debug.Log("ability tutorial end");
        currentAbilityTutorialData = null;
        currentAbilityTutorialStepIndex = 0;
        if (pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.SetActive(false);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        if (pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.SetActive(false);

        abilityTutorialTriggered = false;
    }

    bool HasCompletedTutorial(string abilityName)
    {
        return completedTutorials.Contains(abilityName);
    }

    void MarkTutorialComplete(string abilityName)
    {
        if (!completedTutorials.Contains(abilityName))
            completedTutorials.Add(abilityName);
    }


    IEnumerator TypeForAbility(GameObject speechBubble, string abilitySentenceString)
    {
        Debug.Log(abilitySentenceString);

        while (!pdTutorialPanelAbility.activeInHierarchy)
        {
            yield return null;
        }

        speechBubble.SetActive(true);
        Text speechBubbleText = speechBubble.gameObject.GetComponentInChildren<Text>();

        speechBubbleText.text = "";
        foreach (char letter in abilitySentenceString)
        {
            speechBubbleText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        pDAbilityContinueButton.SetActive(true);//replace with power up tutorial panel continue button
    }



    IEnumerator AnimateFlashButton(GameObject button, AbilityTutorialStepData step)
    {
        float flashButtonTime = .5f;
        currentAbilityTutorialData.buttonColor = button.GetComponent<Image>().color;
        if (button != null || !step.clicked)
        {


            while (!step.clicked && step.stepType == TutorialStepType.FlashButton)
            {

                button.GetComponent<Image>().color = tutorialPanelColor;
                yield return new WaitForSeconds(flashButtonTime);
                button.GetComponent<Image>().color = currentAbilityTutorialData.buttonColor;
                yield return new WaitForSeconds(flashButtonTime);

            }
        }
        else
        {
            Debug.Log("Button set to normal");
        }

    }

}
