using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject numOrderDisplay;
    public Slider progressBar;

    public GameObject numOrderDisplayCollectingGame;
    public Slider progeressBarCollectingGame;

   public GameObject mainGame, collectingMiniGame;

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameManager.progressType)
        {
            case GameManager.ProgressFeedbackType.score:
                if (mainGame.activeInHierarchy)
                {
                    numOrderDisplay.gameObject.SetActive(true);
                    numOrderDisplay.GetComponentInChildren<Text>().text = "Orders completed: " + (gameManager.orderManager.numofOrdersCompleted); //if progress feed back type is score
                }

                if (collectingMiniGame.activeInHierarchy)
                {
                    numOrderDisplayCollectingGame.gameObject.SetActive(true);
                    numOrderDisplayCollectingGame.GetComponentInChildren<Text>().text = "Coins Collected: " + (CollectingGameManager.coinsCollected);
                }

               break;
            case GameManager.ProgressFeedbackType.progressBar:
                if (mainGame.activeInHierarchy)
                {
                    progressBar.gameObject.SetActive(true); //if progress feedback type is progress bar

                    progressBar.maxValue = gameManager.orderManager.ordersToCompletePerLevel[gameManager.orderManager.currLevel];

                    if (gameManager.orderManager.numofOrdersCompleted>=progressBar.maxValue)
                    {
                        progressBar.value = gameManager.orderManager.numofOrdersCompleted = gameManager.orderManager.currentOrderIndex = 0;
                    }
                    else
                    {
                        progressBar.value = gameManager.orderManager.numofOrdersCompleted;
                    }
                }

                if(collectingMiniGame.activeInHierarchy)
                {
                    progeressBarCollectingGame.gameObject.SetActive(true); //if progress feedback type is progress bar
                    progeressBarCollectingGame.value = CollectingGameManager.coinsCollected;
                }
                break;
            case GameManager.ProgressFeedbackType.noScoreOrProgressBar:
                break;

        }


        
    }
}
