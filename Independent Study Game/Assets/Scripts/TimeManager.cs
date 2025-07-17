using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    public Text timeRemainingText;
    public static float timeRemaining = 30 * 60;
    public float tutorialTimeIntro=0;
    public  float[] powerUpTutorialTime = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };//kept in order of unlockability
    int powerUpTimeIndex = 0;
    public static float afkTimer = 15f;
    public GameObject pausePanel, tutorialPanel, WorkIDPanel, afkWarning,powerUpTutorialPanel,collectingGame,quickTime,newLevelFeedBack,endGameScreen;
    static bool  showAFKWarning=true;
    public GameManager gameManager;
    public TutorialManager tutorialManager;
    public OrderManager orderManager;
    public GameObject mainGame;
    public DataMiner dataMiner; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

   public string DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string timeDisplay;

        timeDisplay = string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);

        if (mainGame.activeInHierarchy)
        {
            timeRemainingText.text = "Time Remaining: \n"+string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);
        }

        return timeDisplay;
    }

    // Update is called once per frame
    void Update()
    {

        DisplayTime(timeRemaining);

       if((gameManager.tutorialType==GameManager.TutorialType.wallOfText&&tutorialManager.tutorialPanel.activeInHierarchy))
            {
            tutorialTimeIntro += Time.deltaTime;
            
         }
       else if(gameManager.tutorialType==GameManager.TutorialType.progressiveDisclosure && !ProgressiveDisclosureHandler.introPDTutorialFinished && (!WorkIDPanel.activeInHierarchy && !pausePanel.activeInHierarchy))
        {
            tutorialTimeIntro += Time.deltaTime;
        }

       if(powerUpTutorialPanel.activeInHierarchy)
        {
            powerUpTutorialTime[powerUpTimeIndex] += Time.deltaTime;
        }
        else if(powerUpTutorialTime.Length > powerUpTimeIndex&&powerUpTutorialTime[powerUpTimeIndex]>0)
        {
            powerUpTimeIndex++;
        }

        if (pausePanel.activeInHierarchy)
            ResetAFKTimer();

        if (timeRemaining > 0 && (!WorkIDPanel.activeInHierarchy && !pausePanel.activeInHierarchy && !newLevelFeedBack.activeInHierarchy && !tutorialPanel.activeInHierarchy && !powerUpTutorialPanel.activeInHierarchy&& !collectingGame.activeInHierarchy&&(!(gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure) || ProgressiveDisclosureHandler.introPDTutorialFinished&&!ProgressiveDisclosureHandler.abilityTutorialTriggered))) //assuming that the power up panel is not on, if the game mode is either 
        {

            timeRemainingText.GetComponent<Text>().text = "Time Remaining: ";
           
            timeRemaining -= Time.deltaTime;

            if(afkTimer>0)
                afkTimer -= Time.deltaTime;
        }

        if(timeRemaining<=0 && orderManager.currLevel>=5)
        {
            DataMiner.currLevel = orderManager.currLevel;
            GameManager.gameOver = true;
            
            if(collectingGame.activeInHierarchy)
            {
                collectingGame.SetActive(false);
                mainGame.SetActive(true);
            }

            DataMiner.totalTime = DisplayTime(timeRemaining);

            if(tutorialTimeIntro>0)
            DataMiner.tutorialTimeIntro = DisplayTime(tutorialTimeIntro);
            endGameScreen.SetActive(true);

            for(int i=0;i<powerUpTutorialTime.Length;i++)
            {
                if(powerUpTutorialTime[i]>0)
                DataMiner.powerUpTutorialTime[i] = DisplayTime(powerUpTutorialTime[i]);
            }

            //log the data once done
            dataMiner.logdata();
        }

         if(afkTimer<=0)
        {
            pausePanel.SetActive(true);
            
        }

  

    }

    public static void ResetAFKTimer()
    {
        afkTimer = 15f;
        showAFKWarning = true;
    }

    public void CloseAFKWarning()
    {
        afkWarning.SetActive(false);
        showAFKWarning = false;
    }

}
