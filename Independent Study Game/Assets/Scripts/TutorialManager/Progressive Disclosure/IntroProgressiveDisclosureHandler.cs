using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class IntroProgressiveDisclosureHandler : MonoBehaviour
{
    public string[] sentences;
    public int sentenceIndex, abilitySentenceIndex;
    public float typingSpeed;
    public TutorialManager tutorialManager;
    public GameManager gameManager;
    public CraftingManager craftingManager;
    public GameObject[] slotBGs;
    public bool isForSummon;
    public int introTutorialCallOutNumber = 0;
    Color tutorialPanelColor;
    public GameObject pdCraftButton;
    public GameObject[] introTutorialCallOuts, introPDVisualCues;
    public int visualCueNum = 0;
    static public bool introPDTutorialFinished = false;
    [SerializeField] private int[] showIntroTutorialSpeechBubbleAtSentenceIndex;
    public int progressiveDisclosureStep = 0;

    public OrderManager orderMan;

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
            introPDTutorialFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.inTestingMode && !introPDTutorialFinished) //if beginning tutorial is not completed yet
            HandlePDIntroSteps();


        if (!craftingManager.craftButton.gameObject.activeInHierarchy)
            craftingManager.craftButton.gameObject.SetActive(true);

    }

    private void HandlePDIntroSteps()
    {
        if (gameManager.craftingManager.highlightedSlotIndex_ProgressiveDisclosureTutorial <= CraftingManager.numberOfCraftingSlots && !tutorialManager.tutorialPanel.activeInHierarchy)//visual queues for progressive disclosure
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

    IEnumerator Type()
    {


    
        while (!tutorialManager.tutorialPanel.activeInHierarchy && !introPDTutorialFinished)
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
        if (!tutorialManager.tutorialPanel.activeInHierarchy && progressiveDisclosureStep != (int)PDStepsIntro.DescribeFeedBack&&progressiveDisclosureStep>1)
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
}





    