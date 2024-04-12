using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProgressiveDisclosureHandler : MonoBehaviour
{
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public TutorialManager tutorialManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (tutorialManager.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure)
        {
            tutorialManager = tutorialManager.GetComponent<TutorialManager>();
            tutorialManager.continueButton.gameObject.SetActive(false);

            StartCoroutine(Type());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Type()
    {
        tutorialManager.textbox.text = "";

        foreach(char letter in sentences[index])
        {
            tutorialManager.textbox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        tutorialManager.continueButton.gameObject.SetActive(true);
    }


    public void NextSentence()
    {
        tutorialManager.continueButton.gameObject.SetActive(false);

        if(index<sentences.Length-1)
        {
            index++;
            tutorialManager.textbox.text = "";
            StartCoroutine(Type());
        }
        else
        {
            tutorialManager.textbox.text = "";
            tutorialManager.tutorialPanel.gameObject.SetActive(false);
        }
    }
}
