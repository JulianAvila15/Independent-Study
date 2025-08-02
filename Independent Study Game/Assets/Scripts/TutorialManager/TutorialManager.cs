using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    [SerializeField]ManagerofManagers managerHub;
    public Button continueButton,powerUpButton,collectingGameButton;
    public GameObject tutorialPanel,tutorialPowerUp,tutorialCollectingGame,wallOfTextInstructions;
    public TMP_Text textbox, powerUpTextBox;
     [SerializeField] private IntroProgressiveDisclosureHandler introProgressiveDisclosure;
    public Image summonSprite;
    public static bool penguinTutorialShown = false, adventurerTutorialShown = false, dragonTutorialShown = false, coinTutorialShown = false, timingMiniGameTutorialShown = false;
    public Image[] summonSpriteImages = new Image[4];
    public Sprite wallofTextBG;
    public GameObject craftButton;
    public int  numOfSentences;//integers for call out numbers and the number of sentences per call out
   public AbilityTutorialProgressiveDisclosureHandler abilityPDHandler;

    private void Awake()
    {
        if (managerHub != null && managerHub.tutorialManager == null)
            managerHub.tutorialManager = gameObject.GetComponent<TutorialManager>();
    }


    // Start is called before the first frame update
    void Start()
    {

    


            switch (managerHub.gameManager.tutorialType)
            {
                case GameManager.TutorialType.wallOfText:

                if (!managerHub.gameManager.inTestingMode)
                    tutorialPanel.SetActive(true);

                    tutorialPanel.GetComponent<Image>().color = new Color(255, 255, 255);
                    tutorialPanel.GetComponent<Image>().sprite = wallofTextBG;
                    wallOfTextInstructions.SetActive(true);

                    break;
                case GameManager.TutorialType.progressiveDisclosure:

                if (!managerHub.gameManager.inTestingMode)
                    tutorialPanel.SetActive(true);

                    introProgressiveDisclosure.gameObject.SetActive(true);
                    introProgressiveDisclosure.introTutorialCallOuts[introProgressiveDisclosure.introTutorialCallOutNumber].SetActive(true);
                    break;
                case GameManager.TutorialType.noTutorial:
                    tutorialPanel.gameObject.SetActive(false);
                    break;
            }
       
    }


    public void SetSummonTutorial(string powerUpText,int spriteIndex,int abilityDataIndex)
    {

        if (managerHub.gameManager.tutorialType == GameManager.TutorialType.wallOfText)
        {
            powerUpTextBox.text = powerUpText;
            tutorialPowerUp.SetActive(true);
        }

        else
        {
            //refer to a function in the progressive disclosure handler that sets the ability using an enum in the progressive disclosure handler
            abilityPDHandler.SetCurrentAbilityData(abilityDataIndex);
        }

        if (summonSpriteImages[spriteIndex]!=null)
        summonSprite.sprite = summonSpriteImages[spriteIndex].sprite;

        
       

    }

    public void ContinueButtonIsPressed()
    {

        switch (managerHub.gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPanel.gameObject.SetActive(false);
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                //call some function that handles the progressive disclosure
                introProgressiveDisclosure.GetComponent<IntroProgressiveDisclosureHandler>().NextSentence();
                break;
            case GameManager.TutorialType.noTutorial:
                break;
        }


    }

    public void PowerUpContinueButtonIsPressed()
    {

        switch (managerHub.gameManager.tutorialType)
        {
           
            case GameManager.TutorialType.wallOfText:
                tutorialPowerUp.gameObject.SetActive(false);
                managerHub.timeManager.ResetAFKTimer();
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                //add logic to select the certain index of the corresponding ability
                abilityPDHandler.AdvanceAbilityStep();
                break;
            case GameManager.TutorialType.noTutorial:
                break;


        }


    }

   public bool NoPDTutorialOccuring()
    {
       return  IntroProgressiveDisclosureHandler.introPDTutorialFinished && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered;
    }
    
    public bool IntroTutorialOccuring()
    {
        return managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure && !IntroProgressiveDisclosureHandler.introPDTutorialFinished;
    }

    public bool AbilityPDTutorialOccuring()
    {
      return  managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered;
    }

}
