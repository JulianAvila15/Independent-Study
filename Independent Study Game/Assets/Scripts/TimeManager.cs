using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    public Text timeRemainingText;
    public static float timeRemaining=30*60;
    public static float afkTimer = 15f;
    public GameObject pausePanel, tutorialPanel, WorkIDPanel, afkWarning,powerUpTutorialPanel,collectingGame,quickTime,newLevelFeedBack,endGameScreen;
    static bool  showAFKWarning=true;
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

        if (pausePanel.activeInHierarchy)
            ResetAFKTimer();

        if(timeRemaining>0 && !WorkIDPanel.activeInHierarchy &&!pausePanel.activeInHierarchy && !tutorialPanel.activeInHierarchy&&!powerUpTutorialPanel.activeInHierarchy)
        {
            timeRemainingText.GetComponent<Text>().text = "Time Remaining: ";
           
            timeRemaining -= Time.deltaTime;
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
            endGameScreen.SetActive(true);

            //log the data once done
            dataMiner.logdata();
        }



        if (afkTimer > 0 && (!pausePanel.activeInHierarchy && !tutorialPanel.activeInHierarchy && !WorkIDPanel.activeInHierarchy &&  !newLevelFeedBack.activeInHierarchy&&!endGameScreen.activeInHierarchy && !collectingGame.activeInHierarchy&& !powerUpTutorialPanel.activeInHierarchy))
        {
            afkTimer -= Time.deltaTime;
           
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
