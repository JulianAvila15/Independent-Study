using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class QuickEvent : MonoBehaviour
{
    public GameObject timing;
    public GameObject collectingGame;

    public Image quickEventImage1;
    public Image quickEventImage2;

    public GameObject[] powerUps;

    public CraftingManager craftingMan;
    public GameManager gameManager;

    [Header("quickEventTiming")]
    public float coolDownTiming;
    bool isCoolDownTiming = false;
    public KeyCode quickEventTiming;
    bool pressedTiming = false;

    [Header("quickEventCollecting")]
    public float coolDownCollecting;
    bool isCoolDownCollecting = false;
    public KeyCode quickEventCollecting;
    bool pressedCollecting = false;

    public DataMiner miner;

   public GameObject newLevelAlert,powerUpTutorialPanel;

    [SerializeField] private AbilityTutorialProgressiveDisclosureHandler abilityPDTutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        quickEventImage1.fillAmount = quickEventImage2.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        QuickEventTiming();
        QuickEventCollecting();
    }

    private void QuickEventTiming()
    {
        if ((Input.GetKey(quickEventTiming) || pressedTiming) && isCoolDownTiming == false)
        {
            isCoolDownTiming = true;
            quickEventImage1.fillAmount = 1;
            timing.SetActive(true);

            if (abilityPDTutorialManager != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                abilityPDTutorialManager.OnAbilityButtonClicked();

        }

        if (!GameManager.pause&&(gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || abilityPDTutorialManager.completedTutorials.Contains("Timing") && abilityPDTutorialManager.completedTutorials.Count > 0 || abilityPDTutorialManager.CheckIfCoolDownEnabled()))
        {
            if (isCoolDownTiming == true)
            {
                quickEventImage1.fillAmount -= 1 / coolDownTiming * Time.deltaTime;

                if (quickEventImage1.fillAmount <= 0)
                {
                    quickEventImage1.fillAmount = 0;
                    isCoolDownTiming = false;
                    pressedTiming = false;
                }
            }
        }
    }

    public void TimingGameEnabled()
    {
            pressedTiming = true;
        if(!isCoolDownTiming)
        DataMiner.abilityandEventCount[3]++;
    }

    private void QuickEventCollecting()
    {
        if ((Input.GetKey(quickEventCollecting) || pressedCollecting) && isCoolDownCollecting == false)
        {
            isCoolDownCollecting = true;
            quickEventImage2.fillAmount = 1;
            collectingGame.SetActive(true);

            if (abilityPDTutorialManager != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                abilityPDTutorialManager.OnAbilityButtonClicked();
        }

        if (!GameManager.pause&&(gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || ((abilityPDTutorialManager.completedTutorials.Contains("Collecting") && abilityPDTutorialManager.completedTutorials.Count > 0) || abilityPDTutorialManager.CheckIfCoolDownEnabled())))
        {

            if (isCoolDownCollecting == true)
            {
                quickEventImage2.fillAmount -= 1 / coolDownCollecting * Time.deltaTime;

                if (quickEventImage2.fillAmount <= 0)
                {
                    quickEventImage2.fillAmount = 0;
                    isCoolDownCollecting = false;
                    pressedCollecting = false;
                }
            }
        }
    }

    public void CollectingGameEnabled()
    {
        if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
        {
            pressedCollecting = true;
            if (!isCoolDownCollecting)
                DataMiner.abilityandEventCount[4]++;
        }
    }
}
