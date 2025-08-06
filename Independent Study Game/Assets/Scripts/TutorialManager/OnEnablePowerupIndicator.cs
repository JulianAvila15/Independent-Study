using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OnEnablePowerupIndicator : MonoBehaviour
{

 [SerializeField]   ManagerofManagers managerHub;
    string wallOfTextSentences;
    int spriteIndex;
    int powerUpAbilityDataIndex;
    public static bool enabled = false;

    enum powerUp
    {
        penguin,
        messenger,
        dragon,
        timing,
        collecting
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool IsEnabled()
    {
        return enabled;
    }

    private void OnEnable()
    {


        if(!(managerHub.gameManager.tutorialType==GameManager.TutorialType.noTutorial))
        {
            switch (gameObject.name)
            {
                case "PenguinPowerUp":
                    if (TutorialManager.penguinTutorialShown == false)
                    {
                        wallOfTextSentences = "You have unlocked the penguin! Left mouse click on the penguin button to summon the penguin!  The penguin will help you with the orders by finding the ingredient for the second crafting slot! The penguin will highlight the second slot and the correct ingredient light blue. Just drag and drop the correct ingredient into the corresponding slot! If the second slot is filled, but with the wrong ingredient, it will remove the current item and show you the correct one. You must drag the correct ingredient to the correct slot to satisfy it." +
                            " Penguin button will be disabled when the correct ingredient is the second slot. A blue lock will appear to prevent the wrong ingredient from getting into the second slot. Has a cool down and you must wait to summon it again!";
                        powerUpAbilityDataIndex = 0;
                        spriteIndex = (int)powerUp.penguin;
                        TutorialManager.penguinTutorialShown = true;

                    }
                    break;

                case "MessengerPowerUp":
                    if (TutorialManager.adventurerTutorialShown == false)
                    {
                        wallOfTextSentences= "You have unlocked the messenger! Left mouse click on the messenger button to summon him! The messenger will help you by filling the first slot with the ingredient you need! If first slot is filled but with the wrong ingredient, it will replace it with the correct one. " +
                            " Messenger button will be disabled when the correct ingredient is the first slot. He has a cool down and you must wait to summon him again! ";
                        powerUpAbilityDataIndex = 1;
                        spriteIndex = (int)powerUp.messenger;
                        TutorialManager.adventurerTutorialShown = true;
                    }
                    break;
                case "DragonPowerUp":
                    if (TutorialManager.dragonTutorialShown == false)
                    {
                        powerUpAbilityDataIndex = 2;
                        wallOfTextSentences = "You have unlocked the dragon! Left mouse click on the dragon button to summon it! The dragon will help you by filling the last two slots with the ingredients you need! If the last two slots are filled but with the wrong ingredient, it will replace the items with the correct ones. " +
                            " Dragon button will be disabled when the correct ingredients are in the third and last slots. It has a cool down and you must wait to summon him again!";
                        spriteIndex = (int)powerUp.dragon;
                        TutorialManager.dragonTutorialShown = true;
                    }
                    break;
                case "TimingMiniGameButton":
                    if (TutorialManager.timingMiniGameTutorialShown == false)
                    {

                        wallOfTextSentences = "You have unlocked the ability to stop the timer to fill slots! Just left click the timing mini game button start filling slots! When you stop the timer at the correct time you will be certainly aided!" +
                            "If you stop in the red zone, you will get no ingredients. Stopping in the yellow zone grants you one ingredient in the third slot! Stopping in the green zone grants you two ingredients for the last two slots! Click the stop button to stop the slider.  Timing button will be disabled when the correct ingredients are in the third and last slots. Has cool down and must wait to use again. ";
                        powerUpAbilityDataIndex = 3;
                        spriteIndex = (int)powerUp.timing;
                        TutorialManager.timingMiniGameTutorialShown = true;
                    }
                    break;
                case "CollectingMiniGameButton":
                    if (TutorialManager.coinTutorialShown == false)
                    {
                        wallOfTextSentences = "You have unlocked the ability to collect coins to fill slots! Just left click the coin mini game button to start collecting! Move right with the 'D' key! Move left with the 'A' key!  Jump with spacebar! Coins will spawn from the sky. To collect the coin, simply touch them. You will lose the coin if it drops to the ground. " +
                            "Collect 5 or more coins to fill one slot or collect all 10 to fill first two slots with ingredients! The slots filled will be the first two slots. If the wrong ingredients are in the slots before the mini-game ends, the correct ones will automatically replace the first, second, or both slots as needed. The collecting mini game button will be disabled when the correct ingredients are in the first and second slots. Has a cooldown and must wait to start collecting again.";
                        powerUpAbilityDataIndex = 4;
                        spriteIndex = (int)powerUp.collecting;
                        TutorialManager.coinTutorialShown = true;
                    }
                    break;
                default:
                    break;

            }

            if(!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
            managerHub.tutorialManager.SetSummonTutorial(wallOfTextSentences, spriteIndex, powerUpAbilityDataIndex);

        }
        managerHub.timeManager.ResetAFKTimer();


    }

}
