using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialStepType
{
    FlashButton,

    //summons
    ShowText,
    HighlightSlot,
    SummonedAppeared,
    IngredientDropped,
    ReplaceIngredient,
    ResetSummon,
    WatchSummon,

 
    ShowImage,

    //collecting mini game
    collectingMiniGameStart,
    waitForWalkRight,
    waitForWalkLeft,
    waitForJump,
    spawnCoin,
    demonstrateCollecting,
    demonstrateLosing,
    endingCollectingGame,

    //timing mini game
    waitForStopOnGreen,

}

public class AbilityTutorialData : MonoBehaviour
{
    public string abilityName;
    public List<AbilityTutorialStepData> steps;
    public bool isSummon;
    public Helper summon;
    public Color buttonColor,selectedSlotColor;
    public int slotIndex;   // Used for HighlightSlot
    public bool buttonCLickedFirstTime;
}

[System.Serializable]
public class AbilityTutorialStepData
{

    public TutorialStepType stepType;//define the step type
    public GameObject callOut;//define the callout to display the sentence
  

    [TextArea]
    public string sentence; // Only used for ShowText

    public GameObject targetObject; // Used for WaitForClick, FlashButton, etc.

    public bool clicked=false;

    public bool summonCanContinue;
    public bool needSummon;
    public bool showPenguinLock;

    public bool canClickThroughPanel=false;
    public bool turnOnCoolDown = false;
    public bool playerCanMove,playerCanJump,canStartSpawningCoin,coinCanStartMoving, coinMoveSlowly;

    public int coinsToSpawnBeforeNextStep;

    public bool showStopButton;



}

