using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    public Text timeRemainingText;
    private static float timeRemaining=30*60;
    private static float afkTimer = 15 * 60;
    public GameObject pausePanel, afkWarning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    string DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        string timeDisplay;

        timeDisplay = "AFK Timer: " + string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);
        timeRemainingText.text +=  string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);

        return timeDisplay;
    }

    // Update is called once per frame
    void Update()
    {

        if (pausePanel.activeInHierarchy)
            ResetAFKTimer();

        if(timeRemaining>0&&!pausePanel.activeInHierarchy)
        {
            timeRemainingText.GetComponent<Text>().text = "Time Remaining: ";
            DisplayTime(timeRemaining);
            timeRemaining -= Time.deltaTime;
        }


        if (afkTimer > 0&&!pausePanel.activeInHierarchy)
        {
            afkTimer -= Time.deltaTime;
            Debug.Log(DisplayTime(afkTimer));
        }
        else if(afkTimer>(5*60)&& !pausePanel.activeInHierarchy)//if there are 5 minutes of the timer left
        {
            afkWarning.SetActive(true);
        }

    }

    public void ResetAFKTimer()
    {
        afkTimer = 15 * 60;
    }

    public void CloseAFKWarning()
    {
        afkWarning.SetActive(false);
    }

}
