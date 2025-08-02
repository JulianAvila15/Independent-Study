using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class IntroProgressiveDisclosureHandler : MonoBehaviour
{
 [SerializeField]   ManagerofManagers managerHub;

    public string[] sentences;
    public int sentenceIndex, abilitySentenceIndex;
    public float typingSpeed;
 
    public Image[] slotBGs;
    public bool isForSummon;
    public int introTutorialCallOutNumber = 0;
    Color tutorialPanelColor;
    public GameObject pdCraftButton;
    public GameObject[] introTutorialCallOuts, introPDVisualCues;
    public int visualCueNum = 0;
    static public bool introPDTutorialFinished = false;
    [SerializeField] private int[] showIntroTutorialSpeechBubbleAtSentenceIndex;
    public int progressiveDisclosureStep = 0;
    public int highlightedSlotIndex_ProgressiveDisclosureTutorial = 0; //for the tutorial, it specifies the index of the ingredient in the order that needs to be satisfied 
    Text callOutText;



    public enum PDStepsIntro
    {
        BeginCraftEvent = 3,
        DuringCraftEvent = 4,
        ClickCraftButtonEvent = 6,
        DescribeFeedBack = 7
    }


    public static PDStepsIntro progressive_DisclosureSteps;


    private void Awake()
    {

        if (managerHub != null && managerHub.introPDManager == null)
            managerHub.introPDManager = gameObject.GetComponent<IntroProgressiveDisclosureHandler>();
    }

    private void Start()
    {

        if (!managerHub.gameManager.inTestingMode)
        {
            if (managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure)
            {

                managerHub.tutorialManager.continueButton.gameObject.SetActive(false);
                isForSummon = false;
                StartCoroutine(Type());
            }

            tutorialPanelColor = managerHub.tutorialManager.tutorialPanel.GetComponent<Image>().color;
        }
        else
            introPDTutorialFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!managerHub.gameManager.inTestingMode && !introPDTutorialFinished) //if beginning tutorial is not completed yet
            HandlePDIntroSteps();


        if (!managerHub.craftingManager.craftButton.gameObject.activeInHierarchy)
            managerHub.craftingManager.craftButton.gameObject.SetActive(true);

    }

    private void HandlePDIntroSteps()
    {
        if (highlightedSlotIndex_ProgressiveDisclosureTutorial <= managerHub.craftingManager.numberOfCraftingSlots && !managerHub.tutorialManager.tutorialPanel.activeInHierarchy)//visual queues for progressive disclosure
        {
            visualCueNum = managerHub.craftingManager.ingredientsList.IndexOf(managerHub.orderManager.listOfOrder[highlightedSlotIndex_ProgressiveDisclosureTutorial]);
            introPDVisualCues[visualCueNum].gameObject.SetActive(true);
        }
        else
        {
            HideAllVisualCue();
        }

        if (progressiveDisclosureStep == (int)PDStepsIntro.DuringCraftEvent && !managerHub.tutorialManager.tutorialPanel.activeInHierarchy)
        {
            slotBGs[highlightedSlotIndex_ProgressiveDisclosureTutorial].color = new Color(255, 255, 0);
        }
        else
        {
            slotBGs[highlightedSlotIndex_ProgressiveDisclosureTutorial].color = new Color(255, 255, 255);
        }

        if (progressiveDisclosureStep == (int)PDStepsIntro.ClickCraftButtonEvent)
        {
          managerHub.tutorialManager.continueButton.gameObject.SetActive(false);
            pdCraftButton.SetActive(true);

           managerHub.tutorialManager.tutorialPanel.GetComponent<Image>().color = new Color(tutorialPanelColor.r, tutorialPanelColor.g, tutorialPanelColor.b, 0);
            introPDVisualCues[4].SetActive(true);
            introPDVisualCues[5].SetActive(true);


            if (managerHub.orderManager.currentOrderIndex == 1)
            {
                pdCraftButton.SetActive(false);
                managerHub.tutorialManager.tutorialPanel.GetComponent<Image>().color = tutorialPanelColor;
                introPDVisualCues[4].SetActive(false);
                introPDVisualCues[5].SetActive(false);

                NextSentence();

            }

        }
    }

    IEnumerator Type()
    {


    
        while (!managerHub.tutorialManager.tutorialPanel.activeInHierarchy && !introPDTutorialFinished)
        {
            yield return null;
        }
        //if (timeout <= 0f)
        //{
        //    debug.logwarning("tutorial panel never activated within timeout.");
        //    yield break;
        //}

        introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text = "";


        foreach (char letter in sentences[sentenceIndex])
        {
            introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>().text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        Debug.Log("before stopping coroutine");
        if (!managerHub.tutorialManager.tutorialPanel.activeInHierarchy && progressiveDisclosureStep != (int)PDStepsIntro.DescribeFeedBack&&progressiveDisclosureStep>1)
        {
            Debug.Log("stopping coroutine");
            StopCoroutine(Type());
        }

        if (progressiveDisclosureStep == (int)PDStepsIntro.ClickCraftButtonEvent)
        {
            CraftingManager.canClickCraftButton = true;
        }

        if (progressiveDisclosureStep < (int)PDStepsIntro.ClickCraftButtonEvent || progressiveDisclosureStep == (int)PDStepsIntro.DescribeFeedBack)
        {
            managerHub.tutorialManager.continueButton.gameObject.SetActive(true);
        }

    }

    public void NextSentence()//would be parameterized to include the specific number of sentences per speech bubble
    {
      managerHub.tutorialManager.continueButton.gameObject.SetActive(false);

        callOutText = introTutorialCallOuts[introTutorialCallOutNumber].GetComponentInChildren<Text>();

        if (!introPDTutorialFinished)//intro tutorial has not completed
        {


            //change speech bubble if the sentenceIndex of the sentence is a specific number
            if (showIntroTutorialSpeechBubbleAtSentenceIndex.Contains(progressiveDisclosureStep))
            {
                introTutorialCallOuts[introTutorialCallOutNumber].SetActive(false);

                if (progressiveDisclosureStep == (int)PDStepsIntro.BeginCraftEvent)
                {
                    managerHub.tutorialManager.tutorialPanel.SetActive(false);
                }
                introTutorialCallOutNumber++;
                introTutorialCallOuts[introTutorialCallOutNumber].SetActive(true);
            }

            if (sentenceIndex < sentences.Length - 1)//instead of sentences.length-1 it would only go up to how many sentences there would be per speech bubble
            {
                sentenceIndex++;

                callOutText.text  = "";
                progressiveDisclosureStep++;
                StartCoroutine(Type());
            }
            else
            {
                callOutText.text = "";//clear text
                managerHub.tutorialManager.tutorialPanel.gameObject.SetActive(false); //set tutorial panel unactive
                introPDTutorialFinished = true;//tutorial has finished so inform the time manager
                if (sentenceIndex < introTutorialCallOuts.Length)
                    sentenceIndex++;
                HideAllVisualCue();
                this.gameObject.SetActive(false);
            }
        }
    }

    public void HighlightIngredientandCorrespondingSlot()
    {
        if (highlightedSlotIndex_ProgressiveDisclosureTutorial < managerHub.craftingManager.numberOfCraftingSlots - 1 && managerHub.introPDManager.IntroPDCraftEventOccuring())//Guide player to complete the order till complete
        {
            managerHub.introPDManager.introPDVisualCues[managerHub.introPDManager.visualCueNum].SetActive(false);
            managerHub.introPDManager.slotBGs[highlightedSlotIndex_ProgressiveDisclosureTutorial].color = Color.white;//set color back to white
            highlightedSlotIndex_ProgressiveDisclosureTutorial++;
        }
        else if (!introPDTutorialFinished) //order is complete 
        {
            managerHub.tutorialManager.tutorialPanel.SetActive(true);
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

    public bool IntroPDCraftEventOccuring()
    {
        return progressiveDisclosureStep == (int)PDStepsIntro.DuringCraftEvent && managerHub.orderManager.listOfOrder[highlightedSlotIndex_ProgressiveDisclosureTutorial] == managerHub.craftingManager.currentItem;
    }

}





    