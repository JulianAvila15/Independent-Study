using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public Image pausePanel;
    public Button pauseAndContinueButton;

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
    }

    // Update is called once per frame
    void Update()
    {
        pauseAndContinueButton.GetComponentInChildren<Text>().text = pausePanel.IsActive() ? "Continue" : "Pause";//If the pause panel is active make text say "continue", other wise it should say "pause"

        if(Input.GetKeyDown(KeyCode.P)) //Alternative way instead of using button
        {
            Pause();
        }
    }

    public void Pause()//Pauses game
    {
        if (pausePanel.IsActive())
            pausePanel.gameObject.SetActive(false);
        else
            pausePanel.gameObject.SetActive(true);
    }
}
