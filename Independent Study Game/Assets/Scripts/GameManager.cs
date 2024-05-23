using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public TimeManager timeManager;
    public Image pausePanel;
    public Button pauseButton,continueButton;
    public Canvas mainCanvas,workIDCanvas,bgCanvas;
    public GameObject inputText,placeHolder,validatorText, errorMessageText;
    string workerID;
    public GameObject errorMessagePanel;
    public DataMiner dataMiner;


    public static bool gameOver = false;
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
        

        if(Input.GetKeyDown(KeyCode.P) && workIDCanvas.gameObject.activeInHierarchy==false) //Alternative way instead of using 
            Pause();


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
        if (inputText.GetComponent<Text>().text == "" || inputText.GetComponent<Text>().text[0] == ' '|| inputText.GetComponent<Text>().text[0] == '\n'  || inputText.GetComponent<Text>().text[0] == '\t')
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

}
