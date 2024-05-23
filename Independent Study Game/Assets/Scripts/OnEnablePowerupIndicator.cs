using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OnEnablePowerupIndicator : MonoBehaviour
{

    public TutorialManager tutorialManager;
    public string[] sentences;
    public static bool enabled = false;
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
                        sentences = new string[4];
                        sentences[0] = "Summon the penguin to help you with the orders!";
                        sentences[1] = "Left mouse click on the penguin button to activate the penguin!";
                        sentences[2] = "Has a cool down and you must wait to summon it again!";
                        sentences[3] = "You may not craft while it is out.";
                        tutorialManager.SetSummonTutorial("Summon the penguin to help you with the orders! Left mouse click on the penguin button to activate the penguin! Has a cool down and you must wait to summon it again! You may not craft while it is out.", 0, sentences);
                        TutorialManager.penguinTutorialShown = true;
                    }
                    break;

                case "MessengerPowerUp":
                    if (TutorialManager.adventurerTutorialShown == false)
                    {
                        sentences = new string[4];
                        sentences[0] = "When you summon the adventurer and he will bring you what you one ingredient you need!";
                        sentences[1] = "Left mouse click on the adventurer button to activate the adventurer!";
                        sentences[2] = "He has a cool down and you must wait to summon him again!";
                        sentences[3] = "You may not craft while he is out";

                        tutorialManager.SetSummonTutorial("When you summon the adventurer and he will bring you what you one ingredient you need! Left mouse click on the adventurer button to activate the adventurer! He has a cool down and you must wait to summon him again! You may not craft while he is out.  ", 1, sentences);
                        TutorialManager.adventurerTutorialShown = true;
                    }
                    break;
                case "DragonPowerUp":
                    if (TutorialManager.dragonTutorialShown == false)
                    {
                        sentences = new string[4];
                        sentences[0] = "When you summon the dragon he will bring you two ingredients you will need!";
                        sentences[1] = "Left mouse click on the dragon button to activate the dragon!";
                        sentences[2] = "He has a cool down and you must wait to summon him again!";
                        sentences[3] = "You may not craft while he is out";

                        tutorialManager.SetSummonTutorial("When you summon the dragon he will bring you two ingredients you will need! Left mouse click on the dragon button to activate the dragon! He has a cool down and you must wait to summon him again! You may not craft while he is out.", 2, sentences);
                        TutorialManager.dragonTutorialShown = true;
                    }
                    break;
                case "TimingMiniGameButton":
                    if (TutorialManager.timingMiniGameShown == false)
                    {
                        sentences = new string[4];
                        sentences[0] = "When you stop the timer at the right time you will be certainly aided!";
                        sentences[1] = "Stopping in the green zone grants you two ingredients!";
                        sentences[2] = "Stopping in the yellow zone grants you one ingredient!";
                        sentences[3] = "If you stop in the red zone, you will get no ingredients";

                        tutorialManager.SetSummonTutorial("When you stop the timer at the right time you will be certainly aided! Stopping in the green zone grants you two ingredients! Stopping in the yellow zone grants you one ingredient! If you stop in the red zone, you will get no ingredients.", 3, sentences);
                        TutorialManager.timingMiniGameShown = true;
                    }
                    break;
                case "CollectingMiniGameButton":
                    if (TutorialManager.coinTutorialShown == false)
                    {
                        sentences = new string[2];
                        sentences[0] = "Collect as many coins as you can by moving around using your WASD keys and jumping by pressing the SPACE key!";
                        sentences[1] = "Collect 5 or more coins to fill one slot or collect 10 to fill two slots with ingredients!";

                        tutorialManager.SetSummonTutorial("Collect as many coins as you can by moving around using your WASD keys and jumping by pressing the SPACE key! Collect 5 or more coins to fill one slot or collect 10 to fill two slots with ingredients!", 4, sentences);
                        TutorialManager.coinTutorialShown = true;
                    }
                    break;
                default:
                    break;

            }

        }
        TimeManager.ResetAFKTimer();


    }

}
