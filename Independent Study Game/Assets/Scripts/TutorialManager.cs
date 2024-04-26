using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    public GameManager gameManager;
    public Button continueButton;
    public GameObject tutorialPanel;
    public TMP_Text textbox;
    public TimeManager timeManager;
    public ProgressiveDisclosureHandler progressiveDisclosure;
    // Start is called before the first frame update
    void Start()
    {
        switch (gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPanel.SetActive(true);
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                tutorialPanel.SetActive(true);
                progressiveDisclosure.gameObject.SetActive(true);
                break;
            case GameManager.TutorialType.noTutorial:
                tutorialPanel.gameObject.SetActive(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If tutorial is set to something in the manager indicate which version

       
    }

    public void ContinueButtonIsPressed()
    {

        switch(gameManager.tutorialType)
        {
            case GameManager.TutorialType.wallOfText:
                tutorialPanel.gameObject.SetActive(false);
                break;
            case GameManager.TutorialType.progressiveDisclosure:
                //call some function that handles the progressive disclosure
                progressiveDisclosure.GetComponent<ProgressiveDisclosureHandler>().NextSentence();
                break;
            case GameManager.TutorialType.noTutorial:
                break;

               
        }
        timeManager.enabled = true;


    }

    //potential progressive disclosure function
}
