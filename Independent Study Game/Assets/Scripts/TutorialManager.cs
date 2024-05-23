using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    public GameManager gameManager;
    public Button continueButton,powerUpButton,collectingGameButton;
    public GameObject tutorialPanel,tutorialPowerUp,tutorialCollectingGame;
    public TMP_Text textbox,powerUpTextBox;
    public TimeManager timeManager;
    public ProgressiveDisclosureHandler progressiveDisclosure;
    public Image summonSprite;
    public static bool penguinTutorialShown = false, adventurerTutorialShown = false, dragonTutorialShown = false, coinTutorialShown = false, timingMiniGameShown = false;
    public Image[] summonSpriteImages = new Image[4];

    // Start is called before the first frame update
    void Start()
    {
        switch (gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPanel.SetActive(true);
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                tutorialPanel.SetActive(true);
                progressiveDisclosure.gameObject.SetActive(true);
                
                break;
            case GameManager.TutorialType.noTutorial:
                tutorialPanel.gameObject.SetActive(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If tutorial is set to something in the manager indicate which version

       
    }

    public void SetSummonTutorial(string powerUpText,int spriteIndex,string[] sentences)
    {
       
        if (gameManager.tutorialType == GameManager.TutorialType.wallOfText)
            powerUpTextBox.text = powerUpText;
        else
        {
            progressiveDisclosure.summonSentences = sentences;
            progressiveDisclosure.isForSummon = true;
            progressiveDisclosure.index = 0;
            progressiveDisclosure.gameObject.SetActive(true);
        }

        if (summonSpriteImages[spriteIndex]!=null)
        summonSprite.sprite = summonSpriteImages[spriteIndex].sprite;

        
        tutorialPowerUp.SetActive(true);

    }

    public void ContinueButtonIsPressed()
    {

        switch(gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPanel.gameObject.SetActive(false);
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                //call some function that handles the progressive disclosure
                progressiveDisclosure.GetComponent<ProgressiveDisclosureHandler>().NextSentence();
                break;
            case GameManager.TutorialType.noTutorial:
                break;

               
        }


    }

    public void PowerUpContinueButtonIsPressed()
    {

        switch (gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPowerUp.gameObject.SetActive(false);
                TimeManager.ResetAFKTimer();
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                //call some function that handles the progressive disclosure
                progressiveDisclosure.GetComponent<ProgressiveDisclosureHandler>().NextSentencePowerUp();
                break;
            case GameManager.TutorialType.noTutorial:
                break;


        }


    }




    //potential progressive disclosure function
}
