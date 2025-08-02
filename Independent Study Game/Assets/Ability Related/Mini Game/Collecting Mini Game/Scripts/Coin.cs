using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Transform transformCoin;
  [SerializeField] private float movementSpeed = 3;
    public ParticleSystem badCollision, goodCollision;
    bool hasCollided = false; // Flag to track if collision has occurred
    bool pauseMovement=false;
   [SerializeField]  GameObject highlight,tutorialArrowPD;
    private AbilityTutorialProgressiveDisclosureHandler abilityTutorialProgressiveDisclosureHandler;
    public bool canStartMoving = false, needToSlow = false;
    float slowSpeedByAmount = .5f;
  [SerializeField]  ManagerofManagers managerHub;

 [SerializeField] private float pdTopPosition = 2f, groundPosition = .5f;


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
        if(!GameManager.pause)
        HandleCoinMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCollided) // Check if collision hasn't occurred already
        {
            Instantiate(goodCollision, transformCoin.position, Quaternion.identity);
            managerHub.collectingGameManager.coinsCollected++;
            hasCollided = true; // Set the flag to true to prevent further collisions
            managerHub.timeManager.ResetAFKTimer();

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

    private void HandleCoinMovement()
    {
        if (needToSlow)
        {
            if (movementSpeed > 0 && transformCoin.position.y <= pdTopPosition)
            {
                movementSpeed = slowSpeedByAmount;
                highlight.SetActive(true);
                tutorialArrowPD.SetActive(true);
            }
               
        }

        if (ShouldCoinMove())
        {
            transformCoin.position -= new Vector3(0, movementSpeed * Time.deltaTime,0);
        }
        if (managerHub.abilityPDManager.miniGamePDHandler.NeedToStopCoinDuringTutorial() && ReachedPDTutorialPosition())
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

        if (transformCoin.position.y <= groundPosition) //coin has hit ground
        {
            DestroyCoin();
        }
    }

    private void DestroyCoin()
    {
        if (!hasCollided) // Check if collision hasn't occurred already
        {
            Instantiate(badCollision, transformCoin.position, Quaternion.identity);
            CheckIfCanContinuePDTutorialAfterColliding();
            Destroy(gameObject);
        }
    }

    bool ShouldCoinMove()
    {
        return (!pauseMovement || transformCoin.position.y >= pdTopPosition) || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered;
    }

    bool ReachedPDTutorialPosition()
    {
        return transformCoin.position.y <= pdTopPosition && transformCoin.position.y >= groundPosition;
    }


    
}
