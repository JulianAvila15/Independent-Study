using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class AbilityTutorialProgressiveDisclosureHandler : MonoBehaviour
{


    [SerializeField] private AbilityTutorialData[] abilityTutorialDataArray;
    public AbilityTutorialData currentAbilityTutorialData;//get the steps and data about the current ability's progressive disclosure data
    private int currentAbilityTutorialStepIndex; //get the current ability tutorial step index
    public List<string> completedTutorials = new List<string>();

    private Coroutine currentStepCoroutine;

    public Abilities currentAbilityUsed;

    AbilityTutorialStepData current_step;

    static public bool abilityTutorialTriggered = false;

    public GameObject pdTutorialPanelAbility;

    public GameObject pDAbilityContinueButton;

    public CraftingManager craftingManager;

    public OrderManager orderMan;

    public GameObject[] pdVisualCues;
    public GameObject enabledpdVisualCue;
    int visualCueNum;
    Color transparentColor,defaultPanelColor,currentColor;
    public float typingSpeed;

    public Spawner coinSpawner;

    public GameObject collectingGame;
      public GameObject leftCollectingTrigger, rightCollectingTrigger;

    public bool hasCollidedwithCollectingTrigger = false;
    [SerializeField] private Player playerObj;

    bool collectorHasJumped;

    public bool coinHasSpawned = false, coinCanMove = false;

    Coin spawnedCoin;

    public bool coinMoveSlow = false;

   private Coroutine waitForLanding = null;

  [SerializeField]  private Image leftDirectionArrow, rightDirectionArrow,upDiectionArrow,spaceBarImage,aKeyImage,dKeyImage;

    [SerializeField] private GameObject timingPauseButton;

    Coroutine coinSpawnCoroutine,fadeToBlackCoroutine;

    Vector3 playerPositionDuringDemo;

    public GameObject triggerWarp;

    public GameObject penguinIngredientArrow;


    int ingredientOffSetVisualCue = 5;

    // Start is called before the first frame update
    void Start()
    {
        transparentColor = new Color(0, 0, 0, 0);
        defaultPanelColor = pdTutorialPanelAbility.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    //PD ability functions
    public void SetCurrentAbilityData(int currentAbilityIndex)
    {

        var candidate = abilityTutorialDataArray[currentAbilityIndex];
        if (candidate.steps.Count > 0)
        {
            Debug.Log(abilityTutorialTriggered);
            TriggerAbilityTutorial(candidate);
        }
    }

    public void TriggerAbilityTutorial(AbilityTutorialData triggeredAbility)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);


        if (!HasCompletedTutorial(triggeredAbility.abilityName))
        {
            abilityTutorialTriggered = true;
            currentAbilityTutorialData = triggeredAbility;
            currentAbilityTutorialStepIndex = 0;
            gameObject.SetActive(true);
            StartStep();
        }
        else
            abilityTutorialTriggered = false;

    }

    public TutorialStepType GetStepTutorialType()
    {
        return current_step.stepType;
    }

    private void StartStep()
    {
        var step = currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex];
        current_step = step;

        if (current_step.canClickThroughPanel)
        {
            pdTutorialPanelAbility.GetComponent<Image>().raycastTarget = false;
          currentColor = pdTutorialPanelAbility.GetComponent<Image>().color = transparentColor;
        }
        else
        {
            pdTutorialPanelAbility.GetComponent<Image>().raycastTarget = true;
            currentColor = pdTutorialPanelAbility.GetComponent<Image>().color = defaultPanelColor;
        }


        if (currentAbilityTutorialData.abilityName == "Collecting")//if can click through panel;
        {
            playerObj.playerCanMove = current_step.playerCanMove;
            playerObj.playerCanJump = current_step.playerCanJump;

            coinSpawner.canStartSpawningInTutorial = current_step.canStartSpawningCoin;

                coinCanMove = current_step.coinCanStartMoving;
                coinMoveSlow= current_step.coinMoveSlowly;

        }


        if (currentAbilityTutorialData.abilityName == "Timing")//if can click through panel;
        {
                timingPauseButton.SetActive(current_step.showStopButton);
        }

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
            case TutorialStepType.SummonedAppeared:
                //Summon appeared
                //StartCoroutine(AssignSummonReference());
                currentAbilityTutorialData.summon = currentAbilityUsed.createdObject.GetComponent<Helper>();
                SummonAppeared(currentAbilityTutorialData.summon);
                break;
            case TutorialStepType.IngredientDropped:
                //Ingredient Dropped
                IngredientDropped();
                break;
            case TutorialStepType.ReplaceIngredient:
                ReplaceIngredient();
                break;
            case TutorialStepType.WatchSummon:
                currentStepCoroutine = StartCoroutine(WaitForSummonToGetDestroyed());
                break;
            case TutorialStepType.ResetSummon:
                ResetSummon();
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
            case TutorialStepType.spawnCoin:
                SpawnCoin();
                break;
            case TutorialStepType.demonstrateLosing:
                DemonstrateLosing();
                break;
            case TutorialStepType.miniGameStart:
                coinSpawnCoroutine = StartCoroutine(coinSpawner.SpawnCoin());
                break;



                // Add others as needed
        }

    }

    private void HighlightSlot()
    {
        visualCueNum = craftingManager.ingredientsList.IndexOf(craftingManager.orderManager.listOfOrder[CraftingManager.penguinSlot]);


        if (currentAbilityTutorialData.abilityName=="Penguin")
        {
            pdVisualCues[visualCueNum%ingredientOffSetVisualCue].SetActive(true);
            penguinIngredientArrow.SetActive(true);
        }

         
    }

    private void DemonstrateLosing()
    {

        int bringUpPlayerBy = 6;

        if (fadeToBlackCoroutine == null)
            fadeToBlackCoroutine = StartCoroutine(FadeToBlackCoroutine());

        playerPositionDuringDemo = new Vector3(-23.6000004f, playerObj.transform.position.y + bringUpPlayerBy, playerObj.transform.position.z);
        triggerWarp.SetActive(true);
        triggerWarp.transform.position = playerPositionDuringDemo;
        playerObj.transform.position = playerPositionDuringDemo;
    }

    private void SpawnCoin()
    {
        
        currentStepCoroutine = StartCoroutine(WaitForCoinToSpawn());
       coinSpawnCoroutine =  StartCoroutine(coinSpawner.SpawnCoin());
    }

    public void CheckIfCanContinueAfterCollectingCoin()
    {

        if (playerObj.m_Grounded)
        {
            if (waitForLanding != null)
                StopCoroutine(waitForLanding);

            if ((current_step.stepType == TutorialStepType.demonstrateCollecting || current_step.stepType == TutorialStepType.demonstrateLosing))
                AdvanceAbilityStep();
        }
        else
        {
            waitForLanding = StartCoroutine(playerObj.WaitForLanding());
        }
    }

    private void ResetSummon()
    {
        if (currentAbilityTutorialData.summon != null)
        {
            switch (currentAbilityTutorialData.abilityName)
            {
                case "Penguin":
                    craftingManager.penguinItemSuccessfullyDropped = false;
                    break;
                case "Messenger":
                    currentAbilityTutorialData.summon.selectedTile.SetActive(true);
                    break;
                default:
                    break;

            }

            currentAbilityTutorialData.summon.helperTransform.position = currentAbilityTutorialData.summon.originPosition;

            if (currentAbilityTutorialData.abilityName == "Dragon")
                currentAbilityTutorialData.summon.helperTransform.position += new Vector3(0, .8f, 0);
        }

        AdvanceAbilityStep();
    }

    private void ReplaceIngredient()
    {
        Item randomItem = orderMan.listOfOrder[UnityEngine.Random.Range(0, orderMan.listOfOrder.Count)];

        Slot selectedSlotForReplacement = craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex];

        Debug.Log("trying to replace ingredient");

        int iterations = 1;

        if (currentAbilityTutorialData.abilityName == "Dragon")
            iterations = 2;

        for (int it = 0; it < iterations; it++)
        {
            if (currentAbilityTutorialData.abilityName == "Penguin")
                Helper.penguinHasShownIngredient = false;

            while (randomItem.itemName == orderMan.listOfOrder[currentAbilityTutorialData.slotIndex+it].itemName)
            {
                randomItem = craftingManager.ingredientsList[UnityEngine.Random.Range(0, orderMan.numOfIngredientsAvailable)];

            }
            Debug.Log("trying to replace ingredient");
            selectedSlotForReplacement.item = randomItem;
            selectedSlotForReplacement.GetComponent<Image>().sprite = randomItem.GetComponent<Image>().sprite;
            Debug.Log(craftingManager.craftingSlots[currentAbilityTutorialData.slotIndex].item.itemName);

        }


        AdvanceAbilityStep();

    }

    public bool NeedSummon()
    {
        return current_step.needSummon;
    }

    public bool CheckIfCoolDownEnabled()
    {
        if (current_step != null)
            return current_step.turnOnCoolDown;

        return false;
    }

  
    private void WaitForJump()
    {
        //logic that is executed when the player jumps x number of times
        currentStepCoroutine = StartCoroutine(WaitForJumpCoroutine());

        if(fadeToBlackCoroutine==null)
       fadeToBlackCoroutine =  StartCoroutine(FadeToBlackCoroutine());
        upDiectionArrow.gameObject.SetActive(true);
        spaceBarImage.gameObject.SetActive(true);


        Vector3 playerCurrentPosition = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z);

        playerObj.transform.position = new Vector3(playerObj.playerSpawnPosition.x, playerCurrentPosition.y,playerCurrentPosition.z);

        playerObj.canMoveLeft=playerObj.canMoveRight=false;
     
    }

    public void OnAbilityButtonClicked()
    {
        var step = currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex];

        if (step.stepType != TutorialStepType.FlashButton)
            return; // Only handle click if it's on a FlashButton step

        if (step.clicked)
            return; // Prevent multiple clicks

        step.clicked = true;

        // If summon is needed for a later step
        if (currentAbilityTutorialData != null && currentAbilityUsed.createdObject != null)
        {
            Helper helper = currentAbilityUsed.createdObject.GetComponent<Helper>();
            SetSummon(ref helper);
        }

        AdvanceAbilityStep();
    }

    private void WaitForWalkRightOrLeft(bool walkLeftorWalkRight) //temp argument to signify if the player has walked x number of pixels right or left
    {
        //logic that is executed when the player walks x number to the right

        if(walkLeftorWalkRight)//walk right
        {
            rightCollectingTrigger.SetActive(true);
            rightDirectionArrow.gameObject.SetActive(true);
            playerObj.canMoveLeft = false;
            dKeyImage.gameObject.SetActive(true);
        }
        else //walk left
        {
            leftDirectionArrow.gameObject.SetActive(true);
            leftCollectingTrigger.SetActive(true);
            playerObj.canMoveRight = false;
            aKeyImage.gameObject.SetActive(true);
        }

        currentStepCoroutine = StartCoroutine(WaitToHitTrigger());


    }

    private void IngredientDropped()
    {
        HighlightSlot();
        if (currentAbilityTutorialData.abilityName == "Penguin")
            craftingManager.firstTimeUsePenguin = true;

        craftingManager.nextButton.interactable = craftingManager.prevButton.interactable = false;


        //Ingredient Dropped logic
    }

    private void SummonAppeared(Helper summon)
    {
        StartCoroutine(WaitForSummonToAppear(summon));

    }

    public void SetHasJumped()
    {
        if(!collectorHasJumped)
        collectorHasJumped = true;
    }

    IEnumerator WaitForSummonToAppear(Helper summonHelper)
    {
        yield return StartCoroutine(
       WaitUntilWithTimeout(() => summonHelper.summonedReachedPDTutorialDestination, 5f,
           () => Debug.LogWarning("Summon didn't reach destination in time. Skipping step.")));
        AdvanceAbilityStep();
    }

    IEnumerator WaitForJumpCoroutine()
    {
        yield return StartCoroutine(
      WaitUntilWithTimeout(() => collectorHasJumped, 15f,
          () => Debug.LogWarning("Summon didn't reach destination in time. Skipping step.")));
        AdvanceAbilityStep();
    }

    IEnumerator WaitForSummonToGetDestroyed()
    {
        Debug.Log("starting summon destroy coroutine");
        yield return StartCoroutine(
     WaitUntilWithTimeout(() => !CraftingManager.helperIsActive, 5f,
         () => Debug.LogWarning("Still waiting on helper to be destroyed.")));
        Debug.Log("ending summon destroy coroutine");
        AdvanceAbilityStep();
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        pdTutorialPanelAbility.GetComponent<Image>().color = Color.black;

        foreach (Transform child in pdTutorialPanelAbility.transform)
        {
            child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(.3f);

        foreach (Transform child in pdTutorialPanelAbility.transform)
        {
            child.gameObject.SetActive(true);
        }

        pdTutorialPanelAbility.GetComponent<Image>().color = currentColor;
    }

    private IEnumerator WaitToHitTrigger()
    {
        yield return StartCoroutine(
     WaitUntilWithTimeout(() => hasCollidedwithCollectingTrigger, 15f,
         () => Debug.LogWarning("Still waiting on helper to be destroyed.")));
        Debug.Log("ending summon destroy coroutine");

        AdvanceAbilityStep();
    }

    IEnumerator WaitForCoinToSpawn()
    {
        yield return StartCoroutine(
       WaitUntilWithTimeout(() => coinHasSpawned, 10f,
           () => Debug.LogWarning("Summon didn't reach destination in time. Skipping step.")));
        AdvanceAbilityStep();
    }
    public void SetSummon(ref Helper summon)
    {
        currentAbilityTutorialData.summon = summon;
    }

    private void FlashButton(ref AbilityTutorialStepData step)
    {

        pdTutorialPanelAbility.SetActive(true);
        pDAbilityContinueButton.SetActive(false);
        currentStepCoroutine = StartCoroutine(AnimateFlashButton(step.targetObject, step));
    }

    private void ShowText(ref AbilityTutorialStepData step)
    {

        if (!pdTutorialPanelAbility.activeInHierarchy)
        {
            pdTutorialPanelAbility.SetActive(true);
            pDAbilityContinueButton.SetActive(false);
        }

        if (!current_step.callOut.activeInHierarchy)
            current_step.callOut.SetActive(true);

        Debug.Log(step.sentence);

        step.callOut.GetComponentInChildren<Text>().text = "";

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
        if (currentAbilityTutorialStepIndex < currentAbilityTutorialData.steps.Count - 1)
        {

            switch (current_step.stepType)
            {
                case TutorialStepType.ShowText:
                    EndShowTextStep();
                    break;
                case TutorialStepType.FlashButton:
                    EndFlashButtonStep();
                    break;
                case TutorialStepType.SummonedAppeared:
                    //Summon appeared
                    EndSummonAppeared();
                    break;
                case TutorialStepType.IngredientDropped:
                    //Ingredient Dropped 
                    EndIngredientDropped();
                    break;
                case TutorialStepType.waitForWalkLeft:
                    //wait for player to walk right
                    EndWaitForWalkRightOrLeft(true);
                    break;
                case TutorialStepType.waitForWalkRight:
                    //wait for player to walk right
                    EndWaitForWalkRightOrLeft(false);
                    break;
                case TutorialStepType.waitForJump:
                    EndWaitForJump();
                    break;
                case TutorialStepType.spawnCoin:
                    EndSpawnCoin();
                    break;
                case TutorialStepType.miniGameStart:
                    if (coinSpawnCoroutine != null)
                        StopCoroutine(coinSpawnCoroutine);
                    break;
                case TutorialStepType.demonstrateLosing:
                    triggerWarp.transform.position = playerObj.playerSpawnPosition;
                    break;
            }

            if (currentAbilityTutorialData.summon != null)
            {

                if (current_step.summonCanContinue)
                {
                    currentAbilityTutorialData.summon.summonedCanContinuePDTutorial = true;
                }
                else if (currentAbilityTutorialData.summon != null)
                    currentAbilityTutorialData.summon.summonedCanContinuePDTutorial = false;
            }

            if (currentStepCoroutine != null)
            {

                StopCoroutine(currentStepCoroutine);
                currentStepCoroutine = null;
            }
            if (fadeToBlackCoroutine != null)
            {
                StopCoroutine(fadeToBlackCoroutine);
                fadeToBlackCoroutine = null;
            }
         }
    }

    private void EndHighlightSlot()
    {
        
    }

    private void EndSpawnCoin()
    {
        coinHasSpawned = true;
        coinSpawner.canStartSpawningInTutorial = false;

        if (coinSpawnCoroutine != null)
            StopCoroutine(coinSpawnCoroutine);

    }

    private void EndWaitForStopOnGreen()
    {
        //End WaitForStopOnGreen step
    }

    private void EndWaitForJump()
    {
        //End WaitForJump step
        spaceBarImage.gameObject.SetActive(false);
        upDiectionArrow.gameObject.SetActive(false);
        playerObj.canMoveLeft = playerObj.canMoveRight = true;
    }

    private void EndWaitForWalkRightOrLeft(bool leftOrRight)
    {
        //End WaitForWalkRIghtOrLeft step

        if (leftOrRight) //if directing to move left
        {
            leftCollectingTrigger.SetActive(false);
            playerObj.canMoveRight = true;
            leftDirectionArrow.gameObject.SetActive(false);
            aKeyImage.gameObject.SetActive(false);

        }
        if (!leftOrRight) //if directing to move right
        {
            rightCollectingTrigger.SetActive(false);
            playerObj.canMoveLeft = true;
            rightDirectionArrow.gameObject.SetActive(false);
            dKeyImage.gameObject.SetActive(false);

        }

           

        hasCollidedwithCollectingTrigger = false;
    }

    private void EndIngredientDropped()
    {
        //End IngredientDropped step

        if (currentAbilityTutorialData.abilityName == "Penguin")
        {
            pdVisualCues[visualCueNum%ingredientOffSetVisualCue].SetActive(false);
            penguinIngredientArrow.SetActive(false);
            craftingManager.nextButton.interactable = craftingManager.prevButton.interactable = true;
        }
    }

    private void EndSummonAppeared()
    {
        //End SummonAppeared step
        Debug.Log("summon appeared");
    }


    private void EndFlashButtonStep()
    { 
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].clicked = true;
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].targetObject.GetComponent<Image>().color = currentAbilityTutorialData.buttonColor;
    }

    private void EndShowTextStep()
    {
        current_step.callOut.SetActive(false);
        pDAbilityContinueButton.SetActive(false);

    }

    private void EndAbilityTutorial()
    {
        //add logic to disable panel
        currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex].clicked = true;

        if(!HasCompletedTutorial(currentAbilityTutorialData.abilityName))
        completedTutorials.Add(currentAbilityTutorialData.abilityName);

        Debug.Log("ability tutorial end");
        currentAbilityTutorialData = null;
        currentAbilityTutorialStepIndex = 0;
        if (pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.SetActive(false);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        if (pDAbilityContinueButton.activeInHierarchy)
            pDAbilityContinueButton.SetActive(false);

        

        abilityTutorialTriggered = false;

        current_step.callOut.SetActive(false);
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

        if (pDAbilityContinueButton.activeInHierarchy == false)
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

                button.GetComponent<Image>().color = transparentColor;
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

    IEnumerator WaitUntilWithTimeout(Func<bool> condition, float timeoutSeconds, Action onTimeout = null)
    {
        float timer = 0f;
        while (!condition() && timer < timeoutSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!condition() && onTimeout != null)
            onTimeout();
    }

}

