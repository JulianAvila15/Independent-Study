using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public Text numOrderDisplay,levelsOutOfRemaining;
    public Slider progressBar;

    public Text numOrderDisplayCollectingGame;
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
                    numOrderDisplay.GetComponent<Text>().text = "Order: " + (gameManager.orderManager.numofOrdersCompleted + 1); //if progress feed back type is score
                }

                if (collectingMiniGame.activeInHierarchy)
                {
                    numOrderDisplayCollectingGame.gameObject.SetActive(true);
                    numOrderDisplayCollectingGame.GetComponent<Text>().text = "Coins Collected: " + (CollectingGameManager.coinsCollected);
                }

               break;
            case GameManager.ProgressFeedbackType.progressBar:
                if (mainGame.activeInHierarchy)
                {
                    progressBar.gameObject.SetActive(true); //if progress feedback type is progress bar
                    progressBar.value = gameManager.orderManager.numofOrdersCompleted;
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


        if(gameManager.orderManager.currLevel<5)
        levelsOutOfRemaining.text = "Levels Completed: "+(gameManager.orderManager.currLevel+1) + "  / " + 5; //show which level the player is on until the player finishes more than 5 levels
        
    }
}
