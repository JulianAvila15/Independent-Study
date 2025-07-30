using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{

   public static bool pause = false;
    public OrderManager orderManager;
    public CraftingManager craftingManager;

    public TimeManager timeManager;
    public bool timeCompleted = false;

    public TutorialManager tutorialManager;
    public Image pausePanel;
    public Button pauseButton,continueButton;
    public Canvas mainCanvas,workIDCanvas,bgCanvas;
    public GameObject inputText,placeHolder,validatorText, errorMessageText;
    string workerID;
    public GameObject errorMessagePanel;
    public DataMiner dataMiner;
    public bool isPausedByFocusLoss = false;
    [SerializeField] public bool inTestingMode;

    public static bool gameOver = false;

    [SerializeField] private GameObject collectingGame, endGameScreen, mainGame;
    public enum ProgressFeedbackType
    {
        progressBar,
        score,
        noScoreOrProgressBar
    }

   public enum TutorialType
    {
        progressiveDisclosure,
        wallOfText,
        noTutorial
    }

    



    public ProgressFeedbackType progressType;
    public TutorialType tutorialType;
    // Start is called before the first frame update
    void Start()
    {
        
        pausePanel.gameObject.SetActive(false);
        StartorStopGame(false);
        timeManager.enabled = true;
        errorMessagePanel.SetActive(false);

        switch (tutorialType)
        {
            case TutorialType.noTutorial:
                DataMiner.tutorialMode = "No Tutorial";
                break;
            case TutorialType.progressiveDisclosure:
                DataMiner.tutorialMode = "Progressive Disclosure";
                break;
            case TutorialType.wallOfText:
                DataMiner.tutorialMode = "Wall of Text";
                break;
            

        }

        switch (progressType)
        {
            case ProgressFeedbackType.noScoreOrProgressBar:
                DataMiner.progressFeedbackMode = "No Score or Progress Bar";
                break;
            case ProgressFeedbackType.progressBar:
                DataMiner.progressFeedbackMode = "Progress Bar";
                break;
            case ProgressFeedbackType.score:
                DataMiner.progressFeedbackMode = "Score";
                break;


        }

        


    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.P) && workIDCanvas.gameObject.activeInHierarchy == false) //Alternative way instead of using 
            Pause();


        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Alternative way instead of using button
        {
            EnterWorkID();
        }

        if (pausePanel.gameObject.activeInHierarchy || orderManager.newLevelProgressed.activeInHierarchy || tutorialManager.tutorialPanel.activeInHierarchy || tutorialManager.tutorialPowerUp.activeInHierarchy)
        {
            pauseButton.gameObject.SetActive(false);
        }
        else
        {
            pauseButton.gameObject.SetActive(true);
        }

        if (timeCompleted && orderManager.currLevel >= 5)
            EndGame();
    }

    public void Pause()//Pauses game
    {
      if(!(orderManager.newLevelProgressed.activeInHierarchy || workIDCanvas.gameObject.activeInHierarchy||endGameScreen.activeInHierarchy))
        {
            pausePanel.gameObject.SetActive(true);

        }
        else if(isPausedByFocusLoss)//can only pause with the conditions above so even if focus is lost, game will not pause
        {
            isPausedByFocusLoss = false;
        }

        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered&&tutorialManager.abilityPDHandler.pdTutorialPanelAbility.activeInHierarchy)
            tutorialManager.abilityPDHandler.pdTutorialPanelAbility.SetActive(false);

        pause = true;

        Time.timeScale = 0f;//pause coroutines
    }

    public void Continue()
    {
        if (pausePanel.IsActive())
        {
            pausePanel.gameObject.SetActive(false);
        }

        isPausedByFocusLoss = false;

        pause = false;

        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered&&!tutorialManager.abilityPDHandler.pdTutorialPanelAbility.activeInHierarchy)
            tutorialManager.abilityPDHandler.pdTutorialPanelAbility.SetActive(true);

        Time.timeScale = 1f;//continue coroutines

    }

    public void OnEdit()
    {
        placeHolder.GetComponent<Text>().text="";
    }
    public void EnterWorkID()
    {
        if (inputText.GetComponent<Text>().text == "" || inputText.GetComponent<Text>().text[0] == ' '|| inputText.GetComponent<Text>().text[0] == '\n'  || inputText.GetComponent<Text>().text[0] == '\t')
        {
           validatorText.gameObject.SetActive(true);
        }
        else
        {
            workerID = inputText.GetComponent<Text>().text;
            DataMiner.workerID = workerID;
            StartorStopGame(true);
        }
    }

    private void StartorStopGame(bool trueOrFalse)
    {
        workIDCanvas.gameObject.SetActive(!trueOrFalse);
        mainCanvas.gameObject.SetActive(trueOrFalse);
        bgCanvas.gameObject.SetActive(mainCanvas.isActiveAndEnabled);
       
    }

    public void HandleErrorMessage(string specifiedErrorMessage)
    {
        errorMessagePanel.SetActive(true);
        errorMessageText.gameObject.GetComponent<TextMeshProUGUI>().text = specifiedErrorMessage;
          StartCoroutine(HideErrorMessage());
    }

    IEnumerator HideErrorMessage()
    {
        yield return new WaitForSeconds(6f);
        errorMessagePanel.SetActive(false);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // If the application has lost focus and we haven't already paused it,
        // then call the Pause function.
        if (!hasFocus && !isPausedByFocusLoss&&!inTestingMode&&!QuickTime.quickTimeEnabled&&!orderManager.newLevelProgressed.activeInHierarchy)
        {

            // Set our flag to true so we know the pause was triggered by focus loss.
            isPausedByFocusLoss = true;
            Pause();
        }
    }

    private void EndGame()
    {
        //handling when game is over (Potentially put in game manager)
       
            DataMiner.currLevel = orderManager.currLevel;
            GameManager.gameOver = true;

            if (collectingGame.activeInHierarchy)
            {
                collectingGame.SetActive(false);
                mainGame.SetActive(true);
            }

            if (timeManager.GetTutorialTimeIntro() > 0)
                timeManager.SetTimeCompletedGame();
            endGameScreen.SetActive(true);

        timeManager.SetAbilityTutorialTime();

            
            //log the data once done
            dataMiner.logdata();
        }
    }


    
