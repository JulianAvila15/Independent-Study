using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OnEnablePowerupIndicator : MonoBehaviour
{

    public TutorialManager tutorialManager;
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
        if(!(tutorialManager.gameManager.tutorialType==GameManager.TutorialType.noTutorial))
        {
            switch (gameObject.name)
            {
                case "PenguinPowerUp":
                    if (TutorialManager.penguinTutorialShown == false)
                    {
                        wallOfTextSentences = "You have unlocked the penguin! Left mouse click on the penguin button to summon the penguin!  The penguin will help you with the orders by finding the ingredient for the second crafting slot! Just drag and drop the right ingredient into the corresponding slot! If the second slot is filled, but with the wrong ingredient, it will remove the current item and show you the right one. Has a cool down and you must wait to summon it again!";
                        powerUpAbilityDataIndex = 0;
                        spriteIndex = (int)powerUp.penguin;
                        TutorialManager.penguinTutorialShown = true;

                    }
                    break;

                case "MessengerPowerUp":
                    if (TutorialManager.adventurerTutorialShown == false)
                    {
                        wallOfTextSentences= "You have unlocked the messenger! Left mouse click on the messenger button to summon him! The messenger will help you by filling the first slot with the ingredient you need! If first slot is filled but with the wrong ingredient, it will replace it with the right one. He has a cool down and you must wait to summon him again! ";
                        powerUpAbilityDataIndex = 1;
                        spriteIndex = (int)powerUp.messenger;
                        TutorialManager.adventurerTutorialShown = true;
                    }
                    break;
                case "DragonPowerUp":
                    if (TutorialManager.dragonTutorialShown == false)
                    {
                        powerUpAbilityDataIndex = 2;
                        wallOfTextSentences = "You have unlocked the dragon! Left mouse click on the dragon button to summon it! The dragon will help you by filling the last two slots with the ingredients you need! If the last two slots are filled but with the wrong ingredient, it will replace the items with the correct ones. It has a cool down and you must wait to summon him again!";
                        spriteIndex = (int)powerUp.dragon;
                        TutorialManager.dragonTutorialShown = true;
                    }
                    break;
                case "TimingMiniGameButton":
                    if (TutorialManager.timingMiniGameTutorialShown == false)
                    {

                        wallOfTextSentences = "When you stop the timer at the right time you will be certainly aided! Stopping in the green zone grants you two ingredients for the last two slots! Stopping in the yellow zone grants you one ingredient! If you stop in the red zone, you will get no ingredients.";
                        powerUpAbilityDataIndex = 3;
                        spriteIndex = (int)powerUp.timing;
                        TutorialManager.timingMiniGameTutorialShown = true;
                    }
                    break;
                case "CollectingMiniGameButton":
                    if (TutorialManager.coinTutorialShown == false)
                    {
                        wallOfTextSentences = "You have unlocked the ability to collect coins to fill slots! Move left with the 'A' key! Move right with the 'D' key! Jump with spacebar! Collect 5 or more coins to fill one slot or collect all 10 to fill two slots with ingredients! You fill the first two slots or one of them";
                        powerUpAbilityDataIndex = 4;
                        spriteIndex = (int)powerUp.collecting;
                        TutorialManager.coinTutorialShown = true;
                    }
                    break;
                default:
                    break;

            }

            tutorialManager.SetSummonTutorial(wallOfTextSentences, spriteIndex, powerUpAbilityDataIndex);

        }
        TimeManager.ResetAFKTimer();


    }

}
