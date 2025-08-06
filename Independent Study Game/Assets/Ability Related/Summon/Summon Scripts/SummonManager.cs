using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SummonManager : MonoBehaviour
{
   [SerializeField] ManagerofManagers managerHub;
 
    public Image summonImage1;
    public Image summonImage2;
    public Image summonImage3;

    public GameObject newLevelAlert, powerUpTutorialPanel;

    public GameObject[] powerUps;
    public GameObject createdObject;
    [Header("Ability1")]
    public float coolDown1;
    bool isCoolDown1 = false;
    public KeyCode summon1;
    bool pressed1 = false;

    [Header("Ability2")]
    public float coolDown2;
    bool isCoolDown2 = false;
    public KeyCode summon2;
    bool pressed2 = false;

    [Header("Ability3")]
    public float coolDown3;
    bool isCoolDown3 = false;
    public KeyCode summon3;
    bool pressed3 = false;

    public DataMiner miner;
    Vector3 spawnPosition, dragonPosition;

    bool firstTimeClicked = false, secondTimeClicked = false;

    [SerializeField] private GameObject mainGame;

    //belong to summon handler
    static public bool canClickCraftButton = false;
    public bool firstTimeUsePenguin = false;
    public bool penguinItemSuccessfullyDropped = false;
    private int penguinProspectiveSlotNum = 1, messengerProspectiveSlotNum = 0;
    public int penguinSlot = -1;//Gets the slot the penguin has chosen to color blue
    public Color iceBlue = new Color(0.69f, 1f, 1f);
    [SerializeField] public Sprite iceLockImg;
    public bool messengerIsOut=false;

    public enum summonIndex
    {
        messenger = 0,
        penguin = 1,
        dragon = 2
    }


    private void Awake()
    {
        if (managerHub != null && managerHub.summonManager == null)
            managerHub.summonManager = gameObject.GetComponent<SummonManager>();

        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        summonImage1.fillAmount = summonImage2.fillAmount = summonImage3.fillAmount = 0;

        spawnPosition = new Vector3(-12.13f, -2.4f, 0);

        dragonPosition = new Vector3(12.13f, -2f, 0);

        
    }

    // Update is called once per frame
    void Update()
    {


        //messenger
        if (pressed1)
            TriggerAbility(ref pressed1, ref isCoolDown1, "Messenger", summon1, 0, spawnPosition, ref summonImage1, coolDown1);
        //penguin
        if (pressed2)
            TriggerAbility(ref pressed2, ref isCoolDown2, "Penguin", summon2, 1, spawnPosition, ref summonImage2, coolDown2);

        //dragon
        if (pressed3)
            TriggerAbility(ref pressed3, ref isCoolDown3, "Dragon", summon3, 2, dragonPosition, ref summonImage3, coolDown3);

        if (PenguinInCurrentUse())
        {
            if (managerHub.craftingManager.IsIncorrectIngredientInSlot(penguinSlot))
                managerHub.craftingManager.imageCraftingSlots[penguinSlot].color = Color.yellow;

            if ((managerHub.craftingManager.currentItem != null && !CurrentItemIsPenguinItem()))
                managerHub.craftingManager.imageCraftingSlots[penguinSlot].sprite = iceLockImg;
            else if (Helper.penguinHasShownIngredient)
            {
                managerHub.craftingManager.imageCraftingSlots[penguinSlot].color = Color.cyan;
                managerHub.craftingManager.imageCraftingSlots[penguinSlot].sprite = null;
            }
        }
        

    }

    void TriggerAbility(ref bool pressed, ref bool isCoolDown, string summonName, KeyCode summonKeyCode, int powerUpIndex, Vector3 spawnPosition, ref Image summonImage, float coolDown)
    {
        if (pressed && isCoolDown == false)
        {
            isCoolDown = true;
            ExecuteAbility(powerUps[powerUpIndex], summonImage, isCoolDown, spawnPosition);

            if (!Helper.penguinHasShownIngredient && CraftingManager.helperName == "Penguin")
                penguinItemSuccessfullyDropped = false;

            if (managerHub.abilityPDManager != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                managerHub.abilityPDManager.OnAbilityButtonClicked();

        }

        if (managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || managerHub.abilityPDManager.completedTutorials.Contains(summonName) && managerHub.abilityPDManager.completedTutorials.Count > 0 || managerHub.abilityPDManager.CheckIfCoolDownEnabled())
        {
            if (isCoolDown == true)
            {
                if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
                    CoolDown(summonImage, coolDown, ref isCoolDown, ref pressed);
            }
        }

    }

    private void ExecuteAbility(GameObject powerUp, Image summonImage, bool isCoolDown, Vector3 spawnPosition)
    {
        summonImage.fillAmount = 1;
        createdObject = Instantiate(powerUp, spawnPosition, Quaternion.identity);
        createdObject.transform.parent = mainGame.transform;
        createdObject.gameObject.SetActive(true);
    }

    private void CoolDown(Image summonImage, float coolDown, ref bool isCoolDown, ref bool pressed)
    {

        if (!GameManager.pause)
        {
            summonImage.fillAmount -= 1 / coolDown * Time.deltaTime;

            if (summonImage.fillAmount <= 0)
            {
                summonImage.fillAmount = 0;
                isCoolDown = false;
                pressed = false;
            }
        }
    }

    public void Pressed(string objName)
    {
        if (!(newLevelAlert.activeInHierarchy || powerUpTutorialPanel.activeInHierarchy))
        {
            if (CheckIfCanBePressed(objName))
            {
                switch (objName)
                {

                    case "Messenger":
                        pressed1 = true;
                        if (isCoolDown1 == false)
                            DataMiner.summonandEventCount[0]++;
                        break;
                    case "Penguin":
                        pressed2 = true;
                        CraftingManager.helperName = "Penguin";
                        if (isCoolDown2 == false)
                            DataMiner.summonandEventCount[1]++;
                        break;
                    case "Dragon":
                        pressed3 = true;
                        if (isCoolDown3 == false)
                            DataMiner.summonandEventCount[2]++;
                        break;
                    default:
                        break;
                }

            }

            managerHub.timeManager.ResetAFKTimer();

        }

    }




    public void FillPenguinSlot()
    {


        if (mainGame.activeInHierarchy == true)//if the penguin has shown the ingredient
        {


            if (managerHub.craftingManager.finalOrderList[penguinSlot] != null && managerHub.orderManager.listOfOrder[penguinSlot].itemName == managerHub.craftingManager.finalOrderList[penguinSlot].itemName)//if the correct item is in the in the penguin slot
            {
                penguinItemSuccessfullyDropped = true;

                managerHub.orderManager.listOfOrder[penguinSlot].imageOfItem.color = managerHub.craftingManager.imageCraftingSlots[penguinSlot].color = Color.white;
                Helper.penguinHasShownIngredient = false;

                    penguinSlot = -1;

                if (firstTimeUsePenguin)
                {
                    managerHub.abilityPDManager.AdvanceAbilityStep();
                    firstTimeUsePenguin = false;
                }
            }
        }
    }

   public void SetPenguinSlot()
    {
        penguinSlot = penguinProspectiveSlotNum;
    }

    public bool CorrectIngredientInPenguinSlot()
    {
        return penguinSlot > 0 && managerHub.orderManager.listOfOrder[penguinSlot] == managerHub.craftingManager.currentItem;
    }

    public bool PenguinInCurrentUse()
    {
        return penguinSlot > 0;
    }
 

   public  bool CanSafetlyDropItemWhenPenguinActiveAndItemDragged(int nearestSlotIndex)
    {
        return (((managerHub.craftingManager.craftingSlots[nearestSlotIndex] != managerHub.craftingManager.craftingSlots[penguinSlot] && managerHub.craftingManager.currentItem.itemName != managerHub.orderManager.listOfOrder[penguinSlot].itemName)
            || (managerHub.craftingManager.currentItem == managerHub.orderManager.listOfOrder[penguinSlot])));
    }


   public  bool CanSafetlyDropItemWhenPenguinPenguinActiveAndItemClicked()
    {
      return  (PenguinInCurrentUse() && managerHub.craftingManager.currentItem != null && managerHub.craftingManager.craftingSlots[managerHub.summonManager.penguinSlot].item == null && managerHub.orderManager.listOfOrder[managerHub.summonManager.penguinSlot].itemName == managerHub.craftingManager.currentItem.itemName);
    }
    
    public bool CurrentItemIsPenguinItem()
    {

        return managerHub.craftingManager.currentItem == managerHub.orderManager.listOfOrder[penguinSlot];

    }

    private bool CheckIfCanBePressed(string summonName)
    {
        return managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || (managerHub.abilityPDManager.GetCurrentAbilityDataName() != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && (managerHub.abilityPDManager.GetCurrentAbilityDataName() == summonName));
    }




}
