using UnityEngine;


[CreateAssetMenu(fileName = "ManagerofManagers", menuName = "Managers/Manager of Managers")]
public class ManagerofManagers : ScriptableObject
{
    // Example references to other managers
    public GameManager gameManager;
    public OrderManager orderManager;
    public CraftingManager craftingManager;
    public TimeManager timeManager;
    public TutorialManager tutorialManager;
    public AbilityTutorialProgressiveDisclosureHandler abilityPDManager;
    public IntroProgressiveDisclosureHandler introPDManager;
    public SummonManager summonManager;
    public CollectingGameManager collectingGameManager;
}