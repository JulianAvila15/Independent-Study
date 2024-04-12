using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public Text numOrderDisplay,levelsOutOfRemaining;
    public Slider progressBar;
   
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
                numOrderDisplay.gameObject.SetActive(true);
                numOrderDisplay.GetComponent<Text>().text = "Order: " + (gameManager.orderManager.numofOrdersCompleted + 1); //if progress feed back type is score
                break;
            case GameManager.ProgressFeedbackType.progressBar:
                progressBar.gameObject.SetActive(true); //if progress feedback type is progress bar
                progressBar.value = gameManager.orderManager.numofOrdersCompleted;
                break;
            case GameManager.ProgressFeedbackType.noScoreOrProgressBar:
                break;

        }


        if(gameManager.orderManager.currLevel<5)
        levelsOutOfRemaining.text = "Current level: "+(gameManager.orderManager.currLevel+1) + "  / " + (gameManager.orderManager.ordersToCompletePerLevel.Length+1); //show which level the player is on until the player finishes more than 5 levels
        
    }
}
