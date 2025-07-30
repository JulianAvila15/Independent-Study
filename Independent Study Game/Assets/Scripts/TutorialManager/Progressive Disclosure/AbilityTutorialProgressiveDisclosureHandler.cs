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
    private AbilityTutorialData currentAbilityTutorialData;//get the steps and data about the current ability's progressive disclosure data
    private int currentAbilityTutorialStepIndex; //get the current ability tutorial step index
    public List<string> completedTutorials = new List<string>();

    public Coroutine currentStepCoroutine;
   private AbilityTutorialStepData current_step;
    static public bool abilityTutorialTriggered = false;
    public CraftingManager craftingManager;
    public OrderManager orderMan;
    Color transparentColor, defaultPanelColor;
    public Color currentColor;

    public float typingSpeed;

    //UI
    public GameObject pdTutorialPanelAbility;
    public GameObject pDAbilityContinueButton;




    //Summon
    public SummonManager summonHandler;

    public MiniGamePDHandler miniGamePDHandler;
    public SummonPDTutorialHandler summonPDHandler;


    // Start is called before the first frame update
    void Start()
    {
        transparentColor = new Color(0, 0, 0, 0);
        defaultPanelColor = pdTutorialPanelAbility.GetComponent<Image>().color;



        //set sub ability handlers
        miniGamePDHandler = gameObject.GetComponent<MiniGamePDHandler>();
        summonPDHandler = gameObject.GetComponent<SummonPDTutorialHandler>();

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

    public string GetCurrentAbilityDataName()
    {
        return currentAbilityTutorialData.abilityName;
    }


    private void StartStep()
    {
 
        current_step = currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex]; 

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

        if (currentAbilityTutorialData.isSummon&&currentAbilityTutorialData.summon!=null)
            summonPDHandler.StartSummonPDTutorialStep(ref currentAbilityTutorialData.summon, currentAbilityTutorialData.slotIndex, current_step.needSummon);
        else
            miniGamePDHandler.MiniGameStartStep(current_step, currentAbilityTutorialData.abilityName);

        switch (current_step.stepType)
        {
            case TutorialStepType.ShowText:
                //show text and display speech bubble
                ShowText(ref current_step);
                break;
            case TutorialStepType.FlashButton:
                current_step.clicked = false;
                FlashButton(ref current_step);
                break;
            case TutorialStepType.SummonedAppeared:
                //Summon appeared
                //StartCoroutine(AssignSummonReference());
                currentAbilityTutorialData.summon = summonHandler.createdObject.GetComponent<Helper>();
               summonPDHandler.SummonAppeared(currentAbilityTutorialData.summon);
                break;
            case TutorialStepType.IngredientDropped:
                //Ingredient Dropped
               summonPDHandler.IngredientDropped();
                break;
            case TutorialStepType.ReplaceIngredient:
              summonPDHandler.ReplaceIngredient();
                break;
            case TutorialStepType.WatchSummon:
                summonPDHandler.WatchSummon();
                break;
            case TutorialStepType.ResetSummon:
               summonPDHandler.ResetSummon();
                break;
            case TutorialStepType.waitForWalkRight:
                //wait for player to walk right
              miniGamePDHandler.WaitForWalkRightOrLeft(true);
                break;
            case TutorialStepType.waitForWalkLeft:
                //wait for player to walk right
            miniGamePDHandler.WaitForWalkRightOrLeft(false);
                break;
            case TutorialStepType.waitForJump:
              miniGamePDHandler.WaitForJump();
                break;
            case TutorialStepType.spawnCoin:
             miniGamePDHandler.SpawnCoin();
                break;
            case TutorialStepType.demonstrateLosing:
              miniGamePDHandler.DemonstrateLosing();
                break;
            case TutorialStepType.collectingMiniGameStart:
                miniGamePDHandler.CollectingMiniGameStart();
                break;



                // Add others as needed
        }

    }

    //General ability pd handler functions


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

        step.callOut.GetComponentInChildren<Text>().text = "";

        currentStepCoroutine = StartCoroutine(TypeForAbility(step.callOut, step.sentence));
    }

    public void OnAbilityButtonClicked()
    {
        var step = currentAbilityTutorialData.steps[currentAbilityTutorialStepIndex];

        if (step.stepType != TutorialStepType.FlashButton)
            return; // Only handle click if it's on a FlashButton step

        if (step.clicked)
            return; // Prevent multiple clicks

        step.clicked = true;

        //  // If summon tutorial and summon is needed for a later step
        if (currentAbilityTutorialData != null && summonHandler.createdObject != null)
        {
            Helper helper = summonHandler.createdObject.GetComponent<Helper>();
          summonPDHandler.SetSummon(ref helper);
        }
        
        AdvanceAbilityStep();
    }



    public bool GetButtonIsClicked()
    {
        return current_step.clicked;
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

    public bool CheckIfCoolDownEnabled()
    {
        if (current_step != null)
            return current_step.turnOnCoolDown;

        return false;
    }


    //general end functions
    private void EndShowTextStep()
    {
        current_step.callOut.SetActive(false);
        pDAbilityContinueButton.SetActive(false);

    }



    private void EndFlashButtonStep()
    {
        current_step.clicked = true;
        current_step.targetObject.GetComponent<Image>().color = currentAbilityTutorialData.buttonColor;
    }

    IEnumerator TypeForAbility(GameObject speechBubble, string abilitySentenceString)
    {

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
    }

    public IEnumerator WaitUntilWithTimeout(Func<bool> condition, float timeoutSeconds, Action onTimeout = null)
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
                case TutorialStepType.IngredientDropped:
                    //Ingredient Dropped 
                   summonPDHandler.EndIngredientDropped();
                    break;
                case TutorialStepType.waitForWalkLeft:
                    //wait for player to walk right
                    miniGamePDHandler.EndWaitForWalkRightOrLeft(true);
                    break;
                case TutorialStepType.waitForWalkRight:
                    //wait for player to walk right
                    miniGamePDHandler.EndWaitForWalkRightOrLeft(false);
                    break;
                case TutorialStepType.waitForJump:
                    miniGamePDHandler.EndWaitForJump();
                    break;
                case TutorialStepType.spawnCoin:
                    miniGamePDHandler.EndSpawnCoin();
                    break;
                case TutorialStepType.collectingMiniGameStart:
                    miniGamePDHandler.EndCollectingMiniGame();
                    break;
                case TutorialStepType.demonstrateLosing:
                    miniGamePDHandler.EndDemonstratingLosing();
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
            if (miniGamePDHandler.fadeToBlackCoroutine != null)
            {
                StopCoroutine(miniGamePDHandler.fadeToBlackCoroutine);
                miniGamePDHandler.fadeToBlackCoroutine = null;
            }

        }
    }




    private void EndAbilityTutorial()
    {
        //add logic to disable panel
        current_step.clicked = true;

        if (!HasCompletedTutorial(currentAbilityTutorialData.abilityName))
            completedTutorials.Add(currentAbilityTutorialData.abilityName);

        currentAbilityTutorialData = null;
        currentAbilityTutorialStepIndex = 0;


        if (pdTutorialPanelAbility.activeInHierarchy)
            pdTutorialPanelAbility.SetActive(false);

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



}

