using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProgressiveDisclosureHandler : MonoBehaviour
{
    public string[] sentences,summonSentences;
    public int index;
    public float typingSpeed;
    public TutorialManager tutorialManager;
    public  bool isForSummon;

    private void Start()
    {
        if (tutorialManager.gameManager.tutorialType == GameManager.TutorialType.progressiveDisclosure)
        {

            tutorialManager = tutorialManager.GetComponent<TutorialManager>();
            tutorialManager.continueButton.gameObject.SetActive(false);

            isForSummon = false;
            StartCoroutine(Type());
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
      
            if (isForSummon == true)
            {
                StartCoroutine(TypeForSummon());
                tutorialManager.powerUpButton.gameObject.SetActive(false);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Type()
    {
        if (isForSummon)
        {
            yield return null; 
        }

        tutorialManager.textbox.text = "";

        foreach(char letter in sentences[index])
        {
            tutorialManager.textbox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        tutorialManager.continueButton.gameObject.SetActive(true);
    }

    IEnumerator TypeForSummon()
    {
        tutorialManager.powerUpTextBox.text = "";
        foreach (char letter in summonSentences[index])
        {
            tutorialManager.powerUpTextBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

      tutorialManager.powerUpButton.gameObject.SetActive(true);
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
            this.gameObject.SetActive(false);
        }
    }

    public void NextSentencePowerUp()
    {
        tutorialManager.powerUpButton.gameObject.SetActive(false);

        if (index < summonSentences.Length - 1)
        {
            index++;
            tutorialManager.powerUpTextBox.text = "";
            StartCoroutine(TypeForSummon());
        }
        else
        {
            tutorialManager.powerUpTextBox.text = "";
            tutorialManager.tutorialPowerUp.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
        TimeManager.ResetAFKTimer();
    }
}
