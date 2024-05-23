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
                for (int i = 0; i < 2; i++)
                {
                    slotIndex = i;
                    craftingManager.CreateSlot(slotIndex);
                }
            }
            else if (coinsCollected >= 5) //Benchmark Score
            {
                slotIndex = 0;
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
