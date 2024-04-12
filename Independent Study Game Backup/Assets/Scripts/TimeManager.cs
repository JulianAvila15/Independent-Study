using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    public Text timeRemainingText;
    private static float timeRemaining=30*60;
    public GameObject pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeRemainingText.text += string.Format("{0:00} minutes : {1:00} seconds", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeRemaining>0&&!pausePanel.activeInHierarchy)
        {
            timeRemainingText.GetComponent<Text>().text = "Time Remaining: ";
            DisplayTime(timeRemaining);
            timeRemaining -= Time.deltaTime;
        }
    }
}
