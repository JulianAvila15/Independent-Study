using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
   [SerializeField] private Text timeRemainingText;
    private float timeRemaining = 30 * 60;
  [SerializeField]  private float tutorialTimeIntro=0;
   [SerializeField] private  float[] powerUpTutorialTime = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };//kept in order of unlockability
    int powerUpTimeIndex = 0;
    public static float afkTimer = 15f;
   [SerializeField] private GameObject pausePanel, tutorialPanel, WorkIDPanel, powerUpTutorialPanelWallOfText,newLevelFeedBack;
   [SerializeField] private GameObject quickTime;

    static bool  showAFKWarning=true;
    public GameObject mainGame;
    public DataMiner dataMiner;
    float minutes;
    float seconds;
    string timeDisplay;

   [SerializeField] ManagerofManagers managerHub;

    private void Awake()
    {
        if (managerHub != null && managerHub.timeManager == null)
            managerHub.timeManager = gameObject.GetComponent<TimeManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
   
    }

   public string DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        minutes = Mathf.FloorToInt(timeToDisplay/60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);

        

        timeDisplay = string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);


            timeRemainingText.text = "Time Remaining: \n"+string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);
       

        return timeDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTutorialTiming();
      
        HandleOverallGameAndAFKTimer();
    }

    private void HandleOverallGameAndAFKTimer()
    {
        //handling game timer and afk timer
        if (CanDecrementOverallGameTimer())
        {

            timeRemainingText.GetComponent<Text>().text = "Time Remaining: ";

            timeRemaining -= Time.deltaTime;

            if (afkTimer > 0)
                afkTimer -= Time.deltaTime;
        }
        else if (timeRemaining <= 0)
            managerHub.gameManager.timeCompleted = true;

        if (afkTimer <= 0)
        {
            pausePanel.SetActive(true);

        }


        if (pausePanel.activeInHierarchy)
            ResetAFKTimer();
    }

    private void HandleTutorialTiming()
    {
        //Handle tutorial Timing
        if (CanDisplayTime())
            DisplayTime(timeRemaining);
        else
            timeRemainingText.text = "Tutorial in progress";

        if (PDIntroTutorialIsHappening()||WallOfTextIntroTutorialIsHappening())
            tutorialTimeIntro += Time.deltaTime;

        if (AbilityTutorialIsHappening())
            powerUpTutorialTime[powerUpTimeIndex] += Time.deltaTime;
        else if (CanStopTimingAbilityTutorial())
            powerUpTimeIndex++;
    }

    public void ResetAFKTimer()
    {
        afkTimer = 15f;
    }

    bool ImportantPaneAreActive()
    {
        return WorkIDPanel.activeInHierarchy || pausePanel.activeInHierarchy;
    }

    bool TutorialPanelsAreActive()
    {
        return tutorialPanel.activeInHierarchy || powerUpTutorialPanelWallOfText.activeInHierarchy;
    }

    bool CanDecrementOverallGameTimer()
    {
        return (timeRemaining > 0 && (!ImportantPaneAreActive() && !QuickTime.quickTimeEnabled && !newLevelFeedBack.activeInHierarchy && !TutorialPanelsAreActive() && (!(managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure) || IntroProgressiveDisclosureHandler.introPDTutorialFinished && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)));//returns true if power up panel is not on or if the game mode is anything other than PD or intro pd or ability pd are not happening and there is still time remaining
    }

    bool CanDisplayTime()
    {
        return pausePanel.activeInHierarchy && (managerHub.gameManager.tutorialType == GameManager.TutorialType.noTutorial || (managerHub.gameManager.tutorialType == GameManager.TutorialType.wallOfText && !TutorialPanelsAreActive()) || (IntroProgressiveDisclosureHandler.introPDTutorialFinished && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered));
    }

    bool PDIntroTutorialIsHappening()
    {
        return managerHub.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure && !IntroProgressiveDisclosureHandler.introPDTutorialFinished && (!WorkIDPanel.activeInHierarchy && !pausePanel.activeInHierarchy);
    }

    bool WallOfTextIntroTutorialIsHappening()
    {
        return (managerHub.gameManager.tutorialType == GameManager.TutorialType.wallOfText && managerHub.tutorialManager.tutorialPanel.activeInHierarchy);
    }

    bool AbilityTutorialIsHappening()
    {
        return (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || powerUpTutorialPanelWallOfText.activeInHierarchy) && (!WorkIDPanel.activeInHierarchy && !pausePanel.activeInHierarchy);
    }


    bool CanStopTimingAbilityTutorial()
    {
        return (powerUpTutorialTime.Length > powerUpTimeIndex && powerUpTutorialTime[powerUpTimeIndex] > 0 && !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && !powerUpTutorialPanelWallOfText.activeInHierarchy);
    }

    public void SetTimeCompletedLevel(int currLevel)
    {
        DataMiner.timeSpentOnEachLevel[currLevel] = DisplayTime(timeRemaining);
    }

    public void SetAbilityTutorialTime()
    {
        for (int i = 0; i < powerUpTutorialTime.Length; i++)
        {
            if (powerUpTutorialTime[i] > 0)
                DataMiner.powerUpTutorialTime[i] = DisplayTime(powerUpTutorialTime[i]);
        }

    }

    public string GetTutorialTimeIntro()
    {
        return DisplayTime(tutorialTimeIntro);
    }

}
