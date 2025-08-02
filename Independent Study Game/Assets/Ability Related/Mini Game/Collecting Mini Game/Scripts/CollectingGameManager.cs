using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollectingGameManager : MonoBehaviour
{
   public int coinsCollected=0;
    public int coinsProduced = 0;
    public GameObject collectingGame,mainGame;
 [SerializeField]   ManagerofManagers managerHub;
    public Slider progressBar;
    int slotIndex;
    int firstSlot = 0, secondSlot = 1;

    public AbilityTutorialProgressiveDisclosureHandler abilityTutorialPDHandler;
   public Coroutine coinSpawnCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        if (managerHub != null)
            managerHub.collectingGameManager = gameObject.GetComponent <CollectingGameManager>() ;
    }

    // Update is called once per frame
    void Update()
    {

        if (!mainGame.activeInHierarchy)
        {
            progressBar.value = coinsCollected;
        }

        if(CanEndCollectingMiniGame())
        HandleEndOfCollectingGame();
                
            
        
    }

    void HandleEndOfCollectingGame()
    {
       
            collectingGame.SetActive(false);
            mainGame.SetActive(true);

            if (coinSpawnCoroutine != null)
                StopCoroutine(coinSpawnCoroutine);

            if (coinsCollected >= 10) //Max Score
            {
                for (int slot = 0; slot < 2; slot++)
                {
                    managerHub.craftingManager.FillInSlot(slot,true);
                }

            }
            else if (coinsCollected >= 5) //Benchmark Score
            {
                slotIndex = (DetermineToFillFirstSlot()) ? firstSlot : secondSlot;
                managerHub.craftingManager.FillInSlot(slotIndex,true);
            }

            if (AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                abilityTutorialPDHandler.AdvanceAbilityStep();

            coinsCollected = coinsProduced = 0;
        
    }

    bool CanEndCollectingMiniGame()
    {
        return ((!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && coinsProduced >= 11) || coinsProduced >= 12);
    }

    bool DetermineToFillFirstSlot()
    {
        return managerHub.craftingManager.finalOrderList[firstSlot] == null || managerHub.craftingManager.finalOrderList[firstSlot] != managerHub.orderManager.listOfOrder[firstSlot];
    }

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        
    }

}

