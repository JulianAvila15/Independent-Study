using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainGameControl : MonoBehaviour
{
    public GameObject tutorialPanel;
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
       
    }
}
