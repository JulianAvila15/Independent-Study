using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using System;
using System.Linq;
using UnityEngine.Networking;
using TMPro;

public class DataMiner : MonoBehaviour
{
    public static DataMiner dataMiner;

    List<KeyValuePair<int, int>> questLog = new List<KeyValuePair<int, int>>();
    List<string> questLogStrings = new List<string>();
    string logString = "";
    StreamWriter file;
    KeyValuePair<int, int> currentTime;
    string adjustedTimeStamp = "";
    public static int totalClicks;
    public static int numberOfLogableQuests = 0;
    public static string workerID = "";
    bool dataHasBeenSent = false;
    private static int questCompleted;

    public static int[] summonandEventCount = { 0, 0, 0, 0, 0 };//Keep track of ability/summon/Event count
    public static string progressFeedbackMode, tutorialMode;//Keep track of game modes
    public static int numOfIngredientClicks=0,numOfCrafts=0,numOfSuccessfulCrafts=0,numOfFailedCrafts=0,currLevel=0;//game variables
    public static string totalTime="-1", tutorialTimeIntro="00 minutes:00 seconds";//Keep track of timing
    public static string[] powerUpTutorialTime = { "00 minutes:00 seconds", "00 minutes:00 seconds", "00 minutes:00 seconds", "00 minutes:00 seconds", "00 minutes:00 seconds" };
    public static string[] timeSpentOnEachLevel = { "-1", "-1", "-1", "-1", "-1" , "-1" , "-1" , "-1" , "-1" , "-1" };

    GameManager gameManager;

    private void Awake()
    {
        dataMiner = GameObject.Find("DataMiner").GetComponent<DataMiner>();
    }

    private void Start()
    {
        Debug.Log("Logable Quests: " + numberOfLogableQuests);
    }

    public void AddClickToLog()
    {
        totalClicks++;
    }

    public void LogQuestCompletion(bool completed = true)
    {
        //AdjustTimeStamp(ref questLog);

        string questString = adjustedTimeStamp + ",";
        //questString += GameManager.gameManager.GetDifficultyAverage() + ",";
        questString += completed + ",";
        if (completed) { questCompleted++; }

        
        questLogStrings.Add(questString);
        totalClicks = 0;

        
    }

    public void Update()
    {
    }

    public void logdata()
    {
        string positionlogstring = workerID + "," + DateTime.Now + ",";
       
        if (!dataHasBeenSent)
        {
            dataHasBeenSent = true;
            logString += workerID + "," + DateTime.Now + "," + progressFeedbackMode + ","
                + tutorialMode + "," +  tutorialTimeIntro +"," + numOfIngredientClicks + ","+numOfCrafts+","+ numOfSuccessfulCrafts+","+numOfFailedCrafts+","+currLevel;

            for (int i = 0; i <summonandEventCount.Length; i++)
            {
                logString += ", " + summonandEventCount[i];

                switch(i)
                {
                    case 0: //messenger
                        logString += ", " + powerUpTutorialTime[2];
                        break;
                    case 1: //penguin
                        logString += ", " + powerUpTutorialTime[0];
                        break;
                    case 2: //dragon
                        logString += ", " + powerUpTutorialTime[3];
                        break;
                    case 3: //timing
                        logString += ", " + powerUpTutorialTime[4];
                        break;
                        case 4: //coin
                        logString += ", " + powerUpTutorialTime[1];
                        break;
                }


            }
            for (int i = 0; i < timeSpentOnEachLevel.Length; i++)
                logString += ", " + timeSpentOnEachLevel[i];

            StartCoroutine(WriteTextViaPHP(logString, "https://gamesux.com/fromunity_cookingcrafting.php")); //change php to game name (tbd)
         
        }
    }


    IEnumerator WriteTextViaPHP(string data, string destination)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", data);
        UnityWebRequest www = UnityWebRequest.Post(destination, form);
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            www.SetRequestHeader("User-Agent", "Unity 2020.3");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log($"Error: {www.error}\nResponse Code: {www.responseCode}\nURL: {destination}");
        }
        else
        {
            Debug.Log("good");
        }
    }
}