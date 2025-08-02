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
    Color penguinButtonColor;
    public GameObject penguinButton;
    public Image penguinImage;
    private int penguinProspectiveSlotNum = 1, messengerProspectiveSlotNum = 0;
    public int penguinSlot = -1;//Gets the slot the penguin has chosen to color blue

    
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

        penguinButtonColor = penguinButton.GetComponent<Image>().color;
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

        if (managerHub.orderManager.penguinUnlocked && (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || managerHub.abilityPDManager.GetStepTutorialType() != TutorialStepType.FlashButton))
        {

            HandleSummonButtonEnableorDisable();
        }

        if (penguinSlot > 0 && managerHub.craftingManager.craftingSlots[penguinSlot].item != null && managerHub.craftingManager.finalOrderList[penguinSlot].itemName != managerHub.orderManager.listOfOrder[penguinSlot].itemName)
           managerHub.craftingManager.craftingSlots[penguinSlot].GetComponent<Image>().color = Color.yellow;

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

    private void HandleSummonButtonEnableorDisable()
    {

        if (managerHub.craftingManager.finalOrderList[penguinProspectiveSlotNum] != null && managerHub.orderManager.listOfOrder[penguinProspectiveSlotNum].itemName == managerHub.craftingManager.finalOrderList[penguinProspectiveSlotNum].itemName)
        {
            penguinButton.GetComponent<Button>().interactable = false;
            penguinImage.color = penguinButton.GetComponent<Image>().color = penguinButton.GetComponent<Button>().colors.disabledColor;
        }
        else
        {
            penguinButton.GetComponent<Button>().interactable = true;
            penguinButton.GetComponent<Image>().color = penguinButtonColor;
            penguinImage.color = Color.white;
        }
    }


    public void FillPenguinSlot()
    {


        if (mainGame.activeInHierarchy == true)//if the penguin has shown the ingredient
        {
            if(penguinProspectiveSlotNum>0)
            managerHub.craftingManager.FillInSlot(penguinProspectiveSlotNum,false);


            if (managerHub.craftingManager.finalOrderList[penguinSlot] != null && managerHub.orderManager.listOfOrder[penguinSlot].itemName == managerHub.craftingManager.finalOrderList[penguinSlot].itemName)//if the correct item is in the in the penguin slot
            {
                penguinItemSuccessfullyDropped = true;

                managerHub.orderManager.listOfOrder[penguinSlot].imageOfItem.color = managerHub.craftingManager.imageCraftingSlots[penguinSlot].color = Color.white;
                Helper.penguinHasShownIngredient = false;


                if (!AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered)
                    penguinSlot = -1;

                if (firstTimeUsePenguin)
                {
                    managerHub.abilityPDManager.AdvanceAbilityStep();
                    firstTimeUsePenguin = false;
                }
            }
        }
    }

    public bool PenguinInCurrentUse()
    {
        return penguinSlot > 0 && managerHub.orderManager.listOfOrder[penguinSlot] == managerHub.craftingManager.currentItem;
    }

  public bool CanSafetlyDropItemWhenPenguinIsActive(int neareSlotSelectedIndex=1)
    {
        return (PenguinInCurrentUse() && (CanSafetlyDropItemWhenPenguinActiveAndItemDragged(neareSlotSelectedIndex) || CanSafetlyDropItemWhenPenguinPenguinActiveAndItemClicked()));
    }

 

     bool CanSafetlyDropItemWhenPenguinActiveAndItemDragged(int nearestSlotIndex)
    {
        return (((managerHub.craftingManager.craftingSlots[nearestSlotIndex] != managerHub.craftingManager.craftingSlots[penguinSlot] && managerHub.craftingManager.currentItem.itemName != managerHub.orderManager.listOfOrder[penguinSlot].itemName)
            || (managerHub.craftingManager.currentItem == managerHub.orderManager.listOfOrder[penguinSlot])));
    }


     bool CanSafetlyDropItemWhenPenguinPenguinActiveAndItemClicked()
    {
      return  (Helper.penguinHasShownIngredient && managerHub.craftingManager.currentItem != null && managerHub.craftingManager.craftingSlots[managerHub.summonManager.penguinSlot].item == null && managerHub.orderManager.listOfOrder[managerHub.summonManager.penguinSlot].itemName == managerHub.craftingManager.currentItem.itemName);
    }
    

    private bool CheckIfCanBePressed(string summonName)
    {
        return managerHub.gameManager.tutorialType != GameManager.TutorialType.progressiveDisclosure || !AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered || (managerHub.abilityPDManager.GetCurrentAbilityDataName() != null && AbilityTutorialProgressiveDisclosureHandler.abilityTutorialTriggered && (managerHub.abilityPDManager.GetCurrentAbilityDataName() == summonName));
    }




}
