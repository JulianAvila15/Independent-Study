using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public TimeManager timeManager;
    public Image pausePanel;
    public Button pauseButton,continueButton;
    public Canvas mainCanvas,workIDCanvas;
    public GameObject inputText,placeHolder,validatorText;
    string workerID;

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
        timeManager.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.P)) //Alternative way instead of using button
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Alternative way instead of using button
        {
            EnterWorkID();
        }
    }

    public void Pause()//Pauses game
    {
        if (pausePanel.IsActive())
        {
            pausePanel.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
        else
        {
            pausePanel.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);

        }
    }

    public void OnEdit()
    {
        placeHolder.GetComponent<Text>().text="";
    }
    public void EnterWorkID()
    {
        if (inputText.GetComponent<Text>().text == "" || inputText.GetComponent<Text>().text.Contains(" ") || inputText.GetComponent<Text>().text.Contains("\n") || inputText.GetComponent<Text>().text.Contains("\t"))
        {
           validatorText.gameObject.SetActive(true);
        }
        else
        {
            workerID = inputText.GetComponent<Text>().text;
            DataMiner.workerID = workerID;
            Debug.Log(workerID);
            StartorStopGame(true);
        }
    }

    private void StartorStopGame(bool trueOrFalse)
    {
        workIDCanvas.gameObject.SetActive(!trueOrFalse);
        mainCanvas.gameObject.SetActive(trueOrFalse);
        
       
    }
}
