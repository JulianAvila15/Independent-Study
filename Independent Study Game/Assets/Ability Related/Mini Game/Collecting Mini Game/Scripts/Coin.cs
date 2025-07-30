using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Transform transformCoin;
    float movementSpeed = 3;
    public ParticleSystem badCollision, goodCollision;
    bool hasCollided = false; // Flag to track if collision has occurred
    bool pauseMovement=false;
   [SerializeField]  GameObject highlight,tutorialArrowPD;
    private AbilityTutorialProgressiveDisclosureHandler abilityTutorialProgressiveDisclosureHandler;
    public bool canStartMoving = false, needToSlow = false;
    float slowSpeedByAmount = .5f;
    


    private void Awake()
    {
        abilityTutorialProgressiveDisclosureHandler = (AbilityTutorialProgressiveDisclosureHandler)FindObjectOfType(typeof(AbilityTutorialProgressiveDisclosureHandler));
    }

    // Start is called before the first frame update
    void Start()
    {
        transformCoin = gameObject.GetComponent<Transform>();

        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
        {
            canStartMoving = abilityTutorialProgressiveDisclosureHandler.miniGamePDHandler.coinCanMove;
            needToSlow = abilityTutorialProgressiveDisclosureHandler.miniGamePDHandler.coinMoveSlow;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (needToSlow)
        {
            if (movementSpeed > 0 && transformCoin.position.y <= 2f)
            {
                movementSpeed = slowSpeedByAmount;
                highlight.SetActive(true);
                tutorialArrowPD.SetActive(true);
            }
               
        }

        if((!pauseMovement|| transformCoin.position.y >= 2f)||!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
            transformCoin.position -= new Vector3(0, movementSpeed * Time.deltaTime, 0);

        if (!canStartMoving&& AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && transformCoin.position.y <= 2f && transformCoin.position.y >= .5f)
        {
            pauseMovement = true;
            highlight.SetActive(true);
            abilityTutorialProgressiveDisclosureHandler.miniGamePDHandler.coinHasSpawned = true;
            tutorialArrowPD.SetActive(true);
        }
        else if(canStartMoving&&!needToSlow)
        {
            highlight.SetActive(false);
            pauseMovement = false;
        }

        if (transformCoin.position.y <= .5f)
        {
            if (!hasCollided) // Check if collision hasn't occurred already
            {
                Instantiate(badCollision, transformCoin.position, Quaternion.identity);
                CheckIfCanContinuePDTutorialAfterColliding();
                Destroy(gameObject);
            }
        }

        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCollided) // Check if collision hasn't occurred already
        {
            Instantiate(goodCollision, transformCoin.position, Quaternion.identity);
            CollectingGameManager.coinsCollected++;
            hasCollided = true; // Set the flag to true to prevent further collisions
            TimeManager.ResetAFKTimer();

            if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
            {
                CheckIfCanContinuePDTutorialAfterColliding();
                if (pauseMovement)
                    pauseMovement = false;
            }

            Destroy(gameObject);
            
        }
    }

    private void CheckIfCanContinuePDTutorialAfterColliding()
    {
        if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
            abilityTutorialProgressiveDisclosureHandler.miniGamePDHandler.CheckIfCanContinueAfterCollectingCoin();
    }

}
