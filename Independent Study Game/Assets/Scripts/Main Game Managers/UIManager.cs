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
    

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (managerHub.gameManager.progressType)
        {
            case GameManager.ProgressFeedbackType.score:
                if (mainGame.activeInHierarchy)
                {
                    numOrderDisplay.gameObject.SetActive(true);
                    numOrderDisplay.GetComponentInChildren<Text>().text = "Orders completed: " + (managerHub.orderManager.numofOrdersCompleted); //if progress feed back type is score
                }

                if (collectingMiniGame.activeInHierarchy)
                {
                    numOrderDisplayCollectingGame.gameObject.SetActive(true);
                    numOrderDisplayCollectingGame.GetComponentInChildren<Text>().text = "Coins Collected: " + (managerHub.collectingGameManager.coinsCollected);
                }

               break;
            case GameManager.ProgressFeedbackType.progressBar:
                if (mainGame.activeInHierarchy)
                {
                    progressBar.gameObject.SetActive(true); //if progress feedback type is progress bar

                    progressBar.maxValue = managerHub.orderManager.ordersToCompletePerLevel[managerHub.orderManager.currLevel];

                    if (managerHub.orderManager.numofOrdersCompleted>=progressBar.maxValue)
                    {
                        progressBar.value = managerHub.orderManager.numofOrdersCompleted = managerHub.orderManager.currentOrderIndex = 0;
                    }
                    else
                    {
                        progressBar.value = managerHub.orderManager.numofOrdersCompleted;
                    }
                }

                if(collectingMiniGame.activeInHierarchy)
                {
                    progeressBarCollectingGame.gameObject.SetActive(true); //if progress feedback type is progress bar
                    progeressBarCollectingGame.value = managerHub.collectingGameManager.coinsCollected;
                }
                break;
            case GameManager.ProgressFeedbackType.noScoreOrProgressBar:
                break;

        }


        
    }
}
