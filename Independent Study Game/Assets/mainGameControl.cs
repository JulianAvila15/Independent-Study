using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainGameControl : MonoBehaviour
{
    public GameObject tutorialPanel;
    public OrderManager orderManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        tutorialPanel.SetActive(false);

        if (orderManager.feedBackImages[0].gameObject.activeInHierarchy)
            orderManager.feedBackImages[0].gameObject.SetActive(false);

        if (orderManager.feedBackImages[1].gameObject.activeInHierarchy)
            orderManager.feedBackImages[1].gameObject.SetActive(false);
    }
}
