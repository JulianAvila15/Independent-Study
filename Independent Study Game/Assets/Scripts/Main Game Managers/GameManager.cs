using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{

    public static bool pause = false;
    public bool timeCompleted = false;
    public Image pausePanel;
    public Button pauseButton,continueButton;
    public Canvas mainCanvas,workIDCanvas,bgCanvas;
    public GameObject workerIDInputField, placeHolder, validatorText, errorMessageText;
   [SerializeField] private Text workerIDText;
    string workerID;
    public GameObject errorMessagePanel;
    public DataMiner dataMiner;
    public bool isPausedByFocusLoss = false;
    [SerializeField] public bool inTestingMode;

    public static bool gameOver = false;

    [SerializeField] private GameObject collectingGame, endGameScreen, mainGame;

  [SerializeField]  ManagerofManagers managerHub;

    [SerializeField] int minimumLevelsToBeatGame = 5;

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

    private void Awake()
    {
        if (managerHub != null && managerHub.gameManager == null)
            managerHub.gameManager = gameObject.GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        pausePanel.gameObject.SetActive(false);
        StartorStopGame(false);
        managerHub.timeManager.enabled = true;
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

        if (pausePanel.gameObject.activeInHierarchy || managerHub.orderManager.newLevelProgressed.activeInHierarchy || managerHub.tutorialManager.tutorialPanel.activeInHierarchy || managerHub.tutorialManager.tutorialPowerUp.activeInHierarchy)
        {
            pauseButton.gameObject.SetActive(false);
        }
        else
        {
            pauseButton.gameObject.SetActive(true);
        }

        if (timeCompleted && managerHub.orderManager.currLevel >= minimumLevelsToBeatGame)
            EndGame();
    }

    public void Pause()//Pauses game
    {
      if(!(managerHub.orderManager.newLevelProgressed.activeInHierarchy || workIDCanvas.gameObject.activeInHierarchy||endGameScreen.activeInHierarchy))
        {
            pausePanel.gameObject.SetActive(true);

        }
        else if(isPausedByFocusLoss)//can only pause with the conditions above so even if focus is lost, game will not pause
        {
            isPausedByFocusLoss = false;
        }

        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered&&managerHub.tutorialManager.abilityPDHandler.pdTutorialPanelAbility.activeInHierarchy)
            managerHub.tutorialManager.abilityPDHandler.pdTutorialPanelAbility.SetActive(false);

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

        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered&&!managerHub.tutorialManager.abilityPDHandler.pdTutorialPanelAbility.activeInHierarchy)
            managerHub.tutorialManager.abilityPDHandler.pdTutorialPanelAbility.SetActive(true);

        Time.timeScale = 1f;//continue coroutines

    }

    public void OnEdit()
    {
        placeHolder.GetComponent<Text>().text="";
    }


    public void EnterWorkID()
    { 
        if (CanEnterWorkID())
        {

            workerID = workerIDText.text;
            DataMiner.workerID = workerID;
            StartorStopGame(true);
           
         
        }
        else
        {
            validatorText.gameObject.SetActive(true);
        }


    }

    private void StartorStopGame(bool trueOrFalse)
    {
        workIDCanvas.gameObject.SetActive(!trueOrFalse);
        mainCanvas.gameObject.SetActive(trueOrFalse);
        bgCanvas.gameObject.SetActive(mainCanvas.isActiveAndEnabled);

    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // If the application has lost focus and we haven't already paused it,
        // then call the Pause function.
        if (!hasFocus && !isPausedByFocusLoss&&!inTestingMode&&!QuickTime.quickTimeEnabled&&!managerHub.orderManager.newLevelProgressed.activeInHierarchy)
        {

            // Set our flag to true so we know the pause was triggered by focus loss.
            isPausedByFocusLoss = true;
            Pause();
        }
    }

    private void EndGame()
    {
        //handling when game is over (Potentially put in game manager)
       
            DataMiner.currLevel = managerHub.orderManager.currLevel;
            GameManager.gameOver = true;

            if (collectingGame.activeInHierarchy)
            {
                collectingGame.SetActive(false);
                mainGame.SetActive(true);
            }

     

            endGameScreen.SetActive(true);

        DataMiner.tutorialTimeIntro = managerHub.timeManager.GetTutorialTimeIntro();

        managerHub.timeManager.SetAbilityTutorialTime();

            
            //log the data once done
            dataMiner.logdata();
        }


    bool CanEnterWorkID()
    {
        return workerIDText!=null && workerIDText.text.Length > 0&& !(workerIDText.text == "" || workerIDText.text[0] == ' ' || workerIDText.text[0] == '\n' || workerIDText.text[0] == '\t');
    }


}


