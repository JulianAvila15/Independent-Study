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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        progressBar.value = coinsCollected;

       if(coinsProduced>=11)
        {
            collectingGame.SetActive(false);
            mainGame.SetActive(true);

            if (coinsCollected >= 10) //Max Score
            {
                    
                    craftingManager.CreateSlot(firstSlot);
                craftingManager.CreateSlot(secondSlot);
                
            }
            else if (coinsCollected >= 5) //Benchmark Score
            {
                slotIndex = (craftingManager.finalOrderList[firstSlot] == null || craftingManager.finalOrderList[firstSlot]!=craftingManager.orderManager.listOfOrder[0]) ? firstSlot : secondSlot;
                craftingManager.CreateSlot(slotIndex);
            }
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
