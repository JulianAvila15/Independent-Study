using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

[SerializeField]  ManagerofManagers managerHub;
    public GameObject numOrderDisplay;
    public Slider progressBar;

    public GameObject numOrderDisplayCollectingGame;
    public Slider progeressBarCollectingGame;

   public GameObject mainGame, collectingMiniGame;
    private Text numOrderDisplayCollectingGameText, numOrderDisplayText;

   
    // Start is called before the first frame update
    void Start()
    {
        numOrderDisplayText = numOrderDisplay.GetComponentInChildren<Text>();
       numOrderDisplayCollectingGameText = numOrderDisplayCollectingGame.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameProgressUI();
    }

    private void HandleGameProgressUI()
    {
        switch (managerHub.gameManager.progressType)
        {
            case GameManager.ProgressFeedbackType.score:
                HandleScore();
                break;
            case GameManager.ProgressFeedbackType.progressBar:
                HandleProgressBar();
                break;
            case GameManager.ProgressFeedbackType.noScoreOrProgressBar:
                break;

        }
    }

    void HandleScore()
    {
        if (mainGame.activeInHierarchy)
        {
            if (!numOrderDisplay.gameObject.activeInHierarchy)
                numOrderDisplay.gameObject.SetActive(true);

            numOrderDisplayText.text = "Orders completed: " + (managerHub.orderManager.numofOrdersCompleted); //if progress feed back type is score
        }

        if (collectingMiniGame.activeInHierarchy)
        {
            if (!numOrderDisplayCollectingGame.gameObject.activeInHierarchy)
                numOrderDisplayCollectingGame.gameObject.SetActive(true);

            numOrderDisplayCollectingGameText.text = "Coins Collected: " + (managerHub.collectingGameManager.coinsCollected);
        }
    }

    void HandleProgressBar()
    {
        if (mainGame.activeInHierarchy)
        {
            progressBar.gameObject.SetActive(true); //if progress feedback type is progress bar

            progressBar.maxValue = managerHub.orderManager.ordersToCompletePerLevel[managerHub.orderManager.currLevel];

            if (managerHub.orderManager.numofOrdersCompleted >= progressBar.maxValue)
            {
                progressBar.value = managerHub.orderManager.numofOrdersCompleted = managerHub.orderManager.currentOrderIndex = 0;
            }
            else
            {
                progressBar.value = managerHub.orderManager.numofOrdersCompleted;
            }
        }

        if (collectingMiniGame.activeInHierarchy)
        {
            progeressBarCollectingGame.gameObject.SetActive(true); //if progress feedback type is progress bar
            progeressBarCollectingGame.value = managerHub.collectingGameManager.coinsCollected;
        }
    }

}
