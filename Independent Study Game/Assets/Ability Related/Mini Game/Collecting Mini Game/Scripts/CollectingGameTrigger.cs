using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingGameTrigger : MonoBehaviour
{
   [SerializeField] private AbilityTutorialProgressiveDisclosureHandler abilityTutorialPDHandler;
    bool collided;

    // Start is called before the first frame update
    void Start()
    {
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (collided == false)
        {
            collided = true;
            abilityTutorialPDHandler.miniGamePDHandler.hasCollidedwithCollectingTrigger = true;
            gameObject.SetActive(false);
        }
            

    }
}
