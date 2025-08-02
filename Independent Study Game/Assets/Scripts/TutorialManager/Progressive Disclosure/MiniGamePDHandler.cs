using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class MiniGamePDHandler : MonoBehaviour
{
    //Mini game relevant variables
    public CoinSpawner coinSpawner;
    public GameObject collectingGame;
    public GameObject leftCollectingTrigger, rightCollectingTrigger;
    public bool hasCollidedwithCollectingTrigger = false;
    [SerializeField] private Player playerObj;
    bool collectorHasJumped;
    public bool coinHasSpawned = false, coinCanMove = false;
    public bool coinMoveSlow = false;
    private Coroutine waitForLanding = null;
    [SerializeField] private Image leftDirectionArrow, rightDirectionArrow, upDiectionArrow, spaceBarImage, aKeyImage, dKeyImage;
    [SerializeField] private GameObject timingPauseButton;
    Coroutine coinSpawnCoroutine;
    public Coroutine fadeToBlackCoroutine;
    Vector3 playerPositionDuringDemo;
    public GameObject triggerWarp;


    AbilityTutorialProgressiveDisclosureHandler mainAbilityPDHandler;

    public Coroutine currentMiniGameCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        mainAbilityPDHandler = gameObject.GetComponent<AbilityTutorialProgressiveDisclosureHandler>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MiniGameStartStep(AbilityTutorialStepData current_step,string currentAbilityName)
    {

        if (currentAbilityName == "Collecting")//if can click through panel;
        {
            playerObj.playerCanMoveInPDTutorial = current_step.playerCanMove;
            playerObj.playerCanJumpInPDTutorial = current_step.playerCanJump;

            coinSpawner.canStartSpawningInTutorial = current_step.canStartSpawningCoin;

            coinCanMove = current_step.coinCanStartMoving;
            coinMoveSlow = current_step.coinMoveSlowly;

        }


        if (currentAbilityName == "Timing")//if can click through panel;
        {
            timingPauseButton.SetActive(current_step.showStopButton);
        }
    }


    //Collecting mini game handler stuff
    public void DemonstrateLosing()
    {

        int bringUpPlayerBy = 6;

        if (fadeToBlackCoroutine == null)
            fadeToBlackCoroutine = StartCoroutine(FadeToBlackCoroutine());

        playerPositionDuringDemo = new Vector3(-23.6000004f, playerObj.transform.position.y + bringUpPlayerBy, playerObj.transform.position.z);
        triggerWarp.SetActive(true);
        triggerWarp.transform.position = playerPositionDuringDemo;
        playerObj.transform.position = playerPositionDuringDemo;
    }

    public void SpawnCoin()
    {

        currentMiniGameCoroutine = StartCoroutine(WaitForCoinToSpawn());
        coinSpawnCoroutine = StartCoroutine(coinSpawner.SpawnCoin());
    }

    public void CheckIfCanContinueAfterCollectingCoin()
    {

        if (playerObj.m_Grounded)
        {
            if (waitForLanding != null)
                StopCoroutine(waitForLanding);

            if ((mainAbilityPDHandler.GetStepTutorialType() == TutorialStepType.demonstrateCollecting || mainAbilityPDHandler.GetStepTutorialType() == TutorialStepType.demonstrateLosing))
                mainAbilityPDHandler.AdvanceAbilityStep();
        }
        else
        {
            waitForLanding = StartCoroutine(playerObj.WaitForLanding());
        }
    }

    public void WaitForJump()
    {
        //logic that is executed when the player jumps x number of times
        currentMiniGameCoroutine = StartCoroutine(WaitForJumpCoroutine());

        if (fadeToBlackCoroutine == null)
            fadeToBlackCoroutine = StartCoroutine(FadeToBlackCoroutine());
        upDiectionArrow.gameObject.SetActive(true);
        spaceBarImage.gameObject.SetActive(true);


        Vector3 playerCurrentPosition = new Vector3(playerObj.transform.position.x, playerObj.transform.position.y, playerObj.transform.position.z);

        playerObj.transform.position = new Vector3(playerObj.playerSpawnPosition.x, playerCurrentPosition.y, playerCurrentPosition.z);

        playerObj.canMoveLeft = playerObj.canMoveRight = false;

    }

    public void WaitForWalkRightOrLeft(bool walkLeftorWalkRight) //temp argument to signify if the player has walked x number of pixels right or left
    {
        //logic that is executed when the player walks x number to the right

        if (walkLeftorWalkRight)//walk right
        {
            rightCollectingTrigger.SetActive(true);
            rightDirectionArrow.gameObject.SetActive(true);
            playerObj.canMoveLeft = false;
            dKeyImage.gameObject.SetActive(true);
        }
        else //walk left
        {
            leftDirectionArrow.gameObject.SetActive(true);
            leftCollectingTrigger.SetActive(true);
            playerObj.canMoveRight = false;
            aKeyImage.gameObject.SetActive(true);
        }

        currentMiniGameCoroutine = StartCoroutine(WaitToHitTrigger());


    }

    public void SetHasJumped()
    {
        if (!collectorHasJumped)
            collectorHasJumped = true;
    }

    IEnumerator WaitForJumpCoroutine()
    {
        yield return StartCoroutine(
      mainAbilityPDHandler.WaitUntilWithTimeout(() => collectorHasJumped, 15f));
        mainAbilityPDHandler.AdvanceAbilityStep();
    }

    IEnumerator WaitForSummonToGetDestroyed()
    {
        yield return StartCoroutine(
     mainAbilityPDHandler.WaitUntilWithTimeout(() => !CraftingManager.helperIsActive, 5f));
        mainAbilityPDHandler.AdvanceAbilityStep();
    }

    internal void CollectingMiniGameStart()
    {
        coinSpawnCoroutine = StartCoroutine(coinSpawner.SpawnCoin());
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        mainAbilityPDHandler.pdTutorialPanelAbility.GetComponent<Image>().color = Color.black;

        foreach (Transform child in mainAbilityPDHandler.pdTutorialPanelAbility.transform)
        {
            child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(.3f);

        foreach (Transform child in mainAbilityPDHandler.pdTutorialPanelAbility.transform)
        {
            child.gameObject.SetActive(true);
        }

        mainAbilityPDHandler.pdTutorialPanelAbility.GetComponent<Image>().color = mainAbilityPDHandler.currentColor;
    }

    private IEnumerator WaitToHitTrigger()
    {
        yield return StartCoroutine(
     mainAbilityPDHandler.WaitUntilWithTimeout(() => hasCollidedwithCollectingTrigger, 15f));

        mainAbilityPDHandler.AdvanceAbilityStep();
    }

    IEnumerator WaitForCoinToSpawn()
    {
        yield return StartCoroutine(
       mainAbilityPDHandler.WaitUntilWithTimeout(() => coinHasSpawned, 10f));
        mainAbilityPDHandler.AdvanceAbilityStep();
    }

    //Mini Game specific end functions
    public void EndSpawnCoin()
    {
        coinHasSpawned = true;
        coinSpawner.canStartSpawningInTutorial = false;

        if (coinSpawnCoroutine != null)
            StopCoroutine(coinSpawnCoroutine);

    }

    public void EndWaitForJump()
    {
        //End WaitForJump step
        spaceBarImage.gameObject.SetActive(false);
        upDiectionArrow.gameObject.SetActive(false);
        playerObj.canMoveLeft = playerObj.canMoveRight = true;
    }

    public void EndWaitForWalkRightOrLeft(bool leftOrRight)
    {
        //End WaitForWalkRIghtOrLeft step

        if (leftOrRight) //if directing to move left
        {
            leftCollectingTrigger.SetActive(false);
            playerObj.canMoveRight = true;
            leftDirectionArrow.gameObject.SetActive(false);
            aKeyImage.gameObject.SetActive(false);

        }
        if (!leftOrRight) //if directing to move right
        {
            rightCollectingTrigger.SetActive(false);
            playerObj.canMoveLeft = true;
            rightDirectionArrow.gameObject.SetActive(false);
            dKeyImage.gameObject.SetActive(false);

        }

        hasCollidedwithCollectingTrigger = false;
    }

    internal void EndCollectingMiniGame()
    {
        if (coinSpawnCoroutine != null)
            StopCoroutine(coinSpawnCoroutine);
    }

    internal void EndDemonstratingLosing()
    {
        triggerWarp.transform.position = playerObj.playerSpawnPosition;
    }

  public  bool NeedToStopCoinDuringTutorial()
    {
        return !coinCanMove && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered;
    }

    public bool InCollectingMiniGameTutorial()
    {
        return !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || (mainAbilityPDHandler.GetStepTutorialType() == TutorialStepType.collectingMiniGameStart);
    }
}
