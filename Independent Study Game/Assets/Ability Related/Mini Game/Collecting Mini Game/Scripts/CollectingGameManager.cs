using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollectingGameManager : MonoBehaviour
{
   public static int coinsCollected=0;
    public static int coinsProduced = 0;
    public GameObject collectingGame,mainGame;
    public CraftingManager craftingManager;
    public Slider progressBar;
    int slotIndex;
    int firstSlot = 0, secondSlot = 1;

    public AbilityTutorialProgressiveDisclosureHandler abilityTutorialPDHandler;
   public Coroutine coinSpawnCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!mainGame.activeInHierarchy)
        {
            progressBar.value = coinsCollected;
        }


        if ((!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && coinsProduced >= 11) || coinsProduced >= 12)
        {
            collectingGame.SetActive(false);
            mainGame.SetActive(true);

            if (coinSpawnCoroutine != null)
                StopCoroutine(coinSpawnCoroutine);

            Debug.Log("Trying to disable mini game");
            if (coinsCollected >= 10) //Max Score
            {
                for (int slot = 0; slot < 2; slot++)
                {
                    craftingManager.CreateSlot(slot);
                }

            }
            else if (coinsCollected >= 5) //Benchmark Score
            {
                slotIndex = (craftingManager.finalOrderList[firstSlot] == null || craftingManager.finalOrderList[firstSlot] != craftingManager.orderManager.listOfOrder[0]) ? firstSlot : secondSlot;
                craftingManager.CreateSlot(slotIndex);
            }

            if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                abilityTutorialPDHandler.AdvanceAbilityStep();

            coinsCollected = coinsProduced = 0;
        }
                

                
            
        
    }

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        
    }

}

