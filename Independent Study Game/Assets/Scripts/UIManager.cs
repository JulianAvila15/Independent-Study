using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public Text numOrderDisplay;
    public Slider progressBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.progressType == GameManager.ProgressFeedbackType.score)
        {
            numOrderDisplay.GetComponent<Text>().text = "Order: " + (gameManager.orderManager.numofOrdersCompleted + 1);
        }
    }
}
