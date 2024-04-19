using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public Image pausePanel;
    public Button pauseButton,continueButton;
    public Canvas mainCanvas,workIDCanvas;
    

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
        //workIDCanvas.gameObject.SetActive(true);
        //mainCanvas.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetKeyDown(KeyCode.P)) //Alternative way instead of using button
        {
            Pause();
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
}
