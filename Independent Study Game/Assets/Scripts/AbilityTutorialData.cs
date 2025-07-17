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

 
    ShowImage,

    //collecting mini game
    waitForWalkRight,
    waitForWalkLeft,
    waitForJump,
    waitForSpawnCoin,

    //timing mini game
    waitForStopOnGreen

}

public class AbilityTutorialData : MonoBehaviour
{
    public string abilityName;
    public List<AbilityTutorialStepData> steps;
    public bool isSummon;
    public Helper summon;
    public Color buttonColor,selectedSlotColor;
    public int slotIndex;   // Used for HighlightSlot
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

}

